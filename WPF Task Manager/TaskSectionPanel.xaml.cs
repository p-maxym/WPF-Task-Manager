using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace WPF_Task_Manager
{
    partial class TaskSectionPanel : UserControl
    {
        readonly List<Border> borderList = [];
        public TaskSectionPanel()
        {
            InitializeComponent();
            borderList = [myDayBorder, myDayBorderCheck, importantBorder, importantBorderCheck, groceriesBorder, groceriesBorderCheck, tasksBorder, tasksBorderCheck];
            Loaded += TaskSectionPanel_Loaded;
        }

        private async void TaskSectionPanel_Loaded(object sender, RoutedEventArgs e)
        {
            await AllPendingTasksCount();
        }

        public async Task AllPendingTasksCount()
        {
            List<DBOperations> allTasks = await DBOperations.GetTasksByIdAsync("Tasks", 0);

            var taskCounts = allTasks.GroupBy(task => task.TaskType ?? string.Empty)
                .Where(group => group.Key == "MyDay" || group.Key == "Important" || group.Key == "Groceries" || group.Key == "Tasks")
                .ToDictionary(group => group.Key, group => group.Count(task => task.TaskStatus == "Pending"));

            UpdateUI(taskCounts.TryGetValue("MyDay", out int myDayCount) ? myDayCount : 0, MyDayCountBorder, MyDayCount, -45);

            UpdateUI(taskCounts.TryGetValue("Important", out int importantCount) ? importantCount : 0, ImportantCountBorder, ImportantCount, -45);

            UpdateUI(taskCounts.TryGetValue("Groceries", out int groceriesCount) ? groceriesCount : 0, GroceriesCountBorder, GroceriesCount, -45);

            int allTasksCount = taskCounts.Values.Sum();
            UpdateUI(allTasksCount, TasksCountBorder, TasksCount, -65);
        }

        private void UpdateUI(int count, Border countBorder, TextBlock countTextBlock, int top)
        {
            countBorder.Visibility = count > 0 ? Visibility.Visible : Visibility.Collapsed;
            countTextBlock.Text = count > 0 ? count.ToString() : "";

            if (count > 9 && count < 999)
            {
                countBorder.Width = 25;
                countBorder.CornerRadius = new CornerRadius(9);
                countTextBlock.Margin = new Thickness(272, top, 0, 0);
            }
            else if (count > 999)
            {
                countBorder.Width = 45;
                countBorder.CornerRadius = new CornerRadius(9);
                countTextBlock.Margin = new Thickness(269, top, 0, 0);
                countTextBlock.Text = "+999";
                countTextBlock.Width = 35;
            }
            else
            {
                countBorder.Width = 20;
                countBorder.CornerRadius = new CornerRadius(20);
                countTextBlock.Margin = new Thickness(278, top, 0, 0);
            }
        }

        public void TaskSectionPanelResize(double actualWidth, double actualHeight)
        {
            if (actualWidth <= 380)
            {
                TaskSectionBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                TaskSectionBorder.Height = actualHeight - 185;
                TaskSectionBorder.Visibility = Visibility.Visible;
            }
        }

        private void SectionOpened(object sender)
        {
            Brush selectedBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#727272"));
            Brush defaultBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#343434"));

            for (int i = 0; i < borderList.Count; i += 2)
            {
                Border border = borderList[i];
                border.Background = border == sender ? selectedBrush : defaultBrush;
                borderList[i + 1].Visibility = border == sender ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private static void UpdateSectionUI(string colorTheme, string header, string imagePrefix, string content, string calendarImagePath, double calendarWidth, double calendarHeight, Thickness calendarMargin)
        {
            TasksPanel? md = TasksPanel.MainWindowInstance?.myDayTaskPanel;
            if (md != null)
            {
                try
                {
                    md.UpdateSection(colorTheme, header.Replace(" ", ""), 0);
                    md.focusOnYourDay.Content = content;
                    md.myDay.Content = header;
                    md.plusImage.Source = new Uri($"pack://application:,,,/Resource/plus-{imagePrefix}.svg");
                    md.circleImage.Source = new Uri($"pack://application:,,,/Resource/circleimage-{imagePrefix}.svg");
                    md._circleImagePath = $"pack://application:,,,/Resource/circleimage-{imagePrefix}.svg";
                    md.calendarImage.Source = new BitmapImage(new Uri(calendarImagePath, UriKind.RelativeOrAbsolute));
                    md.calendarImage.Width = calendarWidth;
                    md.calendarImage.Height = calendarHeight;
                    md.calendarImage.Margin = calendarMargin;
                    md.applyImage.Source = new BitmapImage(new Uri($"Resource/arrow-{imagePrefix}.png", UriKind.RelativeOrAbsolute));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
        public void MyDayBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SectionOpened(sender);
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, () => { });
            UpdateSectionUI("#5271ff", "My Day", "5271ff", "       For Today", "Resource/myday-calendar.png", 266, 199, new Thickness(0));
        }

        public void ImportantBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SectionOpened(sender);
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, () => { });
            UpdateSectionUI("#ff5757", "Important", "FF5757", "       Take Note", "Resource/calendarImportant.png", 250, 140, new Thickness(10, 30, 30, 20));
        }

        public void GroceriesBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SectionOpened(sender);
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, () => { });
            UpdateSectionUI("#7ed957", "Groceries", "7ed957", "          To Eat", "Resource/supermarket-shopping.png", 250, 160, new Thickness(7, 15, 0, 20));
        }

        public void TasksBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SectionOpened(sender);
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, () => { });
            UpdateSectionUI("#4fb5dc", "Tasks", "4fb5dc", "          To Do", "Resource/3d-blank.png", 270, 140, new Thickness(5, 30, 0, 0));
        }
    }
}
