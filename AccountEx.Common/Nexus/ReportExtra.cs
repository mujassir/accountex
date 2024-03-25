using System;

namespace AccountEx.Common.Nexus
{
    public class NexusDepartmentSummary
    {
        public int DepartmentId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
    }
    public class NexusDepartmentSummaryByPatient
    {
        public int DepartmentId { get; set; }
        public int TotalPatient { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
    }
    public class NexusBillingByPatient
    {
        public DateTime Date { get; set; }
        public string EmployeeName { get; set; }
        public string PatientName { get; set; }
        public string RefNo { get; set; }
        public decimal Amount { get; set; }
    }
    public class NexusReferralSummary
    {
        public DateTime Date { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public int TotalPatient { get; set; }

    }

    public class NexusReceivablesSummary
    {
        public int DepartmentId { get; set; }
        public decimal BillingAmount { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal NetReceived { get; set; }
    }
    public class NexusReceivablesSummaryByDepartment
    {
        public int DepartmentId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BillingAmount { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal NetReceived { get; set; }
        public int TotalPatient { get; set; }
    }
    public class NexusMonthlyReceiptSummary
    {
        public int DepartmentId { get; set; }
        public DateTime Date { get; set; }
        public string ChequeNumber { get; set; }
        public DateTime? ChqDate { get; set; }
        public decimal Amount { get; set; }
    }
}
