using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace UMN_SCCMApplicationCategorization
{
    /// <summary>
    /// Interaction logic for CategoryChooser.xaml
    /// </summary>
    public partial class CategoryChooser : Window
    {
        public ObservableCollection<CMApplicationCategory> cmAppCategories = new ObservableCollection<CMApplicationCategory>();
        private CMHandler cmHandler = new CMHandler( Properties.Settings.Default.CMSiteServer );

        private string chosenCategory;
        public string ChosenCategory {
            get {
                return chosenCategory;
            } set {
                chosenCategory = value;
            }
        }

        public CategoryChooser()
        {
            InitializeComponent();

            cmAppCategories = cmHandler.GetCMApplicationCategories(false);

            var categories = new ObservableCollection<string>( cmAppCategories.Select( c => c.LocalizedCategoryInstanceName ).OrderBy( c => c ).ToList() );
            
            CategoriesListBox.DataContext = categories;
        }

        private void CategoryOK_Click( object sender, RoutedEventArgs e ) {
            chosenCategory = CategoriesListBox.SelectedItem.ToString();
            Close();
        }
    }
}
