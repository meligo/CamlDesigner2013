// ----------------------------------------------------------------------
// <copyright file="OrderByFieldControl.xaml.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using BPoint.SharePoint.Objects;

    /// <summary>
    /// Interaction logic for OrderByFieldControl.xaml
    /// </summary>
    public partial class OrderByFieldControl : UserControl
    {
     

        private OrderByField orderByField;

        public OrderByField OrderByField
        {
            get { return this.orderByField; }
            set { this.orderByField = value; }
        }

        public OrderByFieldControl(OrderByField orderByField)
        {
            this.InitializeComponent();
            this.orderByField = orderByField;
            DisplayNameTextBlock.Text = orderByField.Field.DisplayName;
            if (orderByField.SortOrder == BPoint.SharePoint.Common.Enumerations.SortOrder.Descending)
                this.LoadImage(SortorderImage, "../Images/descending_32x32.png");
        }

        public event OrderByFieldEventHandler OrderByFieldSelectedEvent;
        public event OrderByFieldEventHandler OrderByFieldDeletedEvent;

        private void SortorderImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.orderByField.SortOrder == BPoint.SharePoint.Common.Enumerations.SortOrder.Ascending)
            {
                this.orderByField.SortOrder = BPoint.SharePoint.Common.Enumerations.SortOrder.Descending;
                this.LoadImage(this.SortorderImage, "../Images/descending_32x32.png");
            }
            else
            {
                this.orderByField.SortOrder = BPoint.SharePoint.Common.Enumerations.SortOrder.Ascending;
                this.LoadImage(this.SortorderImage, "../Images/ascending_32x32.png");
            }

            if (this.OrderByFieldSelectedEvent != null)
                this.OrderByFieldSelectedEvent(this.orderByField);
        }

        private void LoadImage(Image image, string uristring)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(uristring, UriKind.Relative);
            img.EndInit();

            SortorderImage.Source = img;
        }

        private void DeleteCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.OrderByFieldDeletedEvent != null)
                this.OrderByFieldDeletedEvent(this.orderByField);
        }
    }
}
