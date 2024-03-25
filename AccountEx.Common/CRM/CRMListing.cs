using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.CRM
{
    public class ListingBase
    {
        public int TotalRows { get; set; }
        public int TotalFilterRows { get; set; }

    }

    public class CRMCustomerListing : ListingBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string ClusterType { get; set; }
        public string Active { get; set; }

        public string SalePerson { get; set; }
        public string Category { get; set; }
        public string ShippingAddress { get; set; }
        public string Industry { get; set; }
        public string CellNo { get; set; }
        public string Email { get; set; }
        public string Web { get; set; }


    }

    public class CRMInvoiceListing : ListingBase
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public int CustomerId { get; set; }
        public string Customer { get; set; }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public Nullable<int> OGPNo { get; set; }

    }
}
