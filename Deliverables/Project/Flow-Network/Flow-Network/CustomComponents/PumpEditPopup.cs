using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flow_Network.CustomComponents
{
    public partial class PumpEditPopup : EditPopup
    {
        public PumpElement Pump { get { return this.Value as PumpElement; } set { this.Value = value; } }
        public PumpEditPopup(PumpElement pump) : base(pump)
        {
            InitializeComponent();

            this.nudFlow.ValueChanged += (x, y) => { this.Pump.Flow = (float)this.nudFlow.Value; this.OnFlowAltered(); };
        }

        protected override void OnObjectChanged()
        {
            this.nudFlow.Value = (decimal)this.Pump.Flow;
        }

        public void AdjustFlow(float value)
        {
            this.nudFlow.Value += (decimal)value;
        }
    }
}
