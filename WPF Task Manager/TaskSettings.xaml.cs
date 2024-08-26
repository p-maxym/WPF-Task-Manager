using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace WPF_Task_Manager
{
    partial class TaskSettings : UserControl
    {
        public TaskSettings()
        {
            InitializeComponent();
        }

        private void TaskSettings_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Intercept a click inside TaskSettings to prevent it from being closed
            e.Handled = true;
        }

        public void MarkTaskAsNotCompleted(bool status)
        {
            if (status)
            {
                setStatusImage.Source = new Uri("pack://application:,,,/Resource/grey-circle.svg");
                setStatusLabel.Text = "Mark as not completed";
            }
            else
            {
                setStatusImage.Source = new Uri("pack://application:,,,/Resource/circle-check.svg");
                setStatusLabel.Text = "Mark as completed";
            }
        }
    }
}
