namespace microservices.entertainment.Utils
{
    public static class HostingEnvironmentExtensions
    {
        public const string UATEnvironment = "uat";

        public static bool IsUAT(this IHostEnvironment hostingEnvironment)
        {
            return hostingEnvironment.IsEnvironment(UATEnvironment);
        }
    }
}
