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

        public void TaskSectionPanelResize(double newHeight)
        {
            TaskSectionBorder.Height = newHeight;
        }
    }
}
