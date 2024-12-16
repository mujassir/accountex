namespace AccountEx.Common
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public object Data { get; set; }
        public object Logs { get; set; }
    }
}
