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
    public partial class AdjustableSplitterEditPopup : EditPopup
    {
        public AdjustableSplitter Splitter { get { return this.Value as AdjustableSplitter; } set { this.Value = value; } }

        private bool isSetting = false;
        public AdjustableSplitterEditPopup(AdjustableSplitter splitter) : base(splitter)
        {
            InitializeComponent();

            nudDown.ValueChanged += (x, y) =>
            {
                if (isSetting) return;
                isSetting = true;
                Splitter.DownFlowPercent = (int)nudDown.Value;
                nudUp.Value = Splitter.UpFlowPercent;
                OnFlowAltered();
                isSetting = false;
            };

            nudUp.ValueChanged += (x, y) =>
            {
                if (isSetting) return;
                isSetting = true;
                Splitter.UpFlowPercent = (int)nudUp.Value;
                nudDown.Value = Splitter.DownFlowPercent;
                OnFlowAltered();
                isSetting = false;
            };
        }

        protected override void OnObjectChanged()
        {
            if(this.Splitter == null) return;
            nudDown.Value = this.Splitter.DownFlowPercent;
        }
    }
}
