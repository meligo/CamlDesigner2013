using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using Microsoft.SharePoint;
using CustomObjects = BPoint.SharePoint.Objects;
using BPoint.SharePoint.Objects;
using Microsoft.SharePoint.Taxonomy;
using BPoint.SharePoint.Common;

namespace BPoint.SharePoint.DataAccess.SP2013
{
    public class LinqHelper
    {
        #region Static methods for code snippet generation
        public static string FormatCamlString(string listName, BPoint.SharePoint.Common.Enumerations.LanguageType languageType,
          XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, QueryOptions queryOptions, string sharepointURL)
        {
            return FormatCamlStringInCSharp(listName, viewfieldsNode, whereNode, orderByNode, queryOptions, sharepointURL);
        }

        private static string FormatCamlStringInCSharp(string listName, XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, QueryOptions queryOptions, string sharepointURL)
        {
            StringBuilder sb = new StringBuilder(string.Format("using (SPSite site = new SPSite(\"{0}\"){ \n using (SPWeb web = site.OpenWeb()) {   \n    splist = web.Lists.TryGetList(\"{1}\") \n", sharepointURL, listName));
            sb.Append("if (splist != null) \n");
            sb.Append("{ \n");

            sb.Append("IEnumerable listitems = splist.Items.OfType(); \n");

            string wherelinq = BuildLinqWhere(whereNode);

            
            //sb.Append("var items = from obj in listitems where {0} \n",whereNode.InnerXml.ToString());


            string querystring = BPoint.SharePoint.Common.Builder.BuildQuerystring(whereNode, orderByNode);

            sb.Append("   $query = New-Object Microsoft.SharePoint.SPQuery; \n");

            if (!string.IsNullOrEmpty(querystring))
            {
                querystring = BPoint.SharePoint.Common.Builder.IndentCAML(querystring);
                sb.Append("   $query.Query = \n");
                sb.Append(string.Format("   \"{0}\"; \n", querystring));
            }

            if (viewfieldsNode != null)
            {
                //querystring = BPoint.SharePoint.Common.Builder.IndentCAML(viewfieldsNode.InnerXml);
                //sb.Append("   qry.ViewFields = \n");
                //sb.Append(string.Format("   @\"{0}\"; \n", querystring));
                string viewfieldsLinq = BuildLinqViewfields(viewfieldsNode);
                sb.Append(string.Format("   $query.ViewFields = \"{0}\"; \n", viewfieldsNode.InnerXml.Replace("\"", "'")));
                sb.Append("   $query.ViewFieldsOnly = $true; \n");
            }

            if (queryOptions != null)
            {
                if (queryOptions.IncludeMandatoryColumns)
                    sb.Append("   $query.IncludeMandatoryColumns = true; \n");

                if (queryOptions.IncludeAttachmentUrls)
                    sb.Append("   $query.IncludeAttachmentUrls = true; \n");
                if (queryOptions.IncludeAttachmentVersion)
                    sb.Append("   $query.IncludeAttachmentVersion = true; \n");

                if (queryOptions.ExpandUserField)
                    sb.Append("   $query.ExpandUserField = true; \n");

                if (queryOptions.UtcDate)
                    sb.Append("   $query.DatesInUtc = true; \n");

                if (queryOptions.RowLimit > 0)
                    sb.Append(string.Format("   $query.RowLimit = {0}; \n", queryOptions.RowLimit.ToString()));
                else
                    sb.Append("   $query.RowLimit = #ByDefault_2147483647 \n");

                // Handle QueryOptions concerning Files and Folders
                if (queryOptions.QueryFilesAllFoldersDeep || queryOptions.QueryFoldersAllFoldersDeep || queryOptions.QueryFilesAndFoldersAllFoldersDeep)
                {
                    sb.Append("   $query.ViewAttributes = \"Scope='RecursiveAll'\"; \n");
                }
                else if (!string.IsNullOrEmpty(queryOptions.SubFolder))
                {
                    if (queryOptions.QueryFilesInSubFolderDeep)
                        sb.Append("   $query.ViewAttributes = \"Scope='Recursive'\"; \n");
                    else if (queryOptions.QueryFilesAndFoldersInSubFolderDeep)
                        sb.Append("   $query.ViewAttributes = \"Scope='RecursiveAll'\"; \n");
                    else if (queryOptions.QueryFilesInSubFolder)
                        sb.Append("   $query.ViewAttributes = \"Scope='FilesOnly'\"; \n");
                }

                if (!string.IsNullOrEmpty(queryOptions.SubFolder))
                    sb.Append(string.Format("   $query.Folder = $splist.RootFolder.SubFolders[\"{0}\"]; \n", queryOptions.SubFolder));
            }

            sb.Append("   $items = $splist.GetItems($query); \n");
            sb.Append(" } \n } \n");

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }

        private static string BuildLinqViewfields(XmlNode viewfieldsNode)
        {
            StringBuilder vfields = new StringBuilder();

            vfields.Append(@"
            
              select new
                             {
                                 StudentName = c.Title,
                                 Course = c.Course.Title,
                                 Duration = c.Course.Duration,
                                 CreditPoints = c.Course.CreditPoints
                             };

            ");


            return vfields.ToString();
        }

        private static string BuildLinqWhere(XmlNode whereNode)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
