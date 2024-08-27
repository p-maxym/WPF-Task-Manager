using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace WPF_Task_Manager
{
    partial class TaskSettings : UserControl
    {
        public static int _Numeration;
        public TaskSettings()
        {
            InitializeComponent();
        }

        private void TaskSettings_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Intercept a click inside TaskSettings to prevent it from being closed
            e.Handled = true;
        }

        private void SettingsBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border) border.Background = new BrushConverter().ConvertFromString("#242424") as Brush;
        }

        private void SettingsBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border) border.Background = new BrushConverter().ConvertFromString("#1B1B1B") as Brush;
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
