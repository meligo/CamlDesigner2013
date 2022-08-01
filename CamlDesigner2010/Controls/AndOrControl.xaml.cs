// ----------------------------------------------------------------------
// <copyright file="AndOrControl.xaml.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner.Controls
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using BPoint.SharePoint.Objects;

    /// <summary>
    /// Interaction logic for AndOrControl.xaml
    /// </summary>
    public partial class AndOrControl : UserControl
    {
        private WhereField whereField;

        public AndOrControl(WhereField whereField)
        {
            this.InitializeComponent();
            this.whereField = whereField;

            if (!string.IsNullOrEmpty(whereField.AndOrOperator) && whereField.AndOrOperator == "Or")
                SetOrOperator();
        }

        public event WhereFieldEventHandler WhereFieldSelectedEvent;

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (AndRectangle.Visibility == System.Windows.Visibility.Collapsed)
                SetAndOperator();
            else
                SetOrOperator();

            // modify the value in the where field
            this.whereField.AndOrOperator = AndOrTextBlock.Text;

            // fire the operator event handler
            if (this.WhereFieldSelectedEvent != null)
                this.WhereFieldSelectedEvent(this.whereField);
        }

        private void SetOrOperator()
        {
            AndRectangle.Visibility = System.Windows.Visibility.Collapsed;
            OrRectangle.Visibility = System.Windows.Visibility.Visible;
            AndOrTextBlock.Text = "Or";
        }

        private void SetAndOperator()
        {
            AndRectangle.Visibility = System.Windows.Visibility.Visible;
            OrRectangle.Visibility = System.Windows.Visibility.Collapsed;
            AndOrTextBlock.Text = "And";
        }
    }
}
