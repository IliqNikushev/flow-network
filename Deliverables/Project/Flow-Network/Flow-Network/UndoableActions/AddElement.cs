using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network.UndoableActions
{
    class AddElement : UndoableAction
    {
        public Element Element{get; private set;}
        protected System.Windows.Forms.Control pboxParent;

        public AddElement(Element element)
        {
            this.Element = element;
        }

        protected override void OnUndo()
        {
            this.pboxParent = Element.PictureBox.Parent;
            Element.PictureBox.Parent = null;
            Element.AllElements.Remove(this.Element);
        }

        protected override void OnRedo()
        {
            Element.PictureBox.Parent = pboxParent;
            Element.AllElements.Add(this.Element);
        }
    }
}
