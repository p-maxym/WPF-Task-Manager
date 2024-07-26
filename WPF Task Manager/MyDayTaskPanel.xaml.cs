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
        private double _mainWindowActualWidth, _mainWindowActualHeight;

        public MyDayTaskPanel()
        {
            InitializeComponent();

            LabelDataSet();
            TaskGeneration();
        }

        private void LabelDataSet()
        {
            DateTime now = DateTime.Now;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            currentData.Content = $"{textInfo.ToTitleCase(now.ToString("dddd"))}, {textInfo.ToTitleCase(now.ToString("MMMM"))} {now.Day.ToString()}";
        }
       
        public void TaskPanelContentScaling(double actualWidth, double actualHeight)
        {
            _mainWindowActualWidth = actualWidth;
            _mainWindowActualHeight = actualHeight;

            // Main border resize
            TaskPanelBorder.Width = actualWidth + (actualWidth <= 360 ? 370 : 4);
            TaskPanelBorder.Height = actualHeight - 2;

            // Border content resize
            double marginLeft = actualWidth <= 360 ? 25 : 395;

            TaskPanelBorder.Margin = new Thickness(marginLeft, 12, 21, 25);
            TaskBorder.Width = actualWidth + (actualWidth <= 360 ? 320 : -47);
            TaskBox.Width = actualWidth + (actualWidth <= 360 ? 255 : -112);

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

            // ScrollViewer scaling call
            TasksAndScrollViewerScaling();
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

        private async void applyImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(TaskBox.Text))
            {
                string taskDescription = TaskBox.Text;
                string taskStatus = "Pending";
                string taskType = "MyDay";
                DateTime dueDate = DateTime.Now;

                await TaskOperations.AddTaskToDBAsync(taskDescription, taskStatus, taskType, dueDate);
                TaskBox.Text = string.Empty;
                TaskBox_LostFocus(TaskBox, e);
                TaskGeneration();
            }
        }
        public void TasksAndScrollViewerScaling()
        {
            // Set the ScrollViewer height to 40% of the Border height
            double taskScrollViewerHeight = _mainWindowActualHeight * 0.8;

            // Center the ScrollViewer vertically within the Border
            double taskScrollViewerTop = -(_mainWindowActualHeight - taskScrollViewerHeight) * 4.1;


            // Set the width to the same as the Border
            taskScrollViewer.Width = TaskPanelBorder.Width;
            taskScrollViewer.Height = taskScrollViewerHeight;

            // Fixed left position
            double taskScrollViewerLeft = -25;

            // Set the position of the ScrollViewer
            Canvas.SetLeft(taskScrollViewer, taskScrollViewerLeft);
            Canvas.SetTop(taskScrollViewer, taskScrollViewerTop);
        }

        private void AddTaskLabelToScrollViewer(string labelText, string labelName, double left, double top)
        {
            Border border = new Border
            {
                Width = TaskPanelBorder.Width - 48,
                Height = 60,
                Background = new BrushConverter().ConvertFromString("#343434") as Brush,
                CornerRadius = new CornerRadius(20),
            };

            Label newLabel = new Label
            {
                Content = labelText,
                Foreground = Brushes.White,
                FontSize = 17,
                FontWeight = FontWeights.SemiBold
            };

            Canvas.SetLeft(newLabel, left);
            Canvas.SetTop(newLabel, top);

            Canvas.SetLeft(border, left - 75);
            Canvas.SetTop(border, top - 12);

            taskScrollViewerCanvas.Children.Add(border);
            taskScrollViewerCanvas.Children.Add(newLabel);
        }

        private int currentTaskQuantity = 0;
        private double _top = 0;
        private double _left;
        private double _taskHeight = 450;
        public async void TaskGeneration()
        {
            List<TaskOperations> tasks = await TaskOperations.GetTasksByIdAsync("MyDay");

            if (tasks.Count > 0)
            {
                calendarImage.Visibility = Visibility.Collapsed;
                focusOnYourDay.Visibility = Visibility.Collapsed;

                for(; currentTaskQuantity < tasks.Count; currentTaskQuantity++)
                {
                    string? description = tasks[currentTaskQuantity].TaskDescription;
                    string name = "task" + currentTaskQuantity.ToString();
                    _left = (TaskPanelBorder.Width / 2) + (TaskPanelBorder.Width <= 360 ? 30 : -200);
                    _top = (TaskPanelBorder.Height / 2) - (TaskPanelBorder.Height - _taskHeight);

                    if (description != null)
                    {
                        AddTaskLabelToScrollViewer(description, name, _left, _top);
                        _taskHeight += 70;
                    }
                }
            }
        }
    }
}
