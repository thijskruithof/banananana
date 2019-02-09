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

        private bool mIsDragging = false;
        private TaskControl mClickedTask;
        private PileControl mClickedPile;
        private Point mClickedPosition;

        public MainWindow ParentWindow
        {
            get; set;
        }

        public IEnumerable<TaskControl> TaskControls
        {
            get
            {
                // The first two children are fixed (header and add button)
                for (int i=2; i<stackPanel.Children.Count; ++i)
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



        public PileControl(MainWindow inParentWindow)
        {
            InitializeComponent();
            ParentWindow = inParentWindow;
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

        public void DeleteTaskControl(TaskControl inTask)
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
                TaskControl task = AddNewTaskControl();
                task.SetWorkspaceTaskData(task_data);
            }
        }

        private TaskControl AddNewTaskControl()
        {
            TaskControl task_control = new TaskControl(this);
            stackPanel.Children.Insert(2, task_control);

            task_control.moveButton.MouseDown += TaskControl_MouseDown;
            task_control.moveButton.MouseUp += TaskControl_MouseUp;
            task_control.moveButton.MouseMove += TaskControl_MouseMove;

            return task_control;
        }


        private void addTaskButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewTaskControl();
        }


        private TaskControl GetOwnerTaskControlFromControl(Control inSender)
        {
            DependencyObject parent = inSender.Parent;

            while (!(parent is UserControl))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            return parent as TaskControl;
        }


        private void TaskControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Control control = sender as Control;
                mClickedTask = GetOwnerTaskControlFromControl(control);
                mClickedPile = null;
                mClickedPosition = e.GetPosition(Parent as IInputElement);
                mIsDragging = true;
                OnDragTaskStarted(mClickedTask);
                control.CaptureMouse();

                e.Handled = true;
            }
        }

        private void TaskControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point mouse_pos = e.GetPosition(this.Parent as IInputElement);

                if (mIsDragging)
                    OnDragTaskMoved(mClickedTask, mouse_pos);

                e.Handled = true;
            }
        }

        private void TaskControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && mIsDragging)
            {
                Control control = sender as Control;
                OnDragTaskStopped(mClickedTask);
                control.ReleaseMouseCapture();
                mIsDragging = false;
                mClickedTask = null;

                e.Handled = true;
            }
        }

        private void TaskControl_OnDelete(TaskControl inTask)
        {
            PileControl parent_pile = inTask.ParentPile;
            parent_pile.stackPanel.Children.Remove(inTask);
        }


        private void AddTaskRect_MouseEnter(object sender, MouseEventArgs e)
        {
            addTaskRect.Opacity = 1.0;
        }

        private void AddTaskRect_MouseLeave(object sender, MouseEventArgs e)
        {
            addTaskRect.Opacity = 0.25;
        }

        private void AddTaskRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddNewTaskControl();
        }

        private void DeleteButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want delete this pile with all the tasks in it?", "Delete pile?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            ParentWindow.DeletePileControl(this);
        }

        private void MoveButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                mClickedTask = null;
                mClickedPile = this;
                mClickedPosition = e.GetPosition(Parent as IInputElement);

                mIsDragging = true;
                (sender as Label).CaptureMouse();

                OnDragPileStarted(mClickedPile);

                e.Handled = true;
            }
        }

        private void MoveButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (mIsDragging)
                    OnDragPileStopped(this);

                (sender as Label).ReleaseMouseCapture();
                mIsDragging = false;

                e.Handled = true;
            }
        }

        private void MoveButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point mouse_pos = e.GetPosition(this.Parent as IInputElement);

                if (mIsDragging)
                    OnDragPileMoved(this, mouse_pos);

                e.Handled = true;
            }
        }
    }
}
