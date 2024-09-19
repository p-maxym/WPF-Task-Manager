using MySqlX.XDevAPI.Common;
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
using System.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using ZstdSharp;
using System;
using System.Reflection;
using SkiaSharp;
using System.Runtime.CompilerServices;

namespace WPF_Task_Manager
{
    partial class MyDayTaskPanel : UserControl
    {
        double _mainWindowActualWidth, _mainWindowActualHeight;
        const double minWindowValue = 380;
        readonly SoundPlayer _soundPlayer;
        int _Numeration;
        private string _mainColorTheme = "#5271FF";
        public string _circleImagePath = "pack://application:,,,/Resource/circleimage-5271ff.svg";
        string currentSection = "MyDay";
        int importantStatus = 0;

        List<DBOperations> tasks = [];

        //tasks borders list for scaling
        readonly List<(Border, TextBlock, SvgViewbox, SvgViewbox, Border, SvgViewbox, List<Border>)> taskObjects = [];

        static MainWindow? _mainWindow;
        public static MainWindow? MainWindowInstance
        {
            get => _mainWindow;
            private set => _mainWindow = value;
        }

        public MyDayTaskPanel()
        {
            InitializeComponent();
            _soundPlayer = new SoundPlayer("Resource/servant-bell-ring.wav");
            _soundPlayer.LoadAsync(); // Downloading audio in the background

            TaskGeneration(currentSection, 0);
            LabelDataSet();

            if (Application.Current.MainWindow is MainWindow mainWindow) _mainWindow = mainWindow;
        }

        public void UpdateSection(string mainColorTheme, string taskType, int importantValue)
        {
            if (new BrushConverter().ConvertFromString(mainColorTheme) is Brush brush)
            {
                _mainColorTheme = mainColorTheme;
                myDay.Foreground = brush;
                currentData.Foreground = brush;
                focusOnYourDay.Foreground = brush;
                addTaskLabel.Foreground = brush;
                currentSection = taskType;
                importantStatus = importantValue;
            }
            else Debug.WriteLine("Invalid color string provided.");
            ResetTaskObjects();
            TaskGeneration(currentSection, importantStatus);
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

            bool isSmallWindow = actualWidth <= minWindowValue;

            // Main border resize
            TaskPanelBorder.Width = actualWidth + (isSmallWindow ? 370 : 4);
            TaskPanelBorder.Height = actualHeight - 2;

            // Border content resize
            double marginLeft = isSmallWindow ? 25 : 395;
            TaskPanelBorder.Margin = new Thickness(marginLeft, 12, 21, 25);

            TaskBorder.Width = actualWidth + (isSmallWindow ? 320 : -47);
            TaskBox.Width = actualWidth + (isSmallWindow ? 255 : -112);

            // Calendar image + focus on day label
            double calendarImageLeft = (actualWidth / 2) + (isSmallWindow ? 30 : -150);
            double calendarImageTop = (actualHeight / 2) - (actualHeight + 70);
            Canvas.SetLeft(calendarImage, calendarImageLeft);
            Canvas.SetTop(calendarImage, calendarImageTop);

            // Focus on day label
            double focusOnDayLeft = calendarImageLeft + 20;
            double focusOnDayTop = calendarImageTop + 170;
            Canvas.SetLeft(focusOnYourDay, focusOnDayLeft);
            Canvas.SetTop(focusOnYourDay, focusOnDayTop);

            // Apply task image
            double applyImageLeft = actualWidth + (isSmallWindow ? 270 : -100);
            double applyImageTop = 5;
            Canvas.SetLeft(applyImage, applyImageLeft);
            Canvas.SetTop(applyImage, applyImageTop);

            // Data label scaling func call
            DataLabelScaling();
            // ScrollViewer + tasks scaling func call
            ScrollViewerScaling();
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
                string taskType = currentSection;
                DateTime dueDate = DateTime.Now;

                await DBOperations.AddTaskToDBAsync(taskDescription, taskStatus, taskType, dueDate, currentSection == "Important" ? 1 : 0);
                TaskBox.Text = string.Empty;
                TaskBox_LostFocus(TaskBox, e);

                TaskGeneration(currentSection, importantStatus);
            }
        }

        // Linear interpolation
        private static double Lerp(double start, double end, double t) { return start + (end - start) * t; }
        private double GetTaskTopPosition(int i)
        {
            double t = (_mainWindowActualHeight - 500) / (1080 - 500);
            double startValue = 50 + myDay.FontSize * 2;
            double endValue = myDay.FontSize;

            double topPosition = Lerp(startValue, endValue, t);

            for (int j = 0; j < i; j++)
            {
                topPosition += taskObjects[j].Item1.Height + 20;
            }

            // i * 80 - indent between tasks
            return topPosition;
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

                _Numeration = tasks[TaskSettingsIndex].Numeration;

                //  Obtain absolute coordinates of the border relative to the main window
                Point borderPosition = border.TransformToAncestor(mainWindow).Transform(new Point(0, 0));

                // Calculate panel coordinates
                double currentLeftPositionTaskSettings = borderPosition.X + border.ActualWidth - 400;
                double currentTopPositionTaskSettings = borderPosition.Y - 100;

                Storyboard animation;

                // Check if the panel does not extend beyond the bottom border of the window
                if (currentTopPositionTaskSettings + mainWindow.taskSettingsControl.ActualHeight > mainWindow.ActualHeight - 310)
                {
                    currentTopPositionTaskSettings = mainWindow.ActualHeight - mainWindow.taskSettingsControl.ActualHeight - 450; // bottom margin
                    animation = (Storyboard)mainWindow.Resources["SlideUpAnimation"]; // Animation up
                }
                else animation = (Storyboard)mainWindow.Resources["SlideDownAnimation"]; // Animation down

                // Open panel + animation
                bool status = tasks[TaskSettingsIndex].TaskStatus == "Completed";

                mainWindow.OpenTaskSettingsWindow(currentLeftPositionTaskSettings, currentTopPositionTaskSettings, status);
                animation.Begin(mainWindow.taskSettingsControl);

                // Add a scroll lock handler
                taskScrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;

                if (_mainWindow != null)
                {
                    _mainWindow.taskSettingsControl.deleteBorder.PreviewMouseDown -= DeleteBorder_PreviewMouseDown;
                    _mainWindow.taskSettingsControl.markBorder.PreviewMouseDown -= TaskSettingsMarkPending_PreviewMouseDown;
                    _mainWindow.taskSettingsControl.markBorder.PreviewMouseDown -= TaskSettingsMarkCompleted_PreviewMouseDown;

                    _mainWindow.taskSettingsControl.deleteBorder.PreviewMouseDown += DeleteBorder_PreviewMouseDown;
                    if (status) _mainWindow.taskSettingsControl.markBorder.PreviewMouseDown += TaskSettingsMarkPending_PreviewMouseDown;
                    else _mainWindow.taskSettingsControl.markBorder.PreviewMouseDown += TaskSettingsMarkCompleted_PreviewMouseDown;
                }
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

        private async void DeleteBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            await DBOperations.DeleteTask(_Numeration);
            RemoveTaskByIndex();
        }
        private void TaskSettingsMarkPending_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int index = tasks.FindIndex(t => t.Numeration == _Numeration);

            if (index >= 0)
            {
                MarkTaskAsPending(index);

                if (_mainWindow != null)
                {
                    _mainWindow.taskSettingsControl.Visibility = Visibility.Collapsed;
                    _mainWindow.myDayTaskPanel.taskScrollViewer.PreviewMouseWheel -= _mainWindow.myDayTaskPanel.ScrollViewer_PreviewMouseWheel;
                }
            }
        }

        private void TaskSettingsMarkCompleted_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int index = tasks.FindIndex(t => t.Numeration == _Numeration);

            if (index >= 0)
            {
                MarkTaskAsCompleted(index);

                if (_mainWindow != null)
                {
                    _mainWindow.taskSettingsControl.Visibility = Visibility.Collapsed;
                    _mainWindow.myDayTaskPanel.taskScrollViewer.PreviewMouseWheel -= _mainWindow.myDayTaskPanel.ScrollViewer_PreviewMouseWheel;
                }
            }
        }

        private async void MarkTaskAsCompleted(int index)
        {
            if (tasks[index].TaskStatus != "Completed")
            {
                await DBOperations.MarkTaskAsCompleted(tasks[index].Numeration);
                PlayCompletionSound();
                tasks[index].TaskStatus = "Completed";
            }

            ClearCrossOut(index);
            SvgViewbox image = taskObjects[index].Item4;

            image.Source = new Uri("pack://application:,,,/Resource/check.svg");
            image.Opacity = 1;

            image = taskObjects[index].Item3;
            image.MouseEnter -= CircleImage_MouseEnter;
            image.MouseLeave -= CircleImage_MouseLeave;
            image.PreviewMouseDown -= CircleImage_PreviewMouseDown;

            taskObjects[index].Item2.Opacity = 0.6;

            ScrollViewerScaling();
        }

        private async void MarkTaskAsPending(int index)
        {
            await DBOperations.MarkTaskAsPending(tasks[index].Numeration);
            SvgViewbox image = taskObjects[index].Item4;

            image.Source = new Uri("pack://application:,,,/Resource/wait-time.svg");
            image.Opacity = 1;

            image = taskObjects[index].Item3;
            image.MouseEnter += CircleImage_MouseEnter;
            image.MouseLeave += CircleImage_MouseLeave;
            image.PreviewMouseDown += CircleImage_PreviewMouseDown;

            taskObjects[index].Item2.Opacity = 1;

            ClearCrossOut(index);

            tasks[index].TaskStatus = "Pending";
            ScrollViewerScaling();
        }

        private void PlayCompletionSound()
        {
            try
            {
                // Play Audio
                using var soundPlayer = new SoundPlayer("Resource/servant-bell-ring.wav");
                {
                    Task.Run(_soundPlayer.Play);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        private void CircleImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {   
            if (sender is SvgViewbox circleImage)
            {
                //Find index
                int index = taskObjects.FindIndex(t => t.Item3 == circleImage);

                if (index >= 0)
                {
                    MarkTaskAsCompleted(index);
                }
            }
        }

        private void ScrollViewerScaling()
        {
            double taskScrollViewerTop = _mainWindowActualHeight <= 550 ? -(_mainWindowActualHeight) / 1.54 : -(_mainWindowActualHeight) / 1.3;
            double topCorrection = 90;
            double heightFactor = 1;

            var thresholds = new[]
            {
                new { MaxHeight = 500, HeightFactor = 0.6, TaskScrollViewerTopAdjust = 10, TopCorrectionAdjust = 10 },
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
            
            TasksScaling(topCorrection);
        }

        private void TasksScaling(double topCorrection)
        {
            double allTasksHeight = 0;
            double width = _mainWindowActualWidth + (_mainWindowActualWidth <= minWindowValue ? 320 : -47);
            double dotsLeft = _mainWindowActualWidth + (_mainWindowActualWidth <= minWindowValue ? 305 : -65);

            // Tasks Borders + Labels
            using (Dispatcher.DisableProcessing())
            {
                for (int i = 0; i < currentTaskQuantity; i++)
                {
                    var taskObject = taskObjects[i];

                    if (taskObject.Item2.Text is string contentText)
                    {
                        string cleanedText = contentText.Replace("\n", "");
                        if (taskObject.Item2.Text != cleanedText) taskObject.Item2.Text = CheckTextLength(cleanedText);
                    }

                    double top = GetTaskTopPosition(i) - topCorrection;

                    if (taskObject.Item1.Width != width) taskObject.Item1.Width = width;

                    if (taskObject.Item1.Height != lastTaskHeight) taskObject.Item1.Height = lastTaskHeight;

                    Canvas.SetTop(taskObject.Item1, top - 12);
                    Canvas.SetTop(taskObject.Item2, top + 5);
                    Canvas.SetTop(taskObject.Item3, top);
                    Canvas.SetTop(taskObject.Item4, top + 10);
                    Canvas.SetLeft(taskObject.Item5, dotsLeft - 13);
                    Canvas.SetTop(taskObject.Item5, top + 3);
                    Canvas.SetLeft(taskObject.Item6, dotsLeft);
                    Canvas.SetTop(taskObject.Item6, top + 5);

                    if (tasks[i].TaskStatus == "Completed")
                    {
                        ClearCrossOut(i);

                        var crossOutLines = AddCrossOut(taskObject.Item2, top + 7, taskObject.Item1.Width);
                        taskObjects[i] = (taskObject.Item1, taskObject.Item2, taskObject.Item3, taskObject.Item4, taskObject.Item5, taskObject.Item6, crossOutLines);
                    }

                    allTasksHeight += taskObject.Item1.Height + 20;
                }
            }

            if (taskStackPanel.Height != allTasksHeight && allTasksHeight > taskScrollViewer.Height) taskStackPanel.Height = allTasksHeight;
            else if (taskStackPanel.Height != taskScrollViewer.Height) taskStackPanel.Height = taskScrollViewer.Height;
        }

        // The main function that adds a task
        private void AddTaskToScrollViewer(string labelText)
        {
            // Creating controls
            Border newBorder = CreateTaskBorder();
            TextBlock newTextBlock = CreateTaskText(labelText);
            SvgViewbox newCircleImage = CreateCircleImage();
            SvgViewbox newTimeImage = CreateTimeImage();
            Border dotsHitbox = CreateDotsHitbox();
            SvgViewbox threeDotsImage = CreateThreeDotsImage();


            // Linking events
            newBorder.MouseEnter += TaskBorder_MouseEnter;
            newBorder.MouseLeave += TaskBorder_MouseLeave;

            newCircleImage.MouseEnter += CircleImage_MouseEnter;
            newCircleImage.MouseLeave += CircleImage_MouseLeave;
            newCircleImage.PreviewMouseDown += CircleImage_PreviewMouseDown;

            dotsHitbox.MouseEnter += DotsBorder_MouseEnter;
            dotsHitbox.MouseLeave += DotsBorder_MouseLeave;
            dotsHitbox.MouseDown += DotsBorder_MouseDown;

            //Position
            PositionTaskElements(newBorder, newTextBlock, newCircleImage, newTimeImage, dotsHitbox, threeDotsImage);

            // Add to task list
            taskObjects.Add(new(newBorder, newTextBlock, newCircleImage, newTimeImage, dotsHitbox, threeDotsImage, []));

            // Add on Canvas
            var items = taskObjects[currentTaskQuantity];
            foreach (var item in new UIElement[] { items.Item1, items.Item2, items.Item3, items.Item4, items.Item5, items.Item6 })
            {
                taskScrollViewerCanvas.Children.Add(item);
            }
        }

        // Functions for creating controls
        private Border CreateTaskBorder()
        {
            return new Border
            {
                Width = _mainWindowActualWidth + (_mainWindowActualWidth <= minWindowValue ? 320 : -47),
                Height = lastTaskHeight,
                Background = new BrushConverter().ConvertFromString("#343434") as Brush,
                CornerRadius = new CornerRadius(10),
            };
        }

        private Border CreateCrossOutLine(double width)
        {
            return new Border
            {
                Width = width,
                Height = 2.5,
                Background = new BrushConverter().ConvertFromString(_mainColorTheme) as Brush,
                CornerRadius = new CornerRadius(2),
            };
        }

        private TextBlock CreateTaskText(string labelText)
        {
            return new TextBlock
            {
                Text = CheckTextLength(labelText),
                Foreground = Brushes.White,
                FontSize = 17,
                FontWeight = FontWeights.SemiBold,
                IsHitTestVisible = false
            };
        }

        private SvgViewbox CreateCircleImage()
        {
            return new SvgViewbox
            {
                Source = new Uri(_circleImagePath),
                Width = 34,
                Height = 34,    
                IsHitTestVisible = true,
            };
        }

        private static SvgViewbox CreateTimeImage()
        {
            return new SvgViewbox
            {
                Source = new Uri("pack://application:,,,/Resource/wait-time.svg"),
                Width = 15,
                Height = 15,
                IsHitTestVisible = false,
            };
        }

        private static Border CreateDotsHitbox()
        {
            return new Border
            {
                Width = 50,
                Height = 30,
                IsHitTestVisible = true,
                CornerRadius = new CornerRadius(8),
                Background = Brushes.Transparent
            };
        }

        private static SvgViewbox CreateThreeDotsImage()
        {
            return new SvgViewbox
            {
                Source = new Uri("pack://application:,,,/Resource/dots3.svg"),
                Width = 25,
                Height = 25,
                IsHitTestVisible = false,
            };
        }

        // Element positioning function
        private void PositionTaskElements(Border newBorder, TextBlock newTextBlock, SvgViewbox newCircleImage, SvgViewbox newTimeImage, Border dotsHitbox, SvgViewbox threeDotsImage)
        {
            double left = 100;
            double top = GetTaskTopPosition(currentTaskQuantity);
            double dotsLeft = _mainWindowActualWidth + (_mainWindowActualWidth <= minWindowValue ? 305 : -65);

            using (Dispatcher.DisableProcessing())
            {
                Canvas.SetLeft(newBorder, left - 75);
                Canvas.SetTop(newBorder, top - 12);

                Canvas.SetLeft(newTextBlock, left - 15);
                Canvas.SetTop(newTextBlock, top);

                Canvas.SetLeft(newCircleImage, left - 65);
                Canvas.SetTop(newCircleImage, top);

                Canvas.SetLeft(newTimeImage, left - 55.5);
                Canvas.SetTop(newTimeImage, top + 10);

                Canvas.SetLeft(dotsHitbox, dotsLeft);
                Canvas.SetTop(dotsHitbox, top);

                Canvas.SetLeft(threeDotsImage, dotsLeft);
                Canvas.SetTop(threeDotsImage, top);
            }
        }

        double lastTaskHeight;
        private string CheckTextLength(string labelText)
        {
            lastTaskHeight = 60;
            string text = "";
            string tempText = "";

            foreach (var letter in labelText)
            {
                string newTempText = tempText + letter;

                // Use FormattedText to calculate the text width
                var formattedText = new FormattedText(
                    newTempText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    new Typeface("Segoe UI Semibold"), 17, Brushes.Black, new NumberSubstitution(), 1
                );

                // Check text width
                if (formattedText.Width > TaskBorder.Width * 0.85 - 85)
                {
                    text += "\n" + letter;
                    tempText = letter.ToString();
                    lastTaskHeight += 22.5; 
                }
                else
                {
                    text += letter;
                    tempText = newTempText;
                }
            }
            return text;
        }

        private List<Border> AddCrossOut(TextBlock taskTextBlock, double top, double borderWidth)
        {
            List<Border> crossOutLines = [];

            double left = Canvas.GetLeft(taskTextBlock) - 5;
            double lineHeight = 22.5;
            double currentTop = top + 10;

            string[] lines = taskTextBlock.Text.Split('\n');
            int numberOfLines = lines.Length;

            double lineWidth = borderWidth * 0.90 - 85;

            for (int i = 0; i < numberOfLines; i++)
            {
                Border crossOutLine = CreateCrossOutLine(lineWidth);

                Canvas.SetTop(crossOutLine, currentTop);
                Canvas.SetLeft(crossOutLine, left);

                taskScrollViewerCanvas.Children.Add(crossOutLine);
                crossOutLines.Add(crossOutLine);

                currentTop += lineHeight;
            }

            return crossOutLines;
        }

        private void ClearCrossOut(int index)
        {
            var taskObject = taskObjects[index];

            if (taskObject.Item7.Count > 0)
            {
                foreach (var line in taskObject.Item7)
                {
                    taskScrollViewerCanvas.Children.Remove(line);
                }
                taskObject.Item7.Clear(); // Clear the list after removing old lines
            }
        }

        private void RemoveTaskByIndex()
        {
            int index = tasks.FindIndex(t => t.Numeration == _Numeration);

            if (index >= 0 && index < taskObjects.Count && index < tasks.Count)
            {
                var taskObject = taskObjects[index];

                taskScrollViewerCanvas.Children.Remove(taskObject.Item1);
                taskScrollViewerCanvas.Children.Remove(taskObject.Item2);
                taskScrollViewerCanvas.Children.Remove(taskObject.Item3);
                taskScrollViewerCanvas.Children.Remove(taskObject.Item4);
                taskScrollViewerCanvas.Children.Remove(taskObject.Item5);
                taskScrollViewerCanvas.Children.Remove(taskObject.Item6);

                ClearCrossOut(index);

                if (_mainWindow != null)
                {
                    _mainWindow.taskSettingsControl.Visibility = Visibility.Collapsed;
                    _mainWindow.myDayTaskPanel.taskScrollViewer.PreviewMouseWheel -= _mainWindow.myDayTaskPanel.ScrollViewer_PreviewMouseWheel;
                }

                taskObjects.RemoveAt(index);
                tasks.RemoveAt(index);
                currentTaskQuantity = tasks.Count;

                if(tasks.Count == 0) calendarImageAndTextCanvas.Visibility = Visibility.Visible;

                ScrollViewerScaling();
            }
        }

        private void ResetTaskObjects()
        {
            taskScrollViewerCanvas.Children.Clear();
            taskObjects.Clear();
            currentTaskQuantity = 0;
        }

        int currentTaskQuantity = 0;
        private async void TaskGeneration(string taskType, int importantValue)
        {
            tasks = await DBOperations.GetTasksByIdAsync(taskType, importantValue);

            if (tasks.Count > 0)
            {
                calendarImageAndTextCanvas.Visibility = Visibility.Collapsed;
                taskScrollViewer.Visibility = Visibility.Visible;

                for (;currentTaskQuantity < tasks.Count; currentTaskQuantity++)
                {
                    string? description = tasks[currentTaskQuantity].TaskDescription;

                    if (description != null) AddTaskToScrollViewer(description);

                    if (tasks[currentTaskQuantity].TaskStatus == "Completed") MarkTaskAsCompleted(currentTaskQuantity);
                }
            }
            else
            {
                calendarImageAndTextCanvas.Visibility = Visibility.Visible;
                taskScrollViewer.Visibility = Visibility.Collapsed;
            }
            ScrollViewerScaling();
        }
    }
}