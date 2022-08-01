// ----------------------------------------------------------------------
// <copyright file="About.xaml.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner.Controls
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Xml;

    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
       

        #region AboutData Provider
        #region Member data
        private XmlDocument xmlDoc = null;

        private const string PropertyNameTitle = "Title";
        private const string PropertyNameDescription = "Description";
        private const string PropertyNameProduct = "Product";
        private const string PropertyNameCopyright = "Copyright";
        private const string PropertyNameCompany = "Company";
        private const string PathRoot = "ApplicationInfo/";
        private const string PathTitle = PathRoot + PropertyNameTitle;
        private const string PathVersion = PathRoot + "Version";
        private const string PathDescription = PathRoot + PropertyNameDescription;
        private const string PathProduct = PathRoot + PropertyNameProduct;
        private const string PathCopyright = PathRoot + PropertyNameCopyright;
        private const string PathCompany = PathRoot + PropertyNameCompany;
        private const string PathLink = PathRoot + "Link";
        private const string PathLinkUri = PathRoot + "Link/@Uri";
        #endregion

        /// <summary>
        /// Initializes a new instance of the About class, Constructor that takes a parent for this About dialog.
        /// </summary>
        /// <param name="parent">Parent window for this dialog.</param>
        public About(Window parent)
            : this()
        {
            this.Owner = parent;
        }

        protected About()
        {
            this.InitializeComponent();
        }


        #region Properties
        /// <summary>
        /// Gets the title property, which is display in the About dialogs window title.
        /// </summary>
        public string ProductTitle
        {
            get
            {
                string result = this.CalculatePropertyValue<AssemblyTitleAttribute>(PropertyNameTitle, PathTitle);
                if (string.IsNullOrEmpty(result))
                {
                    // otherwise, just get the name of the assembly itself.
                    result = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the application's version information to show.
        /// </summary>
        public string Version
        {
            get
            {

                string result = string.Empty;
                // first, try to get the version string from the assembly.
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                if (version != null)
                {
                    result = version.ToString();
                }
                else
                {
                    // if that fails, try to get the version from a resource in the Application.
                    result = this.GetLogicalResourceString(PathVersion);
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public string Description
        {
            get { return this.CalculatePropertyValue<AssemblyDescriptionAttribute>(PropertyNameDescription, PathDescription); }

        }

        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        public string Product
        {
            get { return this.CalculatePropertyValue<AssemblyProductAttribute>(PropertyNameProduct, PathProduct); }
        }

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public string Copyright
        {
            get { return this.CalculatePropertyValue<AssemblyCopyrightAttribute>(PropertyNameCopyright, PathCopyright); }
        }

        /// <summary>
        /// Gets the product's company name.
        /// </summary>
        public string Company
        {
            get { return this.CalculatePropertyValue<AssemblyCompanyAttribute>(PropertyNameCompany, PathCompany); }
        }

        /// <summary>
        /// Gets the link text to display in the About dialog.
        /// </summary>
        public string LinkText
        {
            get { return this.GetLogicalResourceString(PathLink); }
        }

        /// <summary>
        /// Gets the link uri that is the navigation target of the link.
        /// </summary>
        public string LinkUri
        {
            get { return this.GetLogicalResourceString(PathLinkUri); }
        }
        #endregion


        #region Resource location methods


        /// <summary>
        /// Gets the XmlDataProvider's document from the resource dictionary.
        /// </summary>
        protected virtual XmlDocument ResourceXmlDocument
        {
            get
            {
                if (this.xmlDoc == null)
                {
                    // if we haven't already found the resource XmlDocument, then try to find it.
                    XmlDataProvider provider = this.TryFindResource("aboutProvider") as XmlDataProvider;
                    if (provider != null)
                    {
                        // save away the XmlDocument, so we don't have to get it multiple times.
                        this.xmlDoc = provider.Document;
                    }
                }
                return this.xmlDoc;
            }
        }

        /// <summary>
        /// Gets the specified data element from the XmlDataProvider in the resource dictionary.
        /// </summary>
        /// <param name="xpathQuery">An XPath query to the XML element to retrieve.</param>
        /// <returns>The resulting string value for the specified XML element. 
        /// Returns empty string if resource element couldn't be found.</returns>
        protected virtual string GetLogicalResourceString(string xpathQuery)
        {
            string result = string.Empty;
            // get the About xml information from the resources.
            XmlDocument doc = this.ResourceXmlDocument;
            if (doc != null)
            {
                // if we found the XmlDocument, then look for the specified data. 
                XmlNode node = doc.SelectSingleNode(xpathQuery);

                if (node != null)
                {
                    if (node is XmlAttribute)
                    {
                        // only an XmlAttribute has a Value set.
                        result = node.Value;
                    }
                    else
                    {
                        // otherwise, need to just return the inner text.
                        result = node.InnerText;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the specified property value either from a specific attribute, or from a resource dictionary.

        /// </summary>
        /// <typeparam name="T">Attribute type that we're trying to retrieve.</typeparam>
        /// <param name="propertyName">Property name to use on the attribute.</param>
        /// <param name="xpathQuery">XPath to the element in the XML data resource.</param>
        /// <returns>The resulting string to use for a property.
        /// Returns null if no data could be retrieved.</returns>
        private string CalculatePropertyValue<T>(string propertyName, string xpathQuery)
        {
            string result = string.Empty;
            // first, try to get the property value from an attribute.
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            if (attributes.Length > 0)
            {
                T attrib = (T)attributes[0];
                PropertyInfo property = attrib.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    result = property.GetValue(attributes[0], null) as string;
                }
            }

            // if the attribute wasn't found or it did not have a value, then look in an xml resource.
            if (result == string.Empty)
            {
                // if that fails, try to get it from a resource.
                result = this.GetLogicalResourceString(xpathQuery);
            }
            return result;
        }

        /// <summary>
        /// Handles click navigation on the hyperlink in the About dialog.

        /// </summary>
        /// <param name="sender">Object the sent the event.</param>
        /// <param name="e">Navigation events arguments.</param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri != null && string.IsNullOrEmpty(e.Uri.OriginalString) == false)
            {
                string uri = e.Uri.AbsoluteUri;
                Process.Start(new ProcessStartInfo(uri));
                e.Handled = true;
            }
        }
        #endregion
        #endregion
    }
}
