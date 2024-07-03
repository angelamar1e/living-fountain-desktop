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

namespace Living_Fountain
{
    /// <summary>
    /// Interaction logic for Salary.xaml
    /// </summary>
    public partial class Salary : Page
    {
        private List<employee> Employees;
        int totalToday;

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
            DataGrid salaryGrid;
            using (var dc = new living_fountainContext())
            {
                Employees = dc.employees
                           .Include(e => e.emp_type_codeNavigation) // Include related employee type
                           .ToList();

                foreach (var employee in Employees)
                {
                    var empType = employee.emp_type_code;
                    var nameLabel = new TextBlock
                    {
                        Text = employee.employee_name
                    };
                    
                    // emp type determines where to display details and how to generate salary
                    switch (empType)
                    {
                        case 'D':
                            deliverersContainer.Children.Add(nameLabel);
                            GetDelivererSalary(employee.id);
                            break;
                        case 'W':
                            washersContainer.Children.Add(nameLabel);
                            salaryGrid = GetSalary('W');
                            washersContainer.Children.Add(salaryGrid);
                            break;
                        case 'R':
                            refillersContainer.Children.Add(nameLabel);
                            salaryGrid = GetSalary('R');
                            refillersContainer.Children.Add(salaryGrid);
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
                
                // every regular gallon delivered adds 3 to base pay
                var regularGallonsCount = orders.Sum(o => o.quantity); 
                var totalPay = basePay + (regularGallonsCount * salaryTypes.ElementAt(3).amount);

                // salary details added to collection to be bind to the data grid
                details.Add(new salary_details
                {
                    salary = totalPay,
                    quantityDelivered = regularGallonsCount
                });

                var salaryGrid = new DataGrid
                {
                    AutoGenerateColumns = false,
                    ItemsSource = details,
                    Margin = new Thickness(10),
                    CanUserAddRows = false
                };

                var columns = new List<(string Header, string Binding)>
                {
                    ("Quantity Delivered", "quantityDelivered"),
                    ("Salary", "salary")
                };

                // Add columns using a loop
                foreach (var column in columns)
                {
                    var col = new DataGridTextColumn
                    {
                        Header = column.Header,
                        Binding = new Binding(column.Binding)
                    };

                    if (column.Header == "Salary")
                    {
                        col.Binding.StringFormat = "₱{0:N0}";
                    }

                    salaryGrid.Columns.Add(col);
                }

                deliverersContainer.Children.Add(salaryGrid);
            }
        }

        private DataGrid GetSalary(char empType)
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

            var salaryGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                ItemsSource = details,
                Margin = new Thickness(10),
                CanUserAddRows = false
            };

            var col = new DataGridTextColumn
            {
                Header = "Salary",
                Binding = new Binding("salary")
            };

            col.Binding.StringFormat = "₱{0:N0}";
            salaryGrid.Columns.Add(col);

            return salaryGrid;
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
