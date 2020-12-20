namespace lab9.Models
{
    public class Response <T>
    {
        public T Items { get; set; }
        public string StatusCode { get; set; }
    }
}
