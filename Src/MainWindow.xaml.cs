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



        private void Pile_OnDragTaskStarted(TaskControl inTask)
        {
            mDraggedTask = inTask;

            //mDraggedTask.CaptureMouse();
            mDragPreviousCursor = Cursor;
            Cursor = Cursors.Hand;

            for (int j = 0; j < stackPanel.Children.Count - 1; ++j)
            {
                TaskPile pile = stackPanel.Children[j] as TaskPile;
                for (int i = 1; i < pile.stackPanel.Children.Count; ++i)
                    (pile.stackPanel.Children[i] as TaskControl).DragState = (pile.stackPanel.Children[i] == mDraggedTask) ? TaskControl.EDragState.IsBeingDragged : TaskControl.EDragState.IsNotBeingDragged;
            }

            mDraggingTask = true;
        }

        private void Pile_OnDragTaskMoved(TaskControl inTask, Point inPosition)
        {
            if (!mDraggingTask)
                return;

            // Find out where to place our task
            Point mouse_pos = inPosition;

            int preferred_tile_index = stackPanel.Children.Count - 2;
            TaskPile preferred_tile = stackPanel.Children[preferred_tile_index] as TaskPile;

            // Determine pile we're trying to move our task to
            for (int i = 1; i < stackPanel.Children.Count-1; ++i)
            {
                TaskPile pile = stackPanel.Children[i] as TaskPile;
                Point control_top_left = pile.stackPanel.Children[0].TransformToAncestor(stackPanel).Transform(new Point(0, 0));

                if (mouse_pos.X < control_top_left.X)
                {
                    preferred_tile_index = i - 1;
                    preferred_tile = stackPanel.Children[i-1] as TaskPile;
                    break;
                }
            }

            // Determine task index we're trying to move our task to
            int preferred_task_index = preferred_tile.stackPanel.Children.Count - 1;

            for (int i = 2; i < preferred_tile.stackPanel.Children.Count; ++i)
            {
                Point control_top_left = preferred_tile.stackPanel.Children[i].TransformToAncestor(stackPanel).Transform(new Point(0, 0));

                if (mouse_pos.Y < control_top_left.Y)
                {
                    preferred_task_index = i - 1;
                    break;
                }
            }

            int current_pile_index = -1;
            TaskPile current_pile = null;
            for (int j = 0; j < stackPanel.Children.Count - 1; ++j)
            {
                TaskPile pile = stackPanel.Children[j] as TaskPile;
                if (pile.stackPanel.Children.Contains(mDraggedTask))
                {
                    current_pile_index = j;
                    current_pile = pile;
                    break;
                }
            }

            int current_task_index = current_pile.stackPanel.Children.IndexOf(mDraggedTask);

            // Move dragged task to a different pile?
            if (current_pile_index != preferred_tile_index)
            {
                current_pile.stackPanel.Children.RemoveAt(current_task_index);
                preferred_tile.stackPanel.Children.Insert(preferred_task_index, mDraggedTask);
            }
            // Move dragged task to different spot in same pile?
            else if (current_task_index != preferred_task_index)
            {
                current_pile.stackPanel.Children.RemoveAt(current_task_index);
                current_pile.stackPanel.Children.Insert(preferred_task_index, mDraggedTask);
            }
        }


        private void Pile_OnDragTaskStopped(TaskControl inTask)
        {
            if (!mDraggingTask)
                return;

            Cursor = mDragPreviousCursor;
            //mDraggedTask.ReleaseMouseCapture();

            for (int j = 0; j < stackPanel.Children.Count - 1; ++j)
            {
                TaskPile pile = stackPanel.Children[j] as TaskPile;
                for (int i = 1; i < pile.stackPanel.Children.Count; ++i)
                    (pile.stackPanel.Children[i] as TaskControl).DragState = TaskControl.EDragState.NoDraggingActive;
            }

            mDraggingTask = false;
        }
    }
}
