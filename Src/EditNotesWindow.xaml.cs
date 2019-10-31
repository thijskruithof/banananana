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
using System.Windows.Shapes;

namespace Banananana
{
    /// <summary>
    /// Interaction logic for EditNotesWindow.xaml
    /// </summary>
    public partial class EditNotesWindow : Window
    {
        private Workspace.Task mTask;

        public EditNotesWindow(Workspace inWorkspace, Workspace.Task inTask)
        {
            InitializeComponent();

            // Init task after UI has been initialized, as the init of the UI will trigger a notesTextBox_TextChanged
            mTask = inTask;

            if (inTask.Text != null)
                Workspace.SetFlowDocumentContentFromXML(richTextBox.Document, inTask.Text);

            Workspace.SetFlowDocumentContentFromXML(notesTextBox.Document, inTask.Notes);
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (mTask != null)
                mTask.Notes = Workspace.GetFlowDocumentContentAsXML(notesTextBox.Document);

            DialogResult = true;
        }
    }
}
