namespace microservices.entertainment.Models
{
    public class ApiResponse
    {
        public ICollection<string> Messages { get; set; }
        public dynamic Result { get; set; }
    }
}
