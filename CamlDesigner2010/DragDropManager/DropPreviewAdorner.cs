// ----------------------------------------------------------------------
// <copyright file="Win32" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner.DragDrop
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Shapes;

    internal class Win32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        public static int GWL_EXSTYLE = -20;
        public static int WS_EX_LAYERED = 0x00080000;
        public static int WS_EX_TRANSPARENT = 0x00000020;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowLong(IntPtr heightWnd, int numericIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetWindowLong(IntPtr heightWnd, int numericIndex, int newVal);

    }

    public class DropPreviewAdorner : Adorner
    {
        private Window dragdropWindow = null;
        public Window MyProperty
        {

            get
            {
                return this.dragdropWindow;
            }
            set

            {

                this.dragdropWindow = value;
            }
        }

        private ContentPresenter presenter;
        private double left;
        private double top;
        private AdornerLayer adornerLayer;
        private UIElement adornedElement;

        public double Left
        {
            get { return this.left; }
            set
            {
                this.left = value;
                this.UpdatePosition();
            }
        }

        public double Top
        {
            get { return this.top; }
            set
            {
                this.top = value;
                this.UpdatePosition();
            }
        }

        public DropPreviewAdorner(UIElement feedbackUI, UIElement adornedElt, AdornerLayer layer)
            : base(adornedElt)
        {
            this.adornedElement = adornedElt;
            this.adornerLayer = layer;

            this.presenter = new ContentPresenter();
            this.presenter.Content = feedbackUI;
            this.presenter.IsHitTestVisible = false;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(new TranslateTransform(this.Left, this.Top));
            result.Children.Add(base.GetDesiredTransform(transform));

            return result;
        }

        public void CreateDragDropWindow(Visual dragElement)
        {
            this.dragdropWindow = new Window();
            this.dragdropWindow.WindowStyle = WindowStyle.None;
            this.dragdropWindow.AllowsTransparency = true;
            this.dragdropWindow.AllowDrop = false;
            this.dragdropWindow.Background = null;
            this.dragdropWindow.IsHitTestVisible = false;
            this.dragdropWindow.SizeToContent = SizeToContent.WidthAndHeight;
            this.dragdropWindow.Topmost = true;
            this.dragdropWindow.ShowInTaskbar = false;

            this.dragdropWindow.SourceInitialized += new EventHandler(delegate(object sender, EventArgs args)
            {
                PresentationSource windowSource = PresentationSource.FromVisual(this.dragdropWindow);
                IntPtr handle = ((HwndSource)windowSource).Handle;
                int styles = Win32.GetWindowLong(handle, Win32.GWL_EXSTYLE);
                Win32.SetWindowLong(handle, Win32.GWL_EXSTYLE, styles | Win32.WS_EX_LAYERED | Win32.WS_EX_TRANSPARENT);
            });

            Rectangle r = new Rectangle();
            r.Width = ((FrameworkElement)dragElement).ActualWidth;
            r.Height = ((FrameworkElement)dragElement).ActualHeight;

            r.Fill = new VisualBrush(dragElement);
            this.dragdropWindow.Content = r;

        }

        public void UpdateWindowLocation()
        {
            if (this.dragdropWindow != null)
            {
                Win32.POINT p;
                if (!Win32.GetCursorPos(out p))
                {
                    return;
                }
                this.dragdropWindow.Left = (double)p.X;
                this.dragdropWindow.Top = (double)p.Y;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.presenter;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.presenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.presenter.Measure(constraint);
            return this.presenter.DesiredSize;
        }

        private void UpdatePosition()
        {
            if (this.adornerLayer != null)
            {
                this.adornerLayer.Update(this.adornedElement);

            }
        }
    }
}
