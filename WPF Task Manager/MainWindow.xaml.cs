using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
                    labelMaxmin.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/doublesquare.png"));
                    labelMaxmin.Width = 25;
                    labelMaxmin.Height = 25;
                }
                else
                {
                    WindowNameBorder.BorderThickness = new Thickness(0, 0, 0, 0);
                    labelMaxmin.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/squareround.png"));
                    labelMaxmin.Width = 11;
                    labelMaxmin.Height = 11;
                }
            }
        }

        // handles button hovering
        private void header_MouseEnter(object sender, MouseEventArgs e)
        {
            Border? border = sender as Border;
            if (border == null) return;
            if (border.Name == "closeButton") border.Background = Brushes.Red;

            else border.Background = new BrushConverter().ConvertFromString("#545454") as Brush;
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
            {
                Close();
                DBOperations.CloseDB();
            }

            else if ((border.Name == "maxMinButton"))
            {
                if (WindowState == WindowState.Maximized) WindowState = WindowState.Normal;

                else WindowState = WindowState.Maximized;
            }

            else WindowState = WindowState.Minimized;
        }

        private void FocusReset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (myDayTaskPanel.TaskBox.IsFocused) myDayTaskPanel.HiddenFocusElement.Focus();
        }

        private bool plus = false;
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double actualWidth = MainWindowGrid.ActualWidth - (!plus ? 405 : 420);
            double actualHeight = MainWindowGrid.ActualHeight - (!plus ? 15 : 55);

            plus = true;

            myDayTaskPanel.TaskPanelContentScaling(actualWidth, actualHeight);
            taskSectionPanel.TaskSectionPanelResize(actualWidth, actualHeight);
            profilePanel.ProfilePanelVisible(actualWidth);
        }
    }
}