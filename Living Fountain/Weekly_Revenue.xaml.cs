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
    /// <summary>
    /// Interaction logic for Weekly_Revenue.xaml
    /// </summary>
    public partial class Weekly_Revenue : Page
    {
        public Weekly_Revenue()
        {
            InitializeComponent();
            ComputeRevenueByWeek();
        }

        public void ComputeRevenueByWeek()
        {
            using (var dc = new living_fountainContext())
            {
                var revenueByWeek = dc.orders
                .Where(o => o.date.HasValue && o.price.HasValue && o.quantity.HasValue)
                .AsEnumerable() // Convert to Enumerable for local grouping
                .GroupBy(o => new
                {
                    Year = o.date.Value.Year,
                    Week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(o.date.Value.ToDateTime(new TimeOnly()), CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Week = g.Key.Week,
                    StartDate = g.Min(o => o.date.Value.ToDateTime(new TimeOnly())),
                    EndDate = g.Max(o => o.date.Value.ToDateTime(new TimeOnly())),
                    TotalRevenue = g.Sum(o => o.price.Value * o.quantity.Value)
                })
                .OrderByDescending(g => g.Year)
                .ThenByDescending(g => g.Week)
                .ToList();

                foreach (var weekRevenue in revenueByWeek)
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
                        Text = weekRevenue.StartDate.ToString("MMM d, yyyy") + " to " + weekRevenue.EndDate.ToString("MMM d, yyyy")
                    };

                    revenueGrid.Children.Add(week);

                    var revenue = new TextBlock
                    {
                        Text = weekRevenue.TotalRevenue.ToString("C")
                    };

                    Grid.SetColumn(revenue, 1);
                    revenueGrid.Children.Add(revenue);
                }
            }
        }
    }
}
