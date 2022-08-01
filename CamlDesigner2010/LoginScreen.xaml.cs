// ----------------------------------------------------------------------
// <copyright file="LoginScreen.xaml.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Net;
    using System.Windows;
    using CamlDesigner.Helpers;
    using CAMLDesigner.BusinessObjects;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            this.InitializeComponent();

            SiteUrlComboBox.Items.Clear();
            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;

                if (appSettings != null)
                {
                    for (int i = 0; i < appSettings.Count; i++)
                    {
                        if (appSettings.GetKey(i) == "SPUrl")
                        {
                            string[] urls = appSettings[i].Split(',');
                            foreach (string url in urls)
                                SiteUrlComboBox.Items.Add(url);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile("LoginScreen: An error has occured with the app settings", ex);
            }

            string response = string.Empty;

            try
            {
                bool foundAssembly = Helpers.CheckClass.AssemblyExist("Microsoft.SharePoint");

                if (foundAssembly)
                {
                    chkServerObject.IsEnabled = foundAssembly;
                    chkServerObject.ToolTip = "Microsoft.SharePoint assembly isn't found, server object not supported!";
                }
                else
                {
                    chkClientObject.IsChecked = !foundAssembly;
                    App.GeneralInformation.ConnectionType = BPoint.SharePoint.Common.Enumerations.ConnectionType.ClientObjectModelForDotnet;
                }


            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile("LoginScreen: An error has occured while looking for the server assembly", ex);
            }

            try
            {
                if (App.GeneralInformation != null)
                {
                    // fill out the text boxes
                    if (!string.IsNullOrEmpty(App.GeneralInformation.SharePointUrl))
                        SiteUrlComboBox.Text = App.GeneralInformation.SharePointUrl;
                    else
                        SiteUrlComboBox.Text = string.Empty;

                    switch (App.GeneralInformation.ConnectionType)
                    {
                        case BPoint.SharePoint.Common.Enumerations.ConnectionType.ClientObjectModelForDotnet:
                            chkClientObject.IsChecked = true;
                            break;
                        case BPoint.SharePoint.Common.Enumerations.ConnectionType.ServerObjectModel:
                            chkServerObject.IsChecked = true;
                            break;
                        case BPoint.SharePoint.Common.Enumerations.ConnectionType.WebServices:
                            chkWebServices.IsChecked = true;
                            break;
                    }

                    /*
                    switch (App.GeneralInformation.SharePointVersion)
                    {
                        case SharePointVersion.sp2007:
                            // chk2007.IsChecked = true;
                            break;
                        case SharePointVersion.sp2010:
                            chk2010.IsChecked = true;
                            break;
                    }
                     */

                    if (!string.IsNullOrEmpty(App.GeneralInformation.Username))
                    {
                        txtUsername.Text = App.GeneralInformation.Username;
                        chkCurrentUser.IsChecked = false;
                        CredentialsAccordion.IsSelected = true;
                        CurrentUserAccordionItem.IsSelected = false;
                    }
                    else
                    {
                        txtUsername.Text = string.Empty;
                        CurrentUserAccordionItem.IsSelected = true;
                        CredentialsAccordion.IsSelected = false;
                    }

                    if (!string.IsNullOrEmpty(App.GeneralInformation.Password))
                        txtPassword.Password = App.GeneralInformation.Password;
                    else
                        txtPassword.Password = string.Empty;

                    if (!string.IsNullOrEmpty(App.GeneralInformation.Domain))
                        txtDomain.Text = App.GeneralInformation.Domain;
                    else
                        txtDomain.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLogFile("LoginScreen: An error has occured with the general info", ex);
            }
            SiteUrlComboBox.Focus();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            // capture the filled out data
            if (SiteUrlComboBox.Text.Length == 0)
            {
                MessageBox.Show("Please, fill out the URL to your SharePoint site.", "Insufficient data", MessageBoxButton.OK, MessageBoxImage.Information);
                Logger.WriteToLogFile("LoginScreen: Insufficient data, no URL", null);
                return;
            }

            if ((txtUsername.Text.Length > 0) && (txtPassword.Password.Length > 0) && (txtDomain.Text.Length == 0))
            {
                if (txtUsername.Text.ToLower().Contains("@"))
                {
                    char[] teken = { '@' };
                    string[] split = txtUsername.Text.Split(teken, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length == 2)
                    {
                        txtUsername.Text = split[0];
                        txtDomain.Text = split[1];
                    }
                }
                else
                {
                    MessageBox.Show("Please fill in the domain.", "Insufficient data", MessageBoxButton.OK, MessageBoxImage.Information);
                    Logger.WriteToLogFile("LoginScreen: Insufficient data, no domain", null);
                    return;
                }
            }
            try
            {
                BPoint.SharePoint.Common.Enumerations.ConnectionType connectionType =
                    UtilityFunctions.GetConnectionType(
                    (bool)chkServerObject.IsChecked,
                    (bool)chkClientObject.IsChecked,
                    (bool)chkWebServices.IsChecked);
                //SharePointVersion sharePointVersion = UtilityFunctions.GetSharePointVersion(false, (bool)chk2010.IsChecked);
                App.GeneralInformation = new CAMLDesigner.BusinessObjects.GeneralInfo(SiteUrlComboBox.Text, connectionType);
                if (txtUsername.Text.Length > 0)
                {
                    App.GeneralInformation.Username = txtUsername.Text;
                }

                if (txtPassword.Password.Length > 0)
                {
                    App.GeneralInformation.Password = txtPassword.Password;
                }

                if (txtDomain.Text.Length > 0)
                {
                    App.GeneralInformation.Domain = txtDomain.Text;
                }
            }
            catch(Exception ex)
            {
                Logger.WriteToLogFile("LoginScreen: An error has occured while filling in all the data", ex);
            }
            // set the CAML wrapper to null so that all parameters are correctly set
            App.CamlWrapper = null;

            try
            {
                // store a new URL in the config file
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                if (appSettings != null && SiteUrlComboBox.SelectedItem == null && SiteUrlComboBox.Text.Length > 0)
                {
                    bool isFound = false;
                    for (int i = 0; i < appSettings.Count; i++)
                    {
                        if (appSettings.GetKey(i) == "SPUrl" && appSettings[i].Contains(SiteUrlComboBox.Text))
                        {
                            isFound = true;
                            return;
                        }
                    }
                    if (!isFound)
                    {
                        UtilityFunctions.SetAppSetting("SPUrl", SiteUrlComboBox.Text, false);
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.WriteToLogFile("LoginScreen: An error has occured with the appSettings", ex);
            }

            // TODO: check if logon with current credentials is possible 
            // var newwindow = new MainScreen();
            // newwindow.Owner = this;
            // newwindow.Show();
            // this.Hide();
            App.ClearControls = true;
            this.Close();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != System.Windows.WindowState.Minimized)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
