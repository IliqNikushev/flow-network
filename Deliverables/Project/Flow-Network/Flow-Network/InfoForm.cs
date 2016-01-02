using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flow_Network
{
    public partial class InfoForm : Form
    {
        public InfoForm()
        {
            InitializeComponent();
        }

        public void SetTool(Main.ActiveToolType tool)
        {
            this.infoBox.Text = GetInfo(tool);
        }

        private string GetInfo(Main.ActiveToolType tool)
        {
            switch (tool)
            {
                case Main.ActiveToolType.AdjustableSplitter:
                case Main.ActiveToolType.Merger:
                case Main.ActiveToolType.Pump:
                case Main.ActiveToolType.Sink:
                case Main.ActiveToolType.Splitter:
                    return
@"Click on the panel: Adds a new element, if there are no elements to conflict with.
" + RightClick;
                case Main.ActiveToolType.Pipe:
                    return 
@"Left Click on connection zone: Zone is selected to be used in a new path.
If two zones are active, a path is created between them going from the Outflow -> Inflow.
Paths are only valid if the zones are not of the same type ( Out, Out ; In; In)
Left Click on Path: Creates a midpoint at the desired location if no midpoint already exists.
";
                case Main.ActiveToolType.Delete:
                    return
@"Move mouse over an element / pipe / pipe mid point to highlight
Left click: Deletes the element / pipe / pipe mid point" + RightClick;
                case Main.ActiveToolType.Select:
                    return
@"Move mouse over an element / path mid point to highlight
Left click: Begins to drag the element / mid point; Release to stop dragging and place to new position, if free.
Right click: Cancels dragging.
Move mouse over an element / path to highlight
Right click on path: Opens a dialog to provide information about the pipe and allows manipulation of the max flow." + RightClick;
                default: return "Select a tool from the toolbar to see details";
            }
        }

        string RightClick = 
@"Right click on element:Opens a dialog allowing to make changes to the element.
Right click on the drawing panel: Opens a dialog allowing to add new elements, if there are no elements to conflict with.";
    }
}
