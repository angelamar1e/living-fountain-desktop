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
    /// Interaction logic for Salary.xaml
    /// </summary>
    public partial class Salary : Page
    {
        public Salary()
        {
            InitializeComponent();

            CurrentDateTextBlock.Text = DateTime.Now.ToString("D"); // Long date pattern (e.g., Wednesday, June 20, 2024)
        }
    }
}
