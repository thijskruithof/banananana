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
                // The first two children are fixed (header and add button)
                for (int i = 0; i < linksStackPanel.Children.Count; ++i)
                    yield return linksStackPanel.Children[i] as ExternalLinkControl;
            }
        }

        public TaskControl(PileControl inPileControl, Workspace.Task inTask)
        {
            InitializeComponent();

            mTask = inTask;
            mParentPileControl = inPileControl;

            // Set text
            if (inTask.Text != null)
                Workspace.SetFlowDocumentContentFromXML(richTextBox.Document, inTask.Text);

            // Init external links
            foreach (Workspace.ExternalLink link in inTask.ExternalLinks)
                AddNewExternalLinkControl(link);

            // Reset background linear
            LinearGradientBrush border_background = border.Background as LinearGradientBrush;
            border_background.GradientStops.Last().Color = Color.FromArgb(255, 160, 160, 160);
        }


        public void DeleteExternalLinkAndControl(ExternalLinkControl inLinkControl)
        {
            mTask.ExternalLinks.Remove(inLinkControl.ExternalLink);
            linksStackPanel.Children.Remove(inLinkControl);
        }

        private void OptionsButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            optionsButton.ContextMenu.IsOpen = true;

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
            linksStackPanel.Children.Add(control);

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
            // Crappy, but works for now...

            if (mTask != null)
                mTask.Text = Workspace.GetFlowDocumentContentAsXML(richTextBox.Document);
        }
    }
}
