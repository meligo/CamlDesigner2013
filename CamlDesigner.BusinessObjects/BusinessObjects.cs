using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CAMLDesigner.BusinessObjects
{
    public class GeneralInfo
    {
        public string SharePointUrl { get; set; }
        public BPoint.SharePoint.Common.Enumerations.ApiConnectionType ConnectionType { get; set; }
        public BPoint.SharePoint.Common.Enumerations.SharePointVersions SharePointVersion { get; set; }
        //public bool UseDefaultCredentials { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public bool ExecuteQuery { get; set; }


        public GeneralInfo(string sharePointUrl)
        {
            this.SharePointUrl = sharePointUrl;
            //this.UseDefaultCredentials = true;
        }

        public GeneralInfo(string sharePointUrl, BPoint.SharePoint.Common.Enumerations.ApiConnectionType connectionType)
            :this(sharePointUrl)
        {
            this.ConnectionType = connectionType;
            this.SharePointVersion = BPoint.SharePoint.Common.Enumerations.SharePointVersions.SP2013;
            //this.UseDefaultCredentials = true;
        }

        public GeneralInfo(string sharePointUrl, BPoint.SharePoint.Common.Enumerations.ApiConnectionType connectionType, BPoint.SharePoint.Common.Enumerations.SharePointVersions sharePointVersion)
            : this(sharePointUrl)
        {
            this.ConnectionType = connectionType;
            this.SharePointVersion = sharePointVersion;
            //this.UseDefaultCredentials = true;
        }

        public GeneralInfo(string sharePointUrl, BPoint.SharePoint.Common.Enumerations.ApiConnectionType connectionType, 
            string username, string password, string domain)
            : this(sharePointUrl, connectionType)
        {
            if (string.IsNullOrEmpty(username))
                throw new Exception("Please, fill out a username.");

            if (string.IsNullOrEmpty(password))
                throw new Exception("Please, fill out a password.");

            //this.UseDefaultCredentials = false;
            this.Username = username;
            this.Password = password;
            this.Domain = domain;
        }

        public GeneralInfo(string sharePointUrl, BPoint.SharePoint.Common.Enumerations.ApiConnectionType connectionType, BPoint.SharePoint.Common.Enumerations.SharePointVersions sharePointVersion,
            string username, string password, string domain)
            : this(sharePointUrl, connectionType, sharePointVersion)
        {
            if (string.IsNullOrEmpty(username))
                throw new Exception("Please, fill out a username.");

            if (string.IsNullOrEmpty(password))
                throw new Exception("Please, fill out a password.");

            //this.UseDefaultCredentials = false;
            this.Username = username;
            this.Password = password;
            this.Domain = domain;
        }
    }
}
