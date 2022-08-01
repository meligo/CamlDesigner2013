// ----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System.Windows;
    using System.Xml;
    using BPoint.SharePoint.Objects;
    using CamlDesigner.ViewModels;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool ClearControls = true;
        public static CAMLDesigner.BusinessObjects.GeneralInfo GeneralInformation = null;
        public static CAMLDesigner.BusinessObjects.Wrapper CamlWrapper = null;
        public static ListViewModel SelectedListViewModel = null;
        public static XmlDocument CamlDocument = null;
        public static QueryOptions QueryOptions = null;

        // set a default to CAML
        public static BPoint.SharePoint.Common.Enumerations.ConnectionType ConnectionType = BPoint.SharePoint.Common.Enumerations.ConnectionType.CAML;
        // set a default to C#
        public static BPoint.SharePoint.Common.Enumerations.LanguageType LanguageType = BPoint.SharePoint.Common.Enumerations.LanguageType.CSharp;
        // set a default to CAML Query
        public static BPoint.SharePoint.Common.Enumerations.QueryType QueryType = BPoint.SharePoint.Common.Enumerations.QueryType.CAMLQuery;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // TODO: get the information from a config file
            // GeneralInformation = new CAMLDesigner.BusinessObjects.GeneralInfo();
        }
    }
}
