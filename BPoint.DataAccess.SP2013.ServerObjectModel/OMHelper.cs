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
using BPoint.Common.Objects;

namespace BPoint.SharePoint.DataAccess.SP2013
{
    public class OMHelper
    {
        private string sharePointUrl;

        //private SPWeb spWeb = null;
        //private SPSite spSite = null;

        #region Constructors
        public OMHelper(string url)
        {
            this.sharePointUrl = url;
            //InitializeWeb();
        }

        #endregion

        public string Url
        {
            get { return sharePointUrl; }
            set { sharePointUrl = value; }
        }

        //public CustomObjects.Web GetWeb(string webUrl)
        //{
        //    if (string.IsNullOrEmpty(webUrl)) return null;

        //    CustomObjects.Web web = UtilityFunctions.DoInSPWeb(webUrl, null, spWeb =>
        //    {
        //        CustomObjects.Web w = new CustomObjects.Web();
        //        w.Title = spWeb.Title;
        //        w.ID = spWeb.ID;
        //        w.Url = spWeb.Url;
        //        w.WebTemplate = spWeb.WebTemplate;
        //        return w;
        //    }) as CustomObjects.Web;

        //    return web;

        //}

        //public List<CustomObjects.Web> GetWebs(string webUrl)
        //{
        //    if (string.IsNullOrEmpty(webUrl)) return null;

        //    List<CustomObjects.Web> webs = UtilityFunctions.DoInSPWeb(webUrl, null, spWeb =>
        //    {
        //        List<CustomObjects.Web> subWebs = new List<CustomObjects.Web>();
        //        foreach (SPWeb subWeb in spWeb.Webs)
        //        {
        //            CustomObjects.Web web = null;
        //            web.Title = subWeb.Title;
        //            web.ID = subWeb.ID;
        //            web.Url = subWeb.Url;
        //            web.WebTemplate = subWeb.WebTemplate;
        //            subWebs.Add(web);
        //        }
        //        return subWebs;
        //    }) as List<CustomObjects.Web>;

        //    return webs;
        //}

        //public List<string> GetSiteColumnGroups(string webUrl)
        //{
        //    if (string.IsNullOrEmpty(webUrl)) return null;

        //    SortedList<string, string> existingGroups = UtilityFunctions.DoInSPWeb(webUrl, null, spWeb =>
        //    {
        //        SortedList<string, string> groups = null;

        //        foreach (SPField field in spWeb.Fields)
        //        {
        //            if (groups == null)
        //                groups = new SortedList<string, string>();

        //            if (!groups.ContainsKey(field.Group) && field.Group != "_Hidden")
        //                groups.Add(field.Group, field.Group);
        //        }
        //        return groups;

        //    }) as SortedList<string, string>;

        //    return existingGroups.Keys.ToList<string>();
        //}

        //public List<CustomObjects.List> GetLists(string webUrl)
        //{
        //    if (string.IsNullOrEmpty(webUrl)) return null;

        //    List<CustomObjects.List> lists = UtilityFunctions.DoInSPWeb(webUrl, null, spWeb =>
        //    {
        //        List<CustomObjects.List> slists = new List<CustomObjects.List>();
        //        foreach (SPList splist in spWeb.Lists)
        //        {
        //            CustomObjects.List list = new CustomObjects.List();
        //            list.ID = splist.ID;
        //            list.Title = splist.Title;
        //            list.ImageUrl = splist.ImageUrl;
        //            list.TemplateID = UtilityFunctionsCAML.GetTemplateID(splist.BaseTemplate.ToString());
        //            if (splist.ParentWeb.IsRootWeb)
        //                list.WebUrl = splist.ParentWeb.Site.Url;
        //            else
        //            {
        //                if (splist.ParentWebUrl != "/")
        //                    list.WebUrl = splist.ParentWeb.Site.Url + splist.ParentWebUrl;
        //            }

        //            // TODO: Andy please check, this code runs far too long
        //            // fill document set collection if list is document library
        //            //                    if (splist.BaseType == SPBaseType.DocumentLibrary)
        //            //                    {
        //            //                        Dictionary<string, string> documentSetCollection = new Dictionary<string, string>();

        //            //                        if (splist != null)
        //            //                        {
        //            //                            SPQuery qry = new SPQuery();
        //            //                            qry.Query =
        //            //                            @"   <Where>
        //            //                                      <Eq>
        //            //                                         <FieldRef Name='ContentType' />
        //            //                                         <Value Type='Computed'>Document Set</Value>
        //            //                                      </Eq>
        //            //                                   </Where>";
        //            //                            SPListItemCollection listItems = splist.GetItems(qry);
        //            //                            if ((listItems != null) && (listItems.Count > 0))
        //            //                            {
        //            //                                foreach (SPListItem item in listItems)
        //            //                                {
        //            //                                    documentSetCollection.Add(item.Title, item.Folder.ServerRelativeUrl);
        //            //                                }
        //            //                                list.DocumentSets = documentSetCollection;
        //            //                            }
        //            //                        } 
        //            //                    }

        //            slists.Add(list);
        //        }
        //        return slists;
        //    }) as List<CustomObjects.List>;

        //    return lists;
        //}

        /*
            Attachments,
            Boolean,
            Choice,
            Computed,
            Counter,
            DateTime,
            Lookup,
            MultiChoice,
            Number,
            Text,
            User,
            Note
         */

        //public List<CustomObjects.Field> GetFields(string listName, bool excludeHiddenFields)
        //{
        //    if (string.IsNullOrEmpty(sharePointUrl))
        //        throw new Exception("SharePoint URL is not known.");

        //    if (string.IsNullOrEmpty(listName))
        //        throw new ArgumentNullException("List name cannot be empty.", listName);

        //    return UtilityFunctions.DoInSPWeb(sharePointUrl, null,
        //        spWeb => GetFieldsFunc(spWeb, listName, excludeHiddenFields)) as List<CustomObjects.Field>;
        //}

        //private List<CustomObjects.Field> GetFieldsFunc(SPWeb spWeb, string listName, bool excludeHiddenFields)
        //{
        //    SPList list = null;
        //    List<CustomObjects.Field> fields = null;

        //    try
        //    {
        //        list = spWeb.Lists[listName];
        //    }
        //    catch { }

        //    if (list != null)
        //    {
        //        fields = new List<CustomObjects.Field>();
        //        foreach (SPField spField in list.Fields)
        //        {
        //            if (!excludeHiddenFields || spField.InternalName == "FileRef" || (excludeHiddenFields && !spField.Hidden && (spField.ShowInViewForms == null || !(bool)spField.ShowInViewForms)))
        //            {
        //                switch (spField.TypeAsString)
        //                {
        //                    case "DateTime":
        //                        CustomObjects.DateTimeField dateTimeField = new CustomObjects.DateTimeField(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, spField.AuthoringInfo);
        //                        SPFieldDateTime dtField = spField as SPFieldDateTime;
        //                        if (dtField != null)
        //                        {
        //                            dateTimeField.DisplayFormat = dtField.DisplayFormat.ToString();
        //                        }
        //                        fields.Add(dateTimeField);
        //                        break;

        //                    case "Choice":
        //                        CustomObjects.ChoiceField choiceField = new CustomObjects.ChoiceField(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, spField.AuthoringInfo);
        //                        SPFieldChoice cField = spField as SPFieldChoice;
        //                        if (cField != null && cField.Choices != null && cField.Choices.Count > 0)
        //                        {
        //                            choiceField.Choices = new List<string>();
        //                            foreach (string choice in cField.Choices)
        //                            {
        //                                choiceField.Choices.Add(choice);
        //                            }
        //                        }
        //                        fields.Add(choiceField);
        //                        break;

        //                    case "MultiChoice":
        //                        // TODO: hou rekening met de gedefinieerde render control (checkboxes, list)
        //                        CustomObjects.ChoiceField mchoiceField = new CustomObjects.ChoiceField(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, spField.AuthoringInfo);
        //                        SPFieldMultiChoice mcField = spField as SPFieldMultiChoice;
        //                        if (mcField != null && mcField.Choices != null && mcField.Choices.Count > 0)
        //                        {
        //                            mchoiceField.Choices = new List<string>();
        //                            foreach (string choice in mcField.Choices)
        //                            {
        //                                mchoiceField.Choices.Add(choice);
        //                            }
        //                        }
        //                        fields.Add(mchoiceField);
        //                        break;

        //                    case "Lookup":
        //                    case "LookupMulti":
        //                        CustomObjects.LookupField lookupField = new CustomObjects.LookupField(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, spField.AuthoringInfo);
        //                        SPFieldLookup lField = spField as SPFieldLookup;
        //                        if (lField != null)
        //                        {
        //                            if (!string.IsNullOrEmpty(lField.LookupList) && lField.IsRelationship)
        //                            {
        //                                Guid lookupListId = Guid.Empty;
        //                                if (Guid.TryParse(lField.LookupList, out lookupListId))
        //                                    lookupField.LookupListName = spWeb.Lists.GetList(lookupListId, false).Title;

        //                                if (!string.IsNullOrEmpty(lField.LookupField))
        //                                    lookupField.ShowField = lField.LookupField;
        //                            }
        //                            else if (lField.InternalName == "FileRef")
        //                            {
        //                                // list all folders in 
        //                                lookupField.LookupListName = lField.ParentList.Title;
        //                                lookupField.ShowField = lField.InternalName;
        //                            }

        //                            if (lField.LookupWebId != lField.ParentList.ParentWeb.ID)
        //                                lookupField.LookupWebId = lField.LookupWebId;
        //                            else
        //                                lookupField.LookupWebId = Guid.Empty;
        //                        }
        //                        fields.Add(lookupField);
        //                        break;

        //                    case "Note":
        //                        CustomObjects.NoteField noteField = new CustomObjects.NoteField(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, spField.AuthoringInfo);
        //                        SPFieldMultiLineText nField = spField as SPFieldMultiLineText;
        //                        if (nField != null)
        //                        {
        //                            noteField.NumberOfLines = nField.NumberOfLines;
        //                            noteField.RichText = nField.RichText;
        //                            noteField.RichTextMode = nField.RichTextMode.ToString();
        //                        }
        //                        fields.Add(noteField);
        //                        break;

        //                    case "User":
        //                    case "UserMulti":
        //                        // TODO: check the difference between SPField when User and UserMulti
        //                        CustomObjects.UserField userField = new CustomObjects.UserField(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, spField.AuthoringInfo);
        //                        SPFieldUser uField = spField as SPFieldUser;
        //                        if (uField != null)
        //                        {
        //                            userField.UserSelectionMode = uField.SelectionMode.ToString();
        //                            userField.MultiSelect = false;
        //                        }
        //                        fields.Add(userField);
        //                        break;

        //                    case "TaxonomyFieldType":
        //                    case "TaxonomyFieldTypeMulti":
        //                        CustomObjects.TaxonomyField taxonomyField = new CustomObjects.TaxonomyField(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, spField.AuthoringInfo, Guid.Empty, Guid.Empty);
        //                        Microsoft.SharePoint.Taxonomy.TaxonomyField tField = spField as Microsoft.SharePoint.Taxonomy.TaxonomyField;
        //                        if (tField != null)
        //                        {
        //                            taxonomyField.TermStoreId = tField.SspId;
        //                            taxonomyField.TermSetId = tField.TermSetId;
        //                            taxonomyField.MultiSelect = tField.AllowMultipleValues;
        //                        }
        //                        fields.Add(taxonomyField);
        //                        break;

        //                    default:
        //                        CustomObjects.Field field = new CustomObjects.Field(spField.Id, spField.Title, spField.InternalName, spField.TypeAsString, spField.Required, spField.Hidden, spField.AuthoringInfo);
        //                        fields.Add(field);
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    return fields;
        //}

        //public List<CustomObjects.Field> GetFieldsForContentType(string contentTypeID)
        //{
        //    //List<Field> fields = null;

        //    //return fields;
        //    throw new NotImplementedException();
        //}

        //public List<CustomObjects.GroupValue> GetGroups()
        //{
        //    return UtilityFunctions.DoInSPWeb(sharePointUrl, null,
        //                    spWeb => GetGroupsFunc(spWeb)) as List<CustomObjects.GroupValue>;
        //}

        //public List<CustomObjects.GroupValue> GetGroupsFunc(SPWeb spWeb)
        //{
        //    List<CustomObjects.GroupValue> groups = new List<GroupValue>();

        //    foreach (SPGroup group in spWeb.SiteGroups)
        //    {
        //        groups.Add(new GroupValue(group.ID, group.Name));
        //    }

        //    return groups;

        //}

        //public List<CustomObjects.LookupValue> GetLookupValues(string listName, string showField, Guid lookupWebId)
        //{
        //    if (string.IsNullOrEmpty(listName)) return null;

        //    List<CustomObjects.LookupValue> values = null;

        //    if (lookupWebId == Guid.Empty)
        //    {
        //        values = UtilityFunctions.DoInSPWeb(sharePointUrl, null,
        //            spWeb => GetLookupValuesFunc(spWeb, listName, showField)) as List<CustomObjects.LookupValue>;
        //    }
        //    else
        //    {
        //        values = UtilityFunctions.DoInSPWeb(sharePointUrl, lookupWebId,
        //            spWeb => GetLookupValuesFunc(spWeb, listName, showField)) as List<CustomObjects.LookupValue>;
        //    }

        //    return values;
        //}


        //private List<CustomObjects.LookupValue> GetLookupValuesFunc(SPWeb spWeb, string listName, string showField)
        //{
        //    List<CustomObjects.LookupValue> values = null;
        //    SPList list = null;

        //    try
        //    {
        //        // check if the incoming string is a guid or a real list name
        //        Guid listId = Guid.Empty;
        //        try
        //        {
        //            listId = new Guid(listName);
        //        }
        //        catch { }
        //        if (listId == Guid.Empty)
        //            list = spWeb.Lists[listName];
        //        else
        //            list = spWeb.Lists[listId];
        //    }
        //    catch { }

        //    // TODO: bouw iets in wanneer er te veel list items zijn
        //    if (list != null && list.ItemCount > 0)
        //    {
        //        values = new List<CustomObjects.LookupValue>();
        //        if (showField != "FileRef")
        //        {
        //            SPQuery query = new SPQuery();
        //            foreach (SPListItem listitem in list.GetItems(query))
        //                values.Add(new CustomObjects.LookupValue(listitem.ID, listitem[showField].ToString()));
        //        }
        //        else
        //        {
        //            // add the root folder
        //            values.Add(new CustomObjects.LookupValue(0, list.RootFolder.ServerRelativeUrl));
        //            // add the sub folders
        //            SPQuery query = new SPQuery();
        //            query.Query = BPoint.Common.CAMLConstants.FolderQuery;
        //            query.ViewAttributes = "Scope='RecursiveAll'";
        //            foreach (SPListItem listitem in list.GetItems(query))
        //                values.Add(new CustomObjects.LookupValue(listitem.ID, listitem[showField].ToString()));
        //        }
        //    }

        //    return values;
        //}

        //public List<CustomObjects.TaxonomyValue> GetTaxonomyValues(Guid termStoreId, Guid termSetId)
        //{
        //    if (termStoreId == Guid.Empty || termSetId == Guid.Empty)
        //    {
        //        return null;
        //    }

        //    return UtilityFunctions.DoInSPWeb(sharePointUrl, null,
        //        spWeb => GetTaxonomyValuesFunc(spWeb, termStoreId, termSetId)) as List<CustomObjects.TaxonomyValue>;
        //}


        //private List<CustomObjects.TaxonomyValue> GetTaxonomyValuesFunc(SPWeb spWeb, Guid termStoreId, Guid termSetId)
        //{
        //    //List<CustomObjects.TaxonomyValue> values = null;
        //    TaxonomySession taxonomySession = new TaxonomySession(spWeb.Site);

        //    TermStore termStore = taxonomySession.TermStores[termStoreId];
        //    TermSet termSet = termStore.GetTermSet(termSetId);
        //    Dictionary<Guid, TaxonomyValue> taxonomyDictionary = new Dictionary<Guid, TaxonomyValue>();

        //    if (termSet != null)
        //    {
        //        TermCollection terms = termSet.Terms;

        //        foreach (Term term in terms)
        //        {
        //            int[] wssIds = GetWssIdsForTerm(spWeb.Site, term);
        //            taxonomyDictionary.Add(term.Id, new TaxonomyValue(term.Id, term.Name, Guid.Empty, wssIds));
        //        }

        //        foreach (Term term in terms)
        //        {
        //            TaxonomyValue parent = taxonomyDictionary[term.Id];

        //            if (term.TermsCount > 0)
        //            {
        //                AddTermsToDictionary(spWeb, parent, term.Terms, ref taxonomyDictionary);
        //            }
        //        }
        //    }

        //    return taxonomyDictionary.Values.ToList();
        //}

        //public CustomObjects.TaxonomyValue GetTaxonomyValue(Guid termStoreId, Guid termSetId, string input)
        //{
        //    return UtilityFunctions.DoInSPWeb(sharePointUrl, null,
        //        spWeb => GetTaxonomyValueFunc(spWeb, termStoreId, termSetId, input)) as CustomObjects.TaxonomyValue;

        //}

        //private CustomObjects.TaxonomyValue GetTaxonomyValueFunc(SPWeb spWeb, Guid termStoreId, Guid termSetId, string input)
        //{
        //    TaxonomyValue taxonomyValue = null;

        //    // check if the input is an integer or a string
        //    string termName = null;
        //    int wssId = 0;
        //    int.TryParse(input, out wssId);
        //    if (wssId > 0)
        //    {
        //        // retrieve the taxonomy value based on the wss Id
        //        termName = GetTermForWssId(spWeb.Site, wssId);
        //    }
        //    else
        //        termName = input;

        //    if (!string.IsNullOrEmpty(termName))
        //    {
        //        TaxonomySession taxonomySession = new TaxonomySession(spWeb.Site);

        //        TermStore termStore = taxonomySession.TermStores[termStoreId];
        //        TermSet termSet = termStore.GetTermSet(termSetId);
        //        Term term = termSet.Terms[termName];

        //        // get the wss Id of the term
        //        int[] wssIds = null;
        //        if (termName == input)
        //            wssIds = GetWssIdsForTerm(spWeb.Site, term);
        //        else
        //            wssIds = new int[] { wssId };

        //        taxonomyValue = new TaxonomyValue(term.Id, term.Name, Guid.Empty, wssIds);
        //    }

        //    return taxonomyValue;
        //}


        //public void AddTermsToDictionary(SPWeb spWeb, TaxonomyValue parent, TermCollection terms, ref Dictionary<Guid, TaxonomyValue> taxonomyDictionary)
        //{
        //    foreach (Term child in terms)
        //    {
        //        TaxonomyValue tv = null;
        //        if (taxonomyDictionary.ContainsKey(child.Id))
        //        {
        //            tv = taxonomyDictionary[child.Id];
        //            if (tv.ParentId != parent.ID)
        //            {
        //                tv.ParentId = parent.ID;
        //            }
        //        }
        //        else
        //        {
        //            int[] wssIds = GetWssIdsForTerm(spWeb.Site, child);
        //            tv = new TaxonomyValue(child.Id, child.Name, parent.ID, wssIds);
        //            parent.Terms.Add(tv);
        //        }

        //        if (child.TermsCount > 0 && tv != null)
        //        {
        //            AddTermsToDictionary(spWeb, tv, child.Terms, ref taxonomyDictionary);
        //        }
        //    }
        //}

        //private string GetTermForWssId(SPSite site, int wssId)
        //{
        //    string termName = null;

        //    SPList taxonomyList = site.RootWeb.Lists.TryGetList("TaxonomyHiddenList");
        //    if (taxonomyList != null)
        //    {
        //        SPListItem item = taxonomyList.GetItemById(wssId);
        //        if (item != null)
        //        {
        //            termName = item["Term"].ToString();
        //        }
        //    }

        //    return termName;
        //}

        //public int[] GetWssIdsForTerm(SPSite site, Term term)
        //{
        //    List<int> idList = null;
        //    SPList taxonomyList = site.RootWeb.Lists.TryGetList("TaxonomyHiddenList");
        //    if (taxonomyList != null)
        //    {
        //        SPQuery query = new SPQuery();
        //        query.Query = "<Where><Eq><FieldRef Name='IdForTerm'/><Value Type='Text'>" + term.Id + "</Value></Eq></Where> ";
        //        SPListItemCollection items = taxonomyList.GetItems(query);
        //        if (items.Count > 0)
        //        {
        //            idList = new List<int>();
        //            foreach (SPListItem item in items)
        //            {
        //                idList.Add(item.ID);
        //            }
        //        }
        //    }

        //    if (idList == null)
        //        return null;
        //    else
        //        return idList.ToArray();
        //}

        //public DataTable ExecuteQuery(string listName, MainObject mainobject, BPoint.SharePoint.Common.Enumerations.QueryType queryType)
        //{
        //    return UtilityFunctions.DoInSPWeb(sharePointUrl, null,
        //        spWeb => ExecuteQueryFunc(spWeb, listName, mainobject, queryType)) as DataTable;
        //}

        //private DataTable ExecuteQueryFunc(SPWeb spWeb, string listName, MainObject mainobject, BPoint.SharePoint.Common.Enumerations.QueryType queryType)
        //{
        //    DataTable resultTable = null;

        //    // retrieve the ViewFields element
        //    mainobject.ViewFieldsNode = mainobject.CamlDocument.SelectSingleNode("//ViewFields");

        //    // retrieve the Where element
        //    mainobject.WhereNode = mainobject.CamlDocument.SelectSingleNode("//Where");

        //    // retrieve the OrderBy element
        //    mainobject.OrderByNode = mainobject.CamlDocument.SelectSingleNode("//OrderBy");

        //    // retrieve the QueryOptions element
        //    mainobject.QueryOptionsNode = mainobject.CamlDocument.SelectSingleNode("//QueryOptions");

        //    if (mainobject.WhereNode != null || mainobject.OrderByNode != null || mainobject.ViewFieldsNode != null || mainobject.QueryOptions != null)
        //    {
        //        switch (queryType)
        //        {
        //            case Common.Enumerations.QueryType.CAMLQuery:
        //                resultTable = ExecuteCAMLQuery(spWeb, listName, mainobject);
        //                break;
        //            case Common.Enumerations.QueryType.SiteDataQuery:
        //                resultTable = ExecuteSiteDataQuery(spWeb, mainobject);
        //                break;
        //        }
        //    }

        //    return resultTable;
        //}

        //private DataTable ExecuteCAMLQuery(SPWeb spWeb, string listName, MainObject mainobject)
        //{
        //    SPList list = null;
        //    DataTable resultTable = null;

        //    try
        //    {
        //        list = spWeb.Lists[listName];
        //    }
        //    catch { }

        //    if (list != null)
        //    {
        //        SPQuery query = new SPQuery();

        //        // split the query
        //        if (mainobject.WhereNode != null)
        //            query.Query = mainobject.WhereNode.OuterXml;

        //        if (mainobject.OrderByNode != null)
        //        {
        //            if (string.IsNullOrEmpty(query.Query))
        //                query.Query = mainobject.OrderByNode.OuterXml;
        //            else
        //                query.Query += mainobject.OrderByNode.OuterXml;
        //        }

        //        if (mainobject.ViewFieldsNode != null)
        //            query.ViewFields = mainobject.ViewFieldsNode.InnerXml;

        //        if (mainobject.QueryOptions != null)
        //        {
        //            if (mainobject.QueryOptions.IncludeMandatoryColumns)
        //                query.IncludeMandatoryColumns = true;

        //            if (mainobject.QueryOptions.IncludeAttachmentUrls)
        //                query.IncludeAttachmentUrls = true;
        //            if (mainobject.QueryOptions.IncludeAttachmentVersion)
        //                query.IncludeAttachmentVersion = true;

        //            if (mainobject.QueryOptions.ExpandUserField)
        //                query.ExpandUserField = true;

        //            if (mainobject.QueryOptions.UtcDate)
        //                query.DatesInUtc = true;

        //            if (mainobject.QueryOptions.ViewFieldsOnly)
        //                query.ViewFieldsOnly = mainobject.QueryOptions.ViewFieldsOnly;

        //            if (mainobject.QueryOptions.RowLimit > 0)
        //                query.RowLimit = (uint)mainobject.QueryOptions.RowLimit;

        //            // Handle QueryOptions concerning Files and Folders
        //            if (mainobject.QueryOptions.QueryFilesAllFoldersDeep || mainobject.QueryOptions.QueryFoldersAllFoldersDeep || mainobject.QueryOptions.QueryFilesAndFoldersAllFoldersDeep)
        //            {
        //                query.ViewAttributes = "Scope='RecursiveAll'";
        //            }
        //            else if (!string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
        //            {
        //                if (mainobject.QueryOptions.QueryFilesInSubFolderDeep || mainobject.QueryOptions.QueryFoldersInSubFolder)
        //                    query.ViewAttributes = "Scope='Recursive'";
        //                else if (mainobject.QueryOptions.QueryFilesAndFoldersInSubFolderDeep)
        //                    query.ViewAttributes = "Scope='RecursiveAll'";
        //                else if (mainobject.QueryOptions.QueryFilesInSubFolder)
        //                    query.ViewAttributes = "Scope='FilesOnly'";
        //            }

        //            if (!string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
        //            {
        //                if (mainobject.QueryOptions.SubFolder != list.RootFolder.ServerRelativeUrl)
        //                {
        //                    string subfolder = mainobject.QueryOptions.SubFolder.Substring(list.RootFolder.ServerRelativeUrl.Length + 1);
        //                    query.Folder = list.RootFolder.SubFolders[subfolder];
        //                }
        //                else
        //                {
        //                    query.Folder = list.RootFolder.SubFolders[mainobject.QueryOptions.SubFolder];
        //                }
        //            }
        //            //    // TODO: check for SPQuery
        //            //    //query.OptimizeFor
        //        }

        //        try
        //        {
        //            resultTable = list.GetItems(query).GetDataTable();
        //        }
        //        catch { } // return a null resultTable if query contains an error
        //    }
        //    return resultTable;
        //}

        //private DataTable ExecuteSiteDataQuery(SPWeb spWeb, MainObject mainobject)
        //{
        //    DataTable resultTable = null;

        //    SPSiteDataQuery query = new SPSiteDataQuery();

        //    if (mainobject.ViewFieldsNode != null)
        //        query.ViewFields = mainobject.ViewFieldsNode.InnerXml;

        //    // split the query
        //    if (mainobject.WhereNode != null)
        //    {
        //        query.Query = mainobject.WhereNode.OuterXml;
        //    }

        //    if (mainobject.OrderByNode != null)
        //    {
        //        if (string.IsNullOrEmpty(query.Query))
        //        {
        //            query.Query = mainobject.OrderByNode.OuterXml;
        //        }
        //        else
        //        {
        //            query.Query += mainobject.OrderByNode.OuterXml;
        //        }
        //    }

        //    if (mainobject.QueryOptions != null)
        //    {
        //        if (!string.IsNullOrEmpty(mainobject.QueryOptions.ListTemplate))
        //            query.Lists = string.Format("<Lists ServerTemplate='{0}'/>", mainobject.QueryOptions.ListTemplate);
        //        if (!string.IsNullOrEmpty(mainobject.QueryOptions.WebScope))
        //            query.Webs = string.Format("<Webs Scope='{0}'/>", mainobject.QueryOptions.WebScope);
        //    }

        //    try
        //    {
        //        resultTable = spWeb.GetSiteData(query);
        //    }
        //    catch { } // return a null resultTable if query contains an error

        //    return resultTable;
        //}

        #region Static methods for code snippet generation

        public static string FormatCamlString(
            string listName,
            BPoint.SharePoint.Common.Enumerations.LanguageType languageType,
            XmlNode viewfieldsNode,
            XmlNode whereNode,
            XmlNode orderByNode,
            MainObject mainobject,
            BPoint.SharePoint.Common.Enumerations.QueryType queryType)
        {
            if (languageType == BPoint.SharePoint.Common.Enumerations.LanguageType.CSharp)
            {
                if (queryType == Common.Enumerations.QueryType.CAMLQuery)
                    return FormatCamlStringInCSharp(listName, viewfieldsNode, whereNode, orderByNode, mainobject, queryType);
                else
                    return FormatSiteDataQueryCamlStringInCSharp(listName, viewfieldsNode, whereNode, orderByNode, mainobject, queryType);
            }
            else
                return FormatCamlStringInVbNet(listName, viewfieldsNode, whereNode, orderByNode, mainobject, queryType);
        }

        private static string FormatCamlStringInCSharp(string listName, XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, MainObject mainobject,
            BPoint.SharePoint.Common.Enumerations.QueryType queryType)
        {
            StringBuilder sb = new StringBuilder(string.Format("SPList spList = spWeb.Lists.TryGetList(\"{0}\"); \n", listName));
            sb.Append("if (spList != null) \n");
            sb.Append("{ \n");

            string querystring = BPoint.SharePoint.Common.Builder.BuildQuerystring(whereNode, orderByNode);

            sb.Append("   SPQuery qry = new SPQuery(); \n");

            if (!string.IsNullOrEmpty(querystring))
            {
                querystring = BPoint.SharePoint.Common.Builder.IndentCAML(querystring);
                sb.Append("   qry.Query = \n");
                sb.Append(string.Format("   @\"{0}\"; \n", querystring));
            }

            if (viewfieldsNode != null)
            {
                //querystring = BPoint.SharePoint.Common.Builder.IndentCAML(viewfieldsNode.InnerXml);
                //sb.Append("   qry.ViewFields = \n");
                //sb.Append(string.Format("   @\"{0}\"; \n", querystring));
                if (mainobject.ViewFieldsOnly)
                {
                    sb.AppendLine(string.Format("   qry.ViewFieldsOnly = true;"));
                }
                sb.Append(string.Format("   qry.ViewFields = @\"{0}\"; \n", viewfieldsNode.InnerXml.Replace("\"", "'")));

            }

            if (mainobject.QueryOptions != null)
            {
                if (mainobject.QueryOptions.IncludeMandatoryColumns)
                    sb.Append("   qry.IncludeMandatoryColumns = true; \n");

                if (mainobject.QueryOptions.IncludeAttachmentUrls)
                    sb.Append("   qry.IncludeAttachmentUrls = true; \n");
                if (mainobject.QueryOptions.IncludeAttachmentVersion)
                    sb.Append("   qry.IncludeAttachmentVersion = true; \n");

                if (mainobject.QueryOptions.ExpandUserField)
                    sb.Append("   qry.ExpandUserField = true; \n");

                if (mainobject.QueryOptions.UtcDate)
                    sb.Append("   qry.DatesInUtc = true; \n");

                if (mainobject.QueryOptions.RowLimit > 0)
                    sb.Append(string.Format("   qry.RowLimit = {0}; \n", mainobject.QueryOptions.RowLimit.ToString()));

                // Handle QueryOptions concerning Files and Folders
                if (mainobject.QueryOptions.QueryFilesAllFoldersDeep || mainobject.QueryOptions.QueryFoldersAllFoldersDeep || mainobject.QueryOptions.QueryFilesAndFoldersAllFoldersDeep)
                {
                    sb.Append("   qry.ViewAttributes = \"Scope='RecursiveAll'\"; \n");
                }
                else if (!string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
                {
                    if (mainobject.QueryOptions.QueryFilesInSubFolderDeep)
                        sb.Append("   qry.ViewAttributes = \"Scope='Recursive'\"; \n");
                    else if (mainobject.QueryOptions.QueryFilesAndFoldersInSubFolderDeep)
                        sb.Append("   qry.ViewAttributes = \"Scope='RecursiveAll'\"; \n");
                    else if (mainobject.QueryOptions.QueryFilesInSubFolder)
                        sb.Append("   qry.ViewAttributes = \"Scope='FilesOnly'\"; \n");
                }

                if (!string.IsNullOrEmpty(mainobject.QueryOptions.SubFolder))
                    sb.Append(string.Format("   qry.Folder = spList.RootFolder.SubFolders[\"{0}\"]; \n", mainobject.QueryOptions.SubFolder));
            }

            sb.Append("   SPListItemCollection listItems = spList.GetItems(qry); \n");
            sb.Append("} \n");

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }

        private static string FormatSiteDataQueryCamlStringInCSharp(string listName, XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, MainObject mainobject,
            BPoint.SharePoint.Common.Enumerations.QueryType queryType)
        {
            StringBuilder sb = new StringBuilder("SPSiteDataQuery qry = new SPSiteDataQuery(); \n");

            string querystring = BPoint.SharePoint.Common.Builder.BuildQuerystring(whereNode, orderByNode);
            if (!string.IsNullOrEmpty(querystring))
            {
                querystring = BPoint.SharePoint.Common.Builder.IndentCAML(querystring);
                sb.Append("qry.Query = \n");
                sb.Append(string.Format("@\"{0}\"; \n", querystring));
            }

            if (viewfieldsNode != null)
            {
                sb.Append(string.Format("qry.ViewFields = \"{0}\"; \n", viewfieldsNode.InnerXml.Replace("\"", "'")));
            }

            if (mainobject.QueryOptions != null)
            {
                if (!string.IsNullOrEmpty(mainobject.QueryOptions.ListTemplate))
                    sb.Append(string.Format("qry.Lists = \"<Lists ServerTemplate='{0}'/>\"; \n", mainobject.QueryOptions.ListTemplate));

                if (!string.IsNullOrEmpty(mainobject.QueryOptions.WebScope))
                    sb.Append(string.Format("qry.Webs = \"<Webs Scope='{0}'/>\"; \n", mainobject.QueryOptions.WebScope));
            }

            sb.Append("DataTable resultTable = spWeb.GetSiteData(qry); \n");


            if (sb != null)
                return sb.ToString();
            else
                return null;
        }

        private static string FormatCamlStringInVbNet(string listName, XmlNode viewfieldsNode, XmlNode whereNode, XmlNode orderByNode, MainObject mainobject,
            BPoint.SharePoint.Common.Enumerations.QueryType queryType)
        {
            StringBuilder sb = null;

            if (sb != null)
                return sb.ToString();
            else
                return null;
        }


        #endregion

        #region InitializeMethods

        //private void InitializeWeb()
        //{
        //    if (string.IsNullOrEmpty(sharePointUrl))
        //        throw new Exception("The SharePoint URL is empty.");

        //    if (spSite == null)
        //        spSite = new SPSite(sharePointUrl);
        //    spWeb = spSite.OpenWeb();
        //}

        //private void RefreshWeb(string url)
        //{
        //    if (!string.IsNullOrEmpty(url) || sharePointUrl != url)
        //    {
        //        sharePointUrl = url;
        //        spSite = new SPSite(url);
        //        spWeb = spSite.OpenWeb();
        //    }
        //}

        #endregion

    }
}
