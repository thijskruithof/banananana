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
        public delegate void TaskDragHandler(TaskControl inTaskControl);
        public delegate void TaskDragMoveHandler(TaskControl inTaskControl, Point inPosition);

        public delegate void PileDragHandler(PileControl inPileControl);
        public delegate void PileDragMoveHandler(PileControl inPileControl, Point inPosition);


        public event TaskDragHandler OnDragTaskControlStarted;
        public event TaskDragMoveHandler OnDragTaskControlMoved;
        public event TaskDragHandler OnDragTaskControlStopped;

        public event PileDragHandler OnDragPileControlStarted;
        public event PileDragMoveHandler OnDragPileControlMoved;
        public event PileDragHandler OnDragPileControlStopped;


        private Workspace.Pile mPile;


        public enum EDragState
        {
            NoDraggingActive,
            IsBeingDragged,
            IsNotBeingDragged
        }

        private EDragState mDragState = EDragState.NoDraggingActive;

        private bool mIsDragging = false;
        private TaskControl mClickedTaskControl;
        private PileControl mClickedPileControl;
        private Point mClickedPosition;

        public MainWindow ParentWindow
        {
            get; set;
        }

        public Workspace.Pile Pile
        {
            get
            {
                return mPile;
            }
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


        public PileControl(MainWindow inParentWindow, Workspace.Pile inPile)
        {
            InitializeComponent();

            mPile = inPile;
            ParentWindow = inParentWindow;

            if (inPile.Title != null)
                Workspace.SetFlowDocumentContentFromXML(titleTextBox.Document, inPile.Title);

            headerGrid.Background = new SolidColorBrush(inPile.Color);

            foreach (Workspace.Task task in inPile.Tasks)
                AddNewTaskControl(task);
        }


        public void MoveTaskControlToPileControl(TaskControl inTaskControl, PileControl inDestinationPileControl, int inDestinationTaskControlIndex)
        {
            // Remove from this pile
            this.stackPanel.Children.Remove(inTaskControl);
            this.mPile.Tasks.Remove(inTaskControl.Task);

            // Add to destination pile (at the correct position)
            inDestinationPileControl.stackPanel.Children.Insert(inDestinationTaskControlIndex, inTaskControl);
            inDestinationPileControl.mPile.Tasks.Insert(inDestinationTaskControlIndex - 2, inTaskControl.Task);

            // Set new parent pile
            inTaskControl.ParentPileControl = inDestinationPileControl;
        }


        public void DeleteTaskAndControl(TaskControl inTask)
        {
            mPile.Tasks.Remove(inTask.Task);
            stackPanel.Children.Remove(inTask);
        }


        private TaskControl AddNewTaskControl(Workspace.Task inTask)
        {
            int index = mPile.Tasks.IndexOf(inTask);
            if (index < 0)
                index = mPile.Tasks.Count;

            TaskControl task_control = new TaskControl(this, inTask);

            stackPanel.Children.Insert(2+index, task_control);

            task_control.moveButton.MouseDown += TaskControl_MouseDown;
            task_control.moveButton.MouseUp += TaskControl_MouseUp;
            task_control.moveButton.MouseMove += TaskControl_MouseMove;

            return task_control;
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
                mClickedTaskControl = GetOwnerTaskControlFromControl(control);
                mClickedPileControl = null;
                mClickedPosition = e.GetPosition(Parent as IInputElement);
                mIsDragging = true;
                OnDragTaskControlStarted(mClickedTaskControl);
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
                    OnDragTaskControlMoved(mClickedTaskControl, mouse_pos);

                e.Handled = true;
            }
        }

        private void TaskControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && mIsDragging)
            {
                Control control = sender as Control;
                OnDragTaskControlStopped(mClickedTaskControl);
                control.ReleaseMouseCapture();
                mIsDragging = false;
                mClickedTaskControl = null;

                e.Handled = true;
            }
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
            Workspace.Task new_task = new Workspace.Task();
            mPile.Tasks.Insert(0, new_task);
            AddNewTaskControl(new_task);
        }

        private void MoveButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                mClickedTaskControl = null;
                mClickedPileControl = this;
                mClickedPosition = e.GetPosition(Parent as IInputElement);

                mIsDragging = true;
                (sender as Label).CaptureMouse();

                OnDragPileControlStarted(mClickedPileControl);

                e.Handled = true;
            }
        }

        private void MoveButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (mIsDragging)
                    OnDragPileControlStopped(this);

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
                    OnDragPileControlMoved(this, mouse_pos);

                e.Handled = true;
            }
        }

        private void DeletePileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want delete this pile with all the tasks in it?", "Delete pile?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            ParentWindow.DeletePileAndControl(this);
        }

        private void OptionsButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            optionsButton.ContextMenu.IsOpen = true;
            e.Handled = true;
        }

        private void colorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string color_code = item.Header as String;

            Color color = (Color)ColorConverter.ConvertFromString(color_code);

            mPile.Color = color;
            headerGrid.Background = new SolidColorBrush(color);
        }

        private void titleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Crappy, but works for now...

            if (mPile != null)
                mPile.Title = Workspace.GetFlowDocumentContentAsXML(titleTextBox.Document);
        }
    }
}
