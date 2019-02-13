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
        private bool mDragging;
        private TaskControl mDraggedTask;
        private PileControl mDraggedPile;
        private Cursor mDragPreviousCursor;
        private Workspace mWorkspace;

        public MainWindow()
        {
            LoadWorkspace();

            InitializeComponent();

            // Init our UI
            foreach (Workspace.Pile pile in mWorkspace.Piles)
                AddNewPileControl(pile);
        }


        public IEnumerable<PileControl> PileControls
        {
            get
            {
                for (int i = 0; i < stackPanel.Children.Count - 1; ++i)
                    yield return stackPanel.Children[i] as PileControl;
            }
        }


        private PileControl AddNewPileControl(Workspace.Pile inPile)
        {
            PileControl pile_control = new PileControl(this, inPile);
            pile_control.VerticalAlignment = VerticalAlignment.Top;

            pile_control.OnDragTaskControlStarted += Pile_OnDragTaskStarted;
            pile_control.OnDragTaskControlMoved += Pile_OnDragTaskMoved;
            pile_control.OnDragTaskControlStopped += Pile_OnDragTaskStopped;

            pile_control.OnDragPileControlStarted += Pile_OnDragPileStarted;
            pile_control.OnDragPileControlMoved += Pile_OnDragPileMoved;
            pile_control.OnDragPileControlStopped += Pile_OnDragPileStopped;

            stackPanel.Children.Insert(stackPanel.Children.Count-1, pile_control);

            return pile_control;
        }


        public void DeletePileAndControl(PileControl inPileControl)
        {
            mWorkspace.Piles.Remove(inPileControl.Pile);
            stackPanel.Children.Remove(inPileControl);
        }



        private void Pile_OnDragPileStarted(PileControl inPile)
        {
            mDraggedPile = inPile;

            mDragPreviousCursor = Cursor;
            Cursor = Cursors.Hand;

            // Update dragging state of all piles
            foreach (PileControl pile in PileControls)
                pile.DragState = (pile == mDraggedPile) ? PileControl.EDragState.IsBeingDragged : PileControl.EDragState.IsNotBeingDragged;


            mDragging = true;
        }

        private void Pile_OnDragPileMoved(PileControl inPileControl, Point inPosition)
        {
            if (!mDragging)
                return;

            // Find out where to place our task
            Point mouse_pos = inPosition;

            // Determine pile to place task in
            double pile_width = (stackPanel.Children[0] as PileControl).Width; // Child 0 is always the header of the pile
            int num_piles = stackPanel.Children.Count - 1;

            int preferred_pile_ctrl_index = Math.Min((int)(mouse_pos.X / pile_width), num_piles - 1);
            int current_pile_ctrl_index = stackPanel.Children.IndexOf(inPileControl);

            // Move the pile to a different spot?
            if (current_pile_ctrl_index != preferred_pile_ctrl_index)
            {
                // Remove pile and control
                mWorkspace.Piles.RemoveAt(current_pile_ctrl_index);
                stackPanel.Children.RemoveAt(current_pile_ctrl_index);

                // Insert pile and control at correct spot
                mWorkspace.Piles.Insert(preferred_pile_ctrl_index, inPileControl.Pile);
                stackPanel.Children.Insert(preferred_pile_ctrl_index, inPileControl);
            }
        }

        private void Pile_OnDragPileStopped(PileControl inPile)
        {
            if (!mDragging)
                return;

            Cursor = mDragPreviousCursor;

            // Update dragging state of all piles
            foreach (PileControl pile in PileControls)
                pile.DragState = PileControl.EDragState.NoDraggingActive;

            mDragging = false;
        }




        private void Pile_OnDragTaskStarted(TaskControl inTask)
        {
            mDraggedTask = inTask;

            //mDraggedTask.CaptureMouse();
            mDragPreviousCursor = Cursor;
            Cursor = Cursors.Hand;

            // Update dragging state of all tasks
            foreach (PileControl pile in PileControls)
                foreach (TaskControl task in pile.TaskControls)
                    task.DragState = (task == mDraggedTask) ? TaskControl.EDragState.IsBeingDragged : TaskControl.EDragState.IsNotBeingDragged;

            mDragging = true;
        }

        private void Pile_OnDragTaskMoved(TaskControl inTask, Point inPosition)
        {
            if (!mDragging)
                return;

            // Find out where to place our task
            Point mouse_pos = inPosition;

            // Determine pile to place task in
            double pile_width = (stackPanel.Children[0] as PileControl).Width; // Child 0 is always the header of the pile
            int num_piles = stackPanel.Children.Count - 1;

            int preferred_pile_index = Math.Min((int)(mouse_pos.X / pile_width), num_piles-1);
            PileControl preferred_pile = stackPanel.Children[preferred_pile_index] as PileControl;

            // Determine task index we're trying to move our task to
            int preferred_task_ctrl_index = preferred_pile.stackPanel.Children.Count;

            for (int i = 3; i < preferred_pile.stackPanel.Children.Count; ++i)
            {
                Point control_top_left = preferred_pile.stackPanel.Children[i].TransformToAncestor(stackPanel).Transform(new Point(0, 0));

                if (mouse_pos.Y < control_top_left.Y)
                {
                    preferred_task_ctrl_index = i - 1;
                    break;
                }
            }

            // Determine from which pile we're dragging
            int current_pile_index = -1;
            PileControl current_pile = null;
            for (int j = 0; j < stackPanel.Children.Count - 1; ++j)
            {
                PileControl pile = stackPanel.Children[j] as PileControl;
                if (pile.stackPanel.Children.Contains(mDraggedTask))
                {
                    current_pile_index = j;
                    current_pile = pile;
                    break;
                }
            }

            int current_task_ctrl_index = current_pile.stackPanel.Children.IndexOf(mDraggedTask);

            // Move dragged task to a different pile? Or move dragged task to different spot in same pile?
            if (current_pile_index != preferred_pile_index || current_task_ctrl_index != preferred_task_ctrl_index)
            {
                if (current_pile_index == preferred_pile_index && preferred_task_ctrl_index > current_task_ctrl_index)
                    preferred_task_ctrl_index--;

                current_pile.MoveTaskControlToPileControl(mDraggedTask, preferred_pile, preferred_task_ctrl_index);
            }
        }


        private void Pile_OnDragTaskStopped(TaskControl inTask)
        {
            if (!mDragging)
                return;

            Cursor = mDragPreviousCursor;

            foreach (PileControl pile in PileControls)
                foreach (TaskControl task in pile.TaskControls)
                    task.DragState = TaskControl.EDragState.NoDraggingActive;

            mDragging = false;
        }


        private String GetWorkspaceFilename()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Banananana\\workspace.json";
        }


        private void LoadWorkspace()
        {
            mWorkspace = Workspace.LoadFromFile(GetWorkspaceFilename());
        }


        private void SaveWorkspace()
        {
            mWorkspace.SaveToFile(GetWorkspaceFilename());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveWorkspace();
        }

        private void AddPileRect_MouseEnter(object sender, MouseEventArgs e)
        {
            addPileRect.Opacity = 1.0;
        }

        private void AddPileRect_MouseLeave(object sender, MouseEventArgs e)
        {
            addPileRect.Opacity = 0.25;
        }

        private void AddPileRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Workspace.Pile new_pile = new Workspace.Pile();
            mWorkspace.Piles.Add(new_pile);
            AddNewPileControl(new_pile);
        }
    }
}
