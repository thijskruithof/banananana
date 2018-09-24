using System;
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

        public event TaskDragHandler OnDragTaskStarted;
        public event TaskDragMoveHandler OnDragTaskMoved;
        public event TaskDragHandler OnDragTaskStopped;

        private bool mDragging = false;
        private TaskControl mClickedTask;
        private Point mClickedPosition;

        public TaskPile()
        {
            InitializeComponent();
        }

        private void addTaskButton_Click(object sender, RoutedEventArgs e)
        {
            TaskControl task_control = new TaskControl(this);
            task_control.OnDelete += TaskControl_OnDelete;
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
    }
}
