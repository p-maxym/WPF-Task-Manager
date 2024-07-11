﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace WPF_Task_Manager
{
    partial class TaskPanel : UserControl
    {
        public TaskPanel()
        {
            InitializeComponent();
        }

        public void TaskPanelResize(double newWidth, double newHeight)
        {
            TaskPanelBorder.Width = newWidth;
            TaskPanelBorder.Height = newHeight;
        }

        public void TaskPanelContentResize(double newWidthBorder, double newWidthBox, double calendarPositionX, double calendarPositionY)
        {
            TaskBorder.Width = newWidthBorder;
            TaskBox.Width = newWidthBox;

            if (calendarImage.Visibility == Visibility.Visible)
            {
                Canvas.SetLeft(calendarImage, calendarPositionX);
                Canvas.SetTop(calendarImage, calendarPositionY);
            }
        }
    }
}
