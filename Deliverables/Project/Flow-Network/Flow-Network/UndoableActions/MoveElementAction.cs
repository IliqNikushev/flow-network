using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network.UndoableActions
{
    class MoveElementAction : UndoableAction
    {
        private System.Drawing.Point OldCoordinates;
        private System.Drawing.Point NewCoordinates;
        private Element element;

        public MoveElementAction(Element element, System.Drawing.Point oldCoordinates, System.Drawing.Point newCoordinates)
        {
            this.element = element;
            this.OldCoordinates = oldCoordinates;
            this.NewCoordinates = newCoordinates;
        }

        protected override void OnUndo()
        {
            throw new NotImplementedException();
        }

        protected override void OnRedo()
        {
            throw new NotImplementedException();
        }

        protected override string AsString
        {
            get { return string.Format("Move element from {0} to {1}", this.OldCoordinates, this.NewCoordinates); }
        }
    }
}
