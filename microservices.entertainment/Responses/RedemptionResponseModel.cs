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
        public static RedemptionResponseModel NotFound(string message = null)
        {
            return new RedemptionResponseModel()
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Messages = message == null ? new List<string> { "No redemption(s) found for the given criteria." }
                                           : new List<string> { message }
            };
        }

        /// <summary>
        /// Failure response.
        /// </summary>
        /// <param name="message">String failure message.</param>
        /// <returns></returns>
        public static RedemptionResponseModel Failed(string message = null)
        {
            return new RedemptionResponseModel()
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Messages = message == null ? new List<string> { "Something went wrong while fetching the redemption for given criteria." }
                                           : new List<string> { message }
            };
        }
    }
}
