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
        private bool mDraggingTask;
        private TaskControl mDraggedTask;
        private Point mDragStartMousePosition;
        private Cursor mDragPreviousCursor;

        public TaskPile()
        {
            InitializeComponent();
        }

        private void addTaskButton_Click(object sender, RoutedEventArgs e)
        {
            TaskControl task_control = new TaskControl();
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
            if (e.ChangedButton == MouseButton.Left && !mDraggingTask)
            {
                mDraggingTask = true;
                mDraggedTask = sender as TaskControl;
                mDragStartMousePosition = e.GetPosition(this);
                mDraggedTask.CaptureMouse();
                mDragPreviousCursor = Cursor;
                Cursor = Cursors.Hand;

                for (int i = 1; i < stackPanel.Children.Count; ++i)
                    (stackPanel.Children[i] as TaskControl).DragState = (stackPanel.Children[i] == mDraggedTask) ? TaskControl.EDragState.IsBeingDragged : TaskControl.EDragState.IsNotBeingDragged;
                e.Handled = true;
            }
        }

        private void TaskControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && mDraggingTask)
            {
                // Find out where to place our task
                Point mouse_pos = e.GetPosition(this);

                int preferred_index = stackPanel.Children.Count-1;

                for (int i=2; i<stackPanel.Children.Count; ++i)
                {
                    Point control_top_left = stackPanel.Children[i].TransformToAncestor(this).Transform(new Point(0, 0));

                    if (mouse_pos.Y < control_top_left.Y)
                    {
                        preferred_index = i - 1;
                        break;
                    }
                }

                int current_index = stackPanel.Children.IndexOf(mDraggedTask);

                // Move dragged task to preferred spot
                if (current_index != preferred_index)
                {
                    stackPanel.Children.RemoveAt(current_index);
                    stackPanel.Children.Insert(preferred_index, mDraggedTask);
                }

                e.Handled = true;
            }
        }

        private void TaskControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && mDraggingTask)
            {
                Cursor = mDragPreviousCursor;
                mDraggedTask.ReleaseMouseCapture();

                for (int i = 1; i < stackPanel.Children.Count; ++i)
                    (stackPanel.Children[i] as TaskControl).DragState = TaskControl.EDragState.NoDraggingActive;

                mDraggingTask = false;

                e.Handled = true;
            }
        }

        private void TaskControl_OnDelete(TaskControl inTask)
        {
            stackPanel.Children.Remove(inTask);
        }
    }
}
