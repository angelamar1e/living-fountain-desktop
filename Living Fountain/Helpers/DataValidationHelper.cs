using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Living_Fountain.Helpers
{
    public class DataValidationHelper
    {
        public static void ValidateInput(FrameworkElement field)
        {
            if (field == null)
                return;

            string errorMessage = null;

            if (field is TextBox textBox)
            {
                string validationType = textBox.Tag as string;
                string emptyErrorMessage = (validationType) + " cannot be empty.";
                string input = textBox.Text;

                switch (validationType)
                {
                    case "Block":
                        if (string.IsNullOrEmpty(input))
                        {
                            errorMessage = emptyErrorMessage;
                        }
                        else if (!Regex.IsMatch(input, @"\b(1\d{0,2}|200|[1-9])\b"))
                        {
                            errorMessage = "Invalid block number.";
                        }
                        break;

                    case "Lot":
                        if (string.IsNullOrEmpty(input))
                        {
                            errorMessage = emptyErrorMessage;
                        }
                        else if(!Regex.IsMatch(input, @"\b([1-9]|[1-3][0-9]|40)\b"))
                        {
                            errorMessage = "Invalid lot number.";
                        }
                        break;

                    case "Phase":
                        if (string.IsNullOrEmpty(input))
                        {
                            errorMessage = emptyErrorMessage;
                        }
                        else if(!Regex.IsMatch(input, @"\b[1-7]\b"))
                        {
                            errorMessage = "Invalid phase number.";
                        }
                        break;

                    case "Quantity":
                        if (string.IsNullOrEmpty(input))
                        {
                            errorMessage = emptyErrorMessage;
                        }
                        else if(!Regex.IsMatch(input, @"^(?:[1-9]|[1-9][0-9])$"))
                        {
                            errorMessage = "Invalid quantity.";
                        }
                        break;
                }

                if (errorMessage != null)
                {
                    textBox.ToolTip = errorMessage;
                    textBox.BorderBrush = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    textBox.ToolTip = null;
                    textBox.BorderBrush = System.Windows.Media.Brushes.Black;
                }
            }
            else if (field is ComboBox comboBox)
            {
                if (comboBox.SelectedIndex == -1)
                {
                    errorMessage = "Please select a " + (comboBox.Tag as string) + " option.";
                }

                if (errorMessage != null)
                {
                    comboBox.ToolTip = errorMessage;
                    comboBox.BorderBrush = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    comboBox.ToolTip = null;
                    comboBox.BorderBrush = System.Windows.Media.Brushes.Black;
                }
            }
        }

        public static bool ValidNewOrder(FrameworkElement[] fields)
        {
            List<string> errorFields = new List<string>();

            foreach (var field in fields)
            {
                ValidateInput(field);

                if (!string.IsNullOrEmpty(field.ToolTip as string))
                    errorFields.Add(field.ToolTip as string);
            }

            if (errorFields.Any())
            {
                MessageBox.Show($"The following fields have errors:\n\n- {string.Join("\n- ", errorFields)}",
                                "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
