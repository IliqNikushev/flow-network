using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flow_Network
{
    public class UndoStack
    {
        public bool CanUndo
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
        Stack<Action> stack = new Stack<Action>();

        public bool CanRedo
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
    
        public void AddAction(Action action)
        {
            throw new System.NotImplementedException();
        }

        public void Undo()
        {
            throw new System.NotImplementedException();
        }

        public void Redo()
        {
            throw new System.NotImplementedException();
        }
    }
}
