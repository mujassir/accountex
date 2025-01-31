using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class DoctorActivity : BaseEntity
    {
        public int Product { get; set; }
        public string Code { get; set; }
        public string Tag { get; set; }
        public string Date { get; set; }
        public string CattleStatus { get; set; }
        public string PregnanceyStatus { get; set; }
        public string BreedingStatus { get; set; }
        public string DaysInMilk { get; set; }
        public string BullName { get; set; }
        public string DaysPreg { get; set; }
        public string MonthPreg { get; set; }
        public string Milk { get; set; }
    }
}
