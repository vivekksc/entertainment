using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace microservices.entertainment.Data.DbModels
{
    [Table("redemptions", Schema = "authentication")]
    public class Redemption
    {
        [Key]
        [Column("redemption_id")]
        public int RedemptionId { get; set; }
        
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("membership_id")]
        public int MembershipId { get; set; }

        [Column("offer_id")]
        public Guid OfferId { get; set; }
        
        [Column("redemption_date")]
        public DateTime RedemptionDate { get; set; }
        
        [Column("voucher_ticket")]
        public Guid VoucherTicket { get; set; }
        
        [Column("Voucher_ticket_used_on")]
        public DateTime VoucherTicketUsedOn { get; set; }

        [Column("coupon_type_created")]
        public string CouponTypeCreated { get; set; }

        [Column("redemption_value")]
        public float RedemptionValue { get; set; }

        [Column("redemption_value_currency")]
        public string RedemptionValueCurrency { get; set; }

        [Column("redeemed_via")]
        public string RedeemedVia { get; set; } //[note: "MOBILE / SITE / API"]

        [Column("Redemption_cancelled")]
        public bool RedemptionCancelled { get; set; }

        [Column("redemption_cancelled_date")]
        public DateTime RedemptionCancelledDate { get; set; }

        [Column("redemption_cancelled_reason")]
        public string RedemptionCancelledReason { get; set; }
    }
}
