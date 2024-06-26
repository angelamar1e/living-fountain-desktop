using Living_Fountain.Models;
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
                          .Select(o => new order
                          {
                              id = o.id,
                              block = o.block,
                              lot = o.lot,
                              phase = o.phase,
                              product_codeNavigation = o.product_codeNavigation,
                              quantity = o.quantity,
                              price = o.price,
                              deliverer = o.deliverer,
                              status = o.statusNavigation.status_desc
                          })
                          .ToList();
            }
        }

        private void GetProductTypes()
        {
            using (var dc = new living_fountainContext())
            {
                Products = dc.products
                            .Select(item => new product
                            {
                                code = item.code,
                                product_desc = item.product_desc
                            })
                            .ToList();

                productTypeCombo.ItemsSource = Products;
            }
        }



        private void GetDeliverers() 
        {
            using (var dc = new living_fountainContext())
            {
                Deliverers = dc.employees
                               .Where(e => e.emp_type_code == 'D')
                               .Select
                               (item => new employee
                                 {
                                     employee_name = item.employee_name,
                                     id = item.id
                                 })
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
    }
}
