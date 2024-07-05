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
using Living_Fountain.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Living_Fountain
{
    /// <summary>
    /// Interaction logic for CustomerHistory.xaml
    /// </summary>
    public partial class CustomerHistory : Page
    {
        private List<order> OrderHistory { get; set; }
        public CustomerHistory(customer customer)
        {
            InitializeComponent();

            DisplayAddress(customer);
            GetOrderHistory(customer);
            CountRecords();
        }

        private void DisplayAddress(customer customer)
        {
            blockField.Text = "Block " + customer.block.ToString();
            lotField.Text = "Lot " + customer.lot.ToString();
            phaseField.Text = "Phase " +
                "" + customer.phase.ToString();
        }

        private void GetOrderHistory(customer customer)
        {
            using (var dc = new living_fountainContext())
            {
                var orders = dc.orders
                               .Include(o => o.product_codeNavigation) // Include related product
                               .Include(o => o.deliverer) // Include related deliverer (employee)
                               .Include(o => o.statusNavigation) // Include related status
                               .Where(o => o.block == customer.block && o.lot == customer.lot && o.phase == customer.phase)
                               .ToList();

                var ordersSum = orders.Sum(o => o.price);
                totalAmount.Text = "₱" + ordersSum.ToString();

                OrderHistory = orders;
            }

            orderHistory.ItemsSource = OrderHistory;
        }
        private void CountRecords()
        {
            if (OrderHistory.Count == 0)
            {
                orderHistory.Visibility = Visibility.Hidden;
                totalAmountContainer.Visibility = Visibility.Hidden;
                noRecords.Visibility = Visibility.Visible;
            }
        }

        private void OnDataGridLoaded(object sender, RoutedEventArgs e)
        {
            // AdjustTotalAmountPosition();
        }

        private void OnDataGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // AdjustTotalAmountPosition();
        }

        private void AdjustTotalAmountPosition()
        {
            
        }
    }
}
