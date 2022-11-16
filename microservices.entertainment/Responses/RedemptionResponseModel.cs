using Entertainment.Microservices.Responses;
using microservices.entertainment.Models;

namespace microservices.entertainment.Responses
{
    /// <summary>
    /// Redemption response model class.
    /// </summary>
    public class RedemptionResponseModel : ResponseModel
    {
        public RedemptionModel Redemption { get; set; }

        /// <summary>
        /// Success response.
        /// </summary>
        /// <param name="redemption"><see cref="RedemptionModel"/></param>
        /// <returns></returns>
        public static RedemptionResponseModel Success(RedemptionModel redemption)
        {
            return new RedemptionResponseModel()
            {
                StatusCode = System.Net.HttpStatusCode.Found,
                Redemption = redemption
            };
        }

        /// <summary>
        /// NotFound response.
        /// </summary>
        /// <param name="message">String message.</param>
        /// <returns></returns>
        public static RedemptionResponseModel NotFound(string message)
        {
            return new RedemptionResponseModel()
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Messages = new List<string> { message }
            };
        }

        /// <summary>
        /// Failure response.
        /// </summary>
        /// <param name="message">String failure message.</param>
        /// <returns></returns>
        public static RedemptionResponseModel Failed(string message)
        {
            return new RedemptionResponseModel()
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Messages = new List<string> { message }
            };
        }
    }
}
