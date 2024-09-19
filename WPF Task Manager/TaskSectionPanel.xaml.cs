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

namespace WPF_Task_Manager
{
    partial class TaskSectionPanel : UserControl
    {
        readonly List<Border> borderList = [];
        MyDayTaskPanel? md;
        public TaskSectionPanel()
        {
            InitializeComponent();
            borderList = [myDayBorder, myDayBorderCheck, importantBorder, importantBorderCheck, groceriesBorder, groceriesBorderCheck, tasksBorder, tasksBorderCheck];
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

        private void UpdateSectionUI(string colorTheme, string header, string imagePrefix, string content, string calendarImagePath, double calendarWidth, double calendarHeight, Thickness calendarMargin)
        {
            md = MyDayTaskPanel.MainWindowInstance?.myDayTaskPanel;
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
