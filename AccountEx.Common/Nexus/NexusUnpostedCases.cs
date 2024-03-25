using System;

namespace AccountEx.Common.Nexus
{
    public class NexusUnpostedCases
    {
        public long CaseId { get; set; }
        public long CaseDetailId { get; set; }
        public string CaseNumber { get; set; }
        public string PatientName { get; set; }
        public System.DateTime RegistrationDate { get; set; }
        public Nullable<int> ReferenceID { get; set; }
        public string ReferenceName { get; set; }
        public string ConsultantName { get; set; }
        public string TestName { get; set; }
        public decimal Rate { get; set; }
    }

    public class NexusTestWithPrice
    {
        public int DepartmentAccountId { get; set; }
        public int TestId { get; set; }
        public System.DateTime WithEffectFrom { get; set; }
        public string TestName { get; set; }
        public decimal Rate { get; set; }
    }

    public class NexusInvoicePrinting
    {
       
        public string CaseNumber { get; set; }
        public string Department { get; set; }
        public string PatientName { get; set; }
        public System.DateTime Date { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public string Relationship { get; set; }
        public string OtherInfo { get; set; }
        public string TestName { get; set; }
        public decimal Price { get; set; }
    }
}
