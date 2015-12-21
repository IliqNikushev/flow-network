﻿using System;
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
    public partial class PipeEditPopup : UserControl
    {
        private Graphics controlGraphics;

        private Flow_Network.ConnectionZone.Path currentPath;
        public Flow_Network.ConnectionZone.Path CurrentPath
        {
            get { return currentPath; }
            set
            {
                this.currentPath = value;
                RefreshPath();
            }
        }

        /// <summary>
        /// returns 0...100
        /// </summary>
        private int FlowPercent
        {
            get
            {
                if (currentPath == null) return 0;
                if (currentPath.MaxFlow == 0) return 0;
                int value = (int)((currentPath.Flow / currentPath.MaxFlow) * 100);
                if (value > 100) value = 100;
                return value;
            }
        }

        private float MaxFlow
        {
            get
            {
                if (currentPath == null) return 0;
                return currentPath.MaxFlow;
            }
        }

        private float Flow
        {
            get
            {
                if (currentPath == null) return 0;
                return currentPath.Flow;
            }
        }

        public PipeEditPopup(ConnectionZone.Path path)
        {
            InitializeComponent();

            this.numericValuePicker.ValueChanged += numericValuePicker_ValueChanged;

            this.CurrentPath = path;
        }

        void numericValuePicker_ValueChanged(object sender, EventArgs e)
        {
            if (CurrentPath == null) return;

            currentPath.MaxFlow = (float)numericValuePicker.Value;

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

        private void RefreshPath()
        {
            if (this.CurrentPath == null)
            {
                this.Hide();
                return;
            }
            this.Show();
            this.numericValuePicker.Value = (decimal)MaxFlow;
            
            numericValuePicker_ValueChanged(null, null);
        }
    }
}