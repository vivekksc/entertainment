using microservices.entertainment.Models;

namespace microservices.entertainment.Repositories.Interfaces
{
    public interface IVoucherRepository
    {
        /// <summary>
        /// Gets redemption detail.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="voucherTicket"></param>
        /// <returns><see cref="RedemptionModel"/></returns>
        Task<RedemptionModel> GetRedemptionAsync(Guid userId, Guid voucherTicket);
    }
}
