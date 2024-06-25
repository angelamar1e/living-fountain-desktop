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

namespace Living_Fountain
{
    /// <summary>
    /// Interaction logic for Revenue.xaml
    /// </summary>
    public partial class Revenue : Page
    {
        public ObservableCollection<RevenueItem> WeeklyRevenues { get; set; }
        public ObservableCollection<MonthlyRevenueItem> MonthlyRevenues { get; set; }

        public Revenue()
        {
            InitializeComponent();
            LoadData();

            // Set the DataContext to the current instance to allow data binding
            this.DataContext = this;
        }

        private void LoadData()
        {
            // Sample data for weekly revenues
            WeeklyRevenues = new ObservableCollection<RevenueItem>
            {
                new RevenueItem { DateRange = "Oct 02 2023 to Oct 07 2023", Revenue = "₱16835" },
                new RevenueItem { DateRange = "Oct 09 2023 to Oct 14 2023", Revenue = "₱16730" },
                new RevenueItem { DateRange = "Oct 16 2023 to Oct 21 2023", Revenue = "₱16170" },
                new RevenueItem { DateRange = "Oct 23 2023 to Oct 28 2023", Revenue = "₱16590" }
            };

            // Sample data for monthly revenues
            MonthlyRevenues = new ObservableCollection<MonthlyRevenueItem>
            {
                new MonthlyRevenueItem { Month = "January 2024", MonthlyTotal = "₱280" },
                new MonthlyRevenueItem { Month = "February 2024", MonthlyTotal = "₱510" },
                new MonthlyRevenueItem { Month = "October 2023", MonthlyTotal = "₱71925" },
                new MonthlyRevenueItem { Month = "November 2023", MonthlyTotal = "₱28350" }
            };

        }
    }

    // Data model for weekly revenue items
    public class RevenueItem
    {
        public string DateRange { get; set; }
        public string Revenue { get; set; }
    }

    // Data model for monthly revenue items
    public class MonthlyRevenueItem
    {
        public string Month { get; set; }
        public string MonthlyTotal { get; set; }
    }
}
