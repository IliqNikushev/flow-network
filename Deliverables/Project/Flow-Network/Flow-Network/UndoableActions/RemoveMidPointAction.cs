
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network.UndoableActions
{
    class RemoveMidPointAction : AddMidPointAction
    {
        public RemoveMidPointAction(PathMidPointDrawable point) : base(point)
        {
            this.isApplied = false;
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
            get { return "Remove Midpoint"; }
        }
    }
}
