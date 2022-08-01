// ----------------------------------------------------------------------
// <copyright file="SharePointViewModel.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class SharePointViewModel : TreeViewItemViewModel
    {
        private readonly ReadOnlyCollection<ListViewModel> listViewModels;
        private string viewModelType = null;

        public SharePointViewModel(string viewModelType)
            : base(null, true)
        {
            if (this.CamlWrapper != null)
            {
                this.viewModelType = viewModelType;
                switch (viewModelType)
                {
                    case "list":
                        List<BPoint.SharePoint.Objects.List> listCollection = this.CamlWrapper.GetLists();
                        List<ListViewModel> children = new List<ListViewModel>();
                        foreach (BPoint.SharePoint.Objects.List list in listCollection)
                        {
                            ListViewModel listViewModel = new ListViewModel(list);
                            listViewModel.TreeViewItemViewModelSelected += new TreeViewItemViewModelDelegate(this.ListViewModel_TreeViewItemViewModelSelected);
                            this.Children.Add(listViewModel);
                            children.Add(listViewModel);
                        }
                        this.listViewModels = new ReadOnlyCollection<ListViewModel>(children);
                        break;

                }
            }
        }

        public string SiteUrl
        {
            get { return App.GeneralInformation.SharePointUrl; }
        }

        public ReadOnlyCollection<ListViewModel> ListViewModels
        {
            get { return this.listViewModels; }
        }

        private CAMLDesigner.BusinessObjects.Wrapper CamlWrapper
        {
            get
            {
                if ((App.CamlWrapper == null && App.GeneralInformation != null)
                    || (App.CamlWrapper != null && App.GeneralInformation != null) 
                    && (App.CamlWrapper.Url != App.GeneralInformation.SharePointUrl)
                    && (App.CamlWrapper.SharePointModel != App.GeneralInformation.ConnectionType))
                {
                    if (App.GeneralInformation.ConnectionType == BPoint.SharePoint.Common.Enumerations.ConnectionType.WebServices)
                    {
                        App.CamlWrapper = 
                            new CAMLDesigner.BusinessObjects.Wrapper(
                                App.GeneralInformation.SharePointUrl,
                            App.GeneralInformation.ConnectionType, 
                            App.GeneralInformation.Username, 
                            App.GeneralInformation.Password, 
                            App.GeneralInformation.Domain);
                    }
                    else
                    {
                        App.CamlWrapper = 
                            new CAMLDesigner.BusinessObjects.Wrapper(
                            App.GeneralInformation.SharePointUrl,
                            App.GeneralInformation.ConnectionType);
                    }
                }
                return App.CamlWrapper;
            }
        }

        protected override void LoadChildren()
        {
            switch (this.viewModelType)
            {
               case "list":
                    List<BPoint.SharePoint.Objects.List> listCollection = this.CamlWrapper.GetLists();
                    foreach (BPoint.SharePoint.Objects.List list in listCollection)
                        this.Children.Add(new ListViewModel(list));

                    break;
            }
        }

        public void ListViewModel_TreeViewItemViewModelSelected(TreeViewItemViewModel model, EventArgs e)
        {
            // TODO: all selected viewmodels need to be set to null
            if (model != null)
            {
                App.SelectedListViewModel = model as ListViewModel;

                // the Source fields list must be cleared and populated with the fields of the newly selected list.
            }
        }
    }
}
