using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class Transporter : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Nullable<int> CityId { get; set; }
        public string City { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
    }
}
