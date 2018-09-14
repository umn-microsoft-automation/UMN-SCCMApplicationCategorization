using MahApps.Metro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace UMN_SCCMApplicationCategorization {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        private CMHandler cmHandler = new CMHandler( Properties.Settings.Default.CMSiteServer );

        public List<CMApplication> cmApps = new List<CMApplication>();
        public ObservableCollection<CheckedListBoxItem> appsList = new ObservableCollection<CheckedListBoxItem>();
        public ObservableCollection<CMApplicationCategory> cmAppCategories = new ObservableCollection<CMApplicationCategory>();

        public MainWindow() {
            InitializeComponent();
            ProgressSpinner.IsActive = true;

            ThemeManager.AddAccent( "CustomAccentUMN", new Uri( "pack://application:,,,/UMN-SCCMApplicationCategorization;component/Styles/CustomAccentUMN.xaml" ) );

            SmsProvider smsProvider = new SmsProvider();
            if(smsProvider.TestConnection(Properties.Settings.Default.CMSiteServer)) {
                cmAppCategories = cmHandler.GetCMApplicationCategories( true );

                var categories = new ObservableCollection<string>( cmAppCategories.Select( c => c.LocalizedCategoryInstanceName ).OrderBy( c => c ).ToList() );

                CategoriesListBox.DataContext = categories;
            } else {
                MessageBox.Show( "Issue connecting to site server: " + Properties.Settings.Default.CMSiteServer + Environment.NewLine + "Go to File->Settings to update site server address." );
            }

            if( Properties.Settings.Default.DarkTheme ) {
                ThemeManager.ChangeAppStyle( this, ThemeManager.GetAccent( "CustomAccentUMN" ), ThemeManager.GetAppTheme( "BaseDark" ) );
                DarkThemeSlider.IsChecked = true;
            } else {
                ThemeManager.ChangeAppStyle( this, ThemeManager.GetAccent( "CustomAccentUMN" ), ThemeManager.GetAppTheme( "BaseLight" ) );
                DarkThemeSlider.IsChecked = false;
            }

            ProgressSpinner.IsActive = false;
        }

        public void UpdateApplicationsList() {
            if(!(CategoriesListBox.SelectedItem == null)) {
                appsList.Clear();
                cmApps = cmHandler.GetCMApplicationsByCategory( CategoriesListBox.SelectedItem.ToString() );

                foreach(string app in cmApps.Select( c=>c.ApplicationName).OrderBy(c=>c).ToArray()) {
                    appsList.Add( new CheckedListBoxItem( app, false ) );
                }

                ApplicationsListBox.ItemsSource = appsList;

                CollectionView view = CollectionViewSource.GetDefaultView( ApplicationsListBox.ItemsSource ) as CollectionView;
                view.Filter = ApplicationListBoxFilter;
            }
        }

        private void CategoriesListBox_SelectionChanged( object sender, System.Windows.Controls.SelectionChangedEventArgs e ) {
            ProgressSpinner.IsActive = true;

            UpdateApplicationsList();

            if(CategoriesListBox.SelectedItem.ToString() == "All") {
                RemoveCateogryButton.IsEnabled = false;
                AddCategoryButton.IsEnabled = true;
                CheckAllBoxesButton.IsEnabled = false;
                RefreshAppsButtion.IsEnabled = true;
            } else if(CategoriesListBox.SelectedItem == null) {
                RemoveCateogryButton.IsEnabled = false;
                AddCategoryButton.IsEnabled = false;
                CheckAllBoxesButton.IsEnabled = false;
                RefreshAppsButtion.IsEnabled = false;
            } else {
                RemoveCateogryButton.IsEnabled = true;
                AddCategoryButton.IsEnabled = false;
                CheckAllBoxesButton.IsEnabled = true;
                RefreshAppsButtion.IsEnabled = true;
            }

            cmAppCategories = cmHandler.GetCMApplicationCategories(true);

            var categories = new ObservableCollection<string>( cmAppCategories.Select( c => c.LocalizedCategoryInstanceName ).OrderBy( c => c ).ToList() );

            CategoriesListBox.DataContext = categories;
            ProgressSpinner.IsActive = false;
        }

        private bool ApplicationListBoxFilter(object obj) {
            if( string.IsNullOrEmpty( ApplicationListFilter.Text ) ) {
                return true;
            } else if(ApplicationsListBox.SelectedItems.Contains(obj as CheckedListBoxItem)) {
                return true;
            } else {
                return ( ( obj as CheckedListBoxItem ).Name.IndexOf( ApplicationListFilter.Text, StringComparison.OrdinalIgnoreCase ) >= 0 );
            }
        }

        private void ApplicationListFilter_TextChanged( object sender, System.Windows.Controls.TextChangedEventArgs e ) {
            CollectionViewSource.GetDefaultView( ApplicationsListBox.ItemsSource ).Refresh();
        }

        private void CheckAllBoxesButton_Click( object sender, RoutedEventArgs e ) {
            for(int i = 0; i < appsList.Count; i++ ) {
                appsList[i].IsChecked = true;
            }
        }

        private void RemoveCateogryButton_Click( object sender, RoutedEventArgs e ) {
            foreach( CheckedListBoxItem item in ApplicationsListBox.SelectedItems ) {
                foreach( CheckedListBoxItem app in ApplicationsListBox.SelectedItems ) {
                    cmHandler.RemoveCMApplicationCategory( app.Name, CategoriesListBox.SelectedItem.ToString() );
                }
            }

            UpdateApplicationsList();
        }

        private void AddCategoryButton_Click( object sender, RoutedEventArgs e ) {
            CategoryChooser categoryChooser = new CategoryChooser();
            categoryChooser.ShowDialog();
            
            if(!string.IsNullOrEmpty(categoryChooser.ChosenCategory)) {
                foreach( CheckedListBoxItem app in ApplicationsListBox.SelectedItems) {
                    cmHandler.SetCMApplicationCategory( app.Name, categoryChooser.ChosenCategory );
                }
            }
        }

        private void MenuFileChangeSiteServer_Click( object sender, RoutedEventArgs e ) {
            ProgressSpinner.IsActive = true;
            Settings settings = new Settings();
            settings.ShowDialog();

            Properties.Settings.Default.Save();

            cmHandler = new CMHandler( Properties.Settings.Default.CMSiteServer );
            cmAppCategories = cmHandler.GetCMApplicationCategories( true );

            var categories = new ObservableCollection<string>( cmAppCategories.Select( c => c.LocalizedCategoryInstanceName ).OrderBy( c => c ).ToList() );

            CategoriesListBox.DataContext = categories;
            ProgressSpinner.IsActive = false;
        }

        private void RefreshAppsButtion_Click( object sender, RoutedEventArgs e ) {
            ProgressSpinner.IsActive = true;
            UpdateApplicationsList();
            ProgressSpinner.IsActive = false;
        }

        private void SettingsButton_Click( object sender, RoutedEventArgs e ) {
            SettingsFlyout.IsOpen = true;
        }

        private void SettingsFlyout_ClosingFinished( object sender, RoutedEventArgs e ) {
            ProgressSpinner.IsActive = true;

            Properties.Settings.Default.CMSiteServer = CMSiteServerTextBox.Text;
            Properties.Settings.Default.Save();

            cmHandler = new CMHandler( Properties.Settings.Default.CMSiteServer );
            cmAppCategories = cmHandler.GetCMApplicationCategories( true );

            var categories = new ObservableCollection<string>( cmAppCategories.Select( c => c.LocalizedCategoryInstanceName ).OrderBy( c => c ).ToList() );

            CategoriesListBox.DataContext = categories;

            ProgressSpinner.IsActive = false;
        }

        private void DarkThemeSlider_IsCheckedChanged( object sender, EventArgs e ) {

            if( DarkThemeSlider.IsChecked.Value ) {
                ThemeManager.ChangeAppStyle( this, ThemeManager.GetAccent( "CustomAccentUMN" ), ThemeManager.GetAppTheme( "BaseDark" ) );
                Properties.Settings.Default.DarkTheme = true;
                Properties.Settings.Default.Save();
            } else {
                ThemeManager.ChangeAppStyle( this, ThemeManager.GetAccent( "CustomAccentUMN" ), ThemeManager.GetAppTheme( "BaseLight" ) );
                Properties.Settings.Default.DarkTheme = false;
                Properties.Settings.Default.Save();
            }
        }
    }
}
