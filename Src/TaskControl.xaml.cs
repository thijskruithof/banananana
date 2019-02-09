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
        public delegate void TaskControlHandler(TaskControl inTask);

        static int mCounter = 0;

        public enum EDragState
        {
            NoDraggingActive,
            IsBeingDragged,
            IsNotBeingDragged
        }

        private EDragState mDragState = EDragState.NoDraggingActive;

        private Brush mOriginalBackground;

        public PileControl ParentPile
        {
            get; set;
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

        public TaskControl(PileControl inPile)
        {
            InitializeComponent();

            ParentPile = inPile;
            mOriginalBackground = border.Background;

            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(String.Format("Task {0}", mCounter++))));
        }

        public WorkspaceData.Task GetWorkspaceTaskData()
        {
            WorkspaceData.Task data = new WorkspaceData.Task();
            data.Text = WorkspaceData.GetFlowDocumentContentAsXML(richTextBox.Document);

            foreach (ExternalLinkControl link in ExternalLinkControls)
                data.ExternalLinks.Add(link.GetWorkspaceLinkData());

            return data;
        }

        public void SetWorkspaceTaskData(WorkspaceData.Task inData)
        {
            WorkspaceData.SetFlowDocumentContentFromXML(richTextBox.Document, inData.Text);

            foreach (WorkspaceData.ExternalLink link_data in inData.ExternalLinks)
            {
                ExternalLinkControl link = AddNewExternalLinkControl();
                link.SetWorkspaceLinkData(link_data);
            }
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

            ParentPile.DeleteTaskControl(this);
        }

        private ExternalLinkControl AddNewExternalLinkControl()
        {
            ExternalLinkControl control = new ExternalLinkControl();
            linksStackPanel.Children.Add(control);

            control.Target = "http://www.google.com";

            return control;
        }

        private void AddExternalLinkMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExternalLinkControl new_link = AddNewExternalLinkControl();
        }
    }
}
