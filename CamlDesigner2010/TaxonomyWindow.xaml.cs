// ----------------------------------------------------------------------
// <copyright file="TaxonomyWindow.xaml.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System.Collections.Generic;
    using System.Windows;
    using BPoint.SharePoint.Objects;

    /// <summary>
    /// Interaction logic for TaxonomyWindow.xaml
    /// </summary>
    public partial class TaxonomyWindow : Window
    {
        private string separator = ";";
        private string whereOperator;
        private bool multiSelect;
        private List<TaxonomyValue> taxonomyValues;

        private string selectedValues = null;

        public string SelectedValues
        {
            get { return this.selectedValues; }
            set { this.selectedValues = value; }
        }

        public TaxonomyWindow(List<TaxonomyValue> taxonomyValues, bool multiSelect, string whereOperator)
        {
            this.InitializeComponent();
            this.taxonomyValues = taxonomyValues;
            this.multiSelect = multiSelect;
            this.whereOperator = whereOperator;
            if (whereOperator == "In")
                this.separator = "+";

            TaxonomyTreeView.ItemsSource = taxonomyValues;
        }

        private void TaxonomyTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // this must return the selected value and close the dialog
            if (TaxonomyTreeView.SelectedItem != null)
            {
                TaxonomyValue taxonomyValue = TaxonomyTreeView.SelectedItem as TaxonomyValue;
                if (taxonomyValue != null && !string.IsNullOrEmpty(taxonomyValue.Value))
                {
                    if (this.multiSelect)
                    {
                        if (SelectedValuesTextBox.Text.Length > 0)
                            SelectedValuesTextBox.Text += this.separator;
                        SelectedValuesTextBox.Text += taxonomyValue.Value;
                    }
                    else
                    {
                        SelectedValuesTextBox.Text = taxonomyValue.Value;
                    }
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedValuesTextBox.Text.Length > 0)
            {
                this.DialogResult = true;
                this.selectedValues = SelectedValuesTextBox.Text;
            }
            this.Close();
        }
    }
}
