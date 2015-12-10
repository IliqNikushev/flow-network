using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network.UndoableActions
{
    /// <summary>Uses the inverse of AddElementAction's redo, undo methods</summary>
    class RemoveElementAction : AddElementAction
    {
        public RemoveElementAction(Element element) : base(element)
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
