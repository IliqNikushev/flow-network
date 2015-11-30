using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network.UndoableActions
{
    class MoveElement : UndoableAction
    {
        public MoveElement(Element element, System.Drawing.Point oldCoordinates, System.Drawing.Point newCoordinates)
        {

        }

        protected override void OnUndo()
        {
            throw new NotImplementedException();
        }

        protected override void OnRedo()
        {
            throw new NotImplementedException();
        }
    }
}
