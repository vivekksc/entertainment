using Entertainment.Microservices.Responses;
using MediatR;
using FluentValidation;
using microservices.entertainment.Services.Interfaces;

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

        public class Validator //: BaseValidator<Command>
        {
            public Validator()
            {
                //RuleFor(v => v.Email).NotNull().NotEmpty().EmailAddress();
                //RuleFor(v => v.Password).NotNull().NotEmpty();
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

                    if (getRedemptionResponse.StatusCode == System.Net.HttpStatusCode.Found)
                    {
                        var redemption = getRedemptionResponse?.Redemption;

                        imageUrl = redemption != null
                            ? await _voucherService.GenerateVoucherImageAsync(redemption).ConfigureAwait(false)
                            : string.Empty;
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
