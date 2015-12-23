using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Network.UndoableActions
{
    class AddElementAction : UndoableAction
    {
        /// <summary>The element that was added</summary>
        public Element Element{get; private set;}
        private List<ConnectionZone.Path> paths = new List<ConnectionZone.Path>();

        public AddElementAction(Element element)
        {
            this.Element = element;
        }

        /// <summary>Removes this.Element from the Element.AllElements collection</summary>
        protected override void OnUndo()
        {
            this.paths = new List<ConnectionZone.Path>(this.Element.Connections);
            Element.AllElements.Remove(this.Element);
            foreach (ConnectionZone.Path path in paths)
            {
                path.From.ConnectedZone = null;
                path.To.ConnectedZone = null;
                ConnectionZone.Path.All.Remove(path);
            }
        }

        /// <summary>Adds this.Element from the Element.AllElements collection</summary>
        protected override void OnRedo()
        {
            Element.AllElements.Add(this.Element);
            foreach (ConnectionZone.Path path in this.paths)
            {
                path.From.ConnectedZone = path.To;
                path.To.ConnectedZone = path.From;
                ConnectionZone.Path.All.Add(path);
            }
        }

        static string capitalLetterSplitters = "abcdefghijklmnopqrstuvwxyz".ToUpper();

        protected override string AsString
        {
            get { return "Add" + string.Join("", Element.GetType().Name.Select(x=>capitalLetterSplitters.Contains(x) ? " " + x : x.ToString())); }
        }
    }
}
