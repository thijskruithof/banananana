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
            CloseEditNodesControlForPile(inPileControl.Pile);
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

            // Determine which task we're dragging
            int current_task_ctrl_index = current_pile.stackPanel.Children.IndexOf(mDraggedTask);

            // Find out where to place our task
            Point mouse_pos = inPosition;

            // Determine pile to place task in
            double pile_width = (stackPanel.Children[0] as PileControl).Width; // Child 0 is always the header of the pile
            int num_piles = stackPanel.Children.Count - 1;

            int preferred_pile_index = Math.Min((int)(mouse_pos.X / pile_width), num_piles-1);
            PileControl preferred_pile = stackPanel.Children[preferred_pile_index] as PileControl;

            // Determine task index we're trying to move our task to
            int preferred_task_ctrl_index = -1;

            if (preferred_pile.stackPanel.Children.Count >= 3)
            {
                if (preferred_pile_index == current_pile_index)
                {
                    double cur_top_y = mDraggedTask.TransformToAncestor(current_pile.stackPanel).Transform(new Point(0, 0)).Y;
                    double cur_h = mDraggedTask.ActualHeight;

                    // If our mouse is still within the task that we're dragging, no need to move it.
                    if (mouse_pos.Y >= cur_top_y && mouse_pos.Y < cur_top_y + cur_h)
                        preferred_task_ctrl_index = current_task_ctrl_index;
                }
                
                if (preferred_task_ctrl_index < 0)
                {
                    double top_y = preferred_pile.stackPanel.Children[2].TransformToAncestor(stackPanel).Transform(new Point(0, 0)).Y;
                    preferred_task_ctrl_index = 2;

                    for (int i = 2; i < preferred_pile.stackPanel.Children.Count; ++i)
                    {
                        UIElement element = preferred_pile.stackPanel.Children[i];
                        if (element == mDraggedTask)
                            continue;

                        double h = (preferred_pile.stackPanel.Children[i] as FrameworkElement).ActualHeight;

                        if (mouse_pos.Y < top_y + h && mouse_pos.Y >= top_y)
                            break;

                        top_y += h;
                        preferred_task_ctrl_index++;
                    }
                }
            }
            // Our destination pile is still empty, add task to bottom.
            else
            {
                preferred_task_ctrl_index = preferred_pile.stackPanel.Children.Count;
            }

            // Move dragged task to a different pile? Or move dragged task to different spot in same pile?
            if (current_pile_index != preferred_pile_index || current_task_ctrl_index != preferred_task_ctrl_index)
            {
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

            OpenEditNotesControl(null);
        }

        private EditNotesControl GetActiveEditNotesControl()
        {
            if (mainGrid.Children.Count >= 3)
                return mainGrid.Children[2] as EditNotesControl;
            else
                return null;                           
        }


        public void OpenEditNotesControl(Workspace.Task inTask)
        {
            // First: check if any edit panel is currently open. If so, close it first...
            EditNotesControl active_control = GetActiveEditNotesControl();
            if (active_control != null)
                mainGrid.Children.Remove(active_control);

            // Open new edit panel
            EditNotesControl control = new EditNotesControl(inTask);
            control.OnClosed += EditNotesControl_OnClosed;

            mainGrid.ColumnDefinitions[1].Width = new GridLength(5);
            mainGrid.ColumnDefinitions[2].Width = new GridLength(500);
            mainGrid.ColumnDefinitions[2].MinWidth = 300;

            mainGrid.Children.Add(control);
            Grid.SetColumn(control, 2);
        }

        public void CloseEditNodesControlForPile(Workspace.Pile inPile)
        {
            EditNotesControl active_control = GetActiveEditNotesControl();
            if (active_control != null && inPile.Tasks.Contains(active_control.Task))
                CloseEditNotesControl();
        }

        public void CloseEditNodesControlForTask(Workspace.Task inTask)
        {
            EditNotesControl active_control = GetActiveEditNotesControl();
            if (active_control != null && active_control.Task == inTask)
                CloseEditNotesControl();
        }

        private void CloseEditNotesControl()
        {
            if (mainGrid.Children.Count >= 3)
                mainGrid.Children.RemoveAt(2);

            mainGrid.ColumnDefinitions[2].MinWidth = 0;
            mainGrid.ColumnDefinitions[2].Width = new GridLength(0);
            mainGrid.ColumnDefinitions[1].Width = new GridLength(0);
        }

        private void EditNotesControl_OnClosed(EditNotesControl inControl)
        {
            CloseEditNotesControl();
        }
    }
}
