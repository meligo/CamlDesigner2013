using BPoint.SharePoint.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using BPoint.SharePoint.Common; 

namespace BPoint.Common.Objects
{
    public class MainObject
    {
        public XmlDocument CamlDocument { get; set; }
        public SortedList<int, ViewField> ViewFieldList { get; set; }
        public SortedList<int, WhereField> WhereFieldList { get; set; }
        public SortedList<int, OrderByField> OrderByFieldList { get; set; }
        public SortedList<int, GroupByField> GroupByFieldList { get; set; }
        public BPoint.SharePoint.Objects.QueryOptions QueryOptions { get; set; }
        public bool ViewFieldsOnly { get; set; }
        public Enumerations.LanguageType LanguageType { get; set; }
        public Enumerations.SnippetType SnippetType { get; set; }
        public Enumerations.QueryType QueryType { get; set; }
        public XmlNode ViewFieldsNode { get; set; }
        public XmlNode WhereNode { get; set; }
        public XmlNode OrderByNode { get; set; }
        public XmlNode QueryOptionsNode { get; set; }

        public MainObject()
            : base()
        {

        }

        public MainObject(XmlDocument camldocument)
            : this()
        {
            CamlDocument = camldocument;
        }

        public MainObject(XmlDocument camldocument, SortedList<int, BPoint.SharePoint.Objects.ViewField> viewfieldsList)
            : this(camldocument)
        {
            ViewFieldList = viewfieldsList;
        }

        public MainObject(XmlDocument camldocument, SortedList<int, BPoint.SharePoint.Objects.ViewField> viewfieldsList, SortedList<int, BPoint.SharePoint.Objects.WhereField> whereField)
            : this(camldocument, viewfieldsList)
        {
            WhereFieldList = whereField;
        }

        public MainObject(XmlDocument camldocument, SortedList<int, BPoint.SharePoint.Objects.ViewField> viewfieldsList, SortedList<int, BPoint.SharePoint.Objects.WhereField> whereField, BPoint.SharePoint.Objects.QueryOptions queryOptions)
            : this(camldocument, viewfieldsList, whereField)
        {
            QueryOptions = queryOptions;
        }

        public MainObject(XmlDocument camldocument, SortedList<int, BPoint.SharePoint.Objects.ViewField> viewfieldsList, SortedList<int, BPoint.SharePoint.Objects.WhereField> whereField, BPoint.SharePoint.Objects.QueryOptions queryOptions, bool viewFieldsOnly)
            : this(camldocument, viewfieldsList, whereField, queryOptions)
        {
            ViewFieldsOnly = viewFieldsOnly;
        }

        public MainObject(XmlDocument camldocument, SortedList<int, BPoint.SharePoint.Objects.ViewField> viewfieldsList, SortedList<int, BPoint.SharePoint.Objects.WhereField> whereField, BPoint.SharePoint.Objects.QueryOptions queryOptions, bool viewFieldsOnly, BPoint.SharePoint.Common.Enumerations.LanguageType languageType)
            : this(camldocument, viewfieldsList, whereField, queryOptions, viewFieldsOnly)
        {
            LanguageType = languageType;
        }

        public MainObject(XmlDocument camldocument, SortedList<int, BPoint.SharePoint.Objects.ViewField> viewfieldsList, SortedList<int, BPoint.SharePoint.Objects.WhereField> whereField, BPoint.SharePoint.Objects.QueryOptions queryOptions, bool viewFieldsOnly, BPoint.SharePoint.Common.Enumerations.LanguageType languageType, BPoint.SharePoint.Common.Enumerations.SnippetType snippetType)
            : this(camldocument, viewfieldsList, whereField, queryOptions, viewFieldsOnly, languageType)
        {
            SnippetType = snippetType;
        }

        public MainObject(XmlDocument camldocument, SortedList<int, BPoint.SharePoint.Objects.ViewField> viewfieldsList, SortedList<int, BPoint.SharePoint.Objects.WhereField> whereField, BPoint.SharePoint.Objects.QueryOptions queryOptions, bool viewFieldsOnly, BPoint.SharePoint.Common.Enumerations.LanguageType languageType, BPoint.SharePoint.Common.Enumerations.SnippetType snippetType, BPoint.SharePoint.Common.Enumerations.QueryType queryType)
            : this(camldocument, viewfieldsList, whereField, queryOptions, viewFieldsOnly, languageType, snippetType)
        {
            QueryType = queryType;
        }

        public MainObject(XmlDocument camldocument, SortedList<int, BPoint.SharePoint.Objects.ViewField> viewfieldsList, SortedList<int, BPoint.SharePoint.Objects.WhereField> whereField, BPoint.SharePoint.Objects.QueryOptions queryOptions, bool viewFieldsOnly, BPoint.SharePoint.Common.Enumerations.LanguageType languageType, BPoint.SharePoint.Common.Enumerations.SnippetType snippetType, BPoint.SharePoint.Common.Enumerations.QueryType queryType, SortedList<int, OrderByField> orderByFieldList)
            : this(camldocument, viewfieldsList, whereField, queryOptions, viewFieldsOnly, languageType, snippetType, queryType)
        {
            OrderByFieldList = orderByFieldList;
        }

        public MainObject(XmlDocument camldocument, SortedList<int, BPoint.SharePoint.Objects.ViewField> viewfieldsList, SortedList<int, BPoint.SharePoint.Objects.WhereField> whereField, BPoint.SharePoint.Objects.QueryOptions queryOptions, bool viewFieldsOnly, BPoint.SharePoint.Common.Enumerations.LanguageType languageType, BPoint.SharePoint.Common.Enumerations.SnippetType snippetType, BPoint.SharePoint.Common.Enumerations.QueryType queryType, SortedList<int, OrderByField> orderByFieldList, SortedList<int,GroupByField> groupByFieldList)
            : this(camldocument, viewfieldsList, whereField, queryOptions, viewFieldsOnly, languageType, snippetType, queryType, orderByFieldList)
        {
            GroupByFieldList = groupByFieldList;
        }

        public MainObject(XmlDocument camldocument, SortedList<int, BPoint.SharePoint.Objects.ViewField> viewfieldsList, SortedList<int, BPoint.SharePoint.Objects.WhereField> whereField, BPoint.SharePoint.Objects.QueryOptions queryOptions, bool viewFieldsOnly, BPoint.SharePoint.Common.Enumerations.LanguageType languageType, BPoint.SharePoint.Common.Enumerations.SnippetType snippetType, BPoint.SharePoint.Common.Enumerations.QueryType queryType, SortedList<int, OrderByField> orderByFieldList, SortedList<int, GroupByField> groupByFieldList, XmlNode viewfieldsnode)
            : this(camldocument, viewfieldsList, whereField, queryOptions, viewFieldsOnly, languageType, snippetType, queryType, orderByFieldList,groupByFieldList)
        {
            ViewFieldsNode = viewfieldsnode;
        }

        public MainObject(XmlDocument camldocument, SortedList<int, BPoint.SharePoint.Objects.ViewField> viewfieldsList, SortedList<int, BPoint.SharePoint.Objects.WhereField> whereField, BPoint.SharePoint.Objects.QueryOptions queryOptions, bool viewFieldsOnly, BPoint.SharePoint.Common.Enumerations.LanguageType languageType, BPoint.SharePoint.Common.Enumerations.SnippetType snippetType, BPoint.SharePoint.Common.Enumerations.QueryType queryType, SortedList<int, OrderByField> orderByFieldList, SortedList<int, GroupByField> groupByFieldList, XmlNode viewfieldsnode, XmlNode wherenode)
            : this(camldocument, viewfieldsList, whereField, queryOptions, viewFieldsOnly, languageType, snippetType, queryType, orderByFieldList, groupByFieldList, viewfieldsnode)
        {
            WhereNode = wherenode;
        }

        public MainObject(XmlDocument camldocument, SortedList<int, BPoint.SharePoint.Objects.ViewField> viewfieldsList, SortedList<int, BPoint.SharePoint.Objects.WhereField> whereField, BPoint.SharePoint.Objects.QueryOptions queryOptions, bool viewFieldsOnly, BPoint.SharePoint.Common.Enumerations.LanguageType languageType, BPoint.SharePoint.Common.Enumerations.SnippetType snippetType, BPoint.SharePoint.Common.Enumerations.QueryType queryType, SortedList<int, OrderByField> orderByFieldList, SortedList<int, GroupByField> groupByFieldList, XmlNode viewfieldsnode, XmlNode wherenode, XmlNode orderbynode)
            : this(camldocument, viewfieldsList, whereField, queryOptions, viewFieldsOnly, languageType, snippetType, queryType, orderByFieldList, groupByFieldList, viewfieldsnode, wherenode)
        {
            OrderByNode = orderbynode;
        }

        public MainObject(XmlDocument camldocument, SortedList<int, BPoint.SharePoint.Objects.ViewField> viewfieldsList, SortedList<int, BPoint.SharePoint.Objects.WhereField> whereField, BPoint.SharePoint.Objects.QueryOptions queryOptions, bool viewFieldsOnly, BPoint.SharePoint.Common.Enumerations.LanguageType languageType, BPoint.SharePoint.Common.Enumerations.SnippetType snippetType, BPoint.SharePoint.Common.Enumerations.QueryType queryType, SortedList<int, OrderByField> orderByFieldList, SortedList<int, GroupByField> groupByFieldList, XmlNode viewfieldsnode, XmlNode wherenode, XmlNode orderbynode, XmlNode queryoptionsnode)
            : this(camldocument, viewfieldsList, whereField, queryOptions, viewFieldsOnly, languageType, snippetType, queryType, orderByFieldList, groupByFieldList, viewfieldsnode, wherenode, orderbynode)
        {
            QueryOptionsNode = queryoptionsnode;
        }
    }
}