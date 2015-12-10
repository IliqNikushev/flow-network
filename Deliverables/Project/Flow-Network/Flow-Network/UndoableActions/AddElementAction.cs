using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network.UndoableActions
{
    class AddElementAction : UndoableAction
    {
        /// <summary>The element that was added</summary>
        public Element Element{get; private set;}

        public AddElementAction(Element element)
        {
            this.Element = element;
        }

        /// <summary>Removes this.Element from the Element.AllElements collection</summary>
        protected override void OnUndo()
        {
            Element.AllElements.Remove(this.Element);
        }

        /// <summary>Adds this.Element from the Element.AllElements collection</summary>
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
