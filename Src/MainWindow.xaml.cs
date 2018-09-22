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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool mDraggingTask;
        private TaskControl mDraggedTask;
        private TaskPile mDraggedPile;
        private Cursor mDragPreviousCursor;


        public MainWindow()
        {
            InitializeComponent();

            AddPile();
        }

        private void addTaskButton_Click(object sender, RoutedEventArgs e)
        {
            AddPile();
        }

        private void AddPile()
        {
            TaskPile pile = new TaskPile();
            pile.VerticalAlignment = VerticalAlignment.Top;

            pile.OnDragTaskStarted += Pile_OnDragTaskStarted;
            pile.OnDragTaskMoved += Pile_OnDragTaskMoved;
            pile.OnDragTaskStopped += Pile_OnDragTaskStopped;

            stackPanel.Children.Insert(stackPanel.Children.Count-1, pile);
        }


        private void Pile_OnDragTaskStarted(TaskPile inPile, TaskControl inTask)
        {
            mDraggingTask = true;
            mDraggedTask = inTask;
            mDraggedPile = inPile;
            mDraggedTask.CaptureMouse();
            mDragPreviousCursor = mDraggedPile.Cursor;
            mDraggedPile.Cursor = Cursors.Hand;

            for (int i = 1; i < mDraggedPile.stackPanel.Children.Count; ++i)
                (mDraggedPile.stackPanel.Children[i] as TaskControl).DragState = (mDraggedPile.stackPanel.Children[i] == mDraggedTask) ? TaskControl.EDragState.IsBeingDragged : TaskControl.EDragState.IsNotBeingDragged;

        }

        private void Pile_OnDragTaskMoved(TaskPile inPile, TaskControl inTask, Point inPosition)
        {
            // Find out where to place our task
            Point mouse_pos = inPosition;

            int preferred_index = mDraggedPile.stackPanel.Children.Count - 1;

            for (int i = 2; i < mDraggedPile.stackPanel.Children.Count; ++i)
            {
                Point control_top_left = mDraggedPile.stackPanel.Children[i].TransformToAncestor(this).Transform(new Point(0, 0));

                if (mouse_pos.Y < control_top_left.Y)
                {
                    preferred_index = i - 1;
                    break;
                }
            }

            int current_index = mDraggedPile.stackPanel.Children.IndexOf(mDraggedTask);

            // Move dragged task to preferred spot
            if (current_index != preferred_index)
            {
                mDraggedPile.stackPanel.Children.RemoveAt(current_index);
                mDraggedPile.stackPanel.Children.Insert(preferred_index, mDraggedTask);
            }

        }


        private void Pile_OnDragTaskStopped(TaskPile inPile, TaskControl inTask)
        {
            mDraggedPile.Cursor = mDragPreviousCursor;
            mDraggedTask.ReleaseMouseCapture();

            for (int i = 1; i < mDraggedPile.stackPanel.Children.Count; ++i)
                (mDraggedPile.stackPanel.Children[i] as TaskControl).DragState = TaskControl.EDragState.NoDraggingActive;

            mDraggingTask = false;
        }
    }
}
