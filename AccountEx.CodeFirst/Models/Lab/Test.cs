using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AccountEx.CodeFirst.Models.Lab
{
    public partial class Test : BaseEntity
    {
        public Test()
        {
            this.TestParameters = new HashSet<TestParameter>();
        }
        public string Name { get; set; }
        public Nullable<int> MainCategoryId { get; set; }
        public Nullable<int> SubCategoryId { get; set; }
        public Nullable<int> DepartmentAccountId { get; set; }

        public decimal ProposedRate { get; set; }
        public decimal ApprovedRate { get; set; }
        public virtual ICollection<TestParameter> TestParameters { get; set; }
    }
}
