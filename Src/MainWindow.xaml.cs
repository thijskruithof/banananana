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


        public MainWindow()
        {
            InitializeComponent();

            Load();
        }


        public IEnumerable<PileControl> PileControls
        {
            get
            {
                for (int i = 0; i < stackPanel.Children.Count - 1; ++i)
                    yield return stackPanel.Children[i] as PileControl;
            }
        }


        private void addTaskButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewPileControl();
        }

        private PileControl AddNewPileControl()
        {
            PileControl pile = new PileControl(this);
            pile.VerticalAlignment = VerticalAlignment.Top;

            pile.OnDragTaskStarted += Pile_OnDragTaskStarted;
            pile.OnDragTaskMoved += Pile_OnDragTaskMoved;
            pile.OnDragTaskStopped += Pile_OnDragTaskStopped;

            pile.OnDragPileStarted += Pile_OnDragPileStarted;
            pile.OnDragPileMoved += Pile_OnDragPileMoved;
            pile.OnDragPileStopped += Pile_OnDragPileStopped;

            stackPanel.Children.Insert(stackPanel.Children.Count-1, pile);

            return pile;
        }


        public void DeletePileControl(PileControl inPile)
        {
            stackPanel.Children.Remove(inPile);
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

        private void Pile_OnDragPileMoved(PileControl inPile, Point inPosition)
        {
            if (!mDragging)
                return;

            // Find out where to place our task
            Point mouse_pos = inPosition;

            // Determine pile to place task in
            double pile_width = (stackPanel.Children[0] as PileControl).Width; // Child 0 is always the header of the pile
            int num_piles = stackPanel.Children.Count - 1;

            int preferred_pile_index = Math.Min((int)(mouse_pos.X / pile_width), num_piles - 1);
            int current_pile_index = stackPanel.Children.IndexOf(inPile);

            // Move the pile to a different spot?
            if (current_pile_index != preferred_pile_index)
            {
                stackPanel.Children.RemoveAt(current_pile_index);
                stackPanel.Children.Insert(preferred_pile_index, inPile);
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
            int preferred_task_index = preferred_pile.stackPanel.Children.Count - 2;

            for (int i = 3; i < preferred_pile.stackPanel.Children.Count; ++i)
            {
                Point control_top_left = preferred_pile.stackPanel.Children[i].TransformToAncestor(stackPanel).Transform(new Point(0, 0));

                if (mouse_pos.Y < control_top_left.Y)
                {
                    preferred_task_index = i - 1;
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

            int current_task_index = current_pile.stackPanel.Children.IndexOf(mDraggedTask);

            // Move dragged task to a different pile? Or move dragged task to different spot in same pile?
            if (current_pile_index != preferred_pile_index || current_task_index != preferred_task_index)
            {
                current_pile.MoveTaskToPile(mDraggedTask, preferred_pile, preferred_task_index);
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


        private WorkspaceData GetWorkspaceData()
        {
            WorkspaceData data = new WorkspaceData();


            foreach (PileControl pile in PileControls)
            { 
                WorkspaceData.Pile pile_data = pile.GetWorkspacePileData();
                data.Piles.Add(pile_data);
            }

            return data;
        }


        private void SetWorkspaceData(WorkspaceData inData)
        {
            foreach (WorkspaceData.Pile pile_data in inData.Piles)
            {
                PileControl pile = AddNewPileControl();
                pile.SetWorkspacePileData(pile_data);
            }
        }


        private void Load()
        {
            WorkspaceData data = WorkspaceData.LoadFromFile(GetWorkspaceFilename());

            SetWorkspaceData(data);
        }


        private void Save()
        {
            WorkspaceData data = GetWorkspaceData();

            data.SafeToFile(GetWorkspaceFilename());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Save();
        }
    }
}
