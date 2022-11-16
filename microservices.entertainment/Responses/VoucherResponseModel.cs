using Entertainment.Microservices.Responses;
using microservices.entertainment.Utils;

namespace microservices.entertainment.Responses
{
    public class VoucherResponseModel : ResponseModel
    {
        public string VoucherImageUrl { get; set; }
        public bool IsSuccess => StatusCode == System.Net.HttpStatusCode.Created;
        public static VoucherResponseModel Success(string voucherImageUrl)
        {
            return new VoucherResponseModel()
            {
                StatusCode = System.Net.HttpStatusCode.Created,
                VoucherImageUrl = voucherImageUrl
            };
        }

        public static VoucherResponseModel Failed(string message)
        {
            return new VoucherResponseModel()
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Messages = new List<string> { message }
            };
        }
    }
}
