using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class PileControl : UserControl
    {
        public delegate void TaskDragHandler(TaskControl inTask);
        public delegate void TaskDragMoveHandler(TaskControl inTask, Point inPosition);

        public delegate void PileDragHandler(PileControl inPile);
        public delegate void PileDragMoveHandler(PileControl inPile, Point inPosition);


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

        private bool mRequestDragging = false;
        private bool mIsDragging = false;
        private TaskControl mClickedTask;
        private PileControl mClickedPile;
        private Point mClickedPosition;


        public IEnumerable<TaskControl> TaskControls
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



        public PileControl()
        {
            InitializeComponent();
        }


        public void MoveTaskToPile(TaskControl inTask, PileControl inDestinationPile, int inDestinationTaskIndex)
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

        public WorkspaceData.Pile GetWorkspacePileData()
        {
            WorkspaceData.Pile data = new WorkspaceData.Pile();
            data.Title = WorkspaceData.GetFlowDocumentContentAsXML(titleTextBox.Document);

            foreach (TaskControl task in TaskControls)
                data.Tasks.Add(task.GetWorkspaceTaskData());

            return data;
        }

        public void SetWorkspacePileData(WorkspaceData.Pile inData)
        {
            WorkspaceData.SetFlowDocumentContentFromXML(titleTextBox.Document, inData.Title);

            foreach (WorkspaceData.Task task_data in inData.Tasks)
            {
                TaskControl task = AddNewTask();
                task.SetWorkspaceTaskData(task_data);
            }
        }

        private TaskControl AddNewTask()
        {
            TaskControl task_control = new TaskControl(this);
            stackPanel.Children.Insert(1, task_control);

            task_control.MouseDown += TaskControl_MouseDown;
            task_control.MouseUp += TaskControl_MouseUp;
            task_control.MouseMove += TaskControl_MouseMove;

            return task_control;
        }


        private void addTaskButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewTask();
        }

        private void TaskControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                mClickedTask = sender as TaskControl;
                mClickedPile = null;
                mClickedPosition = e.GetPosition(Parent as IInputElement);
                mRequestDragging = true;
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

                if (mRequestDragging && !mIsDragging && mouse_move_dist >= 1.0)
                {
                    OnDragTaskStarted(mClickedTask);

                    mIsDragging = true;
                    mRequestDragging = false;
                }


                if (mIsDragging)
                    OnDragTaskMoved(sender as TaskControl, mouse_pos);

                e.Handled = true;
            }
        }

        private void TaskControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && mIsDragging)
            {
                OnDragTaskStopped(sender as TaskControl);
                mClickedTask.ReleaseMouseCapture();
                mIsDragging = false;
                mRequestDragging = false;

                e.Handled = true;
            }
        }

        private void TaskControl_OnDelete(TaskControl inTask)
        {
            PileControl parent_pile = inTask.ParentPile;
            parent_pile.stackPanel.Children.Remove(inTask);
        }



        private void headerGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                mClickedTask = null;
                mClickedPile = this;
                mClickedPosition = e.GetPosition(Parent as IInputElement);
                mRequestDragging = true;
                (sender as Border).CaptureMouse();
                

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

                if (mRequestDragging && !mIsDragging && mouse_move_dist >= 1.0)
                {
                    OnDragPileStarted(mClickedPile);

                    mIsDragging = true;
                    mRequestDragging = false;
                }

                if (mIsDragging)
                    OnDragPileMoved(this, mouse_pos);

                e.Handled = true;
            }
        }

        private void headerGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (mIsDragging)
                    OnDragPileStopped(this);

                (sender as Border).ReleaseMouseCapture();
                mIsDragging = false;
                mRequestDragging = false;

                e.Handled = true;
            }
        }
    }
}
