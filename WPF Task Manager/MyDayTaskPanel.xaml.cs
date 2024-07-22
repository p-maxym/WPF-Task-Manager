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
    partial class MyDayTaskPanel : UserControl
    {
        public MyDayTaskPanel()
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
            // Main border resize
            TaskPanelBorder.Width = actualWidth + (actualWidth <= 360 ? 370 : 0);
            TaskPanelBorder.Height = actualHeight;

            // Border content resize
            double marginLeft = actualWidth <= 360 ? 25 : 397;
            double marginTop = actualWidth <= 360 ? 10 : 10;

            TaskPanelBorder.Margin = new Thickness(marginLeft, marginTop, 21, 25);
            TaskBorder.Width = actualWidth + (actualWidth <= 360 ? 320 : -50);
            TaskBox.Width = actualWidth + (actualWidth <= 360 ? 255 : -115);

            // Calendar image + focus on day label
            if (calendarImage.Visibility == Visibility.Visible)
            {
                double calendarImageLeft = (actualWidth / 2) + (actualWidth <= 360 ? 30 : -150);
                double calendarImageTop = (actualHeight / 2) - (actualHeight + 70);
                double focusOnYourDayLeft = (actualWidth / 2) + (actualWidth <= 360 ? 75 : -105);
                double focusOnYourDayTop = (actualHeight / 2) - (actualHeight - 100);

                Canvas.SetLeft(calendarImage, calendarImageLeft);
                Canvas.SetTop(calendarImage, calendarImageTop);
                Canvas.SetLeft(focusOnYourDay, focusOnYourDayLeft);
                Canvas.SetTop(focusOnYourDay, focusOnYourDayTop);
            }

            // Data Label
            double dataLabelLeft = actualWidth - (actualWidth + 5);
            double myDayTop = (actualHeight / 2) - (actualHeight * 1.5 - 90);
            double currentDataTop = (actualHeight / 2) - (actualHeight * 1.5 - 130);

            Canvas.SetLeft(myDay, dataLabelLeft);
            Canvas.SetTop(myDay, myDayTop);
            Canvas.SetLeft(currentData, dataLabelLeft);
            Canvas.SetTop(currentData, currentDataTop);

            // Apply task image
            double applyImageLeft = actualWidth + (actualWidth <= 360 ? 270 : -100);
            double applyImageTop = actualHeight - (actualHeight - 5);

            Canvas.SetLeft(applyImage, applyImageLeft);
            Canvas.SetTop(applyImage, applyImageTop);
        }

        private void TaskBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            TaskBorder.Background = new BrushConverter().ConvertFromString("#444444") as Brush;
            TaskBox.Background = new BrushConverter().ConvertFromString("#444444") as Brush;
        }
        private void TaskBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            TaskBorder.Background = new BrushConverter().ConvertFromString("#343434") as Brush;
            TaskBox.Background = new BrushConverter().ConvertFromString("#343434") as Brush;
        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e) => TaskBorder_MouseEnter(sender, e);
        private void TextBox_MouseLeave(object sender, MouseEventArgs e) => TaskBorder_MouseLeave(sender, e);

        private void TaskBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TaskBox.Text))
            {
                addTaskLabel.Visibility = Visibility.Collapsed;
                plusImage.Visibility = Visibility.Collapsed;
                applyImage.Visibility = Visibility.Visible;
            }

            else
            {
                addTaskLabel.Visibility = Visibility.Visible;
                applyImage.Visibility = Visibility.Collapsed;
            }
        }

        private void TaskBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TaskBox.Text))
            {
                addTaskLabel.Content = "Add a task";
                addTaskLabel.Opacity = 1;
                addTaskLabel.Visibility = Visibility.Visible;
                plusImage.Visibility = Visibility.Visible;
            }
        }

        private void TaskBox_GotFocus(object sender, RoutedEventArgs e)
        {
            addTaskLabel.Content = "Try typing 'I need to feed the dog'";
            addTaskLabel.Opacity = 0.6;
            plusImage.Visibility = Visibility.Collapsed;
        }

        private void applyImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(TaskBox.Text))
            {
                string taskDescription = TaskBox.Text;
                string taskStatus = "Pending";
                string taskType = "MyDay";
                DateTime dueDate = DateTime.Now;

                _ = TaskOperations.AddTaskToDBAsync(taskDescription, taskStatus, taskType, dueDate);
            }
        }
    }
}
