using Living_Fountain.Helpers;
using Living_Fountain.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;

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
                var border = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#396cae")),
                    Width = 600,
                    CornerRadius = new CornerRadius(10),
                    Margin = new Thickness(0, 0, 0, 20),
                    Padding = new Thickness(10),
                    Effect = new DropShadowEffect
                    {
                        Color = Colors.Gray,
                        BlurRadius = 10,
                        Opacity = 0.1,
                        Direction = 290
                    }
                };
                
                var recordContainer = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                };
                
                border.Child = recordContainer;

                // Add a label for the date
                var dateLabel = new TextBlock
                {
                    Text = group.Key.ToString("MMMM d, yyyy"), // Format the date as desired
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(10),
                    Foreground = new SolidColorBrush(Colors.AliceBlue),
                    FontSize = 20
                };

                recordContainer.Children.Add(dateLabel);

                var gridBorder = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4e78ae")),
                    Width = 580,
                    CornerRadius = new CornerRadius(10)
                };

                recordContainer.Children.Add(gridBorder);

                var dataGridContainer = new StackPanel
                {
                    Orientation = Orientation.Vertical
                };

                gridBorder.Child = dataGridContainer;

                // Create and bind the DataGrid
                var dataGrid = new DataGrid
                {
                    AutoGenerateColumns = false,
                    ItemsSource = group.Value,
                    Margin = new Thickness(10),
                    CanUserAddRows = false,
                    HorizontalGridLinesBrush = new SolidColorBrush(Colors.LightGray),
                    VerticalGridLinesBrush = new SolidColorBrush(Colors.LightGray)
                };

                DataGridStyle(dataGrid);

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
                    var col = new DataGridTemplateColumn
                    {
                        Header = column.Header
                    };

                    // Create DataTemplate for the cell
                    FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                    textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding(column.Binding));
                    textBlockFactory.SetValue(TextBlock.StyleProperty, dataGrid.Resources["CellContent"]);

                    DataTemplate cellTemplate = new DataTemplate
                    {
                        VisualTree = textBlockFactory
                    };

                    col.CellTemplate = cellTemplate;

                    // If the column is "Price", apply string formatting
                    if (column.Header == "Price")
                    {
                        var binding = new Binding(column.Binding) { StringFormat = "₱{0:N0}" };
                        textBlockFactory.SetBinding(TextBlock.TextProperty, binding);
                    }

                    dataGrid.Columns.Add(col);
                }

                // Add CheckBox column using DataTemplate
                var checkBoxColumn = new DataGridTemplateColumn
                {
                    Header = "Mark as Paid",
                    CellTemplate = (DataTemplate)Resources["CheckBoxCellTemplate"]
                };
                dataGrid.Columns.Add(checkBoxColumn);

                dataGridContainer.Children.Add(dataGrid);
                container.Children.Add(border);
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
                noRecordsViewer.Visibility = Visibility.Visible;
            }
        }

        private void DataGridStyle(DataGrid dataGrid)
        {
            // Create Style for DataGridRow
            Style dataGridRowStyle = new Style(typeof(DataGridRow));
            dataGridRowStyle.Setters.Add(new Setter(DataGridRow.FontSizeProperty, 12.0));
            dataGridRowStyle.Setters.Add(new Setter(DataGridRow.VerticalAlignmentProperty, VerticalAlignment.Center));
            dataGridRowStyle.Setters.Add(new Setter(DataGridRow.MinHeightProperty, 25.0));
            dataGridRowStyle.Setters.Add(new Setter(DataGridRow.BackgroundProperty, Brushes.White));
            dataGridRowStyle.Setters.Add(new Setter(DataGridRow.ForegroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF343D53"))));
            dataGridRowStyle.Setters.Add(new Setter(DataGridRow.FontWeightProperty, FontWeights.DemiBold));

            // Create Style for TextBlock with x:Key="CellContent"
            Style textBlockStyle = new Style(typeof(TextBlock));
            textBlockStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center));
            textBlockStyle.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
            textBlockStyle.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(8.0)));

            // Create Style for DataGridColumnHeader
            Style dataGridColumnHeaderStyle = new Style(typeof(DataGridColumnHeader));
            dataGridColumnHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.PaddingProperty, new Thickness(8)));
            dataGridColumnHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.FontWeightProperty, FontWeights.Bold));
            dataGridColumnHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.ForegroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6587b1"))));
            dataGridColumnHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
            dataGridColumnHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.VerticalContentAlignmentProperty, VerticalAlignment.Center));
            dataGridColumnHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.HeightProperty, 35.0));

            dataGrid.Resources.Add(typeof(DataGridRow), dataGridRowStyle);
            dataGrid.Resources.Add("CellContent", textBlockStyle);
            dataGrid.Resources.Add(typeof(DataGridColumnHeader), dataGridColumnHeaderStyle);
        }
    }
}
