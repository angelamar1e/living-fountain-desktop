using Living_Fountain.Helpers;
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

namespace Living_Fountain
{
    /// <summary>
    /// Interaction logic for LivingFountainHome.xaml
    /// </summary>
    public partial class LivingFountainHome : Page
    {
        public LivingFountainHome()
        {
            InitializeComponent();
        }

        private void Sales_Button_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Sales(OrderHelper.GetCurrentDate());
        }

        private void Credits_Button_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Credits();
        }

        private void Salary_Button_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Salary();
        }

        private void Main_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
}
