using Dapper;
using microservices.entertainment.Data.Contracts;
using microservices.entertainment.Models;
using Npgsql;

namespace microservices.entertainment.Data
{
    /// <summary>
    /// Data manager for Voucher operations.
    /// </summary>
    public class VoucherDataManager : IVoucherDataManager
    {
        private readonly IConfiguration _configuration;

        public VoucherDataManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        ///<inheritdoc />
        public RedemptionModel GetRedemption(Guid userId, Guid voucherTicket)
        {
            var redemption = new RedemptionModel();

            using (var connection = OpenDBConnection())
            {
                var query = $"SELECT TOP 1 * FROM Redemption WHERE User_id = '{userId}' AND Voucher_ticket = '{voucherTicket}'";
                var redemptions = connection.Query<RedemptionModel>(query);
                connection.Close();

                if (redemptions?.Count() > 0)
                {
                    redemption = redemptions.FirstOrDefault();
                }
            }

            // ONLY FOR TESTING PURPOSE.
            //redemption = new Redemption
            //{
            //    Redemption_id = 123456,
            //    User_id = userId,
            //    Voucher_ticket = voucherTicket,
            //    Redemption_value = 25,
            //    Redemption_value_currency = "$",
            //    Redemption_date = DateTime.Now
            //}

            return redemption;
        }

        #region PRIVATE METHODS

        /// <summary>
        /// Creates database connection and returns connection object <see cref="NpgsqlConnection"/>.
        /// </summary>
        /// <returns><see cref="NpgsqlConnection"/></returns>
        private NpgsqlConnection OpenDBConnection()
        {
            var connectionString = _configuration.GetValue<string>("DBConnectionString");
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            return connection;
        }

        #endregion
    }
}
