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
        private IconDrawable element;

        public MoveElementAction(IconDrawable element, System.Drawing.Point oldCoordinates, System.Drawing.Point newCoordinates)
        {
            this.element = element;
            this.OldCoordinates = oldCoordinates;
            this.NewCoordinates = newCoordinates;
        }

        /// <summary>Changes the coordinates of the element to the old ones</summary>
        protected override void OnUndo()
        {
            this.element.Location = this.OldCoordinates;
        }

        /// <summary>Changes the coordinates of the element to the new ones</summary>
        protected override void OnRedo()
        {
            this.element.Location = this.NewCoordinates;
        }

        protected override string AsString
        {
            get { return string.Format("Move {2} from {0} to {1}", this.OldCoordinates, this.NewCoordinates, this.element is Element ? "element" : "midpoint"); }
        }
    }
}
