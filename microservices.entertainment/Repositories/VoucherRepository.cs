using microservices.entertainment.Data.DbContexts;
using microservices.entertainment.Models;
using microservices.entertainment.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace microservices.entertainment.Repositories
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly NpgsqlDbContext _dbContext;

        public VoucherRepository(NpgsqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<RedemptionModel> GetRedemptionAsync(Guid userId, Guid voucherTicket)
        {
            var redemption = await _dbContext.Redemptions
                .Where(redemption => redemption.UserId == userId && redemption.VoucherTicket == voucherTicket)
                .FirstOrDefaultAsync();

            return RedemptionModel.GetRedemptionModel(redemption);
        }
    }
}
