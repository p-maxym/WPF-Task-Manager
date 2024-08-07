﻿using SharpVectors.Converters;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

            currentData.Content = $"{textInfo.ToTitleCase(now.ToString("dddd"))}, {textInfo.ToTitleCase(now.ToString("MMMM"))} {now.Day}";
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
            if (calendarImageAndTextCanvas.Visibility == Visibility.Visible)
            {
                double calendarImageLeft = (actualWidth / 2) + (actualWidth <= 360 ? 30 : -150);
                double calendarImageTop = (actualHeight / 2) - (actualHeight + 70);

                Canvas.SetLeft(calendarImage, calendarImageLeft);
                Canvas.SetTop(calendarImage, calendarImageTop);
                Canvas.SetLeft(focusOnYourDay, calendarImageLeft + 45);
                Canvas.SetTop(focusOnYourDay, calendarImageTop + 170);
            }

            // Apply task image
            double applyImageLeft = actualWidth + (actualWidth <= 360 ? 270 : -100);
            double applyImageTop = actualHeight - (actualHeight - 5);

            Canvas.SetLeft(applyImage, applyImageLeft);
            Canvas.SetTop(applyImage, applyImageTop);

            //Data label scaling func call
            DataLabelScaling();
            // ScrollViewer scaling func call
            TasksAndScrollViewerScaling();
        }

        private void DataLabelScaling()
        {
            //Label size
            myDay.FontSize = (_mainWindowActualHeight) / (10 + (_mainWindowActualHeight / 100));
            currentData.FontSize = myDay.FontSize - 15;
            currentData.Margin = new Thickness(0, 0, 0, 25);

            // Data Label
            double dataLabelLeft = _mainWindowActualWidth - (_mainWindowActualWidth + 5);
            double myDayTop = (_mainWindowActualHeight / 2) - (_mainWindowActualHeight * 1.5 - 90);
            double currentDataTop = (_mainWindowActualHeight / 2) - (_mainWindowActualHeight * 1.5 - (105 + myDay.FontSize));

            Canvas.SetLeft(myDay, dataLabelLeft);
            Canvas.SetTop(myDay, myDayTop);
            Canvas.SetLeft(currentData, dataLabelLeft);
            Canvas.SetTop(currentData, currentDataTop);
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
                TaskBox.Background = new BrushConverter().ConvertFromString("#343434") as Brush;
                TaskBorder.Background = new BrushConverter().ConvertFromString("#343434") as Brush;
            }
        }

        private void TaskBox_GotFocus(object sender, RoutedEventArgs e)
        {
            addTaskLabel.Content = "Try typing 'I need to feed the dog'";
            addTaskLabel.Opacity = 0.6;
            plusImage.Visibility = Visibility.Collapsed;
            TaskBox.Background = new BrushConverter().ConvertFromString("#444444") as Brush;
            TaskBorder.Background = new BrushConverter().ConvertFromString("#444444") as Brush;
        }

        private void TaskBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MouseButtonEventArgs mouseEventArgs = new (Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = MouseLeftButtonDownEvent };
                ApplyImage_PreviewMouseDown(sender, mouseEventArgs);
            }
        }

        private async void ApplyImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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

        // Linear interpolation
        private static double Lerp(double start, double end, double t) { return start + (end - start) * t; }
        private double GetTaskTopPosition(int i)
        {
            double t = (_mainWindowActualHeight - 500) / (1080 - 500);
            double startValue = 50 + myDay.FontSize * 2;
            double endValue = myDay.FontSize;

            // i * 80 - indent between tasks
            return (i * 80) + Lerp(startValue, endValue, t);
        } 
        private void TaskBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border) border.Background = new BrushConverter().ConvertFromString("#444444") as Brush;
        }

        private void TaskBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border) border.Background = new BrushConverter().ConvertFromString("#343434") as Brush;
        }

        private void CircleImage_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is SvgViewbox circleImage)
            {
                int index = tasksCircleImage.IndexOf(circleImage);
                SvgViewbox timeImage = tasksCheckImage[index];
                timeImage.Source = new Uri("pack://application:,,,/Resource/check.svg");
                timeImage.Opacity = 0.45;
            }
        }

        private void CircleImage_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is SvgViewbox circleImage)
            {
                int index = tasksCircleImage.IndexOf(circleImage);
                SvgViewbox timeImage = tasksCheckImage[index];
                timeImage.Source = new Uri("pack://application:,,,/Resource/wait-time.svg");
                timeImage.Opacity = 1;
            }
        }
        public void TasksAndScrollViewerScaling()
        {
            double taskScrollViewerTop = _mainWindowActualHeight <= 550 ? -(_mainWindowActualHeight) / 1.54 : -(_mainWindowActualHeight) / 1.3;
            double topCorrection = 90;
            double heightFactor = 1;

            var thresholds = new[]
            {
                new { MaxHeight = 500, HeightFactor = 0.6, TaskScrollViewerTopAdjust = 10, TopCorrectionAdjust = 0 },
                new { MaxHeight = 550, HeightFactor = 0.63, TaskScrollViewerTopAdjust = -8, TopCorrectionAdjust = 0 },
                new { MaxHeight = 650, HeightFactor = 0.66, TaskScrollViewerTopAdjust = 50, TopCorrectionAdjust = 0 },
                new { MaxHeight = 700, HeightFactor = 0.7, TaskScrollViewerTopAdjust = 35, TopCorrectionAdjust = -3 },
                new { MaxHeight = 800, HeightFactor = 0.71, TaskScrollViewerTopAdjust = 30, TopCorrectionAdjust = -13 },
                new { MaxHeight = 900, HeightFactor = 0.72, TaskScrollViewerTopAdjust = 20, TopCorrectionAdjust = -25 },
                new { MaxHeight = 1080, HeightFactor = 0.75, TaskScrollViewerTopAdjust = -10, TopCorrectionAdjust = -50 }
            };

            foreach (var threshold in thresholds)
            {
                if (_mainWindowActualHeight <= threshold.MaxHeight)
                {
                    heightFactor = threshold.HeightFactor;
                    taskScrollViewerTop += threshold.TaskScrollViewerTopAdjust;
                    topCorrection += threshold.TopCorrectionAdjust;
                    break;
                }
            }

            taskScrollViewer.Height = TaskPanelBorder.Height * heightFactor;
            taskScrollViewer.Width = TaskPanelBorder.Width;

            Canvas.SetLeft(taskScrollViewer, -25);
            Canvas.SetTop(taskScrollViewer, taskScrollViewerTop);
            
            taskStackPanel.Height = currentTaskQuantity * 87 > taskScrollViewer.Height ? currentTaskQuantity * 80 : taskScrollViewer.Height;

            Debug.WriteLine(_mainWindowActualHeight);

            // Tasks Borders + Lables
            for (int i = 0; i < currentTaskQuantity; i++)
            {
                tasksBordersList[i].Width = _mainWindowActualWidth + (_mainWindowActualWidth <= 360 ? 320 : -47);

                double top = GetTaskTopPosition(i) - topCorrection;

                // Border
                Canvas.SetTop(tasksBordersList[i], top - 12);

                // Task description
                Canvas.SetTop(tasksLabelList[i], top);

                //Circle + check
                Canvas.SetTop(tasksCheckImage[i], top + 10);
                Canvas.SetTop(tasksCircleImage[i], top);
            }
        }

        //tasks borders list for scaling
        readonly List<Border> tasksBordersList = [];
        readonly List<Label> tasksLabelList = [];
        readonly List<SvgViewbox> tasksCircleImage = [];
        readonly List<SvgViewbox> tasksCheckImage = [];
        private void AddTaskToScrollViewer(string labelText)
        {
            double left = _mainWindowActualWidth - (_mainWindowActualWidth - 100);
            double top = GetTaskTopPosition(currentTaskQuantity);

            Border newBorder = new()
            {
                Width = _mainWindowActualWidth + (_mainWindowActualWidth <= 360 ? 320 : -47),
                Height = 60,
                Background = new BrushConverter().ConvertFromString("#343434") as Brush,
                CornerRadius = new CornerRadius(10),
            };

            newBorder.MouseEnter += TaskBorder_MouseEnter;
            newBorder.MouseLeave += TaskBorder_MouseLeave;

            Label newLabel = new()
            {
                Content = labelText,
                Foreground = Brushes.White,
                FontSize = 17,
                FontWeight = FontWeights.SemiBold,
                IsHitTestVisible = false
            };

            SvgViewbox newCircleImage = new()
            {
                Source = new Uri("pack://application:,,,/Resource/circleimage.svg"),
                Width = 34,
                Height = 34,
                IsHitTestVisible = true,
            };

            SvgViewbox newTimeImage = new()
            {
                Source = new Uri("pack://application:,,,/Resource/wait-time.svg"),
                Width = 15,
                Height = 15,
                IsHitTestVisible = false,
            };

            newCircleImage.MouseEnter += CircleImage_MouseEnter;
            newCircleImage.MouseLeave += CircleImage_MouseLeave;

            Canvas.SetLeft(newBorder, left - 75);
            Canvas.SetTop(newBorder, top - 12);

            Canvas.SetLeft(newLabel, left - 20);
            Canvas.SetTop(newLabel, top);

            Canvas.SetLeft(newCircleImage, left - 65);
            Canvas.SetTop(newCircleImage, top);

            Canvas.SetLeft(newTimeImage, left - 55.5);
            Canvas.SetTop(newTimeImage, top + 10);

            tasksBordersList.Add(newBorder);
            tasksLabelList.Add(newLabel);
            tasksCircleImage.Add(newCircleImage);
            tasksCheckImage.Add(newTimeImage);

            taskScrollViewerCanvas.Children.Add(newBorder);
            taskScrollViewerCanvas.Children.Add(newLabel);
            taskScrollViewerCanvas.Children.Add(newCircleImage);
            taskScrollViewerCanvas.Children.Add(newTimeImage);
        }

        private int currentTaskQuantity = 0;
        public async void TaskGeneration()
        {
            List<TaskOperations> tasks = await TaskOperations.GetTasksByIdAsync("MyDay");

            if (tasks.Count > 0)
            {
                calendarImageAndTextCanvas.Visibility = Visibility.Collapsed;
                taskScrollViewer.Visibility = Visibility.Visible;

                for (;currentTaskQuantity < tasks.Count; currentTaskQuantity++)
                {
                    string? description = tasks[currentTaskQuantity].TaskDescription;

                    if (description != null) AddTaskToScrollViewer(description);
                }
            }
            else
            {
                calendarImageAndTextCanvas.Visibility = Visibility.Visible;
                taskScrollViewer.Visibility = Visibility.Collapsed;
            }
            TasksAndScrollViewerScaling();
        }
    }
}
