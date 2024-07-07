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
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media.Effects;

namespace Living_Fountain
{
    /// <summary>
    /// Interaction logic for Salary.xaml
    /// </summary>
    public partial class Salary : Page
    {
        private List<employee> Employees;
        int totalToday;
        TextBlock nameLabel;

        public Salary()
        {
            InitializeComponent();

            // display current date
            currentDate.Text = OrderHelper.GetCurrentDate().ToString("MMMM d, yyyy");

            // counts total products sold for the day
            totalToday = CountTotalQty();
            GetEmployees();
        }


        private void GetEmployees()
        {
            List<salary_details> details;
            Border recordContainer;

            using (var dc = new living_fountainContext())
            {
                Employees = dc.employees
                           .Include(e => e.emp_type_codeNavigation) // Include related employee type
                           .ToList();

                foreach (var employee in Employees)
                {
                    var empType = employee.emp_type_code;
                    nameLabel = new TextBlock
                    {
                        Text = employee.employee_name,
                        Foreground = new SolidColorBrush(Colors.AliceBlue),
                        FontSize = 16,
                        Margin = new Thickness(20, 0, 0, 0)
                    };
                    
                    // emp type determines where to display details and how to generate salary
                    switch (empType)
                    {
                        case 'D':
                            GetDelivererSalary(employee.id);
                            break;
                        case 'W':
                            details = GetSalary('W');
                            recordContainer = CreateSalaryListView(details);
                            washersContainer.Children.Add(recordContainer);
                            break;
                        case 'R':
                            details = GetSalary('R');
                            recordContainer = CreateSalaryListView(details);
                            refillersContainer.Children.Add(recordContainer);
                            break;
                    }
                }
            }
        }

        private void GetDelivererSalary(int delivererId)
        {
            // collection to store the salary and qty delivered
            var details = new List<salary_details>();

            using (var dc = new living_fountainContext())
            {
                // retrieves all orders delivered
                var orders = dc.orders
                    .Include(o => o.deliverer)
                    .Where(o => o.deliverer_id == delivererId && o.date == OrderHelper.GetCurrentDate() && o.product_code == "R" && o.status == "PD")
                    .ToList();

                // gets salary types for deliverers
                var salaryTypes = GetSalaryTypes('D');

                var basePay = salaryTypes.First().amount; //250

                // total products sold determines base pay
                if (totalToday > 50 && totalToday < 100)
                {
                    basePay = salaryTypes.ElementAt(1).amount; // 350
                }
                else if (totalToday > 100)
                {
                    basePay = salaryTypes.ElementAt(2).amount; // 450
                }
                
                // every regular gallon delivered adds 3 pesos to base pay
                var regularGallonsCount = orders.Sum(o => o.quantity); 
                var totalPay = basePay + (regularGallonsCount * salaryTypes.ElementAt(3).amount);

                // salary details added to collection to be bind to the data grid
                details.Add(new salary_details
                {
                    salary = totalPay,
                    quantityDelivered = regularGallonsCount
                });

                DisplayDelivererSalary(details);
            }
        }

        private void DisplayDelivererSalary(List<salary_details> details)
        {
            Style headerStyle = (Style)FindResource("WrappedHeaderStyle");
            Style listViewItemStyle = (Style)FindResource("ListViewItemStyle");


            var border = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3a84a7")),
                Width = 210,
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(0, 0, 0, 20),
                Padding = new Thickness(8),
                Effect = new DropShadowEffect
                {
                    Color = Colors.Gray,
                    BlurRadius = 10,
                    Opacity = 0.1,
                    Direction = 290
                }
            };

            var container = new StackPanel 
            {
                Orientation = Orientation.Vertical,
            };

            border.Child = container;
            container.Children.Add(nameLabel);

            var salaryListView = new ListView
            {
                Margin = new Thickness(10),
                Width = 170,
                ItemContainerStyle = listViewItemStyle
            };

            var gridView = new GridView();
            salaryListView.View = gridView;

            var columns = new List<(string Header, string Binding)>
            {
                ("Regular Gallons Delivered", "quantityDelivered"),
                ("Salary", "salary")
            };

            // Add columns using a loop
            foreach (var column in columns)
            {
                var gridViewColumn = new GridViewColumn
                {
                    Header = column.Header,
                    DisplayMemberBinding = new Binding(column.Binding),
                    HeaderContainerStyle = headerStyle
                };

                if (column.Header == "Salary")
                {
                    gridViewColumn.DisplayMemberBinding.StringFormat = "₱{0:N0}";
                }

                gridView.Columns.Add(gridViewColumn);
            }

            salaryListView.ItemsSource = details;

            container.Children.Add(salaryListView);

            deliverersContainer.Children.Add(border);
        }

        private List<salary_details> GetSalary(char empType)
        {
            // collection to store the salary and qty delivered
            var details = new List<salary_details>();

            var types = GetSalaryTypes(empType);
            var salary = types.First().amount;

            if (totalToday > 100)
            {
                salary = types.ElementAt(1).amount;
            }

            details.Add(new salary_details
            {
                salary = salary
            });

            return details;
        }

        private Border CreateSalaryListView(List<salary_details> details)
        {
            Style headerStyle = (Style)FindResource("WrappedHeaderStyle");
            Style listViewItemStyle = (Style)FindResource("ListViewItemStyle");

            var border = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3a84a7")),
                Width = 210,
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(0, 0, 0, 20),
                Padding = new Thickness(8),
                Effect = new DropShadowEffect
                {
                    Color = Colors.Gray,
                    BlurRadius = 10,
                    Opacity = 0.1,
                    Direction = 290
                }
            };

            var container = new StackPanel
            {
                Orientation = Orientation.Vertical,
            };

            border.Child = container;
            container.Children.Add(nameLabel);

            // Create a new ListView
            var salaryListView = new ListView
            {
                Margin = new Thickness(10),
                Width = 90,
                ItemContainerStyle = listViewItemStyle
            };

            // Create a GridView to define columns
            var gridView = new GridView();
            salaryListView.View = gridView;

            // Define the Salary column
            var salaryColumn = new GridViewColumn
            {
                Header = "Salary",
                HeaderContainerStyle = headerStyle,
                DisplayMemberBinding = new Binding("salary")
                {
                    StringFormat = "₱{0:N0}" // Format as currency without decimal places
                }
            };

            // Add the Salary column to the GridView
            gridView.Columns.Add(salaryColumn);

            // Bind the data
            salaryListView.ItemsSource = details;

            container.Children.Add(salaryListView);

            return border;
        }

        private List<salary_type> GetSalaryTypes(char empType)
        {
            using (var dc = new living_fountainContext())
            {
                var types = dc.salary_types
                    .Where(s => s.emp_type_code == empType)
                    .ToList();

                return types;
            }
        }

        private int CountTotalQty()
        {
            using (var dc = new living_fountainContext())
            {
                var orders = dc.orders
                    .Where(o => o.date == OrderHelper.GetCurrentDate() && o.status == "PD");

                var totalToday = orders.Count();

                return totalToday;
            }
        }
    }
}
