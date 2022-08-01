// ----------------------------------------------------------------------
// <copyright file="CamlEventArgs.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System.Xml;

    using BPoint.SharePoint.Objects;
    using System.Collections.Generic;

    public delegate void CAMLEventHandler(CamlEventArgs args);

    public class CamlEventArgs
    {
            public CamlEventArgs(
                string listName,
                XmlDocument camlDocument,
                SortedList<int, WhereField> whereFieldsList,
                QueryOptions queryOptions,
                BPoint.SharePoint.Common.Enumerations.ConnectionType connectionType,
                BPoint.SharePoint.Common.Enumerations.LanguageType languageType,
                BPoint.SharePoint.Common.Enumerations.QueryType queryType)
            {
                this.ListName = listName;
                this.CamlDocument = camlDocument;
                this.WhereFieldsList = whereFieldsList;
                this.QueryOptions = queryOptions;
                this.ConnectionType = connectionType;
                this.LanguageType = languageType;
                this.QueryType = queryType;
            }

            public XmlDocument CamlDocument
            {
                get;
                set;
            }

            public BPoint.SharePoint.Common.Enumerations.ConnectionType ConnectionType
            {
                get;
                set;
            }

            public BPoint.SharePoint.Common.Enumerations.LanguageType LanguageType
            {
                get;
                set;
            }

            public BPoint.SharePoint.Common.Enumerations.QueryType QueryType
            {
                get;
                set;
            }

            public string ListName
            {
                get;
                set;
            }

            public SortedList<int, WhereField> WhereFieldsList
            {
                get;
                set;
            }

            public QueryOptions QueryOptions
            {
                get;
                set;
            }
    }
}