namespace microservices.entertainment.Models
{
    public class Voucher
    {
        public Asset Asset { get; set; }
        public string Code { get; set; }
        public bool External { get; set; }
        public string Instructions { get; set; }
        public string Uri { get; set; }
    }

    public class Asset
    {
        public string AssetType { get; set; }
        public string AssetUri { get; set; }
    }
}
