using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountEx.CodeFirst.Models
{
    [Table("VehicleStatuses")]
    public partial class VehicleStatuse : BaseEntity
    {
        public string Name { get; set; }
        public string SerialNo { get; set; }
        public bool IsFinal { get; set; }
    }
}
