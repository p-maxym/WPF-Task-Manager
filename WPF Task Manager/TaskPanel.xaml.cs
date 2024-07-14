using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF_Task_Manager
{
    partial class TaskPanel : UserControl
    {
        public TaskPanel()
        {
            InitializeComponent();

            LabelDataSet();
        }

        private void LabelDataSet()
        {
            DateTime now = DateTime.Now;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            currentData.Content = $"{textInfo.ToTitleCase(now.ToString("dddd"))}, {textInfo.ToTitleCase(now.ToString("MMMM"))} {now.Day.ToString()}";
        }
       
        public void TaskPanelContentResize(double actualWidth, double actualHeight)
        {
            if (actualWidth <= 360)
            {
                // Main border resize
                TaskPanelBorder.Width = actualWidth + 370;
                TaskPanelBorder.Height = actualHeight;

                // Border content resize
                TaskPanelBorder.Margin = new Thickness(25, 10, 21, 25);
                TaskBorder.Width = actualWidth + 320;
                TaskBox.Width = actualWidth + 245;

                // Calendar image
                if (calendarImage.Visibility == Visibility.Visible)
                {
                    Canvas.SetLeft(calendarImage, (actualWidth / 2) + 30);
                    Canvas.SetTop(calendarImage, (actualHeight / 2) - (actualHeight + 70));
                }

                //Data Label
                Canvas.SetLeft(myDay, actualWidth - (actualWidth + 5));
                Canvas.SetTop(myDay, (actualHeight / 2) - (actualHeight * 1.5 - 90));

                Canvas.SetLeft(currentData, actualWidth - (actualWidth + 5));
                Canvas.SetTop(currentData, (actualHeight / 2) - (actualHeight * 1.5 - 130));
            }

            else
            {
                // Main border resize
                TaskPanelBorder.Width = actualWidth;
                TaskPanelBorder.Height = actualHeight;

                // Border content resize
                TaskPanelBorder.Margin = new Thickness(397, 10, 21, 25);
                TaskBorder.Width = actualWidth - 50;
                TaskBox.Width = actualWidth - 122;

                // Calendar image
                if (calendarImage.Visibility == Visibility.Visible)
                {
                    Canvas.SetLeft(calendarImage, (actualWidth / 2) - 150);
                    Canvas.SetTop(calendarImage, (actualHeight / 2) - (actualHeight + 70));
                }

                //Data Label
                Canvas.SetLeft(myDay, actualWidth - (actualWidth + 5));
                Canvas.SetTop(myDay, (actualHeight / 2) - (actualHeight * 1.5 - 90));

                Canvas.SetLeft(currentData, actualWidth - (actualWidth + 5));
                Canvas.SetTop(currentData, (actualHeight / 2) - (actualHeight * 1.5 - 130));
            }
        }

        private void TaskBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            TaskBorder.Background = new BrushConverter().ConvertFromString("#504F4F") as Brush;
            TaskBox.Background = new BrushConverter().ConvertFromString("#504F4F") as Brush;
        }

        private void TaskBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            TaskBorder.Background = new BrushConverter().ConvertFromString("#343434") as Brush;
            TaskBox.Background = new BrushConverter().ConvertFromString("#343434") as Brush;
        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            TaskBorder.Background = new BrushConverter().ConvertFromString("#504F4F") as Brush;
            TaskBox.Background = new BrushConverter().ConvertFromString("#504F4F") as Brush;
        }

        private void TextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            TaskBorder.Background = new BrushConverter().ConvertFromString("#343434") as Brush;
            TaskBox.Background = new BrushConverter().ConvertFromString("#343434") as Brush;
        }

        private void TaskPanelBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TaskBox.IsFocused) HiddenFocusElement.Focus();
        }

        private void TaskBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TaskBox.Text))
            {
                addTaskLabel.Visibility = Visibility.Collapsed;
                plusImage.Visibility = Visibility.Collapsed;
            }
            else
            {
                addTaskLabel.Visibility = Visibility.Visible;
                plusImage.Visibility = Visibility.Visible;
            }
        }

        private void TaskBox_GotFocus(object sender, RoutedEventArgs e)
        {
            addTaskLabel.Visibility = Visibility.Collapsed;
            plusImage.Visibility = Visibility.Collapsed;
        }
    }
}
