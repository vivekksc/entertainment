using System.Collections.Generic;

namespace microservices.entertainment.Utils
{
    public static class Constants
    {
        public const string VOUCHER_IMAGE_GENERATION_FAILED = "Unable to generate voucher image.";
        public const string REDEMPTION_NOTFOUND = "No redemption(s) found for the given criteria.";

        public readonly static List<string> VOUCHER_TYPES = new() { "IN-STORE", "CLICK-THRU", "PRINT" };
    }
}
