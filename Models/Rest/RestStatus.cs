namespace ASP_32.Models.Rest
{
    public class RestStatus
    {
        public String Phrase { get; set; } = "Ok";
        public int Code { get; set; } = 200;
        public bool IsOk { get; set; } = true;

        public static readonly RestStatus Status400 = new() { IsOk = false, Code = 400, Phrase = "Bad Request" };
        public static readonly RestStatus Status401 = new() { IsOk = false, Code = 401, Phrase = "Unauthorized" };
    }
}
