//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AccountEx.CodeFirst.Models.Nexus
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    public partial class Nexus_CaseDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }
        public long CaseID { get; set; }
        public int TestID { get; set; }
        public string TestName { get; set; }
        public decimal Rate { get; set; }
        public short TestStatus { get; set; }
        public int ConductedAt { get; set; }
        public System.DateTime ReportingDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public bool Status { get; set; }
        public short TemplateID { get; set; }
        public string Comments { get; set; }
        public bool IsDelayed { get; set; }
        public bool IsPosted { get; set; }
        public string ConductedBy { get; set; }
        public string ApprovedBy { get; set; }
    
    }
}