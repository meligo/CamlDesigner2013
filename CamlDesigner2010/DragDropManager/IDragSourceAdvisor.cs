// ----------------------------------------------------------------------
// <copyright file="IDragSourceAdvisor.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner.DragDrop
{
    using System.Windows;

    public interface IDragSourceAdvisor
    {
        UIElement SourceUI
        {
            get;
            set;
        }

        DragDropEffects SupportedEffects
        {
            get;
        }

        DataObject GetDataObject(UIElement draggedElt);
        UIElement GetVisualFeedback(UIElement draggedElt);
        void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects);

        bool IsDraggable(UIElement dragElt);
    }
}
