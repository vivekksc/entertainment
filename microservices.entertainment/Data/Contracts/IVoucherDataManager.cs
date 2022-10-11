using microservices.entertainment.Models;

namespace microservices.entertainment.Data.Contracts
{
    public  interface IVoucherDataManager
    {
        /// <summary>
        /// Gets redemption detail.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="voucherTicket"></param>
        /// <returns><see cref="Redemption"/></returns>
        Redemption GetRedemption(Guid userId, Guid voucherTicket);
    }
}
