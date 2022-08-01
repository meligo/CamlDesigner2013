// ----------------------------------------------------------------------
// <copyright file="DataTypeTextControl.xaml.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for DataTypeTextControl.xaml
    /// </summary>
    public partial class DataTypeTextControl : UserControl
    {
        public DataTypeTextControl()
        {
            this.InitializeComponent();
        }

        public bool EnforceUniqueValues
        {
            get 
            {
                return (bool)EnforceUniqueValuesRadioButton.IsChecked; 
            }
            set
            {
                if (value)
                {
                    EnforceUniqueValuesRadioButton.IsChecked = true;
                }
                else
                {
                    NotEnforceUniqueValuesRadioButton.IsChecked = false;
                }
            }
        }

        public int MaximumNumberOfCharacters
        {
            get 
            {
                return System.Convert.ToInt32(NumberOfCharactersTextBox.Text); 
            }
            set
            {
                NumberOfCharactersTextBox.Text = value.ToString();
            }
        }
    }
}
