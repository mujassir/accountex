using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;

namespace AccountEx.DbMapping
{
    public class WidgetObject : Widget
    {
        public List<WidgetParameter> Parameters { get; set; }

        public WidgetObject()
        {
            Parameters = new List<WidgetParameter>();
        }
        public WidgetObject(Widget w) : this(w, new List<WidgetParameter>()) { }

        public WidgetObject(Widget w, List<WidgetParameter> sqlParams)
        {
            this.Id = w.Id;
            //this.CompanyId = rpt.CompanyId;
            this.Description = w.Description;
            this.IsExecuted = w.IsExecuted;
            this.IsExternal = w.IsExternal;
            this.JsFunction = w.JsFunction;
            this.Location = w.Location;
            this.Name = w.Name;
            this.SequenceNumber = w.SequenceNumber;
            this.ShowFirstRow = w.ShowFirstRow;
            this.ShowFooter = w.ShowFooter;
            this.CategoryIndex = w.CategoryIndex;
            this.LegendStart = w.LegendStart;
            this.LegendEnd = w.LegendEnd;
            this.GraphEnabled = w.GraphEnabled;
            this.GraphEnabled = w.GraphEnabled;
            this.GraphFucntion = w.GraphFucntion;
            this.DirectRun = w.DirectRun;
            this.SkipLegends = w.SkipLegends;
            this.IsTileWidget = w.IsTileWidget;

            Parameters = sqlParams;
        }
    }
}
