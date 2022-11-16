using Entertainment.Microservices.Responses;
using MediatR;
using FluentValidation;
using microservices.entertainment.Services.Interfaces;
using microservices.entertainment.Validation;
using System.Text.Json;
using Newtonsoft.Json;
using microservices.entertainment.Utils;

namespace microservices.entertainment.Consumer.Commands.Voucher
{
    public class GenerateVoucher
    {
        public class Command : IRequest<Envelope<ResponseModel>>
        {
            public Guid Token { get; set; }
            public string Type { get; set; }
        }

        public class Validator : BaseValidator<Command>
        {
            public Validator()
            {
                RuleFor(v => v.Token).NotNull().NotEmpty().NotEqual(Guid.Empty);
                RuleFor(v => v.Type).NotNull().NotEmpty();
                RuleFor(v => v.Type).Must(type => Constants.VOUCHER_TYPES.Contains(type));
            }
        }

        public class Handler : IRequestHandler<Command, Envelope<ResponseModel>>
        {
            private readonly IVoucherService _voucherService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            public Handler(IVoucherService userService, IHttpContextAccessor httpContextAccessor)
            {
                _voucherService = userService;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Envelope<ResponseModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Guid userId = Guid.Parse(_httpContextAccessor.HttpContext.User.ToString());
                    var getRedemptionResponse = await _voucherService.GetRedemptionAsync(request.Token, userId).ConfigureAwait(false);

                    if (getRedemptionResponse.StatusCode == System.Net.HttpStatusCode.Found
                        && getRedemptionResponse.Redemption != null)
                    {
                        var redemption = getRedemptionResponse.Redemption;
                        var response = await _voucherService.GenerateVoucherImageAsync(redemption).ConfigureAwait(false);

                        return response.IsSuccess
                            ? Envelope<ResponseModel>.Created(response.VoucherImageUrl)
                            : Envelope<ResponseModel>.Error(JsonConvert.SerializeObject(response.Messages));
                    }
                    else
                    {
                        return Envelope<ResponseModel>.Fail(getRedemptionResponse.Messages.FirstOrDefault());
                    }
                }
                catch (Exception ex)
                {
                    return Envelope<ResponseModel>.Fail(ex.Message);
                }
            }
        }
    }
}
