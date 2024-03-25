using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Pharmaceutical
{
   public class Doctor:BaseEntity
    {
       public string Name { get; set; }
       public string Speciality { get; set; }
       public string AreaManager { get; set; }
       public string Designation { get; set; }
       public string Timing { get; set; }
       public string SaleRep { get; set; }
       public string ClinicName { get; set; }
       public string Address { get; set; }
       public string Medecine { get; set; }
       public string PhoneNumber { get; set; }
       public string City { get; set; }
       public decimal AmmountAllocation { get; set; }
       public decimal EstimateSale { get; set; }
       public string MedicalStore { get; set; }
       public string Remark { get; set; }
    }
}
