using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
    public class Dashboard
    {
        public Dashboard()
        {
            Widgets = new List<Widget>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public List<Widget> Widgets { get; set; }
    }
}
