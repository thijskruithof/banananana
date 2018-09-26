﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Banananana
{
    /// <summary>
    /// Interaction logic for TaskPile.xaml
    /// </summary>
    public partial class TaskPile : UserControl
    {
        public delegate void TaskDragHandler(TaskControl inTask);
        public delegate void TaskDragMoveHandler(TaskControl inTask, Point inPosition);

        public delegate void PileDragHandler(TaskPile inPile);
        public delegate void PileDragMoveHandler(TaskPile inPile, Point inPosition);


        public event TaskDragHandler OnDragTaskStarted;
        public event TaskDragMoveHandler OnDragTaskMoved;
        public event TaskDragHandler OnDragTaskStopped;

        public event PileDragHandler OnDragPileStarted;
        public event PileDragMoveHandler OnDragPileMoved;
        public event PileDragHandler OnDragPileStopped;


        public enum EDragState
        {
            NoDraggingActive,
            IsBeingDragged,
            IsNotBeingDragged
        }

        private EDragState mDragState = EDragState.NoDraggingActive;

        private bool mDragging = false;
        private TaskControl mClickedTask;
        private TaskPile mClickedPile;
        private Point mClickedPosition;


        public IEnumerable<TaskControl> Tasks
        {
            get
            {
                for (int i=1; i<stackPanel.Children.Count-1; ++i)
                    yield return stackPanel.Children[i] as TaskControl;
            }
        }


        public EDragState DragState
        {
            get { return mDragState; }

            set
            {
                if (mDragState != value)
                {
                    switch (value)
                    {
                        case EDragState.NoDraggingActive:
                            border.BorderBrush = null;
                            border.Opacity = 1.0;
                            break;
                        case EDragState.IsBeingDragged:
                            border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 220, 0));
                            border.Opacity = 1.0;
                            break;
                        case EDragState.IsNotBeingDragged:
                            border.BorderBrush = null;
                            border.Opacity = 0.5;
                            break;
                    }
                }

                mDragState = value;
            }
        }



        public TaskPile()
        {
            InitializeComponent();
        }


        public void MoveTaskToPile(TaskControl inTask, TaskPile inDestinationPile, int inDestinationTaskIndex)
        {
            // Remove
            int task_index = this.stackPanel.Children.IndexOf(inTask);
            this.stackPanel.Children.RemoveAt(task_index);

            // Add
            inDestinationPile.stackPanel.Children.Insert(inDestinationTaskIndex, inTask);

            // Set new parent pile
            inTask.ParentPile = inDestinationPile;
        }

        public void DeleteTask(TaskControl inTask)
        {
            stackPanel.Children.Remove(inTask);
        }


        private void addTaskButton_Click(object sender, RoutedEventArgs e)
        {
            TaskControl task_control = new TaskControl(this);
            stackPanel.Children.Insert(1, task_control);

            //Keyboard.Focus(tc.richTextBox);
            task_control.richTextBox.Focus();
            //Keyboard.Focus(tc.richTextBox);

            task_control.MouseDown += TaskControl_MouseDown;
            task_control.MouseUp += TaskControl_MouseUp;
            task_control.MouseMove += TaskControl_MouseMove;
        }

        private void TaskControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                mClickedTask = sender as TaskControl;
                mClickedPile = null;
                mClickedPosition = e.GetPosition(Parent as IInputElement);
                mClickedTask.CaptureMouse();

                e.Handled = true;
            }
        }

        private void TaskControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point mouse_pos = e.GetPosition(this.Parent as IInputElement);
                Point delta = new Point(mouse_pos.X - mClickedPosition.X, mouse_pos.Y - mClickedPosition.Y);
                double mouse_move_dist = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);

                if (!mDragging && mouse_move_dist >= 1.0)
                {
                    OnDragTaskStarted(mClickedTask);

                    mDragging = true;
                }


                if (mDragging)
                    OnDragTaskMoved(sender as TaskControl, mouse_pos);

                e.Handled = true;
            }
        }

        private void TaskControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && mDragging)
            {
                OnDragTaskStopped(sender as TaskControl);
                mClickedTask.ReleaseMouseCapture();
                mDragging = false;

                e.Handled = true;
            }
        }

        private void TaskControl_OnDelete(TaskControl inTask)
        {
            TaskPile parent_pile = inTask.ParentPile;
            parent_pile.stackPanel.Children.Remove(inTask);
        }



        private void headerGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                mClickedTask = null;
                mClickedPile = this;
                mClickedPosition = e.GetPosition(Parent as IInputElement);
                (sender as Grid).CaptureMouse();

                e.Handled = true;
            }
        }


        private void headerGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point mouse_pos = e.GetPosition(this.Parent as IInputElement);
                Point delta = new Point(mouse_pos.X - mClickedPosition.X, mouse_pos.Y - mClickedPosition.Y);
                double mouse_move_dist = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);

                if (!mDragging && mouse_move_dist >= 1.0)
                {
                    OnDragPileStarted(mClickedPile);

                    mDragging = true;
                }


                if (mDragging)
                    OnDragPileMoved(this, mouse_pos);

                e.Handled = true;
            }
        }

        private void headerGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && mDragging)
            {
                OnDragPileStopped(this);
                (sender as Grid).ReleaseMouseCapture();
                mDragging = false;

                e.Handled = true;
            }
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //titleLabel.Visibility = Visibility.Hidden;
            //titleTextBox.Visibility = Visibility.Visible;
            //titleTextBox.Text = titleLabel.Content as string;
            //titleTextBox.SelectAll();

            //e.Handled = true;
        }
    }
}
