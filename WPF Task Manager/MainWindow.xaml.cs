using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace WPF_Task_Manager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SizeChanged += OnWindow_SizeChanged;
        }

        // track changes in the width of the entire window
        void OnWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowNameBorder.BorderThickness = new Thickness(7);
                    labelMaxmin.Content = "❐";
                }
                else
                {
                    WindowNameBorder.BorderThickness = new Thickness(0, 0, 0, 0);
                    labelMaxmin.Content = "☐";
                }
            }
        }

        // handles button hovering
        private void header_MouseEnter(object sender, MouseEventArgs e)
        {
            Border? border = sender as Border;
            if (border == null) return;
            if (border.Name == "closeButton")
                border.Background = Brushes.Red;
            else
            {
                border.Background = new BrushConverter().ConvertFromString("#545454") as Brush;
            }
        }

        // handles: mouse leaves the button
        private void header_MouseLeave(object sender, MouseEventArgs e)
        {
            Border? border = sender as Border;
            if (border == null) return;
            border.Background = new BrushConverter().ConvertFromString("#222222") as Brush;
            border.Opacity = 1;
        }

        // handles pressing the button
        private void header_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Border? border = sender as Border;
            if (border == null) return;
            if (border.Name == "closeButton")
            {
                border.Background = Brushes.Red;
                border.Opacity = 0.7;
            }
            else
            {
                border.Background = new BrushConverter().ConvertFromString("#545454") as Brush;
                border.Opacity = 0.7;
            }
        }

        // control the action of the buttons: close, full screen, small window, minimize to taskbar
        private void header_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Border? border = sender as Border;
            if (border == null) return;
            if (border.Name == "closeButton")
                Close();
            else if ((border.Name == "maxMinButton"))
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Maximized;
            }
            else
                WindowState = WindowState.Minimized;
        }

        private void FocusReset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (taskPanel.TaskBox.IsFocused) taskPanel.HiddenFocusElement.Focus();
        }

        private bool plus = false;

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double actualWidth = MainWindowGrid.ActualWidth - 420;
            double actualHeight = MainWindowGrid.ActualHeight - 55;
            if (!plus)
            {
                actualWidth += 15;
                actualHeight += 40;
                plus = true;
            }
            taskPanel.TaskPanelContentResize(actualWidth, actualHeight);
            taskSectionPanel.TaskSectionPanelResize(actualWidth, actualHeight);
            profilePanel.ProfilePanelVisible(actualWidth);
        }
    }
}