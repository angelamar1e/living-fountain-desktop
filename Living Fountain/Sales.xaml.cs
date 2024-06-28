using Living_Fountain.Models;
using Living_Fountain.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;

namespace Living_Fountain
{
    /// <summary>
    /// Interaction logic for Sales.xaml
    /// </summary>
    public partial class Sales : Page
    {
        public List<order> Orders { get; set; }
        public List<product> Products { get; set; }
        public List<employee> Deliverers { get; set; }
        public List<order_status> Statuses { get; set; }

        public Sales()
        {
            InitializeComponent();

            GetInitialData();

            // To bind the combo boxes to items from db
            GetProductTypes();
            GetDeliverers();
            GetStatusTypes();
        }

        // retrieving order records from db
        private void LoadData(DateOnly date)
        {
            using (var dc = new living_fountainContext())
            {
                Orders = dc.orders
                           .Include(o => o.product_codeNavigation) // Include related product
                           .Include(o => o.deliverer) // Include related deliverer (employee)
                           .Include(o => o.statusNavigation) // Include related status
                           .Where(o => o.date == date)
                           .ToList();
            }

            OrderRecords.ItemsSource = Orders;
            countRecords();
        }

        private void GetProductTypes()
        {
            using (var dc = new living_fountainContext())
            {
                Products = dc.products.ToList();

                productTypeCombo.ItemsSource = Products;
            }
        }

        private void GetDeliverers()
        {
            using (var dc = new living_fountainContext())
            {
                Deliverers = dc.employees
                               .Where(e => e.emp_type_code == 'D')
                               .ToList();
            }
            delivererCombo.ItemsSource = Deliverers;
        }

        private void GetStatusTypes()
        {
            using (var dc = new living_fountainContext())
            {
                Statuses = dc.order_statuses
                               .ToList();
            }
            statusCombo.ItemsSource = Statuses;
        }

        // the default data displayed in the table are orders for the current date
        private void GetInitialData()
        {
            DateTime dateTime = DateTime.Now;
            DateOnly dateOnly = DateOnly.FromDateTime(dateTime);
            currentDate.Text = dateOnly.ToString("MMMM dd, yyyy");

            LoadData(dateOnly);
        }

        // order records are filtered as date picker is changed
        private void datePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            DateTime dateTime = datePicker.SelectedDate ?? DateTime.Now;
            DateOnly dateOnly = DateOnly.FromDateTime(dateTime);
            currentDate.Text = dateOnly.ToString("MMMM dd, yyyy");

            LoadData(dateOnly);
        }

        // table of order records is hidden if no records are found for the selected date
        private void countRecords()
        {
            if (Orders.Count == 0)
            {
                OrderRecords.Visibility = Visibility.Hidden;
                noRecords.Visibility = Visibility.Visible;
            }
            else
            {
                OrderRecords.Visibility = Visibility.Visible;
            }
        }

        // navigates to edit_order page with the id of the order record
        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            Sales sales = this;
            if (sender is System.Windows.Controls.Button button)
            {
                // Retrieve the id from the button's Tag property
                int id = (int)button.Tag;

                // Access the parent Frame (MainFrame)
                Frame parentFrame = FindParentFrame(this);

                if (parentFrame != null)
                {
                    // Navigate MainFrame to Edit_Order.xaml with the id parameter
                    parentFrame.NavigationService.Navigate(new Edit_Order(id, sales));
                }
            }
        }

        private Frame FindParentFrame(DependencyObject child)
        {
            // Traverse up the visual tree to find the parent Frame
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            if (parentObject is Frame frame)
            {
                return frame;
            }

            return FindParentFrame(parentObject);
        }

        // confirms deletion
        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button)
            {
                // Retrieve the order from the button's Tag property
                int orderId = (int)button.Tag;
                var orderToDelete = Orders.FirstOrDefault(o => o.id == orderId);

                // Show confirmation message box
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this order record?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // Handle the user's response
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // Proceed with deletion
                        DeleteOrder(orderToDelete);
                        break;
                    case MessageBoxResult.No:
                        // Do nothing or handle cancellation
                        break;
                }
            }
        }

        private void DeleteOrder(order orderToDelete)
        {
            DateOnly date;
            using (var dc = new living_fountainContext())
            {
                // Find the order in the database
                var order = dc.orders.SingleOrDefault(o => o.id == orderToDelete.id);
                if (order != null)
                {
                    date = order.date.GetValueOrDefault();
                    // Remove the order from the database
                    dc.orders.Remove(order);
                    dc.SaveChanges();
                    LoadData(date);
                }

            }

            // Remove the order from the ObservableCollection to update the UI
            Orders.Remove(orderToDelete);

            // Show a success message
            MessageBox.Show("Order deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // submission of new order record
        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            using (var dc = new living_fountainContext())
            {
                order newOrder = new order();
                var existingCustomer = OrderHelper.GetOrCreateCustomer(dc, int.Parse(blockField.Text), int.Parse(lotField.Text), int.Parse(phaseField.Text));

                // Update properties of orderToUpdate with SelectedOrder's values
                newOrder.block = existingCustomer.block;
                newOrder.lot = existingCustomer.lot;
                newOrder.phase = existingCustomer.phase;
                newOrder.date = DateOnly.FromDateTime(DateTime.Now);

                // Get selected from comboboxes
                var selectedProduct = (product)productTypeCombo.SelectedItem;
                var selectedDeliverer = (employee)delivererCombo.SelectedItem;
                var selectedStatus = (order_status)statusCombo.SelectedItem;

                newOrder.product_code = selectedProduct.code;
                newOrder.quantity = int.Parse(quantityField.Text);
                newOrder.price = OrderHelper.ComputeNewPrice(newOrder.quantity, selectedProduct.price);
                newOrder.deliverer_id = selectedDeliverer.id;
                newOrder.status = selectedStatus.code;

                // Add newOrder to the context
                dc.orders.Add(newOrder);

                // Save changes to the database
                dc.SaveChanges();


                MessageBox.Show("Order added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                GetInitialData();
            }
        }
    }
}
