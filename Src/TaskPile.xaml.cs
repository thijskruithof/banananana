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
        public delegate void TaskDragHandler(TaskPile inPile, TaskControl inTask);
        public delegate void TaskDragMoveHandler(TaskPile inPile, TaskControl inTask, Point inPosition);

        public event TaskDragHandler OnDragTaskStarted;
        public event TaskDragMoveHandler OnDragTaskMoved;
        public event TaskDragHandler OnDragTaskStopped;


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
            if (e.ChangedButton == MouseButton.Left)
            {
                OnDragTaskStarted(this, sender as TaskControl);

                e.Handled = true;
            }
        }

        private void TaskControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                OnDragTaskMoved(this, sender as TaskControl, e.GetPosition(this));

                e.Handled = true;
            }
        }

        private void TaskControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left )
            {
                OnDragTaskStopped(this, sender as TaskControl);

                e.Handled = true;
            }
        }

        private void TaskControl_OnDelete(TaskControl inTask)
        {
            stackPanel.Children.Remove(inTask);
        }
    }
}
