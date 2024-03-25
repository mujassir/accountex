using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class Project : BaseEntity
    {
        public int FiscalId { get; set; }
        public int AccountId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string AccountTitle { get; set; }
        public string Employees { get; set; }
        public string PONumber { get; set; }
        public string PictureUrl { get; set; }
        public Nullable<DateTime> POIssueDate { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public decimal GrossCost { get; set; }
        public decimal GST { get; set; }
        public decimal GSTPercent { get; set; }
        public decimal WHT { get; set; }
        public decimal WHTPercent { get; set; }
        public decimal Miscellaneous { get; set; }
        public decimal NetCost { get; set; }
        public string WorkScope { get; set; }
        public decimal Engineering_Planned { get; set; }
        public decimal Engineering_Actual { get; set; }
        public decimal Planning_Planned { get; set; }
        public decimal Planning_Actual { get; set; }
        public decimal Procurement_Planned { get; set; }
        public decimal Procurement_Actual { get; set; }
        public decimal Construction_Planned { get; set; }
        public decimal Construction_Actual { get; set; }
        public decimal Commissioning_Planned { get; set; }
        public decimal Commissioning_Actual { get; set; }
        public decimal HandOver_Planned { get; set; }
        public decimal HandOver_Actual { get; set; }
        public decimal Manpower_Planned { get; set; }
        public decimal Manpower_Actual { get; set; }
        public decimal DirectMaterial_Planned { get; set; }
        public decimal DirectMaterial_Actual { get; set; }
        public decimal Transportation_Planned { get; set; }
        public decimal Transportation_Actual { get; set; }
        public decimal ToolnEquipment_Planned { get; set; }
        public decimal ToolnEquipment_Actual { get; set; }
        public decimal SiteOfficeCost_Planned { get; set; }
        public decimal SiteOfficeCost_Actual { get; set; }
        public decimal Misc_Planned { get; set; }
        public decimal Misc_Actual { get; set; }
    }
}
