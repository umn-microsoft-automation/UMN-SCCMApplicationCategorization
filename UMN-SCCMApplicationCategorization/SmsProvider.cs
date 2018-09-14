using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using System;
using System.Windows;

namespace UMN_SCCMApplicationCategorization {
    // Based on smsprovider from NickolajA
    // https://github.com/NickolajA/ConfigMgrWebService/blob/master/ConfigMgrWebService/smsProvider.cs
    class SmsProvider {
        public WqlConnectionManager Connect(string serverName) {
            try {
                SmsNamedValuesDictionary namedValues = new SmsNamedValuesDictionary();
                WqlConnectionManager connection = new WqlConnectionManager( namedValues );
                connection.Connect( serverName );

                return connection;
            } catch(SmsException ex) {
                MessageBox.Show( "SmsException: " + ex.Message );
            } catch(UnauthorizedAccessException ex) {
                MessageBox.Show( "Unauthorized access exception: " + ex.Message );
            } catch(Exception ex) {
                MessageBox.Show( "Exception: " + ex.Message );
            }

            return null;
        }

        public bool TestConnection(string serverName) {
            bool connectionStatus = true;
            try {
                SmsNamedValuesDictionary namedValues = new SmsNamedValuesDictionary();
                WqlConnectionManager connection = new WqlConnectionManager( namedValues );
                connection.Connect( serverName );
            } catch {
                connectionStatus = false;
            }
            return connectionStatus;
        }
    }
}
