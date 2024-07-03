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
                    var revenueGrid = new Grid();

                    for (var x = 0; x <= 2; x++)
                    {
                        revenueGrid.ColumnDefinitions.Add(new ColumnDefinition
                        {
                            Width = new GridLength(50, GridUnitType.Star)
                        });
                    }

                    container.Children.Add(revenueGrid);

                    var week = new TextBlock
                    {
                        Text = monthRevenue.MonthName + " " + monthRevenue.Year
                    };

                    revenueGrid.Children.Add(week);

                    var revenue = new TextBlock
                    {
                        Text = monthRevenue.TotalRevenue.ToString("C")
                    };

                    Grid.SetColumn(revenue, 1);
                    revenueGrid.Children.Add(revenue);
                }
            }
        }
    }
}
