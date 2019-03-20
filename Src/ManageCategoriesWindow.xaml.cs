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
    /// Interaction logic for ManageCategoriesWindow.xaml
    /// </summary>
    public partial class ManageCategoriesWindow : Window
    {
        private Workspace mWorkspace;

        public ManageCategoriesWindow(Workspace inWorkspace)
        {
            mWorkspace = inWorkspace;

            InitializeComponent();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            Workspace.Category cat = new Workspace.Category();
            cat.Color = Color.FromArgb(255, 255, 0, 0);
            cat.Title = "New category";

            mWorkspace.Categories.Add(cat);

            RebuildListView();
        }

        private void RebuildListView()
        {
            categoriesListView.Items.Clear();
            // TODO: Populate listview

            //foreach (Workspace.Category category in mWorkspace.Categories)
            //{
            //    ListViewItem item = new ListViewItem();
            //    item
            //    categoriesListView.Items.Add()
            //}
        }
    }
}
