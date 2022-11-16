using Entertainment.Microservices.Responses;
using MediatR;
using microservices.entertainment.Data.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static microservices.entertainment.Consumer.Commands.Voucher.GenerateVoucher;

namespace microservices.entertainment.Controllers
{
    [Authorize(AuthenticationSchemes = "JwtAuth")]
    [ApiController]
    [Route("consumer")]
    public class ConsumerController : ControllerBase
    {
        private readonly string contentRootPath;
        private readonly IConfiguration _configuration;
        private readonly IVoucherDataManager _data;
        private readonly IMediator _mediator;
        private readonly IActionResultFactory _actionResultFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConsumerController(IMediator mediator, IActionResultFactory actionResultFactory, IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment environment, IConfiguration configuration, IVoucherDataManager data)
        {
            _mediator = mediator;
            _actionResultFactory = actionResultFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _data = data;
            contentRootPath = environment.ContentRootPath;
        }

        /// <summary>
        /// Get voucher details.
        /// </summary>
        /// <param name="token">Voucher Access Token from Redemption call.</param>
        /// <param name="type">Voucher type.</param>
        /// <returns></returns>
        [HttpGet("voucher")]
        public async Task<IActionResult> GetVoucher(Guid token, string type)
        {
            var command = new Command { Token = token };

            var response = await _mediator.Send(command).ConfigureAwait(false);

            return _actionResultFactory.CreateResultFromResponseModel(response);
        }

    }
}