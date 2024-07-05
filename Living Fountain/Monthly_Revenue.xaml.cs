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
                    var revenueGrid = CreateRevenueGrid(monthRevenue.MonthName + " " + monthRevenue.Year, monthRevenue.TotalRevenue.ToString("C"));
                    container.Children.Add(revenueGrid);
                }
            }
        }

        private Grid CreateRevenueGrid(string period, string totalRevenue)
        { 
            var revenueGrid = new Grid();

            for (var x = 0; x <= 2; x++)
            {
                revenueGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                  Width = new GridLength(50, GridUnitType.Star)
                });
            }

            var periodTextBlock = new TextBlock
            {
                Text = period,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(150, 5, -125, 0),
                FontWeight = FontWeights.SemiBold
            };

            revenueGrid.Children.Add(periodTextBlock);

            var outerRectangleContainer = new Grid
            {
                Width = 250,
                Height = 50,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(15)
            };

            var outerRectangle = new Rectangle
            {
                Width = outerRectangleContainer.Width + 250,
                Height = outerRectangleContainer.Height,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                RadiusX = 10,
                RadiusY = 10,
                Margin = new Thickness(0, 5, 0, 5)
            };

            Grid.SetColumnSpan(outerRectangle, 3);
            revenueGrid.Children.Add(outerRectangle);

            var rectangleContainer = new Grid
            {
                Width = 100,
                Height = 50,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };

            var rectangle = new Rectangle
            {
                Fill = new SolidColorBrush(Color.FromRgb(44, 97, 236)),
                Width = rectangleContainer.Width - 10,
                Height = rectangleContainer.Height - 10,
                RadiusX = 8,
                RadiusY = 8,
                Margin = new Thickness(5)
            };

            var revenueTextBlock = new TextBlock
            {
                Text = totalRevenue,
                Foreground = new SolidColorBrush(Colors.White),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            rectangleContainer.Children.Add(rectangle);
            rectangleContainer.Children.Add(revenueTextBlock);

            Grid.SetColumn(rectangleContainer, 2);
            revenueGrid.Children.Add(rectangleContainer);

            return revenueGrid;
        }
    }
}

