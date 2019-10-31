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
    /// Interaction logic for TaskControl.xaml
    /// </summary>
    public partial class TaskControl : UserControl
    {
        public delegate void TaskControlHandler(TaskControl inTaskControl);

        public enum EDragState
        {
            NoDraggingActive,
            IsBeingDragged,
            IsNotBeingDragged
        }

        private Workspace mWorkspace;
        private Workspace.Task mTask;
        private PileControl mParentPileControl;

        private EDragState mDragState = EDragState.NoDraggingActive;

        public Workspace.Task Task
        {
            get
            {
                return mTask;
            }
        }

        public PileControl ParentPileControl
        {
            get
            {
                return mParentPileControl;
            }
            set
            {
                mParentPileControl = value;
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
                            RenderTransform = Transform.Identity;
                            border.BorderBrush = null;
                            border.Opacity = 1.0;
                            break;
                        case EDragState.IsBeingDragged:
                            RenderTransform = new TranslateTransform(12.0, 0.0);
                            border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 220, 0));
                            border.Opacity = 1.0;
                            break;
                        case EDragState.IsNotBeingDragged:
                            RenderTransform = Transform.Identity;
                            border.BorderBrush = null;
                            border.Opacity = 0.5;
                            break;
                    }
                }

                mDragState = value;
            }
        }

        public IEnumerable<ExternalLinkControl> ExternalLinkControls
        {
            get
            {
                for (int i = 1; i < linksAndNotesStackPanel.Children.Count; ++i)
                    if (linksAndNotesStackPanel.Children[i] is ExternalLinkControl)
                        yield return linksAndNotesStackPanel.Children[i] as ExternalLinkControl;
            }
        }

        public TaskControl(PileControl inPileControl, Workspace inWorkspace, Workspace.Task inTask)
        {
            InitializeComponent();

            mWorkspace = inWorkspace;
            mTask = inTask;
            mParentPileControl = inPileControl;

            // Set text
            if (inTask.Text != null)
                Workspace.SetFlowDocumentContentFromXML(richTextBox.Document, inTask.Text);

            // Init external links
            foreach (Workspace.ExternalLink link in inTask.ExternalLinks)
                AddNewExternalLinkControl(link);

            // Set the task's header color
            UpdateTaskHeaderColor();

            // Init notes
            if (inTask.Notes != null)
                AddNewNotesControl();
        }

        public void UpdateTaskHeaderColor()
        {
            Color color;

            if (mTask.CategoryIndex >= 0)
                color = mWorkspace.Categories[mTask.CategoryIndex].Color;
            else
                color = Color.FromArgb(255, 160, 160, 160);

            // Set background linear
            LinearGradientBrush border_background = border.Background as LinearGradientBrush;
            border_background.GradientStops.Last().Color = color;
        }


        public void DeleteExternalLinkAndControl(ExternalLinkControl inLinkControl)
        {
            mTask.ExternalLinks.Remove(inLinkControl.ExternalLink);
            linksAndNotesStackPanel.Children.Remove(inLinkControl);
        }

        public void DeleteNotesAndControl(NotesControl inNotesControl)
        {
            mTask.Notes = null;
            linksAndNotesStackPanel.Children.Remove(inNotesControl);
        }


        private void OptionsButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            optionsButton.ContextMenu.IsOpen = true;
            OptionsButton_ContextMenuOpening(sender, null);

            e.Handled = true;
        }

        private void DeleteTaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want delete this task?", "Delete task?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            ParentPileControl.DeleteTaskAndControl(this);
        }

        private ExternalLinkControl AddNewExternalLinkControl(Workspace.ExternalLink inExternalLink)
        {
            ExternalLinkControl control = new ExternalLinkControl(this, inExternalLink);
            linksAndNotesStackPanel.Children.Insert(0, control);

            return control;
        }

        private NotesControl AddNewNotesControl()
        {
            NotesControl control = new NotesControl(this, mTask);
            linksAndNotesStackPanel.Children.Insert(linksAndNotesStackPanel.Children.Count, control);
            return control;
        }

        private void AddExternalLinkMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditExternalLinkWindow window = new EditExternalLinkWindow(null);
            window.Owner = this.ParentPileControl.ParentWindow;

            if (window.ShowDialog() != true)
                return;

            Workspace.ExternalLink new_link = new Workspace.ExternalLink();
            new_link.Target = window.Target;
            mTask.ExternalLinks.Add(new_link);
            AddNewExternalLinkControl(new_link);            
        }

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Crappy/inefficient, but works for now...

            if (mTask != null)
                mTask.Text = Workspace.GetFlowDocumentContentAsXML(richTextBox.Document);
        }

        private void AddNotesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            mTask.Notes = Workspace.Task.cNewNotesText;
            AddNewNotesControl();
        }

        private void OptionsButton_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            addNotesMenuItem.IsEnabled = (mTask.Notes == null);

            // Rebuild our context menu
            while (categoryMenuItem.Items.Count > 2)
                categoryMenuItem.Items.RemoveAt(0);

            for (int i = -1; i < mWorkspace.Categories.Count; ++i)
            {
                Workspace.Category cat = (i >= 0) ? mWorkspace.Categories[i] : null;

                MenuItem item = new MenuItem();
                item.Header = (i < 0) ? "None" : cat.Title;
                item.Tag = (Object)i;
                item.IsCheckable = true;
                item.IsChecked = mTask.CategoryIndex == i;
                item.Click += CategoryMenuItem_Click;
                categoryMenuItem.Items.Insert(i + 1, item);
            }
        }

        private void CategoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            item.IsChecked = true;
            mTask.CategoryIndex = (int)item.Tag;

            UpdateTaskHeaderColor();
        }

        private void manageCategoriesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ManageCategoriesWindow window = new ManageCategoriesWindow(mWorkspace);
            window.Owner = mParentPileControl.ParentWindow;

            window.ShowDialog();
        }
    }
}
