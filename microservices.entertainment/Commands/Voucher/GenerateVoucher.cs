using Entertainment.Microservices.Responses;
using MediatR;
using FluentValidation;
using microservices.entertainment.Services.Interfaces;
using microservices.entertainment.Validation;

namespace microservices.entertainment.Consumer.Commands.Voucher
{
    public class GenerateVoucher
    {
        public class Command : IRequest<Envelope<ResponseModel>>
        {
            public Guid Token { get; set; }
            public Guid UserId { get; set; }
            public string Type { get; set; }
        }

        public class Validator : BaseValidator<Command>
        {
            public Validator()
            {
                RuleFor(v => v.Token).NotNull().NotEmpty().NotEqual(Guid.Empty);
                RuleFor(v => v.UserId).NotNull().NotEmpty().NotEqual(Guid.Empty);
                RuleFor(v => v.Type).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Envelope<ResponseModel>>
        {
            private readonly IVoucherService _voucherService;

            public Handler(IVoucherService userService)
            {
                _voucherService = userService;
            }

            public async Task<Envelope<ResponseModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    string imageUrl = string.Empty;
                    var getRedemptionResponse = await _voucherService.GetRedemptionAsync(request.Token, request.UserId).ConfigureAwait(false);

                    if (getRedemptionResponse.StatusCode == System.Net.HttpStatusCode.Found
                        && getRedemptionResponse.Redemption != null)
                    {
                        var redemption = getRedemptionResponse.Redemption;
                        var response = await _voucherService.GenerateVoucherImageAsync(redemption).ConfigureAwait(false);
                        if (response.IsSuccess)
                            imageUrl = response.VoucherImageUrl;
                    }
                    else
                    {
                        return Envelope<ResponseModel>.Fail(getRedemptionResponse.Messages.FirstOrDefault());
                    }

                    return !string.IsNullOrWhiteSpace(imageUrl)
                        ? Envelope<ResponseModel>.Created(imageUrl)
                        : Envelope<ResponseModel>.Error(string.Empty);
                }
                catch (Exception ex)
                {
                    return Envelope<ResponseModel>.Fail(ex.Message);
                }
            }
        }
    }
}
