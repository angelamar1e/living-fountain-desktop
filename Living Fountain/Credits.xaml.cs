using Living_Fountain.Helpers;
using Living_Fountain.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Living_Fountain
{
    /// <summary>
    /// Interaction logic for Credits.xaml
    /// </summary>
    public partial class Credits : Page
    {
        List<order> Credit { get; set; }
        public Credits()
        {
            InitializeComponent();

            LoadData();
            CountRecords();
        }

        private void LoadData()
        {
            var credits = GetCredits();
            var groupedCredit = GroupDataByDate(credits);
            CreateAndBindDataGrids(groupedCredit);
        }

        private List<order> GetCredits()
        {
            using (var dc = new living_fountainContext())
            {
                Credit = dc.orders
                        .Include(o => o.product_codeNavigation) // Include related product
                        .Include(o => o.deliverer) // Include related deliverer (employee)
                        .Include(o => o.statusNavigation) // Include related status
                        .Where(o => o.status == "D")
                        .ToList();
            }
            return Credit;
        }
        private Dictionary<DateOnly, List<order>> GroupDataByDate(List<order> records)
        {
            return records.GroupBy(record => record.date ?? OrderHelper.GetCurrentDate())
                          .ToDictionary(group => group.Key, group => group.ToList());
        }

        private void CreateAndBindDataGrids(Dictionary<DateOnly, List<order>> groupedData)
        {
            foreach (var group in groupedData)
            {
                // Add a label for the date
                var dateLabel = new TextBlock
                {
                    Text = group.Key.ToString("MMMM dd, yyyy"), // Format the date as desired
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(10)
                };
                dataGridContainer.Children.Add(dateLabel);

                // Create and bind the DataGrid
                var dataGrid = new DataGrid
                {
                    AutoGenerateColumns = false,
                    ItemsSource = group.Value,
                    Margin = new Thickness(10),
                    CanUserAddRows = false
                };

                var columns = new List<(string Header, string Binding)>
                {
                    ("Block", "block"),
                    ("Lot", "lot"),
                    ("Phase", "phase"),
                    ("Product", "product_codeNavigation.product_desc"),
                    ("Quantity", "quantity"),
                    ("Price", "price"),
                    ("Deliverer", "deliverer.employee_name")
                };

                // Add columns using a loop
                foreach (var column in columns)
                {
                    dataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = column.Header,
                        Binding = new Binding(column.Binding)
                    });
                }

                // Add CheckBox column using DataTemplate
                var checkBoxColumn = new DataGridTemplateColumn
                {
                    Header = "Mark as Paid",
                    CellTemplate = (DataTemplate)Resources["CheckBoxCellTemplate"]
                };
                dataGrid.Columns.Add(checkBoxColumn);

                dataGridContainer.Children.Add(dataGrid);
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var record = checkBox.DataContext as order;
            if (record != null)
            {
                record.status = "PD"; // Update the order status in the record

                // Update the order status in the database
                UpdateOrderStatus(record);

                // Access the parent Frame (MainFrame)
                Frame parentFrame = NavigationHelper.FindParentFrame(this);

                if (parentFrame != null)
                {
                    // Navigate MainFrame to Edit_Order.xaml with the id parameter
                    parentFrame.NavigationService.Navigate(new Credits());
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
            if (Credit.Count == 0)
            {
                noRecords.Visibility = Visibility.Visible;
            }
        }
    }
}
