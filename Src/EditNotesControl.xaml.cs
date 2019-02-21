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
    /// Interaction logic for EditNotesControl.xaml
    /// </summary>
    public partial class EditNotesControl : UserControl
    {
        public delegate void CloseHandler(EditNotesControl inControl);

        public event CloseHandler OnClosed;

        private Workspace.Task mTask;

        public Workspace.Task Task
        {
            get
            {
                return mTask;
            }
        }

        public EditNotesControl(Workspace.Task inTask)
        {
            InitializeComponent();

            TaskControl new_task_control = new TaskControl(null, inTask);
            taskBorder.Child = new_task_control;
            new_task_control.HorizontalAlignment = HorizontalAlignment.Stretch;

            // Init task after UI has been initialized, as the init of the UI will trigger a notesTextBox_TextChanged
            mTask = inTask;

//            Workspace.SetFlowDocumentContentFromXML(titleTextBox.Document, inTask.Text);
            Workspace.SetFlowDocumentContentFromXML(notesTextBox.Document, inTask.Notes);
        }
               

        private void closeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            closeButton.Opacity = 1.0;
        }

        private void closeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            closeButton.Opacity = 0.25;
        }

        private void closeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnClosed(this);
        }

        private void notesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Crappy/inefficient, but works for now...

            if (mTask != null)
                mTask.Notes = Workspace.GetFlowDocumentContentAsXML(notesTextBox.Document);
        }
    }
}
