//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AccountEx.CodeFirst.Models
{
    using System;
    using System.Collections.Generic;

    public partial class WidgetColumn : GlobalBaseEntity
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public int WidgetId { get; set; }
        public string Name { get; set; }
        public string HeaderText { get; set; }
        public Nullable<bool> ShowSum { get; set; }
        public string FooterText { get; set; }
        public Nullable<int> Colspan { get; set; }
        public int Precision { get; set; }
        public byte FooterFormatingType { get; set; }
        public byte BodyFormatingType { get; set; }
    }
}
