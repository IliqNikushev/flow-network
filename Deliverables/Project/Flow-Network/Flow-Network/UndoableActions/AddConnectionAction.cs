using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network.UndoableActions
{
    class AddConnectionAction : UndoableAction
    {
        /// <summary>The connection that was added</summary>
        public ConnectionZone.Path Connection { get; private set; }

        public AddConnectionAction(ConnectionZone.Path connection)
        {
            this.Connection = connection;
        }

        /// <summary>Invokes ConnectionZone.Path.Remove of this.Connection</summary>
        protected override void OnUndo()
        {
            this.Connection.Remove();
        }

        /// <summary>Invokes ConnectionZone.Path.Add of this.Connection</summary>
        protected override void OnRedo()
        {
            this.Connection.Add();
        }

        protected override string AsString
        {
            get { return "Add Connection"; }
        }
    }
}
