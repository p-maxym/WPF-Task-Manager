using SharpVectors.Converters;
using SharpVectors.Dom;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
                HiddenFocusElement.Focus();
            }
        }

        private void TaskBox_GotFocus(object sender, RoutedEventArgs e)
        {
            addTaskLabel.Content = "Try typing 'I need to feed the dog'";
            addTaskLabel.Opacity = 0.6;
            plusImage.Visibility = Visibility.Collapsed;
            TaskBox.Background = new BrushConverter().ConvertFromString("#404040") as Brush;
            TaskBorder.Background = new BrushConverter().ConvertFromString("#404040") as Brush;
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

                await DBOperations.AddTaskToDBAsync(taskDescription, taskStatus, taskType, dueDate);
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
            if (sender is Border border) border.Background = new BrushConverter().ConvertFromString("#404040") as Brush;
        }

        private void TaskBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border) border.Background = new BrushConverter().ConvertFromString("#343434") as Brush;
        }

        private void DotsBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border) border.Background = new BrushConverter().ConvertFromString("#505050") as Brush;
        }

        private void DotsBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border) border.Background = Brushes.Transparent;
        }

        private void DotsBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow && sender is Border border)
            {
                int TaskSettingsIndex = taskObjects.FindIndex(t => t.Item5 == border);

                //  Obtain absolute coordinates of the border relative to the main window
                Point borderPosition = border.TransformToAncestor(mainWindow).Transform(new Point(0, 0));

                // Calculate panel coordinates
                double currentLeftPositionTaskSettings = borderPosition.X + border.ActualWidth - 400;
                double currentTopPositionTaskSettings = borderPosition.Y - 100;

                Storyboard animation;

                // Check if the panel does not extend beyond the bottom border of the window
                if (currentTopPositionTaskSettings + mainWindow.taskSettingsControl.ActualHeight > mainWindow.ActualHeight - 300)
                {
                    currentTopPositionTaskSettings = mainWindow.ActualHeight - mainWindow.taskSettingsControl.ActualHeight - 450; // bottom margin
                    animation = (Storyboard)mainWindow.Resources["SlideUpAnimation"]; // Animation up
                }
                else animation = (Storyboard)mainWindow.Resources["SlideDownAnimation"]; // Animation down

                // Open panel + animation
                mainWindow.OpenTaskSettingsWindow(currentLeftPositionTaskSettings, currentTopPositionTaskSettings);
                animation.Begin(mainWindow.taskSettingsControl);

                // Add a scroll lock handler
                taskScrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
            }
        }

        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true; // Block scrolling
        }

        private void CircleImage_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is SvgViewbox circleImage)
            {
                //Find index
                int index = taskObjects.FindIndex(t => t.Item3 == circleImage);

                if (index >= 0)
                {
                    SvgViewbox timeImage = taskObjects[index].Item4;
                    timeImage.Source = new Uri("pack://application:,,,/Resource/check.svg");
                    timeImage.Opacity = 0.5;
                }
            }
        }

        private void CircleImage_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is SvgViewbox circleImage)
            {
                //Find index
                int index = taskObjects.FindIndex(t => t.Item3 == circleImage);

                if (index >= 0)
                {
                    SvgViewbox timeImage = taskObjects[index].Item4;
                    timeImage.Source = new Uri("pack://application:,,,/Resource/wait-time.svg");
                    timeImage.Opacity = 1;
                }
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

            // Tasks Borders + Lables
            for (int i = 0; i < currentTaskQuantity; i++)
            {
                taskObjects[i].Item1.Width = _mainWindowActualWidth + (_mainWindowActualWidth <= 360 ? 320 : -47);

                double top = GetTaskTopPosition(i) - topCorrection;

                // Border
                Canvas.SetTop(taskObjects[i].Item1, top - 12);

                // Task description
                Canvas.SetTop(taskObjects[i].Item2, top);

                //Circle + check + 3dots
                Canvas.SetTop(taskObjects[i].Item3, top);
                Canvas.SetTop(taskObjects[i].Item4, top + 10);

                //3 dots + border
                double dotsLeft = _mainWindowActualWidth + (_mainWindowActualWidth <= 360 ? 305 : -65);
                Canvas.SetLeft(taskObjects[i].Item5, dotsLeft - 13);
                Canvas.SetTop(taskObjects[i].Item5, top + 3);

                Canvas.SetLeft(taskObjects[i].Item6, dotsLeft);
                Canvas.SetTop(taskObjects[i].Item6, top + 5);
            }
        }

        //tasks borders list for scaling
        readonly List<(Border, Label, SvgViewbox, SvgViewbox, Border, SvgViewbox)> taskObjects = [];
        private void AddTaskToScrollViewer(string labelText)
        {
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

            newCircleImage.MouseEnter += CircleImage_MouseEnter;
            newCircleImage.MouseLeave += CircleImage_MouseLeave;

            SvgViewbox newTimeImage = new()
            {
                Source = new Uri("pack://application:,,,/Resource/wait-time.svg"),
                Width = 15,
                Height = 15,
                IsHitTestVisible = false,
            };

            Border dotsHitbox = new()
            {
                Width = 50,
                Height = 30,
                IsHitTestVisible = true,
                CornerRadius = new CornerRadius(8),
                Background = Brushes.Transparent
            };

            dotsHitbox.MouseEnter += DotsBorder_MouseEnter;
            dotsHitbox.MouseLeave += DotsBorder_MouseLeave;
            dotsHitbox.MouseDown += DotsBorder_MouseDown;

            SvgViewbox threeDotsImage = new()
            {
                Source = new Uri("pack://application:,,,/Resource/dots3.svg"),
                Width = 25,
                Height = 25,
                IsHitTestVisible = false,
            };

            double left = _mainWindowActualWidth - (_mainWindowActualWidth - 100);
            double top = GetTaskTopPosition(currentTaskQuantity);
            double dotsLeft = _mainWindowActualWidth + (_mainWindowActualWidth <= 360 ? 305 : -65);

            Canvas.SetLeft(newBorder, left - 75);
            Canvas.SetTop(newBorder, top - 12);

            Canvas.SetLeft(newLabel, left - 20);
            Canvas.SetTop(newLabel, top);

            Canvas.SetLeft(newCircleImage, left - 65);
            Canvas.SetTop(newCircleImage, top);

            Canvas.SetLeft(newTimeImage, left - 55.5);
            Canvas.SetTop(newTimeImage, top + 10);

            Canvas.SetLeft(dotsHitbox, dotsLeft);
            Canvas.SetTop(dotsHitbox, top);

            Canvas.SetLeft(threeDotsImage, dotsLeft);
            Canvas.SetTop(threeDotsImage, top);

            taskObjects.Add(new(newBorder, newLabel, newCircleImage, newTimeImage,dotsHitbox, threeDotsImage));

            //Add to Canvas
            var items = taskObjects[currentTaskQuantity];
            foreach (var item in new UIElement[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6 })
            {
                taskScrollViewerCanvas.Children.Add(item);
            }
        }

        private int currentTaskQuantity = 0;
        public async void TaskGeneration()
        {
            List<DBOperations> tasks = await DBOperations.GetTasksByIdAsync("MyDay");

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
