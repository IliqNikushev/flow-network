using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flow_Network
{
    public abstract class UndoableAction
    {
        public bool IsUndone { get; private set; }
        public bool IsDone { get; private set; }

        public UndoableAction()
        {
            IsDone = true;
        }

        public void Undo()
        {
            if (IsUndone) return;
            OnUndo();
        }

        public void Redo()
        {
            if (IsDone) return;
            Redo();
        }

        protected abstract void OnUndo();

        protected abstract void OnRedo();
    }
}
