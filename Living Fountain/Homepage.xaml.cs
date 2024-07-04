using Living_Fountain.Helpers;
using Living_Fountain.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Living_Fountain
{
    /// <summary>
    /// Interaction logic for Homepage.xaml
    /// </summary>
    public partial class Homepage : Page
    {
        private DateOnly today;
        private DispatcherTimer timer;
        private List<order> UnpaidDeliveries;
        private List<order> UndeliveredOrders;

        public Homepage()
        {
            InitializeComponent();

            // set current date
            today = OrderHelper.GetCurrentDate();
            
            // running clock
            InitializeTimer();

            // displays today's revenue and qty of products sold
            GetRevenueToday();
            GetProductsSoldToday();
            
            // adds check box column to mark deliveries as paid
            AddCheckBoxCol("Mark as Paid",unpaidDeliveries);

            // adds check box column to mark paid orders as delivered
            AddCheckBoxCol("Mark as Delivered", undeliveredOrders);

            // bind unpaid and undelivered orders to data grids
            GetUnpaid();
            GetUndelivered();
            CountRecords(); // hides data grids if no records found
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Update every second
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            currentDateTime.Content = DateTime.Now.ToString("MMMM dd, yyyy HH:mm:ss");
        }

        private void GetRevenueToday()
        {
            using (var dc = new living_fountainContext())
            {
                var revenueToday = dc.orders
                                .Where(o => o.date == today)
                                .Sum(o => o.price);

                 revenue.Text = "Revenue:\t\t₱" + revenueToday.ToString();
            }
        }

        private void GetProductsSoldToday()
        {
            using (var dc = new living_fountainContext())
            {
                // Query to get the quantity sold today, grouped by product type
                var quantitiesSold = dc.orders
                    .Where(o => o.date == today)
                    .GroupBy(o => o.product_codeNavigation.product_desc)
                    .Select(g => new product_quantity_sold
                    {
                        ProductType = g.Key,
                        TotalQuantity = g.Sum(o => o.quantity ?? 0)
                    })
                    .ToList();

                // Bind the results to the ListView
                productTypeListView.ItemsSource = quantitiesSold;
            }
        }

        private void AddCheckBoxCol(string header, DataGrid dataGrid)
        {
            var checkBoxColumn = new DataGridTemplateColumn
            {
                Header = header,
                CellTemplate = (DataTemplate)Resources["CheckBoxCellTemplate"]
            };

            dataGrid.Columns.Add(checkBoxColumn);
        }

        private void GetUnpaid()
        {
            using (var dc = new living_fountainContext())
            {
                UnpaidDeliveries = dc.orders
                                       .Include(o => o.product_codeNavigation) // Include related product
                                       .Include(o => o.deliverer) // Include related deliverer (employee)
                                       .Include(o => o.statusNavigation) // Include related status
                                       .Where(o => o.date == today && o.status == "D")
                                       .ToList();
            }

            unpaidDeliveries.ItemsSource = UnpaidDeliveries;
        }

        private void GetUndelivered()
        {
            using (var dc = new living_fountainContext())
            {
                UndeliveredOrders = dc.orders
                                       .Include(o => o.product_codeNavigation) // Include related product
                                       .Include(o => o.deliverer) // Include related deliverer (employee)
                                       .Include(o => o.statusNavigation) // Include related status
                                       .Where(o => o.date == today && o.status == "P")
                                       .ToList();
            }

            undeliveredOrders.ItemsSource = UndeliveredOrders;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var record = checkBox.DataContext as order;
            if (record != null)
            {
                record.status = "PD"; // Update the order status in the data context

                // Update the order status in the database
                UpdateOrderStatus(record);

                // Access the parent Frame
                Frame parentFrame = NavigationHelper.FindParentFrame(this);

                if (parentFrame != null)
                {
                    // Navigate to refresh
                    parentFrame.NavigationService.Navigate(new Homepage());
                }
            }
        }

        private void UpdateOrderStatus(order order)
        {
            using (var dc = new living_fountainContext())
            {
                var orderToUpdate = dc.orders
                                    .FirstOrDefault(o => o.id == order.id);

                if (orderToUpdate != null)
                {
                    orderToUpdate.status = "PD";
                    dc.SaveChanges();
                }
            }
        }

        private void CountRecords()
        {
            if (UnpaidDeliveries.Count == 0)
            {
                unpaidDeliveries.Visibility = Visibility.Hidden;
                noUnpaid.Visibility = Visibility.Visible;
            }
            else
            {
                unpaidDeliveries.Visibility = Visibility.Visible;
            }

            if (UndeliveredOrders.Count == 0)
            {
                undeliveredOrders.Visibility = Visibility.Hidden;
                noUndelivered.Visibility = Visibility.Visible;
            }
            else
            {
                undeliveredOrders.Visibility = Visibility.Visible;
            }
        }
    }
}
