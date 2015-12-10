using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network.UndoableActions
{
    /// <summary>Uses the inverse of AddConnectionAction's redo, undo methods</summary>
    class RemoveConnectionAction : AddConnectionAction
    {
        public RemoveConnectionAction(ConnectionZone.Path connection) : base(connection)
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
                return "Remove Connection";
            }
        }
    }
}
