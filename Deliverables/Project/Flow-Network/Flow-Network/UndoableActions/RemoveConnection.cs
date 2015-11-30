using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network.UndoableActions
{
    class RemoveConnection : AddConnection
    {
        public RemoveConnection(ConnectionZone.Path connection) : base(connection)
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
    }
}
