using System.Net;

namespace MarketAssetApi.Services
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public HttpStatusCode Status { get; set; }
        public ErrorResponse? Error { get; set; }
        public static implicit operator ServiceResponse<T>(ServiceResponse serviceResponse) => new ServiceResponse<T>() { Status = serviceResponse.Status, Error = serviceResponse.Error };
    }
    public class ServiceResponse
    {
        public HttpStatusCode Status { get; set; }
        public ErrorResponse? Error { get; set; }
    }
}
