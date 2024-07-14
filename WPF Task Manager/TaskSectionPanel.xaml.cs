using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPF_Task_Manager
{
    partial class TaskSectionPanel : UserControl
    {
        public TaskSectionPanel()
        {
            InitializeComponent();
        }

        public void TaskSectionPanelResize(double actualWidth, double actualHeight)
        {
            if (actualWidth <= 360)
            {
                TaskSectionBorder.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                TaskSectionBorder.Height = actualHeight - 185;
                TaskSectionBorder.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
