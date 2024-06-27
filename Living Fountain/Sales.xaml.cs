using Living_Fountain.Models;
using Living_Fountain.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Living_Fountain
{
    /// <summary>
    /// Interaction logic for Sales.xaml
    /// </summary>
    public partial class Sales : Page
    {
        public List<product> Products { get; set; }
        public List<order> Orders { get; set; }
        public List<employee> Deliverers { get; set; }

        private LivingFountainHome home;
        public Sales()
        {
            InitializeComponent();

            SetDate();
            GetProductTypes();
            GetDeliverers();
        }

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

        private void SetDate()
        {
            DateTime dateTime = DateTime.Now;
            DateOnly dateOnly = DateOnly.FromDateTime(dateTime);
            currentDate.Text = dateOnly.ToString("MMMM dd, yyyy");

            LoadData(dateOnly);
            OrderRecords.ItemsSource = Orders;
            countRecords();
        }

        private void datePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            DateTime dateTime = datePicker.SelectedDate ?? DateTime.Now;
            DateOnly dateOnly = DateOnly.FromDateTime(dateTime);
            currentDate.Text = dateOnly.ToString("MMMM dd, yyyy");

            LoadData(dateOnly);
            OrderRecords.ItemsSource = Orders;
            countRecords();
        }

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

                // Get selected product and deliverer
                var selectedProduct = (product)productTypeCombo.SelectedItem;
                var selectedDeliverer = (employee)delivererCombo.SelectedItem;

                newOrder.product_code = selectedProduct.code;
                newOrder.quantity = int.Parse(quantityField.Text);
                newOrder.price = OrderHelper.ComputeNewPrice(newOrder.quantity, selectedProduct.price);
                newOrder.deliverer_id = selectedDeliverer.id;

                // Add newOrder to the context
                dc.orders.Add(newOrder);

                // Save changes to the database
                dc.SaveChanges();
                countRecords();
            }
        }
    }
}
