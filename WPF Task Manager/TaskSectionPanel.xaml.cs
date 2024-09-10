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
            for(int i = 0; i < borderList.Count; i+=2)
            {
                Border border = borderList[i];
                if (border == sender)
                {
                    border.Background = new BrushConverter().ConvertFromString("#727272") as Brush;
                    borderList[i + 1].Visibility = Visibility.Visible;
                }
                else
                {
                    border.Background = new BrushConverter().ConvertFromString("#343434") as Brush;
                    borderList[i + 1].Visibility = Visibility.Collapsed;
                }
            }
        }

        public void MyDayBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var md = MyDayTaskPanel.MainWindowInstance?.myDayTaskPanel;
            if (md != null)
            {
                try
                {
                    SectionOpened(sender);
                    md.UpdateSection("#5271ff", "MyDay", 0);
                    md.focusOnYourDay.Content = "Focus on your day";
                    md.myDay.Content = "My Day";
                    md.plusImage.Source = new Uri("pack://application:,,,/Resource/plus-5271ff.svg");
                    md.circleImage.Source = new Uri("pack://application:,,,/Resource/circleimage-5271ff.svg");
                    md._circleImagePath = "pack://application:,,,/Resource/circleimage-5271ff.svg";
                    md.calendarImage.Source = new BitmapImage(new Uri("Resource/myday-calendar.png", UriKind.RelativeOrAbsolute));
                    md.calendarImage.Width = 266;
                    md.calendarImage.Height = 199;
                    md.calendarImage.Margin = new(0);
                    md.applyImage.Source = new BitmapImage(new Uri("Resource/arrow-5271ff.png", UriKind.RelativeOrAbsolute));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public void ImportantBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var md = MyDayTaskPanel.MainWindowInstance?.myDayTaskPanel;
            if (md != null)
            {
                try
                {
                    SectionOpened(sender);
                    md.UpdateSection("#ff5757", "Important", 1);
                    md.focusOnYourDay.Content = "       Take Note";
                    md.myDay.Content = "Important";
                    md.plusImage.Source = new Uri("pack://application:,,,/Resource/plus-FF5757.svg");
                    md.circleImage.Source = new Uri("pack://application:,,,/Resource/circleimage-FF5757.svg");
                    md._circleImagePath = "pack://application:,,,/Resource/circleimage-FF5757.svg";
                    md.calendarImage.Source = new BitmapImage(new Uri("Resource/calendarImportant.png", UriKind.RelativeOrAbsolute));
                    md.calendarImage.Width = 250;
                    md.calendarImage.Height = 140;
                    md.calendarImage.Margin = new(10,30,30,20);
                    md.applyImage.Source = new BitmapImage(new Uri("Resource/arrow-ff5757.png", UriKind.RelativeOrAbsolute));
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public void GroceriesBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var md = MyDayTaskPanel.MainWindowInstance?.myDayTaskPanel;
            if (md != null)
            {
                try
                {
                    SectionOpened(sender);
                    md.UpdateSection("#7ed957", "Groceries", 0);
                    md.focusOnYourDay.Content = "          To Eat";
                    md.myDay.Content = "Groceries";
                    md.plusImage.Source = new Uri("pack://application:,,,/Resource/plus-7ed957.svg");
                    md.circleImage.Source = new Uri("pack://application:,,,/Resource/circleimage-7ed957.svg");
                    md._circleImagePath = "pack://application:,,,/Resource/circleimage-7ed957.svg";
                    md.calendarImage.Source = new BitmapImage(new Uri("Resource/supermarket-shopping.png", UriKind.RelativeOrAbsolute));
                    md.calendarImage.Width = 250;
                    md.calendarImage.Height = 160;
                    md.calendarImage.Margin = new(7, 15, 0, 20);
                    md.applyImage.Source = new BitmapImage(new Uri("Resource/arrow-7ed957.png", UriKind.RelativeOrAbsolute));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
