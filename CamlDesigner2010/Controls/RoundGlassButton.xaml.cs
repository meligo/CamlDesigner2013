// ----------------------------------------------------------------------
// <copyright file="RoundGlassButton.xaml.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for RoundGlassButton.xaml
    /// </summary>
    public partial class RoundGlassButton : UserControl
    {
        private bool isArrowUp = true;

        public RoundGlassButton()
        {
            this.InitializeComponent();
        }

        public bool ArrowUp
        {
            get { return this.isArrowUp; }
            set
            {
                this.isArrowUp = value;
                if (this.isArrowUp)
                {
                    ArrowDownImage.Visibility = System.Windows.Visibility.Collapsed;
                    ArrowUpImage.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    ArrowUpImage.Visibility = System.Windows.Visibility.Collapsed;
                    ArrowDownImage.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
    }
}
