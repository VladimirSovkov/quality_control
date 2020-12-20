using System.Runtime.Serialization;

namespace lab9.Models
{
    [DataContract]
    public class Product
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }


        [DataMember(Name = "category_id")]
        public int Category_id { get; set; }


        [DataMember(Name = "title")]
        public string Title { get; set; }


        [DataMember(Name = "alias")]
        public string Alias { get; set; }


        [DataMember(Name = "content")]
        public string Content { get; set; }


        [DataMember(Name = "price")]
        public double Price { get; set; }


        [DataMember(Name = "old_price")]
        public double Old_price { get; set; }


        [DataMember(Name = "status")]
        public int Status { get; set; }


        [DataMember(Name = "keywords")]
        public string Keywords { get; set; }


        [DataMember(Name = "description")]
        public string Description { get; set; }


        [DataMember(Name = "img")]
        public string Img { get; set; }


        [DataMember(Name = "hit")]
        public int Hit { get; set; }


        [DataMember(Name = "cat")]
        public string Cat { get; set; }

    }
}
