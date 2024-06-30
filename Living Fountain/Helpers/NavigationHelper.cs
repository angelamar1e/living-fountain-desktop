using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Living_Fountain.Helpers
{
    public class NavigationHelper
    {
        public static Frame FindParentFrame(DependencyObject child)
        {
            // Traverse up the visual tree to find the parent Frame
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            if (parentObject is Frame frame)
            {
                return frame;
            }

            return FindParentFrame(parentObject);
        }
    }
}
