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
        protected bool isApplied = false;
        public bool IsUndone { get { return !isApplied; } }
        public bool IsDone { get { return isApplied; } }

        public UndoableAction()
        {
        }

        /// <summary>Removes changes defined by the action</summary>
        public void Undo()
        {
            if (IsUndone) return;
            OnUndo();
            this.isApplied = false;
        }

        /// <summary>Applies changes defined by the action</summary>
        public void Redo()
        {
            if (IsDone) return;
            OnRedo();
            this.isApplied = true;
        }

        /// <summary>Calls Redo</summary>
        public void Apply()
        {
            Redo();
        }

        /// <summary>Calls Undo</summary>
        public void Cancel()
        {
            Undo();
        }

        /// <summary>Defines changes to remove</summary>
        protected abstract void OnUndo();
        /// <summary>Defines changes to apply</summary>
        protected abstract void OnRedo();
        /// <summary>Defines how the action will be named in String format</summary>
        protected abstract string AsString { get; }
        /// <summary>
        /// Defines how the action will be in string format
        /// </summary>
        /// <returns>(IsDone?Perform:Undo)+AsString</returns>
        public override string ToString()
        {
            return
                (this.IsDone ? "Perform " : "Undo ")
                +
                this.AsString;
        }
    }
}
