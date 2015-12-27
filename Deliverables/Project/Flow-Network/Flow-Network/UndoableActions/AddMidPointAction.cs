using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network.UndoableActions
{
    class AddMidPointAction : UndoableAction
    {
        public PathMidPointDrawable Midpoint { get; private set; }
        private ConnectionZone.Path Parent;
        private int Index;

        public AddMidPointAction(PathMidPointDrawable point)
        {
            this.isApplied = true;
            this.Midpoint = point;
            this.Parent = point.Path;
            for (int i = 0; i < Parent.UserDefinedMidPoints.Count; i++)
            {
                if (Parent.UserDefinedMidPoints[i] == Midpoint)
                {
                    this.Index = i;
                    break;
                }
            }
        }

        protected override void OnUndo()
        {
            this.Parent.UserDefinedMidPoints.RemoveAt(this.Index);
        }

        protected override void OnRedo()
        {
            this.Parent.UserDefinedMidPoints.Insert(this.Index, this.Midpoint);
        }

        protected override string AsString
        {
            get { return "Add Midpoint"; }
        }
    }
}
