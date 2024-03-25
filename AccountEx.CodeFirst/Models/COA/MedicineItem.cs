using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class MedicineItem : BaseEntity
    {
        public string EntryNo { get; set; }
        public string Name { get; set; }
        public string BNo { get; set; }
        public Nullable<DateTime> PurchaseDate { get; set; }
        public Nullable<DateTime> MFDDate { get; set; }
        public Nullable<DateTime> ExpDate { get; set; }
        public string Rate { get; set; }
        public Nullable<int> PackQuantity { get; set; }
        public Nullable<int> InPackQuantity { get; set; }
        public Nullable<int> MedicineQuantity { get; set; }
        public Nullable<int> Total { get; set; }
        public Nullable<int> MRP { get; set; }
        public string StoragePlace { get; set; }
        public string PurchaseFrom { get; set; }
        public string Remarks { get; set; }
        public string Symptoms { get; set; }
    }
}
