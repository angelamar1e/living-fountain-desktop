using Living_Fountain.Models;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;


namespace Living_Fountain
{
    /// <summary>
    /// Interaction logic for Edit_Order.xaml
    /// </summary>
    public partial class Edit_Order : Page
    {
        public Living_Fountain.Models.order SelectedOrder { get; set; }
        public Edit_Order(int id, Sales sales)
        {
            InitializeComponent();
            orderId.Text = id.ToString();
            LoadData(id);
            List<product> ProductTypes = sales.Products;
            productTypeCombo.ItemsSource = ProductTypes;
            List<employee> Deliverers = sales.Deliverers;
            delivererCombo.ItemsSource = Deliverers;
            

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
                    
                    var existingCustomer = GetOrCreateCustomer(SelectedOrder.block ?? 0, SelectedOrder.lot ?? 0, SelectedOrder.phase ?? 0, dc);

                    // Update properties of orderToUpdate with SelectedOrder's values
                    orderToUpdate.block = existingCustomer.block;
                    orderToUpdate.lot = existingCustomer.lot;
                    orderToUpdate.phase = existingCustomer.phase;
                    orderToUpdate.product_code = SelectedOrder.product_code;

                    if (orderToUpdate.quantity != SelectedOrder.quantity)
                    {
                        orderToUpdate.price = ComputeNewPrice(SelectedOrder.quantity, orderToUpdate.product_codeNavigation.price);
                        orderToUpdate.quantity = SelectedOrder.quantity;
                    }

                    orderToUpdate.deliverer_id = SelectedOrder.deliverer_id;
                    orderToUpdate.status = SelectedOrder.status;

                    // Save changes to the database
                    dc.SaveChanges();
                }
            }
        }

        private customer GetOrCreateCustomer(int block, int lot, int phase, living_fountainContext dc)
        {
            var existingCustomer = dc.customers.FirstOrDefault(c => c.block == block && c.lot == lot && c.phase == phase);

            if (existingCustomer == null)
            {
                // Customer does not exist, create a new customer
                existingCustomer = new customer
                {
                    block = block,
                    lot = lot,
                    phase = phase
                };

                dc.customers.Add(existingCustomer);
                dc.SaveChanges(); // Save changes to add new customer to the database
            }

            return existingCustomer;
        }

        private decimal? ComputeNewPrice(int? quantity, decimal? price)
        {
            decimal? newPrice = quantity * price;

            return newPrice;
        }


        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            SaveChanges();
        }
    }

    
}