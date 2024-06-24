using Living_Fountain.Models;
using Microsoft.EntityFrameworkCore;
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
        public ObservableCollection<order> Orders { get; set; }
        private LivingFountainHome home;
        public Sales()
        {
            InitializeComponent();

            Orders = new ObservableCollection<order>();
            
            DateOnly currentDate = SetCurrentDate();
            
            LoadData(currentDate);

            if (Orders.Count == 0)
            {
                OrderRecords.Visibility = Visibility.Hidden;
                noRecords.Visibility = Visibility.Visible;
            }

            OrderRecords.ItemsSource = Orders;
        }

        private void LoadData(DateOnly date)
        {
            using(var dc = new living_fountainContext())
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

        public List<order_status> Status { get; set; }
        private void GetStatusTypes()
        {
            living_fountainContext dataContext = new living_fountainContext();
            var items = dataContext.order_statuses.ToList();

            Status = items.Select(item => new order_status
            {
                code = item.code,
                status_desc = item.status_desc,
            }).ToList();
        }

        private DateOnly SetCurrentDate()
        {
            DateTime currDateTime = new DateTime();
            currDateTime = DateTime.Now;
            DateOnly dateOnly = DateOnly.FromDateTime(currDateTime);
            currentDate.Text = dateOnly.ToString("MMMM dd, yyyy");

            return dateOnly;
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            // Access the parent Frame (MainFrame)
            Frame parentFrame = FindParentFrame(this);

            if (parentFrame != null)
            {
                // Navigate MainFrame to Edit_Order.xaml
                parentFrame.NavigationService.Navigate(new Edit_Order());
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
