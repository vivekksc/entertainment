namespace microservices.entertainment.Models
{
    public class Redemption
    {
        public int Redemption_id { get; set; }
        public Guid User_id { get; set; }
        public int Membership_id { get; set; }
        public Guid Offer_id { get; set; }
        public DateTime Redemption_date { get; set; }
        public Guid Voucher_ticket { get; set; }
        public DateTime Voucher_ticket_used_on { get; set; }
        public string Coupon_type_created { get; set; }
        public float Redemption_value { get; set; }
        public string Redemption_value_currency { get; set; }
        public string Redeemed_via { get; set; } //[note: "MOBILE / SITE / API"]
        public bool Redemption_cancelled { get; set; }
        public DateTime Redemption_cancelled_date { get; set; }
        public string Redemption_cancelled_reason { get; set; }
    }
}
