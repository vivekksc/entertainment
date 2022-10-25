using microservices.entertainment.Models;
using microservices.entertainment.Responses;

namespace microservices.entertainment.Services.Interfaces
{
    public interface IVoucherService
    {
        /// <summary>
        /// Gets redemption detail.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="voucherTicket"></param>
        /// <returns><see cref="RedemptionModel"/></returns>
        Task<RedemptionResponseModel> GetRedemptionAsync(Guid userId, Guid voucherTicket);

        /// <summary>
        /// Generates voucher image.
        /// </summary>
        /// <param name="redemption"></param>
        /// <returns></returns>
        Task<string> GenerateVoucherImageAsync(RedemptionModel redemption);
    }
}
