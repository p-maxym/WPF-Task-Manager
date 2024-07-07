using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace WPF_Task_Manager
{
    partial class TaskPanel : UserControl
    {
        public TaskPanel()
        {
            InitializeComponent();
        }

        public void TaskPanelResize(double newWidth, double newHeight)
        {
            TaskPanelBorder.Width = newWidth;
            TaskPanelBorder.Height = newHeight;
        }
    }
}
