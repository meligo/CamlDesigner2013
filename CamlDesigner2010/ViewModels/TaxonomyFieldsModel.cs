// -----------------------------------------------------------------------
// <copyright file="TaxonomyFieldsModel.cs" company="Biwug">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CamlDesigner.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TaxonomyFieldsModel : INotifyPropertyChanged
    {
        public bool HasDummyChild
        {
            get;
            set;
        }
        public bool IsExpanded { get; set; }


        public bool IsSelected { get; set; }
        public TreeViewItemViewModel Parent { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
