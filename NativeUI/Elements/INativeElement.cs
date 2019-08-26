using GTA;
using System.Drawing;

namespace NativeUI.Elements
{
    /// <summary>
    /// Interface that should be implemented with all NativeUI elements.
    /// </summary>
    public interface INativeElement : UIElement
    {
        /// <summary>
        /// The size of the element.
        /// </summary>
        Size Size { get; set; }
        /// <summary>
        /// The rotation or heading of the element.
        /// </summary>
        float Rotation { get; set; }
        /// <summary>
        /// Function that recalculates the Position and Size when required.
        /// </summary>
        void Recalculate();
    }
}
