using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPF_Task_Manager
{
    partial class ProfilePanel : UserControl
    {
        public ProfilePanel()
        {
            InitializeComponent();
        }

        public void ProfilePanelVisible(double actualWidth)
        {
            if (actualWidth <= 360) ProfilePanelBorder.Visibility = System.Windows.Visibility.Collapsed;
            else ProfilePanelBorder.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
