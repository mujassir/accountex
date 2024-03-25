using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class Employee : BaseEntity
    {
        
        public int AccountId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string CNIC { get; set; }
        public decimal BasicSalary { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string BloodGroup { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountTitle { get; set; }
        public string Address { get; set; }
        public string PermanentAddress { get; set; }
        public string Qualification { get; set; }
        public string Designation { get; set; }
        public string PictureUrl { get; set; }
        public string Reference { get; set; }
        public string PassportNumber { get; set; }
        public string NTN { get; set; }
        public string DeploymentStatus { get; set; }
        public Nullable<DateTime> DOB { get; set; }
        public Nullable<DateTime> DOJ { get; set; }
       
    }
}
