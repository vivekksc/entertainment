using microservices.entertainment.Data.Contracts;
using microservices.entertainment.Data;

namespace microservices.entertainment
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IVoucherDataManager, VoucherDataManager>();
        }
    }
}
