using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.CRM
{
    public class CRMCustomerProjectExtra
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Product { get; set; }
        public int ProductId { get; set; }

    }
    public class CRMCustomerProjectWithPMCExtra
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        public string Product { get; set; }
        public string Division { get; set; }

        public Nullable<int> ActualProductId { get; set; }
        public string ActualProduct { get; set; }
        public string ActualProductDivision { get; set; }



    }
    public class CRMProjectWithPMCExtra
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PMCItemId { get; set; }

    }
}
