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
    public partial class PipeEditPopup : EditPopup
    {
        public Flow_Network.ConnectionZone.Path CurrentPath
        {
            get { return base.Value as Flow_Network.ConnectionZone.Path; }
            set
            {
                this.Value = value;
            }
        }

        protected override void OnObjectChanged()
        {
            this.numericValuePicker.Value = (decimal)MaxFlow;

            numericValuePicker_ValueChanged(null, null);
        }

        /// <summary>
        /// returns 0...100
        /// </summary>
        private int FlowPercent
        {
            get
            {
                if (CurrentPath == null) return 0;
                if (CurrentPath.MaxFlow == 0) return 0;
                int value = (int)((CurrentPath.Flow / CurrentPath.MaxFlow) * 100);
                if (value > 100) value = 101;
                return value;
            }
        }

        private float MaxFlow
        {
            get
            {
                if (CurrentPath == null) return 0;
                return CurrentPath.MaxFlow;
            }
        }

        private float Flow
        {
            get
            {
                if (CurrentPath == null) return 0;
                return CurrentPath.Flow;
            }
        }

        public PipeEditPopup(ConnectionZone.Path path) : base(path)
        {
            InitializeComponent();

            this.numericValuePicker.ValueChanged += numericValuePicker_ValueChanged;
        }

        void numericValuePicker_ValueChanged(object sender, EventArgs e)
        {
            if (CurrentPath == null) return;

            CurrentPath.MaxFlow = (float)numericValuePicker.Value;

            if(FlowPercent > 100)
                this.lblCurrentPercent.Text = ">100%";
            else
                this.lblCurrentPercent.Text = FlowPercent + "%";
            this.lblCurrentPercent.Top = ArrowTop;

            this.lblCurrentFlow.Text = "("+Flow+")";
            this.lblCurrentFlow.Top = lblCurrentPercent.Top - lblCurrentFlow.Height;

            this.lblCurrentText.Top = lblCurrentPercent.Top;
        }

        int SliderTop
        {
            get
            {
                return lblMin.Top + lblMin.Height + 2;
            }
        }

        int SliderHeight
        {
            get
            {
                return numericValuePicker.Top - this.SliderTop - 10;
            }
        }

        int ArrowTop
        {
            get
            {
                int arrowTop = this.SliderHeight;
                arrowTop = (int)(arrowTop * (FlowPercent / 100.0f));
                arrowTop = SliderTop + arrowTop;
                return arrowTop;
            }
        }
    }
}
