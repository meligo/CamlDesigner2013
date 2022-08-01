// ----------------------------------------------------------------------
// <copyright file="MetalStackControl.xaml.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MetalStackControl.xaml
    /// </summary>
    public partial class MetalStackControl : UserControl
    {
        // public static readonly DependencyProperty ContentPresenterHeightProperty = DependencyProperty.Register("ContentPresenterHeight",
        // typeof(double), typeof(ScrollViewer), null);

        public MetalStackControl()
        {
            this.InitializeComponent();
        }

        // public double ContentPresenterHeight
        // {
        // get
        // {
        // return (double)base.GetValue(ContentPresenterHeightProperty);
        // }
        // set
        // {
        // base.SetValue(ContentPresenterHeightProperty, value);
        // }
        // }

     
        public double PanelWidth
        {
            get { return ScrollViewerCanvas.Width; }
            set
            {
                if (value > 50.0)
                {
                    ScrollViewerCanvas.Width = value;
                    MetalScrollViewer.Width = value;
                }
            }
        }

        public double PanelHeight
        {
            get { return ScrollViewerCanvas.Height; }
            set
            {
                if (value > 300.0)
                {
                    ScrollViewerCanvas.Height = value;
                    MetalScrollViewer.Height = value;
                    // ContentPresenterHeight = value - 50;
                }
                else
                {
                    ScrollViewerCanvas.Height = 300.0;
                    MetalScrollViewer.Height = 300.0;
                }
            }
        }

        public void PopulatePanel(List<UserControl> childControls)
        {
            ChildControlStackPanel.Children.Clear();
            if (childControls != null)
            {
                foreach (UserControl ctl in childControls)
                    ChildControlStackPanel.Children.Add(ctl);
            }
        }


        public void Clear()
        {
            ChildControlStackPanel.Children.Clear();
        }

        public void AddChild(UserControl childControl)
        {
            if (childControl != null)
            {
                ChildControlStackPanel.Children.Add(childControl);
            }
        }

        private void UpButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void DownButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

       }
    }
}
