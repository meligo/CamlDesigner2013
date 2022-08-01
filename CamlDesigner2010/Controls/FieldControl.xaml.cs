// ----------------------------------------------------------------------
// <copyright file="FieldControl.xaml.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Animation;
    using BPoint.SharePoint.Objects;

    /// <summary>
    /// Interaction logic for FieldControl.xaml
    /// </summary>
    public partial class FieldControl : UserControl
    {
        

        private Field field;

        public Field Field
        {
            get { return this.field; }
            set { this.field = value; }
        }

        public FieldControl(Field field)
        {
            this.InitializeComponent();
            this.field = field;
            // TODO
            if (string.IsNullOrEmpty(field.AuthoringInfo))
                DisplayNameTextBlock.Text = field.DisplayName;
            else
                DisplayNameTextBlock.Text = field.DisplayName + " " + field.AuthoringInfo;
        }

        public event FieldEventHandler FieldSelectedEvent;

        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            Storyboard sb = this.FindResource("MouseEnterStoryboard") as Storyboard;
            if (sb != null)
            {
                sb.Begin();
            }
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            Storyboard sb = this.FindResource("MouseLeaveStoryboard") as Storyboard;
            if (sb != null)
            {
                sb.Begin();
            }
        }

        /*
            Here's the simple solution:
            1.On MouseDown with left button down, record the position (on MouseLeave erase the position)
            2.On MouseMove with {left button down, position recorded, current mouse position differs by more than delta} set a flag saying drag operation is in progress & capture the mouse
            3.On MouseMove with drag operation in progress, use hit testing to determine where your panel should be (ignoring the panel itself) and adjust its parenting and position accordingly.
            4.On MouseUp with drag operation in progress, release the mouse capture and clear the "drag operation is in progress" flag
         */

        // just start with a click event
        private void FieldCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // raise an event that the field is clicked
            if (this.FieldSelectedEvent != null)
            {
                this.FieldSelectedEvent(this.field);
            }
        }

        // private void FieldCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        // {
        // }
        // private void FieldCanvas_MouseMove(object sender, MouseEventArgs e)
        // {
        // }
    }
}
