using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flow_Network
{
    public static class UndoStack
    {
        public static event Action<int, UndoableAction> OnUndoAltered = (x, y) => { };
        public static event Action<int, UndoableAction> OnRedoAltered = (x, y) => { };

        public static bool CanUndo
        {
            get
            {
                return activitiesStack.Count != 0;
            }
        }

        public static bool CanRedo
        {
            get
            {
                return redoStack.Count != 0;
            }
        }

        private static Stack<UndoableAction> activitiesStack = new Stack<UndoableAction>();
        private static Stack<UndoableAction> redoStack = new Stack<UndoableAction>();

        public static void AddAction(UndoableAction action, bool isNew = true)
        {
            if (redoStack.Count > 0) redoStack.Clear();

            activitiesStack.Push(action);

            OnUndoAltered(activitiesStack.Count, action);
            OnRedoAltered(redoStack.Count, null);
        }

        public static void Undo()
        {
            if (!CanUndo) return;
            UndoableAction action = activitiesStack.Pop();
            action.Undo();
            redoStack.Push(action);

            OnRedoAltered(redoStack.Count, action);
            if (activitiesStack.Count == 0)
                OnUndoAltered(activitiesStack.Count, null);
            else
                OnUndoAltered(activitiesStack.Count, activitiesStack.Peek());
        }

        public static void Redo()
        {
            if (!CanRedo) return;

            UndoableAction action = redoStack.Pop();
            action.Redo();
            activitiesStack.Push(action);

            OnUndoAltered(activitiesStack.Count, action);
            if (redoStack.Count == 0)
                OnRedoAltered(redoStack.Count, null);
            else
                OnRedoAltered(redoStack.Count, redoStack.Peek());
        }
    }
}