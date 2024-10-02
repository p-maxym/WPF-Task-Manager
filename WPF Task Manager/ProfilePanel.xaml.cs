using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPF_Task_Manager
{
    partial class ProfilePanel : UserControl
    {
        public ProfilePanel()
        {
            InitializeComponent();
            Loaded += ProfilePanel_Loaded;
        }

        private async void ProfilePanel_Loaded(object sender, RoutedEventArgs e)
        {
            await SetCompletedTasksValue();
        }

        private async Task SetCompletedTasksValue()
        {
            await Task.Delay(300);
            string value = (await DBOperations.GetCompletedTaskValue()).ToString();
            completedCount.Text = value;
        }

        public void ProfilePanelVisible(double actualWidth)
        {
            if (actualWidth <= 360) ProfilePanelBorder.Visibility = Visibility.Collapsed;
            else ProfilePanelBorder.Visibility = Visibility.Visible;
        }
    }
}
