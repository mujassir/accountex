using System.ComponentModel.DataAnnotations.Schema;
namespace AccountEx.CodeFirst.Models
{
    [Table("Manufacturers")]
    public partial class Manufacturer : BaseEntity
    {

        public string Name { get; set; }

    }
}