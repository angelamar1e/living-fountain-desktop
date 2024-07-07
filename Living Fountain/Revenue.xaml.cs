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
    /// Interaction logic for Revenue.xaml
    /// </summary>
    public partial class Revenue : Page
    {
        public Revenue()
        {
            InitializeComponent();
            revenueFrame.Content = new Weekly_Revenue();
            SelectButton(weekly);
        }

        private void weekly_Click(object sender, RoutedEventArgs e)
        {
            revenueFrame.Content = new Weekly_Revenue();
            SelectButton(weekly);
        }

        private void monthly_Click(object sender, RoutedEventArgs e)
        {
            revenueFrame.Content = new Monthly_Revenue();
            SelectButton(monthly);
        }

        private void SetButtonStyle(Button button, Style style)
        {
            button.Style = style;
        }

        private void SelectButton(Button selectedButton)
        {
            // Reset all buttons to default style
            foreach (Button button in new Button[] { weekly, monthly })
            {
                SetButtonStyle(button, (Style)FindResource("RoundedButtonStyle"));
            }

            // Apply selected style to the selected button
            SetButtonStyle(selectedButton, (Style)FindResource("RoundedButtonStyle_Selected"));
        }
    }
}
