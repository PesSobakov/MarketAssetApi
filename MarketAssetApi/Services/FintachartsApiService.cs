using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Net;
using System.Linq;
using System.Globalization;
using MarketAssetApi.Models.DTOs;
using System.Text;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using MarketAssetApi.Models.MarketAssetDatabase;
using System.Text.Unicode;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json.Serialization.Metadata;

namespace MarketAssetApi.Services
{
    public class FintachartsApiService : IFintachartsApiService
    {
        private readonly string _wssUrl;
        private readonly string _httpsUrl;
        private readonly string _getTokenPath;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FintachartsOptions _options;
        private static string? _token;
        public FintachartsApiService(IHttpClientFactory httpClientFactory, IOptions<FintachartsOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _wssUrl = "wss://platform.fintacharts.com/api/streaming/ws/v1/realtime?token=";
            _httpsUrl = "https://platform.fintacharts.com";
            _getTokenPath = "/identity/realms/fintatech/protocol/openid-connect/token";
            _options = options.Value;
        }

        public async Task<ServiceResponse<HistoricalPricesDto>> GetHistoricalPrices(MarketAsset marketAsset, int interval, string periodicity, DateTime startDate, DateTime endDate)
        {
            HistoricalPricesDto? historicalPrices;

            HttpResponseMessage? response = await TrySendRequest(HttpMethod.Get, new Uri(_httpsUrl + $"/api/bars/v1/bars/date-range?instrumentId={marketAsset.InstrumentId}&provider={_options.Provider}&interval={interval}&periodicity={periodicity}&startDate={startDate.ToString("yyyy-MM-ddTHH\\:mm\\:ss", CultureInfo.InvariantCulture)}&endDate={endDate.ToString("yyyy-MM-ddTHH\\:mm\\:ss", CultureInfo.InvariantCulture)}"));
            if (response == null)
            {
                return new ServiceResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Error = new ErrorResponse()
                    {
                        Error = new ErrorMessage() { Code = "3rd party api error", Message = "Cannot get authentication token" }
                    }
                };
            }
            if (response.IsSuccessStatusCode)
            {
                historicalPrices = JsonSerializer.Deserialize<HistoricalPricesDto>(response.Content.ReadAsStream());
                return new ServiceResponse<HistoricalPricesDto>()
                {
                    Data = historicalPrices,
                    Status = HttpStatusCode.OK
                };
            }
            else
            {
                return new ServiceResponse()
                {
                    Status = HttpStatusCode.BadRequest,// response.StatusCode,
                    Error = new ErrorResponse()
                    {
                        Error = new ErrorMessage()
                        {
                            Code = "3rd party api error",
                            Message = await response.Content.ReadAsStringAsync()
                        }
                    }
                };
            }
        }

        public async Task<ServiceResponse> GetRealTimePrices(WebSocket webSocket, List<MarketAsset> marketAssets)
        {
            JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver
                {
                    Modifiers =
                    {
                        static typeInfo =>
                        {
                            if (typeInfo.Kind != JsonTypeInfoKind.Object)
                                return;
                            foreach (JsonPropertyInfo propertyInfo in typeInfo.Properties)
                            {
                                propertyInfo.IsRequired = false;
                            }
                        }
                    }
                }
            };

            ClientWebSocket stockWebSocket = await TryConnect(_wssUrl, CancellationToken.None);
            for (int i = 0; i < marketAssets.Count; i++)
            {
                RealTimePricesMessageDto realTimePricesMessage = new()
                {
                    Type = "l1-subscription",
                    Id = i.ToString(),
                    InstrumentId = marketAssets[i].InstrumentId,
                    Provider = _options.Provider,
                    Subscribe = true,
                    Kinds = ["ask", "bid", "last"]
                };
                await stockWebSocket.SendAsync(
                    Encoding.UTF8.GetBytes(JsonSerializer.Serialize(realTimePricesMessage)),
                    WebSocketMessageType.Text,
                    endOfMessage: true,
                    CancellationToken.None
                );
            }

            Task? checkConnection = null;
            CancellationTokenSource checkConnectionCancellationTokenSource = new CancellationTokenSource();
            byte[] message;
            while (webSocket.State == WebSocketState.Open && stockWebSocket.State == WebSocketState.Open)
            {
                if (checkConnection?.IsCompleted != false)
                {
                    checkConnection = CheckConnection(webSocket, checkConnectionCancellationTokenSource.Token);
                }
                message = await ReceiveFullMessage(stockWebSocket, CancellationToken.None);
                string messageString = Encoding.UTF8.GetString(message);
                RealTimePricesUpdateDto? update = JsonSerializer.Deserialize<RealTimePricesUpdateDto>(messageString, serializerOptions);
                if (update?.Type == "l1-update")
                {
                    RealTimePricesResponseDto responseDto = new RealTimePricesResponseDto
                    {
                        Symbol = marketAssets.Where(asset => asset.InstrumentId == update.InstrumentId).FirstOrDefault()?.Name ?? "unknown",
                        Ask = update.Ask,
                        Bid = update.Bid,
                        Last = update.Last
                    };
                    await webSocket.SendAsync(
                        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(responseDto, new JsonSerializerOptions() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull})),
                        WebSocketMessageType.Text,
                        endOfMessage: true,
                        CancellationToken.None
                    );
                }
            }
            if (webSocket.State == WebSocketState.CloseReceived)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
            }
            checkConnectionCancellationTokenSource.Cancel();
            return new ServiceResponse() { Status = HttpStatusCode.OK };
        }

        private async Task GetToken()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            Dictionary<string, string> credentials = new Dictionary<string, string>();
            credentials.Add("grant_type", "password");
            credentials.Add("client_id", "app-cli");
            credentials.Add("username", _options.User);
            credentials.Add("password", _options.Password);
            HttpRequestMessage message = new();
            message.RequestUri = new Uri(_httpsUrl + _getTokenPath);
            message.Method = HttpMethod.Post;
            message.Content = new FormUrlEncodedContent(credentials);
            var response = await httpClient.SendAsync(message);
            if (response.IsSuccessStatusCode)
            {
                GetTokenDto? token = JsonSerializer.Deserialize<GetTokenDto>(response.Content.ReadAsStream());
                _token = token?.AccessToken;
            }
        }

        private async Task<HttpResponseMessage?> TrySendRequest(HttpMethod method, Uri uri)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            HttpRequestMessage message;
            HttpResponseMessage response;
            if (_token == null)
            {
                await GetToken();
                if (_token == null)
                {
                    return null;
                }
                message = new();
                message.Method = method;
                message.RequestUri = uri;
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                response = await httpClient.SendAsync(message);
            }
            else
            {
                message = new();
                message.Method = method;
                message.RequestUri = uri;
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                response = await httpClient.SendAsync(message);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await GetToken();
                    if (_token == null)
                    {
                        return null;
                    }
                    message = new();
                    message.Method = method;
                    message.RequestUri = uri;
                    message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                    response = await httpClient.SendAsync(message);
                }
            }
            return response;
        }


        private async Task<ClientWebSocket> TryConnect(string path, CancellationToken cancellationToken)
        {
            ClientWebSocket clientWebSocket;
            HttpClient httpClient = _httpClientFactory.CreateClient();
            if (_token == null)
            {
                await GetToken();
                clientWebSocket = new();
                try
                {
                    await clientWebSocket.ConnectAsync(new Uri(path + _token), cancellationToken);
                    return clientWebSocket;
                }
                catch (System.Net.WebSockets.WebSocketException) { }
            }
            else
            {
                clientWebSocket = new();
                try
                {
                    await clientWebSocket.ConnectAsync(new Uri(path + _token), cancellationToken);
                    return clientWebSocket;
                }
                catch (System.Net.WebSockets.WebSocketException) { }
                await GetToken();
                clientWebSocket = new();
                try
                {
                    await clientWebSocket.ConnectAsync(new Uri(path + _token), cancellationToken);
                    return clientWebSocket;
                }
                catch (System.Net.WebSockets.WebSocketException) { }
            }
            return null;
        }

        private async Task<byte[]> ReceiveFullMessage(ClientWebSocket clientWebSocket, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4];
            using var memoryStream = new MemoryStream();
            WebSocketReceiveResult result;
            byte[] array;
            do
            {
                result = await clientWebSocket.ReceiveAsync(buffer, cancellationToken);
                memoryStream.Write(buffer, 0, result.Count);
            } while (!result.EndOfMessage);
            array = memoryStream.ToArray();
            return array;
        }

        private async Task CheckConnection(WebSocket webSocket, CancellationToken cancellationToken)
        {
            byte[] buffer = Array.Empty<byte>();
            var response = await webSocket.ReceiveAsync(buffer, cancellationToken);
        }


    }
}
