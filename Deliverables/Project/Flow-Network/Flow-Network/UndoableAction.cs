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
            IsUndone = true;
            IsDone = false;
        }

        public void Redo()
        {
            if (IsDone) return;
            OnRedo();
            IsDone = true;
            IsUndone = false;
        }

        protected abstract void OnUndo();

        protected abstract void OnRedo();

        protected abstract string AsString { get; }

        public override string ToString()
        {
            return
                (this.IsDone ? "Perform " : "Undo ")
                +
                this.AsString;
        }
    }
}
