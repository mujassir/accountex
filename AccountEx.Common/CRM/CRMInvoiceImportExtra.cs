using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.CRM
{
    public class CRMImportItems
    {
        public int InvoiceNumber { get; set; }
        public int? OGP { get; set; }
        public string Date { get; set; }
        public string Customer { get; set; }
        public string InvoiceType { get; set; }
        public string Product { get; set; }
        public string Currency { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Total { get; set; }
        public decimal Tax { get; set; }
    }
    public class CRMImportExtra
    {
        public CRMImportExtra()
        {
            Invoices = new List<CRMImportItems>();
        }
        public List<CRMImportItems> Invoices { get; set; }
    }

    public class CRMIGRNImportContainerExtra
    {
       public List<CRMIGRNImportExtra> IGRN { get; set; }
    }
    
    public class CRMIGRNImportExtra
    {
        public string PINo { get; set; }
        public string PIDate { get; set; }
        public string Customer { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string BLBRNo { get; set; }
        public string BLRRDate { get; set; }
        public string Product { get; set; }
        public string Currency { get; set; }
        public decimal Quantity { get; set; }
        public decimal ExRate { get; set; }
        public decimal LCRate { get; set; }
        public decimal LCValue { get; set; }
    }

}
