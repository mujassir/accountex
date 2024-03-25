using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
    public partial class Lead : BaseEntity
    {

        public Lead()
        {
            LeadExpectedItems = new HashSet<LeadExpectedItem>();
            LeadConcernedItems = new HashSet<LeadConcernedItem>();
        }   
 
        public string LeadOwner { get; set; }
        public int LeadOwnerId { get; set; }
        public int FiscalId { get; set; }
        public int LeadNo { get; set; }
        public string Customer { get; set; }
        public string Company { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string Website { get; set; }
        public int LeadSourceId { get; set; }
        public string LeadSourceName { get; set; }
        public string LeadStatus { get; set; }
        public int IndustryId { get; set; }
        public string IndustryName { get; set; }
        public int NoOfEmployees { get; set; }
        public decimal AnnualRevenue { get; set; }
        public string Rating { get; set; }
        public String Sector { get; set; }       
        public bool IsEmailOption { get; set; }
        public string SkypeId { get; set; }
        public string SecondaryEmail { get; set; }
        public string TwitterId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string PreviousCompany { get; set; }
        public string MaturityLevel { get; set; }
        public string Remarks { get; set; }
        public decimal ExpectedVolume { get; set; }
     
        
        
        public string ContactPerson1 { get; set; }
        public string ContactPerson1Phone { get; set; }
        public string ContactPerson1Cell { get; set; }
        public string ContactPerson1Email { get; set; }
        public string ContactPerson2 { get; set; }
        public string ContactPerson2Phone { get; set; }
        public string ContactPerson2Cell { get; set; }
        public string ContactPerson2Email { get; set; }
        public string Reference { get; set; }
        public string ExpectedMaturityLevel { get; set; }
        public string ExpectedSaleVolume { get; set; }
        public string CustomerRemarks { get; set; }
        public string Instructions { get; set; }

        public Nullable<DateTime> Date { get; set; }

        public virtual ICollection<LeadExpectedItem> LeadExpectedItems { get; set; }
        public virtual ICollection<LeadConcernedItem> LeadConcernedItems { get; set; }
    }
}
