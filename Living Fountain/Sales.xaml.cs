using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Living_Fountain
{
    /// <summary>
    /// Interaction logic for Sales.xaml
    /// </summary>
    public partial class Sales : Page
    {
        public Sales()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            string query = "SELECT block, lot, phase, product_desc as 'product', quantity, o.price, employee_name as 'deliverer', s.status_desc as 'status' " +
                "FROM living_fountain.orders as o " +
                "LEFT JOIN living_fountain.products as p ON o.product_code = p.code " +
                "LEFT JOIN living_fountain.employees as e ON o.deliverer_id = e.id " +
                "LEFT JOIN living_fountain.order_status as s ON o.status = s.code";

            DataGridHelper.LoadData(OrderRecords, query, "Block", "Lot", "Phase", "Product", "Quantity", "Price", "Deliverer", "Status");
        }
    }
}
