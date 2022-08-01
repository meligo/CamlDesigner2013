// ----------------------------------------------------------------------
// <copyright file="UtilityFunctions.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Windows.Media;
    using System.Xml;
    using CAMLDesigner.BusinessObjects;

    public class UtilityFunctions
    {
        public static BPoint.SharePoint.Common.Enumerations.ConnectionType GetConnectionType(bool useServerObjectModel, bool useClientObjectModel, bool useWebServices)
        {
            if (useServerObjectModel)
                return BPoint.SharePoint.Common.Enumerations.ConnectionType.ServerObjectModel;
            else if (useClientObjectModel)
                return BPoint.SharePoint.Common.Enumerations.ConnectionType.ClientObjectModelForDotnet;
            else
                return BPoint.SharePoint.Common.Enumerations.ConnectionType.WebServices;
        }

        /*
        public static SharePointVersion GetSharePointVersion(bool isSP2007, bool isSP2010)
        {
            if (isSP2007)
                return SharePointVersion.sp2007;
            else 
                return SharePointVersion.sp2010;
        }
         */

        public static Color GetColorFromHexString(string s)
        {
            s = s.Replace("#", string.Empty);
            byte a = System.Convert.ToByte(s.Substring(0, 2), 16);
            byte r = System.Convert.ToByte(s.Substring(2, 2), 16);
            byte g = System.Convert.ToByte(s.Substring(4, 2), 16);
            byte b = System.Convert.ToByte(s.Substring(6, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }

        public static void SetAppSetting(string key, string value, bool mustUpdate)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) return;

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (mustUpdate)
            {
                bool isFound = false;
                // check if there is already a value stored
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                if (appSettings != null)
                {
                    for (int i = 0; i < appSettings.Count; i++)
                    {
                        if (appSettings.GetKey(i) == key)
                        {
                            isFound = true;
                            if (appSettings[i] != value)
                            {
                                // existing appSettings are readonly
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

                                foreach (XmlElement element in xmlDoc.DocumentElement)
                                {
                                    if (element.Name.Equals("appSettings"))
                                    {
                                        foreach (XmlNode node in element.ChildNodes)
                                        {
                                            if (node.Attributes[0].Value.Equals(key))
                                            {
                                                node.Attributes[1].Value = value;
                                            }
                                        }
                                    }
                                }
                                xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                            }
                        }
                    }
                    if (!isFound)
                    {
                        config.AppSettings.Settings.Add(key, value);
                        config.Save(ConfigurationSaveMode.Modified, false);
                    }
                }
            }
            else
            {
                config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified, false);
            }

            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
        }

    }
}
