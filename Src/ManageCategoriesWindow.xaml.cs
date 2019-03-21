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
        private class CategoriesListItem
        {
            public string Color { get; set; }
            public string Title { get; set; }

            public Workspace.Category Category { get; set; }
        }

        private Workspace mWorkspace;

        public ManageCategoriesWindow(Workspace inWorkspace)
        {
            mWorkspace = inWorkspace;

            InitializeComponent();
            RebuildListView();
            UpdateSelectedItemFields();
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
            Workspace.Category selected_item = null;

            if (categoriesListView.SelectedItem != null)
                selected_item = (categoriesListView.SelectedItem as CategoriesListItem).Category;

            categoriesListView.Items.Clear();

            foreach (Workspace.Category category in mWorkspace.Categories)
            {
                CategoriesListItem item = new CategoriesListItem();
                item.Color = category.Color.ToString();
                item.Title = category.Title;
                item.Category = category;

                categoriesListView.Items.Add(item);
            }

            if (selected_item != null)
            {
                int idx = mWorkspace.Categories.IndexOf(selected_item);
                if (idx >= 0)
                    categoriesListView.SelectedIndex = idx;
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want delete this category? Any task that's assigned to this category will be assigned to the None category.", "Delete category?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            CategoriesListItem item = categoriesListView.SelectedItem as CategoriesListItem;

            if (item != null)
            {
                int category_index = mWorkspace.Categories.IndexOf(item.Category);
                if (category_index >= 0)
                {
                    // Reset all tasks that are assigned to this category
                    foreach (Workspace.Pile pile in mWorkspace.Piles)
                        foreach (Workspace.Task task in pile.Tasks)
                            if (task.CategoryIndex == category_index)
                                task.CategoryIndex = -1;

                    mWorkspace.Categories.RemoveAt(category_index);
                    RebuildListView();
                }
            }            
        }

        private void categoriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedItemFields();
        }

        private void UpdateSelectedItemFields()
        {
            CategoriesListItem item = categoriesListView.SelectedItem as CategoriesListItem;

            if (item != null)
            {
                titleTextBox.IsEnabled = true;
                colorRect.Fill = new SolidColorBrush(item.Category.Color);
                titleTextBox.Text = item.Title;
                modifyButton.IsEnabled = true;
            }
            else
            {
                titleTextBox.IsEnabled = false;
                titleTextBox.Text = "";
                modifyButton.IsEnabled = false;
            }
        }

        private void modifyButton_Click(object sender, RoutedEventArgs e)
        {
            CategoriesListItem item = categoriesListView.SelectedItem as CategoriesListItem;

            if (item != null)
            {
                item.Category.Title = titleTextBox.Text;
                item.Category.Color = (colorRect.Fill as SolidColorBrush).Color;
                RebuildListView();
            }
        }

        private void pickColorRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CategoriesListItem item = categoriesListView.SelectedItem as CategoriesListItem;

            if (item != null)
            {
                item.Category.Color = ((sender as Rectangle).Fill as SolidColorBrush).Color;
                colorRect.Fill = new SolidColorBrush(item.Category.Color);
            }
        }
    }
}
