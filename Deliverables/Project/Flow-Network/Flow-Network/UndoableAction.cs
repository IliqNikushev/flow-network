using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flow_Network
{
    /// <summary>An action performed in the network that can be undone</summary>
    public abstract class UndoableAction
    {
        /// <summary>True = Is done, False = is not done / undone</summary>
        private bool isApplied = true;
        public bool IsUndone { get { return !isApplied; } }
        public bool IsDone { get { return isApplied; } }

        public UndoableAction()
        {
        }

        /// <summary></summary>
        public void Undo()
        {
            if (IsUndone) return;
            OnUndo();
            this.isApplied = false;
        }

        public void Redo()
        {
            if (IsDone) return;
            OnRedo();
            this.isApplied = true;
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
