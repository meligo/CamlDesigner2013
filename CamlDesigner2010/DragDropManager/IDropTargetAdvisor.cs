// ----------------------------------------------------------------------
// <copyright file="IDropTargetAdvisor.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner.DragDrop
{
    using System.Windows;

    public interface IDropTargetAdvisor
    {
        UIElement TargetUI
        {
            get;
            set;

        }


        bool IsValidDataObject(IDataObject obj);
        void OnDropCompleted(IDataObject obj, Point dropPoint);
    }
}
