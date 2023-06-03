using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace GridViewWebApp.Models
{
    public class Customers_View
    {
        [HiddenInput]
        public int CustomerID { get; set; }
        public int StatusID { get; set; }

        [DisplayName("First name")]
        public string FName { get; set; }

        [DisplayName("Last name")]
        public string LName { get; set; }

        [DisplayName("Status")]
        public string StatusName { get; set; }
        public string Picture { get; set; }
    }
}
