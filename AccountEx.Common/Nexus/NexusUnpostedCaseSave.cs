using System;

namespace AccountEx.Common.Nexus
{
    public class NexusUnpostedCaseSave
    {
        public long CaseId { get; set; }
        public string CaseNumber { get; set; }
        public Nullable<DateTime> RegistrationDate { get; set; }
        public long CaseDetailId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string PatientName { get; set; }
        public string Relationship { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string TestName { get; set; }
        public int TestId { get; set; }
        public decimal Price { get; set; }
    }
}
