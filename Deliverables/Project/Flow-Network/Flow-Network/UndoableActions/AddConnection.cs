using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network.UndoableActions
{
    class AddConnection : UndoableAction
    {
        public ConnectionZone.Path Connection { get; private set; }

        public AddConnection(ConnectionZone.Path connection)
        {
            this.Connection = connection;
        }

        protected override void OnUndo()
        {
            ConnectionZone.Path.All.Remove(this.Connection);
        }

        protected override void OnRedo()
        {
            ConnectionZone.Path.All.Add(this.Connection);
        }
    }
}
