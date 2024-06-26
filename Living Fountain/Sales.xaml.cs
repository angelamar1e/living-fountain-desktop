using Living_Fountain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Living_Fountain
{
    /// <summary>
    /// Interaction logic for Sales.xaml
    /// </summary>
    public partial class Sales : Page
    {
        public List<product> Products { get; set; }
        public ObservableCollection<order> Orders { get; set; }
        public ObservableCollection<employee> Deliverers { get; set; }


        private LivingFountainHome home;
        public Sales()
        {
            InitializeComponent();

            Orders = new ObservableCollection<order>();
            SetDate();
            GetProductTypes();
            GetDeliverers();
        }

        private void LoadData(DateOnly date)
        {
            using(living_fountainContext dc = new living_fountainContext())
            {
                var orders = from o in dc.orders
                             join p in dc.products on o.product_code equals p.code
                             join e in dc.employees on o.deliverer_id equals e.id
                             join s in dc.order_statuses on o.status equals s.code
                             where o.date == date
                             select new Living_Fountain.order
                             {
                                id = o.id,
                                block = o.block,
                                lot = o.lot,
                                phase = o.phase,
                                product = new Living_Fountain.product
                                {
                                  product_desc = p.product_desc
                                },
                                quantity = o.quantity,
                                price = o.price,
                                deliverer = new Living_Fountain.employee
                                {
                                    employee_name = e.employee_name
                                },
                                 status = s.status_desc
                             };

                foreach (var order in orders){
                    Orders.Add(order);
                }
            }
        }

        private void GetProductTypes()
        {
            living_fountainContext dc = new living_fountainContext();
            var items = dc.products.ToList();

            Products = items.Select(item => new product
            {
                code = item.code,
                product_desc = item.product_desc
            }).ToList();

            productTypeCombo.ItemsSource = Products;
        }

        private void GetDeliverers() 
        {
            Deliverers = new ObservableCollection<employee>();
            using (living_fountainContext dc = new living_fountainContext())
            {
                var deliverers = from emp in dc.employees
                                 where emp.emp_type_code == 'D'
                                 select new employee
                                 {
                                     employee_name = emp.employee_name
                                 };
                foreach (var deliverer in deliverers)
                {
                    Deliverers.Add(deliverer);
                }
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
            if (sender is System.Windows.Controls.Button button)
            {
                // Retrieve the id from the button's Tag property
                int id = (int)button.Tag;

                // Access the parent Frame (MainFrame)
                Frame parentFrame = FindParentFrame(this);

                if (parentFrame != null)
                {
                    // Navigate MainFrame to Edit_Order.xaml with the id parameter
                    parentFrame.NavigationService.Navigate(new Edit_Order(id));
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
