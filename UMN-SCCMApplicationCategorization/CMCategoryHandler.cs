using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UMN_SCCMApplicationCategorization
{
    class CMHandler
    {
        public string SiteServer { get; set; }

        public CMHandler(string siteServer) {
            SiteServer = siteServer;
        }

        public void SetCMApplicationCategory(string appName, string categoryName) {
            SmsProvider smsProvider = new SmsProvider();
            WqlConnectionManager connectionManager = smsProvider.Connect( SiteServer );

            ObservableCollection<CMApplicationCategory> applicationCategories = GetCMApplicationCategories( false );

            string appQuery = string.Format( "SELECT * FROM SMS_ApplicationLatest WHERE (IsHidden = 0) AND (LocalizedDisplayName = '{0}')", appName );

            IResultObject appQueryResult = connectionManager.QueryProcessor.ExecuteQuery( appQuery );

            if(appQueryResult != null) {
                foreach( IResultObject app in appQueryResult ) {
                    ArrayList appCategories = new ArrayList( app["CategoryInstance_UniqueIDs"].StringArrayValue );

                    CMApplicationCategory applicationCategory = applicationCategories.Where( c => c.LocalizedCategoryInstanceName == categoryName ).First();
                    if(appCategories.Contains(applicationCategory.CategoryInstance_UniqueID)) {
                        MessageBox.Show( appName + " already contains " + applicationCategory.LocalizedCategoryInstanceName );
                    } else {
                        appCategories.Add( applicationCategory.CategoryInstance_UniqueID );

                        app["CategoryInstance_UniqueIDs"].StringArrayValue = (string[])appCategories.ToArray( typeof( string ) );

                        app.Put();
                    }
                }
            }
        }

        public void RemoveCMApplicationCategory(string appName, string categoryName) {
            SmsProvider smsProvider = new SmsProvider();
            WqlConnectionManager connectionManager = smsProvider.Connect( SiteServer );

            ObservableCollection<CMApplicationCategory> applicationCategories = GetCMApplicationCategories( false );

            string appQuery = string.Format( "SELECT * FROM SMS_ApplicationLatest WHERE (IsHidden = 0) AND (LocalizedDisplayName = '{0}')", appName );

            IResultObject appQueryResult = connectionManager.QueryProcessor.ExecuteQuery( appQuery );

            if(appQueryResult != null) {
                foreach(IResultObject app in appQueryResult) {
                    ArrayList appCategories = new ArrayList( app["CategoryInstance_UniqueIDs"].StringArrayValue );

                    CMApplicationCategory applicationCategory = applicationCategories.Where( c => c.LocalizedCategoryInstanceName == categoryName ).First();

                    if(appCategories.Contains(applicationCategory.CategoryInstance_UniqueID)) {
                        appCategories.Remove( applicationCategory.CategoryInstance_UniqueID );

                        app["CategoryInstance_UniqueIDs"].StringArrayValue = (string[])appCategories.ToArray( typeof( string ) );
                        app.Put();
                    } else {
                        MessageBox.Show( appName + " doesn't contain " + categoryName );
                    }
                }
            }
        }

        public List<CMApplication> GetCMApplications( string filter ) {
            List<CMApplication> cmApps = new List<CMApplication>();

            SmsProvider smsProvider = new SmsProvider();
            WqlConnectionManager connectionManager = smsProvider.Connect( SiteServer );

            string appQuery = null;
            if( !string.IsNullOrEmpty( filter ) ) {
                appQuery = string.Format( "SELECT * FROM SMS_ApplicationLatest WHERE (IsHidden = 0) AND (LocalizedDisplayName = '{0}')", filter );
            } else {
                appQuery = "SELECT * FROM SMS_ApplicationLatest WHERE (IsHidden = 0)";
            }

            IResultObject appQueryResult = connectionManager.QueryProcessor.ExecuteQuery( appQuery );

            if( appQueryResult != null ) {
                foreach( IResultObject app in appQueryResult ) {
                    CMApplication application = new CMApplication();
                    
                    application.ApplicationName = app["LocalizedDisplayName"].StringValue;

                    if( app["LocalizedCategoryInstanceNames"] != null ) {
                        foreach( string category in app["LocalizedCategoryInstanceNames"].StringArrayValue ) {
                            if( !String.IsNullOrEmpty( category ) ) {
                                
                            }
                        }
                    }
                    cmApps.Add( application );
                }
            }

            return cmApps;
        }

        public List<CMApplication> GetCMApplicationsByCategory( string category ) {
            List<CMApplication> cmApps = new List<CMApplication>();

            SmsProvider smsProvider = new SmsProvider();
            WqlConnectionManager connectionManager = smsProvider.Connect( SiteServer );

            string appQuery = null;
            if( category == "All" ) {
                appQuery = string.Format( "SELECT * FROM SMS_ApplicationLatest WHERE (IsHidden = 0)" );
            } else {
                appQuery = string.Format( "SELECT * FROM SMS_ApplicationLatest WHERE (IsHidden = 0) AND (LocalizedCategoryInstanceNames = '{0}')", category );
            }

            try {
                IResultObject appQueryResult = connectionManager.QueryProcessor.ExecuteQuery( appQuery );

                if( appQueryResult != null ) {
                    foreach( IResultObject app in appQueryResult ) {
                        CMApplication application = new CMApplication();
                        application.ApplicationName = app["LocalizedDisplayName"].StringValue;

                        if( app["LocalizedCategoryInstanceNames"] != null ) {
                            foreach( string appCategory in app["LocalizedCategoryInstanceNames"].StringArrayValue ) {
                                if( !string.IsNullOrEmpty( appCategory ) ) {
                                    application.ApplicationCategories.Add( appCategory );
                                }
                            }
                        }

                        cmApps.Add( application );
                    }
                }

                
            } catch( SmsException e ) {
                MessageBox.Show( "Error in GetCMApplicationsByCategory SmsException: " + e.Message );
            } catch( NullReferenceException e ) {
                MessageBox.Show( "Error in GetCMApplicationsByCategory NullReferenceException: " + e.Message );
            }

            return cmApps;
        }

        public ObservableCollection<CMApplicationCategory> GetCMApplicationCategories(bool includeAllApplications) {
            ObservableCollection<CMApplicationCategory> applicationCategories = new ObservableCollection<CMApplicationCategory>();

            SmsProvider smsProvider = new SmsProvider();
            WqlConnectionManager connectionManager = smsProvider.Connect( SiteServer );

            string appCatagoriesQuery = "SELECT * FROM SMS_CategoryInstance WHERE (CategoryTypeName = 'AppCategories')";
            try {
                IResultObject appCategoriesQueryResult = connectionManager.QueryProcessor.ExecuteQuery( appCatagoriesQuery );

                if( appCategoriesQueryResult != null ) {
                    foreach( IResultObject appCategory in appCategoriesQueryResult ) {
                        CMApplicationCategory cmApplicationCategory = new CMApplicationCategory();

                        cmApplicationCategory.CategoryInstanceID = appCategory["CategoryInstanceID"].IntegerValue;
                        cmApplicationCategory.CategoryInstance_UniqueID = appCategory["CategoryInstance_UniqueID"].StringValue;
                        cmApplicationCategory.CategoryTypeName = appCategory["CategoryTypeName"].StringValue;
                        cmApplicationCategory.LocalizedCategoryInstanceName = appCategory["LocalizedCategoryInstanceName"].StringValue;
                        cmApplicationCategory.ParentCategoryInstanceID = appCategory["ParentCategoryInstanceID"].IntegerValue;
                        cmApplicationCategory.SourceSite = appCategory["SourceSite"].StringValue;

                        applicationCategories.Add( cmApplicationCategory );
                    }

                    // Add All Categories Option
                    if( includeAllApplications ) {
                        CMApplicationCategory allCategory = new CMApplicationCategory {
                            LocalizedCategoryInstanceName = "All"
                        };

                        applicationCategories.Add( allCategory );
                    }
                }
            } catch(SmsException e) {
                MessageBox.Show( "Error connecting to site server " + SiteServer + ": " + e.Message );
            } catch( NullReferenceException e) {
                MessageBox.Show( "Error in GetCMApplicationCategories NullReferenceException: " + e.Message );
            }

            return applicationCategories;
        }
    }
}
