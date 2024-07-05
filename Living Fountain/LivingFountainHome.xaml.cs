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
            Main.Content = new Homepage();
        }

        private void Sales_Button_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Sales(OrderHelper.GetCurrentDate());
            SelectButton(salesButton);
        }

        private void Credits_Button_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Credits();
            SelectButton(creditsButton);
        }

        private void Salary_Button_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Salary();
            SelectButton(salaryButton);
        }

        private void Customers_Button_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Customers();
            SelectButton(customersButton);
        }

        private void Revenue_Button_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Revenue();
            SelectButton(revenueButton);
        }

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Homepage();
            SelectButton(homeButton);
        }

        private void SetButtonStyle(Button button, Style style)
        {
            button.Style = style;
        }

        private void SelectButton(Button selectedButton)
        {
            // Reset all buttons to default style
            foreach (Button button in new Button[] { salesButton, homeButton, customersButton, creditsButton, salaryButton, revenueButton })
            {
                SetButtonStyle(button, (Style)FindResource("RoundedButtonStyle"));
            }

            // Apply selected style to the selected button
            SetButtonStyle(selectedButton, (Style)FindResource("RoundedButtonStyle_Selected"));
        }
    }
}
