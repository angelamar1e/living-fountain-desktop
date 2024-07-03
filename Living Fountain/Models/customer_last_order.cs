using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Living_Fountain.Models
{
    public class customer_last_order
    {
        public customer Customer { get; set; }
        public DateOnly? LastOrderDate { get; set; }
    }

}
