using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network.UndoableActions
{
    class RemoveElement : AddElement
    {
        public RemoveElement(Element element) : base(element)
        {
        }

        protected override void OnUndo()
        {
            base.OnRedo();
        }

        protected override void OnRedo()
        {
            base.OnUndo();
        }

        protected override string AsString
        {
            get
            {
                return "Remove Element";
            }
        }
    }
}
