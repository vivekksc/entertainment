using microservices.entertainment.Data.DbModels;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace microservices.entertainment.Models
{
    public class RedemptionModel
    {
        public int RedemptionId { get; set; }
        public Guid UserId { get; set; }
        public int MembershipId { get; set; }
        public Guid OfferId { get; set; }
        public DateTime RedemptionDate { get; set; }
        public Guid VoucherTicket { get; set; }
        public DateTime VoucherTicketUsedOn { get; set; }
        public string CouponTypeCreated { get; set; }
        public float RedemptionValue { get; set; }
        public string RedemptionValueCurrency { get; set; }
        public string RedeemedVia { get; set; } //[note: "MOBILE / SITE / API"]
        public bool RedemptionCancelled { get; set; }
        public DateTime RedemptionCancelledDate { get; set; }
        public string RedemptionCancelledReason { get; set; }

        public static RedemptionModel GetRedemptionModel(Redemption redemption)
        {
            return new()
                {
                    RedemptionId = redemption.RedemptionId,
                    UserId = redemption.UserId,
                    MembershipId = redemption.MembershipId,
                    OfferId = redemption.OfferId,
                    RedemptionDate = redemption.RedemptionDate,
                    VoucherTicket = redemption.VoucherTicket,
                    VoucherTicketUsedOn = redemption.VoucherTicketUsedOn,
                    CouponTypeCreated = redemption.CouponTypeCreated,
                    RedemptionValue = redemption.RedemptionValue,
                    RedemptionValueCurrency = redemption.RedemptionValueCurrency,
                    RedeemedVia = redemption.RedeemedVia,
                    RedemptionCancelled = redemption.RedemptionCancelled,
                    RedemptionCancelledDate = redemption.RedemptionCancelledDate,
                    RedemptionCancelledReason = redemption.RedemptionCancelledReason
                };
        }
    }
}