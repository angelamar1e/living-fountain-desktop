using Living_Fountain.Models;
using Living_Fountain.Helpers;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;


namespace Living_Fountain
{
    /// <summary>
    /// Interaction logic for Edit_Order.xaml
    /// </summary>
    public partial class Edit_Order : Page
    {
        public Living_Fountain.Models.order SelectedOrder { get; set; }
        public Sales sales = new Sales(OrderHelper.GetCurrentDate());
        public Edit_Order(int id)
        {
            InitializeComponent();

            // to display order id on edit order title
            orderId.Text = id.ToString();
            
            // retrieving data with id
            LoadData(id);

            //bind lists from Sales page as items for combo boxes
            BindComboBoxes();

            this.DataContext = this;
        }

        private void LoadData(int id)
        {
            using (var dc = new living_fountainContext())
            {
                var order = dc.orders
                          .Include(o => o.product_codeNavigation) // Include related product
                          .Include(o => o.deliverer) // Include related employee
                          .FirstOrDefault(o => o.id == id);

                SelectedOrder = order;
            }
        }

        private void BindComboBoxes()
        {
            List<product> ProductTypes = sales.Products;
            productTypeCombo.ItemsSource = ProductTypes;

            List<employee> Deliverers = sales.Deliverers;
            delivererCombo.ItemsSource = Deliverers;

            List<order_status> Statuses = sales.Statuses;
            statusCombo.ItemsSource = Statuses;
        }

        private void SaveChanges()
        {
            using (var dc = new living_fountainContext())
            {
                // Retrieve the order from the database to update
                var orderToUpdate = dc.orders
                                       .Include(o => o.product_codeNavigation)
                                       .Include(o => o.deliverer)
                                       .FirstOrDefault(o => o.id == SelectedOrder.id);

                if (orderToUpdate != null)
                {
                    
                    var existingCustomer = OrderHelper.GetOrCreateCustomer(dc, SelectedOrder.block ?? 0, SelectedOrder.lot ?? 0, SelectedOrder.phase ?? 0);

                    // Update properties of orderToUpdate with SelectedOrder's values
                    orderToUpdate.block = existingCustomer.block;
                    orderToUpdate.lot = existingCustomer.lot;
                    orderToUpdate.phase = existingCustomer.phase;
                    orderToUpdate.product_code = SelectedOrder.product_code;

                    if (orderToUpdate.quantity != SelectedOrder.quantity)
                    {
                        orderToUpdate.price = OrderHelper.ComputeNewPrice(SelectedOrder.quantity, orderToUpdate.product_codeNavigation.price);
                        orderToUpdate.quantity = SelectedOrder.quantity;
                    }

                    orderToUpdate.deliverer_id = SelectedOrder.deliverer_id;
                    orderToUpdate.status = SelectedOrder.status;

                    // Save changes to the database
                    dc.SaveChanges();
                }
            }
        }

        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            SaveChanges();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            // Access the parent Frame (MainFrame)
            Frame parentFrame = NavigationHelper.FindParentFrame(this);

            if (parentFrame != null)
            {
                // Navigate MainFrame to Edit_Order.xaml with the id parameter
                parentFrame.NavigationService.Navigate(new Sales(OrderHelper.GetDateOnly(SelectedOrder.date)));
            }
        }
    }

    
}