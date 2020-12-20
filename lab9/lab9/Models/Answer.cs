using System.Runtime.Serialization;

namespace lab9.Models
{
    [DataContract]
    public class Answer
    {
        [DataMember(Name="status")]
        public int Status { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }
    }
}
