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

namespace UMN_SCCMApplicationCategorization
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();

            SiteServerFQDN.Text = Properties.Settings.Default.CMSiteServer;
        }

        private void OKButton_Click( object sender, RoutedEventArgs e ) {
            Properties.Settings.Default.CMSiteServer = SiteServerFQDN.Text;
            Close();
        }
    }
}
