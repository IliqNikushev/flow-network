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

        public AddElement(Element element)
        {
            this.Element = element;
        }

        protected override void OnUndo()
        {
            Element.AllElements.Remove(this.Element);
        }

        protected override void OnRedo()
        {
            Element.AllElements.Add(this.Element);
        }

        protected override string AsString
        {
            get { return "Add Element"; }
        }
    }
}
