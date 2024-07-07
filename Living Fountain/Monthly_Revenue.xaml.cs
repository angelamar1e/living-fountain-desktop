using Living_Fountain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Living_Fountain
{
    public partial class Monthly_Revenue : Page
    {
        public Monthly_Revenue()
        {
            InitializeComponent();
            ComputeRevenueByWeek();
        }

        public void ComputeRevenueByWeek()
        {
            using (var dc = new living_fountainContext())
            {
                var revenueByMonth = dc.orders
                    .Where(o => o.date.HasValue && o.price.HasValue && o.quantity.HasValue)
                    .AsEnumerable() // Convert to Enumerable for local grouping
                    .GroupBy(o => new
                    {
                        Year = o.date.Value.Year,
                        Month = o.date.Value.Month
                    })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM"),
                        TotalRevenue = g.Sum(o => o.price.Value)
                    })
                    .OrderByDescending(g => g.Year)
                    .ThenByDescending(g => g.MonthName)
                    .ToList();

                foreach (var monthRevenue in revenueByMonth)
                {
                    CreateRevenueGrid(monthRevenue.MonthName + " " + monthRevenue.Year, monthRevenue.TotalRevenue.ToString("₱#,0"));
                }
            }
        }

        private void CreateRevenueGrid(string period, string totalRevenue)
        {
            var border = new Border()
            {
                CornerRadius = new CornerRadius(10),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1e90bf")),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(10),
                Padding = new Thickness(10),
                Width = 400

            };

            var revenueGrid = new Grid();

            revenueGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(200)
            });

            revenueGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(200)
            });

            border.Child = revenueGrid;

            var month = new TextBlock
            {
                Text = period,
                Foreground = new SolidColorBrush(Colors.AliceBlue),
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.SemiBold,
                FontSize  = 20,
                Width = 200

            };

            revenueGrid.Children.Add(month);
            Grid.SetColumn(month, 0);

            var revenueBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1e90bf")),
                CornerRadius = new CornerRadius(10),
                Width = 100
            };

            var revenue = new TextBlock
            {
                Text = totalRevenue,
                Foreground = new SolidColorBrush(Colors.White),
                TextAlignment = TextAlignment.Center,
                Padding = new Thickness(5),
                FontSize = 20,
                Width = 100
            };

            revenueBorder.Child = revenue;

            revenueGrid.Children.Add(revenueBorder);
            Grid.SetColumn(revenueBorder, 1);

            container.Children.Add(border);
        }
    }
}

