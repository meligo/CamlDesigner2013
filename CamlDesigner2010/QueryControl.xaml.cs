// ----------------------------------------------------------------------
// <copyright file="QueryControl.xaml.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using BPoint.SharePoint.Objects;
    using CamlDesigner.Controls;
    /// <summary>
    /// Interaction logic for QueryControl.xaml
    /// </summary>
    public partial class QueryControl : UserControl
    {
        private QueryType? queryType = null;
        private List<Field> fieldList = null;

        private SortedList<int, ViewField> viewfieldList = null;
        private SortedList<int, OrderByField> orderByFieldList = null;
        private SortedList<int, WhereField> whereFieldList = null;

        public QueryControl()
        {
            this.InitializeComponent();
        }

        public QueryControl(QueryType queryType)
        {
            this.InitializeComponent();

            this.queryType = queryType;
            this.InitializeFieldsComboBox(0.0);
            this.InitializeTargetComboBox(queryType, 0.0, 0.0);
            // InitializeDragDropFunctionality();
        }

        public event CAMLEventHandler CAMLEvent;

        // private void InitializeDragDropFunctionality()
        // {
        // SourceFieldsPanel.PreviewMouseLeftButtonDown += DragDrop.DragDropManager.DragSource_PreviewMouseLeftButtonDown;
        // SourceFieldsPanel.PreviewMouseMove += DragDrop.DragDropManager.DragSource_PreviewMouseMove;
        // }

        private CAMLDesigner.BusinessObjects.Wrapper CamlWrapper
        {
            get
            {
                if (App.CamlWrapper == null)
                {
                    App.CamlWrapper = new CAMLDesigner.BusinessObjects.Wrapper(
                        App.GeneralInformation.SharePointUrl,
                        App.GeneralInformation.ConnectionType);
                }
                return App.CamlWrapper;
            }
        }

        #region Public properties
        public SortedList<int, WhereField> WhereFieldsList
        {
            get
            {
                return this.whereFieldList;
            }
        }
        #endregion

        #region Public methods

        public void Clear(bool clearSourceFieldsList)
        {
            if (this.viewfieldList != null)
                this.viewfieldList.Clear();
            if (this.orderByFieldList != null)
                this.orderByFieldList.Clear();
            if (this.whereFieldList != null)
                this.whereFieldList.Clear();

            this.ViewFieldsPanel.Clear();
            this.OrderByPanel.Clear();
            this.WherePanel.Clear();
            this.QueryOptionsPanel.Clear();

            if (clearSourceFieldsList)
                SourceFieldsPanel.Clear();

            this.InitializeTargetComboBox(this.queryType, 0.0, 0.0);
        }

        public void InitializeFieldsComboBox(double parentHeight)
        {
            // This must come from the treeview
            if (App.SelectedListViewModel != null)
            {
                this.fieldList = this.CamlWrapper.GetFields(App.SelectedListViewModel.Title, true);
                List<UserControl> childControls = new List<UserControl>();
                foreach (Field field in this.fieldList)
                {
                    FieldControl fieldControl = new FieldControl(field);
                    fieldControl.FieldSelectedEvent += new FieldEventHandler(this.SourceFieldControl_FieldSelectedEvent);
                    fieldControl.Margin = new Thickness(3.0);
                    childControls.Add(fieldControl);
                }
                // set the height of the panel based on the height of the query control, minus the margin
                if (parentHeight > 0.0)
                    SourceFieldsPanel.PanelHeight = parentHeight - 30;
                SourceFieldsPanel.Visibility = System.Windows.Visibility.Visible;
                SourceFieldsPanel.PopulatePanel(childControls);
            }
        }

        public void InitializeTargetComboBox(QueryType? queryType, double parentWidth, double parentHeight)
        {
            if (queryType != null)
            {
                this.queryType = queryType;
                switch (queryType)
                {
                    case QueryType.ViewFields:
                        this.InitializeViewFieldsPanel(parentHeight);
                        break;
                    case QueryType.OrderBy:
                        this.InitializeOrderByPanel(parentHeight);
                        break;
                    case QueryType.Where:
                        this.InitializeWherePanel(parentWidth, parentHeight);
                        break;
                }
            }
            else
            {
                ViewFieldsPanel.Visibility = Visibility.Collapsed;
                OrderByPanel.Visibility = Visibility.Collapsed;
                WherePanel.Visibility = Visibility.Collapsed;
            }
        }

        public void InitializeViewFieldsPanel(double parentHeight)
        {
            ViewFieldsPanel.Visibility = Visibility.Visible;
            OrderByPanel.Visibility = Visibility.Collapsed;
            WherePanel.Visibility = Visibility.Collapsed;
            // QueryOptionsPanel.Visibility = Visibility.Collapsed;

            // set the height of the panel based on the height of the query control, minus the margin
            if (parentHeight > 0.0)
                ViewFieldsPanel.PanelHeight = parentHeight - 30;

            // fill it with the already build query
            if (this.viewfieldList != null)
            {
                ViewFieldsPanel.Clear();
                foreach (KeyValuePair<int, ViewField> pair in this.viewfieldList)
                {
                    ViewFieldsPanel.AddChild(this.Add(pair.Value));
                }
            }
        }

        public void InitializeOrderByPanel(double parentHeight)
        {
            SourceFieldsPanel.Visibility = Visibility.Visible;
            ViewFieldsPanel.Visibility = Visibility.Collapsed;
            OrderByPanel.Visibility = Visibility.Visible;
            WherePanel.Visibility = Visibility.Collapsed;
            // QueryOptionsPanel.Visibility = Visibility.Collapsed;

            // set the height of the panel based on the height of the query control, minus the margin
            if (parentHeight > 0.0)
                OrderByPanel.PanelHeight = parentHeight - 30;

            // fill it with the already build query
            if (this.orderByFieldList != null)
            {
                OrderByPanel.Clear();
                foreach (KeyValuePair<int, OrderByField> pair in this.orderByFieldList)
                {
                    OrderByPanel.AddChild(this.Add(pair.Value));
                }
            }
        }

        public void InitializeWherePanel(double parentWidth, double parentHeight)
        {
            SourceFieldsPanel.Visibility = Visibility.Visible;
            ViewFieldsPanel.Visibility = Visibility.Collapsed;
            OrderByPanel.Visibility = Visibility.Collapsed;
            WherePanel.Visibility = Visibility.Visible;
            // QueryOptionsPanel.Visibility = Visibility.Collapsed;

            // set the height of the panel based on the size of the query control, minus the margin
            if (parentHeight > 0.0)
                WherePanel.PanelHeight = parentHeight - 30;

            // fill it with the already build query
            if (this.whereFieldList != null)
            {
                WherePanel.Clear();
                foreach (KeyValuePair<int, WhereField> pair in this.whereFieldList)
                {
                    if (pair.Value.PositionInList > 0 && !string.IsNullOrEmpty(pair.Value.AndOrOperator))
                    {
                        // add an And operator
                        AndOrControl andControl = new AndOrControl(pair.Value);
                        andControl.WhereFieldSelectedEvent += new WhereFieldEventHandler(this.WhereFieldControl_WhereFieldSelectedEvent);
                        WherePanel.AddChild(andControl);
                    }
                    WherePanel.AddChild(this.Add(pair.Value));
                }
            }
        }

        public void InitializeQueryOptions(bool isVisible)
        {
            switch (App.QueryType)
            {
                case BPoint.SharePoint.Common.Enumerations.QueryType.CAMLQuery:
                    if (isVisible)
                        QueryOptionsPanel.Visibility = Visibility.Visible;
                    else
                        QueryOptionsPanel.Visibility = Visibility.Collapsed;
                    break;
                case BPoint.SharePoint.Common.Enumerations.QueryType.SiteDataQuery:
                    QueryOptionsPanel.Visibility = Visibility.Visible;
                    break;
            }
            QueryOptionsPanel.Reset();
        }

        #endregion

        private ViewFieldControl Add(ViewField viewField)
        {
            ViewFieldControl viewFieldControl = new ViewFieldControl(viewField);
            viewFieldControl.Margin = new Thickness(3.0);
            viewFieldControl.ViewFieldSelectedEvent += new ViewFieldEventHandler(this.ViewFieldControl_ViewFieldSelectedEvent);
            viewFieldControl.ViewFieldDeletedEvent += new ViewFieldEventHandler(this.ViewFieldControl_ViewFieldDeletedEvent);
            return viewFieldControl;
        }

        private OrderByFieldControl Add(OrderByField orderByField)
        {
            OrderByFieldControl orderByFieldControl = new OrderByFieldControl(orderByField);
            orderByFieldControl.Margin = new Thickness(3.0);
            orderByFieldControl.OrderByFieldSelectedEvent += new OrderByFieldEventHandler(this.OrderByFieldControl_OrderByFieldSelectedEvent);
            orderByFieldControl.OrderByFieldDeletedEvent += new OrderByFieldEventHandler(this.OrderByFieldControl_OrderByFieldDeletedEvent);
            return orderByFieldControl;
        }

        private WhereFieldControl Add(WhereField whereField)
        {
            WhereFieldControl whereControl = new WhereFieldControl(whereField, this);
            whereControl.Name = "ctlWhere" + whereField.PositionInList.ToString(); 
            whereControl.Margin = new Thickness(3.0);
            whereControl.WhereFieldSelectedEvent += new WhereFieldEventHandler(this.WhereFieldControl_WhereFieldSelectedEvent);
            whereControl.WhereFieldDeletedEvent += new WhereFieldEventHandler(this.WhereFieldControl_WhereFieldDeletedEvent);
            return whereControl;
        }

        public void SourceFieldControl_FieldSelectedEvent(Field field)
        {
            if (field != null)
            {
                switch (this.queryType)
                {
                    case QueryType.ViewFields:
                        // add the field to the ViewFieldsPanel
                        ViewField viewField = new ViewField(field);
                        ViewFieldsPanel.AddChild(this.Add(viewField));

                        if (this.viewfieldList == null)
                            this.viewfieldList = new SortedList<int, ViewField>();
                        this.viewfieldList.Add(this.viewfieldList.Count, viewField);

                        this.GenerateCamlString();
                        break;

                    case QueryType.OrderBy:
                        OrderByField orderByField = new OrderByField(field);
                        OrderByPanel.AddChild(this.Add(orderByField));

                        if (this.orderByFieldList == null)
                            this.orderByFieldList = new SortedList<int, OrderByField>();
                        this.orderByFieldList.Add(this.orderByFieldList.Count, orderByField);

                        this.GenerateCamlString();
                        break;

                    case QueryType.Where:
                        if (this.whereFieldList == null)
                            this.whereFieldList = new SortedList<int, WhereField>();

                        // instantiate the where field and define its position here, so that it can be passed to the AndOrControl
                        WhereField whereField = new WhereField(field);
                        this.whereFieldList.Add(this.whereFieldList.Count, whereField);
                        whereField.PositionInList = this.whereFieldList.Count - 1;

                        // check if a where field already exists in the clause
                        if (this.whereFieldList != null && this.whereFieldList.Count > 1)
                        {
                            // add an And operator
                            AndOrControl andControl = new AndOrControl(whereField);
                            andControl.WhereFieldSelectedEvent += new WhereFieldEventHandler(this.WhereFieldControl_WhereFieldSelectedEvent);
                            WherePanel.AddChild(andControl);
                        }

                        // add the where field
                        WherePanel.AddChild(this.Add(whereField));

                        this.GenerateCamlString();
                        break;
                }
            }
        }

        public void ViewFieldControl_ViewFieldSelectedEvent(ViewField field)
        {
            if (field != null)
            {
                if (this.viewfieldList == null)
                {
                    this.viewfieldList = new SortedList<int, ViewField>();
                    this.viewfieldList.Add(this.viewfieldList.Count, field);
                }
                else
                {
                    // change the sort order of the viewfield in the list
                    for (int i = 0; i < this.viewfieldList.Count; i++)
                    {
                        ViewField vfield = this.viewfieldList[i];
                        if (vfield.Field.ID == field.Field.ID)
                        {
                            vfield.IsNullable = field.IsNullable;
                            break;
                        }
                    }
                }

                this.GenerateCamlString();
            }
        }

        public void ViewFieldControl_ViewFieldDeletedEvent(ViewField viewfield)
        {
            if (viewfield != null && this.viewfieldList != null)
            {
                for (int i = 0; i < this.viewfieldList.Count; i++)
                {
                    ViewField field = this.viewfieldList[i];
                    if (field.Field.ID == viewfield.Field.ID)
                    {
                        this.viewfieldList.RemoveAt(i);
                        break;
                    }
                }

                if (this.viewfieldList.Count > 0)
                {
                    SortedList<int, ViewField> tempList = new SortedList<int, ViewField>();

                    for (int i = 0; i < this.viewfieldList.Count; i++)
                    {
                        if (this.viewfieldList.ContainsKey(i))
                            tempList.Add(i, this.viewfieldList[i]);
                        else if (i + 1 <= this.viewfieldList.Count)
                        {
                            // this means that there is an empty space
                            tempList.Add(i, this.viewfieldList[i + 1]);
                        }
                    }
                    this.viewfieldList = tempList;
                }

                this.GenerateCamlString();
                this.InitializeViewFieldsPanel(this.ActualHeight);
            }
        }

        public void OrderByFieldControl_OrderByFieldSelectedEvent(OrderByField field)
        {
            if (field != null)
            {
                if (this.orderByFieldList == null)
                {
                    this.orderByFieldList = new SortedList<int, OrderByField>();
                    this.orderByFieldList.Add(this.orderByFieldList.Count, field);
                }
                else
                {
                    // change the sort order of the viewfield in the list
                    for (int i = 0; i < this.orderByFieldList.Count; i++)
                    {
                        OrderByField obfield = this.orderByFieldList[i];
                        if (obfield.Field.ID == field.Field.ID)
                        {
                            obfield.SortOrder = field.SortOrder;
                            break;
                        }
                    }
                }

                this.GenerateCamlString();
            }
        }

        public void OrderByFieldControl_OrderByFieldDeletedEvent(OrderByField orderByField)
        {
            if (orderByField != null && this.orderByFieldList != null)
            {
                for (int i = 0; i < this.orderByFieldList.Count; i++)
                {
                    OrderByField field = this.orderByFieldList[i];
                    if (field.Field.ID == orderByField.Field.ID)
                    {
                        this.orderByFieldList.RemoveAt(i);
                        break;
                    }
                }

                if (this.orderByFieldList.Count > 0)
                {
                    SortedList<int, OrderByField> tempList = new SortedList<int, OrderByField>();

                    for (int i = 0; i < this.orderByFieldList.Count; i++)
                    {
                        if (this.orderByFieldList.ContainsKey(i))
                            tempList.Add(i, this.orderByFieldList[i]);
                        else if (i + 1 <= this.orderByFieldList.Count)
                        {
                            // this means that there is an empty space
                            tempList.Add(i, this.orderByFieldList[i + 1]);
                        }
                    }
                    this.orderByFieldList = tempList;
                }

                this.GenerateCamlString();
                this.InitializeOrderByPanel(this.ActualHeight);
            }
        }

        public void WhereFieldControl_WhereFieldSelectedEvent(WhereField field)
        {
            if (field != null)
            {
                if (this.whereFieldList == null)
                {
                    this.whereFieldList = new SortedList<int, WhereField>();
                    this.whereFieldList.Add(this.whereFieldList.Count, field);
                }
                else
                {
                    // if the field is found, change its operator and value
                    for (int i = 0; i < this.whereFieldList.Count; i++)
                    {
                        WhereField wfield = this.whereFieldList[i];
                        if (field.PositionInList == i && wfield.Field.ID == field.Field.ID)
                        {
                            wfield.WhereOperator = field.WhereOperator;
                            wfield.Values = field.Values;
                            break;
                        }
                    }
                }

                this.GenerateCamlString();
            }
        }

        public void WhereFieldControl_WhereFieldDeletedEvent(WhereField whereField)
        {
            if (whereField != null && this.whereFieldList != null)
            {
                // remove the where field
                for (int i = 0; i < this.whereFieldList.Count; i++)
                {
                    WhereField field = this.whereFieldList[i];
                    if (field.PositionInList == i && field.Field.ID == whereField.Field.ID)
                    {
                        this.whereFieldList.RemoveAt(i);
                        break;
                    }
                }

                if (this.whereFieldList.Count > 0)
                {
                    SortedList<int, WhereField> tempList = new SortedList<int, WhereField>();

                    for (int i = 0; i < this.whereFieldList.Count; i++)
                    {
                        if (this.whereFieldList.ContainsKey(i))
                            tempList.Add(i, this.whereFieldList[i]);
                        else if (i + 1 <= this.whereFieldList.Count)
                        {
                            // this means that there is an empty space
                            tempList.Add(i, this.whereFieldList[i + 1]);
                        }
                    }
                    this.whereFieldList = tempList;
                }

                this.GenerateCamlString();
                this.InitializeWherePanel(this.ActualWidth, this.ActualHeight);
            }
        }

        private void GenerateCamlString()
        {
            switch (this.queryType)
            {
                case QueryType.ViewFields:
                    BPoint.SharePoint.Common.Builder.GenerateViewFields(this.viewfieldList, ref App.CamlDocument);
                    break;
                case QueryType.OrderBy:
                    BPoint.SharePoint.Common.Builder.GenerateOrderByFields(this.orderByFieldList, ref App.CamlDocument);
                    break;
                case QueryType.Where:
                    BPoint.SharePoint.Common.Builder.GenerateWhereFields(this.whereFieldList, App.QueryOptions, ref App.CamlDocument);
                    break;
            }

            if (this.CAMLEvent != null && App.CamlDocument != null)
            {
                CamlEventArgs args = new CamlEventArgs(App.SelectedListViewModel.Title, App.CamlDocument, this.whereFieldList, App.QueryOptions, App.ConnectionType, App.LanguageType, App.QueryType);
                this.CAMLEvent(args);
            }
        }

        internal List<LookupValue> GetLookupValues(string listName, string showField, Guid lookupWebId)
        {
            return this.CamlWrapper.GetLookupValues(listName, showField, lookupWebId);
        }

        internal List<TaxonomyValue> GetTaxonomyValues(Guid termStoreId, Guid termSetId)
        {
            return this.CamlWrapper.GetTaxonomyValues(termStoreId, termSetId);
        }

        private void QueryOptionsPanel_QueryOptionsEvent(QueryOptions queryOptions)
        {
            if (queryOptions != null)
            {
                App.QueryOptions = queryOptions;
                BPoint.SharePoint.Common.Builder.GenerateQueryOptions(queryOptions, ref App.CamlDocument);

                if (queryOptions.QueryFilesAndFoldersAllFoldersDeep || queryOptions.QueryFilesAllFoldersDeep || queryOptions.QueryFoldersAllFoldersDeep 
                    || queryOptions.QueryFilesAndFoldersInRootFolder || queryOptions.QueryFilesInRootFolder || queryOptions.QueryFoldersInRootFolder
                    || queryOptions.QueryFilesAndFoldersInSubFolder || queryOptions.QueryFilesInSubFolderDeep || queryOptions.QueryFilesAndFoldersInSubFolderDeep)
                    BPoint.SharePoint.Common.Builder.GenerateWhereFields(this.whereFieldList, App.QueryOptions, ref App.CamlDocument);

                if (this.CAMLEvent != null && App.CamlDocument != null)
                {
                    CamlEventArgs args = new CamlEventArgs(App.SelectedListViewModel.Title, App.CamlDocument, this.whereFieldList, App.QueryOptions, App.ConnectionType, App.LanguageType, App.QueryType);
                    this.CAMLEvent(args);
                }
            }
        }
    }
}
