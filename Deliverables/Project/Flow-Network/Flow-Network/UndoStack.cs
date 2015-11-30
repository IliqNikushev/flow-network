using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flow_Network
{
    public static class UndoStack
    {
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
            if(redoStack.Count > 0) redoStack.Clear();

            activitiesStack.Push(action);
        }

        public static void Undo()
        {
            if (!CanUndo) return;
            UndoableAction action = activitiesStack.Pop();
            action.Undo();
            redoStack.Push(action);
        }

        public static void Redo()
        {
            if (!CanRedo) return;

            UndoableAction action = redoStack.Pop();
            action.Redo();
            activitiesStack.Push(action);
        }
    }
}
