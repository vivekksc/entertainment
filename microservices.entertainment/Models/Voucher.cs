namespace microservices.entertainment.Models
{
    public class Voucher
    {
        public Guid Token { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; }
    }
}
