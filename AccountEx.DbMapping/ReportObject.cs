using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountEx.DbMapping
{
    public class ReportObject : Report
    {
        public List<ReportParameter> Parameters { get; set; }

        public ReportObject()
        {
            Parameters = new List<ReportParameter>();
        }
        public ReportObject(Report rpt) : this(rpt, new List<ReportParameter>()) { }

        public ReportObject(Report rpt, List<ReportParameter> sqlParams)
        {
            this.Id = rpt.Id;
            //this.CompanyId = rpt.CompanyId;
            this.Description = rpt.Description;
            this.IsDeleted = rpt.IsDeleted;
            this.IsExecuted = rpt.IsExecuted;
            this.IsExternal = rpt.IsExternal;
            this.JsFunction = rpt.JsFunction;
            this.Location = rpt.Location;
            this.Name = rpt.Name;
            this.SequenceNumber = rpt.SequenceNumber;
            this.ShowFirstRow = rpt.ShowFirstRow;
            this.ShowFooter = rpt.ShowFooter;
            this.CategoryIndex=rpt.CategoryIndex;
            this.LegendStart=rpt.LegendStart;
            this.LegendEnd = rpt.LegendEnd;
            this.GraphEnabled = rpt.GraphEnabled;
            this.GraphEnabled = rpt.GraphEnabled;
            this.GraphFucntion = rpt.GraphFucntion;
            this.DirectRun = rpt.DirectRun;
            this.SkipLegends = rpt.SkipLegends;


            Parameters = sqlParams;
        }
    }
}
