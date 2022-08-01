// ----------------------------------------------------------------------
// <copyright file="ViewFieldControl.xaml.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using BPoint.SharePoint.Objects;

    /// <summary>
    /// Interaction logic for ViewFieldControl.xaml
    /// </summary>
    public partial class ViewFieldControl : UserControl
    {
        private ViewField viewfield;

        public ViewField Field
        {
            get { return this.viewfield; }
            set { this.viewfield = value; }
        }

        public ViewFieldControl(ViewField viewfield)
        {
            this.InitializeComponent();

            this.viewfield = viewfield;
            DisplayNameTextBlock.Text = viewfield.Field.DisplayName;
        }

        public event ViewFieldEventHandler ViewFieldSelectedEvent;
        public event ViewFieldEventHandler ViewFieldDeletedEvent;

        // private void IsNullableCheckBox_Checked(object sender, RoutedEventArgs e)
        // {
        // viewfield.IsNullable = (bool)IsNullableCheckBox.IsChecked;
        // if (ViewFieldSelectedEvent != null)
        // ViewFieldSelectedEvent(viewfield);
        // }
        private void DeleteCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.ViewFieldDeletedEvent != null)
                this.ViewFieldDeletedEvent(this.viewfield);
        }

        private void IsNullableCheckBox_Click(object sender, RoutedEventArgs e)
        {
            this.viewfield.IsNullable = (bool)IsNullableCheckBox.IsChecked;
            if (this.ViewFieldSelectedEvent != null)
                this.ViewFieldSelectedEvent(this.viewfield);
        }

    }
}
