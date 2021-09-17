using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementWebApplication.Models
{
    public partial class Admin
    {
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public bool? Edit { get; set; }
        public bool? AddAdmin { get; set; }
    }
}
