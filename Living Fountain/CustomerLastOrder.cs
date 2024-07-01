using Living_Fountain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Living_Fountain
{
    public class CustomerLastOrder
    {
        public customer Customer { get; set; }
        public DateOnly? LastOrderDate { get; set; }
    }

}
