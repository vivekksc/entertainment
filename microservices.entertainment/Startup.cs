using Entertainment.Microservices.Middleware;
using Entertainment.Microservices.Responses;
using Entertainment.Microservices.Security;
using FluentValidation.AspNetCore;
using MediatR;
using microservices.entertainment.Consumer.Commands.Voucher;
using microservices.entertainment.Data;
using microservices.entertainment.Data.Contracts;
using microservices.entertainment.Data.DbContexts;
using microservices.entertainment.Repositories;
using microservices.entertainment.Repositories.Interfaces;
using microservices.entertainment.Services;
using microservices.entertainment.Services.Interfaces;
using microservices.entertainment.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace microservices.entertainment
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(o => { o.JsonSerializerOptions.PropertyNamingPolicy = null; });
            services.ConfigureResponses();

            services.AddDbContext<NpgsqlDbContext>(options =>
                options.UseNpgsql(Configuration["ConnectionStrings:AuthenticationDB"]));

            services.AddScoped<IVoucherDataManager, VoucherDataManager>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();

            services.AddScoped<IVoucherService, VoucherService>();

            //Middleware
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials().Build()));
            services.AddMediatR(typeof(GenerateVoucher));
            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(x => x.FullName);
                options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "ENT Voucher",
                        Description = "Solution for ENT Voucher",
                        Version = "v1"
                    });
            });

            services.AddJWTAuthentication(Configuration);
            services.ConfigureEntertainmentSecurity(Configuration);

            services.AddMvc().AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssembly(Assembly.Load("Authentication"));
            });

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment() || env.IsUAT())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });
            }

            app.UseEntertainmentMiddleware();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
            app.UseHealthChecks("/health");

            loggerFactory.AddAWSProvider(Configuration.GetAWSLoggingConfigSection());
        }
    }
}
