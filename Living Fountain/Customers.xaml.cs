using Living_Fountain.Helpers;
using Living_Fountain.Models;
using Microsoft.EntityFrameworkCore;
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
    public partial class Customers : Page
    {
        public Customers()
        {
            InitializeComponent();
            GetLastOrders();
        }

        private void GetLastOrders()
        {
            using (var dc = new living_fountainContext())
            {
                var customerLastOrders = dc.customers
                    .Select(customer => new CustomerLastOrder
                    {
                        Customer = customer,
                        LastOrderDate = dc.orders
                            .Where(o => o.block == customer.block && o.lot == customer.lot && o.phase == customer.phase)
                            .Max(o => (DateOnly?) o.date)
                    })
                    .ToList();

                customerRecords.ItemsSource = customerLastOrders;
            }
        }

        private void CustomerRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (customerRecords.SelectedItem is CustomerLastOrder selectedRecord)
            {
                var detailPage = new CustomerHistory(selectedRecord.Customer);

                // Access the parent Frame (MainFrame)
                Frame parentFrame = NavigationHelper.FindParentFrame(this);

                if (parentFrame != null)
                {
                    // Navigate MainFrame to Edit_Order.xaml with the id parameter
                    parentFrame.NavigationService.Navigate(detailPage);
                }
            }
        }
    }
}
