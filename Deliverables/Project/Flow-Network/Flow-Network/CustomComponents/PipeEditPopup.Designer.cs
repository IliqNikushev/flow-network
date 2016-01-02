namespace Flow_Network.CustomComponents
{
    partial class PipeEditPopup
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblCurrentPercent = new System.Windows.Forms.Label();
            this.numericValuePicker = new System.Windows.Forms.NumericUpDown();
            this.lblCurrentFlow = new System.Windows.Forms.Label();
            this.lblMax = new System.Windows.Forms.Label();
            this.lblMin = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblCurrentText = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericValuePicker)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(106, 0);
            // 
            // lblCurrentPercent
            // 
            this.lblCurrentPercent.AutoSize = true;
            this.lblCurrentPercent.Location = new System.Drawing.Point(51, 30);
            this.lblCurrentPercent.Name = "lblCurrentPercent";
            this.lblCurrentPercent.Size = new System.Drawing.Size(36, 13);
            this.lblCurrentPercent.TabIndex = 1;
            this.lblCurrentPercent.Text = "###%";
            // 
            // numericValuePicker
            // 
            this.numericValuePicker.Location = new System.Drawing.Point(51, 127);
            this.numericValuePicker.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numericValuePicker.Name = "numericValuePicker";
            this.numericValuePicker.Size = new System.Drawing.Size(74, 20);
            this.numericValuePicker.TabIndex = 2;
            this.numericValuePicker.Value = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            // 
            // lblCurrentFlow
            // 
            this.lblCurrentFlow.AutoSize = true;
            this.lblCurrentFlow.Location = new System.Drawing.Point(50, 17);
            this.lblCurrentFlow.Name = "lblCurrentFlow";
            this.lblCurrentFlow.Size = new System.Drawing.Size(35, 13);
            this.lblCurrentFlow.TabIndex = 3;
            this.lblCurrentFlow.Text = "####";
            // 
            // lblMax
            // 
            this.lblMax.AutoSize = true;
            this.lblMax.Location = new System.Drawing.Point(5, 129);
            this.lblMax.Name = "lblMax";
            this.lblMax.Size = new System.Drawing.Size(27, 13);
            this.lblMax.TabIndex = 4;
            this.lblMax.Text = "Max";
            // 
            // lblMin
            // 
            this.lblMin.AutoSize = true;
            this.lblMin.Location = new System.Drawing.Point(5, 0);
            this.lblMin.Name = "lblMin";
            this.lblMin.Size = new System.Drawing.Size(24, 13);
            this.lblMin.TabIndex = 5;
            this.lblMin.Text = "Min";
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(8, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(2, 112);
            this.label3.TabIndex = 6;
            // 
            // lblCurrentText
            // 
            this.lblCurrentText.AutoSize = true;
            this.lblCurrentText.Location = new System.Drawing.Point(10, 61);
            this.lblCurrentText.Name = "lblCurrentText";
            this.lblCurrentText.Size = new System.Drawing.Size(41, 13);
            this.lblCurrentText.TabIndex = 7;
            this.lblCurrentText.Text = "Current";
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(2, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 2);
            this.label5.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(2, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 2);
            this.label6.TabIndex = 9;
            // 
            // PipeEditPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblCurrentText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblMin);
            this.Controls.Add(this.lblMax);
            this.Controls.Add(this.lblCurrentFlow);
            this.Controls.Add(this.numericValuePicker);
            this.Controls.Add(this.lblCurrentPercent);
            this.Name = "PipeEditPopup";
            this.Size = new System.Drawing.Size(126, 148);
            this.Controls.SetChildIndex(this.lblCurrentPercent, 0);
            this.Controls.SetChildIndex(this.numericValuePicker, 0);
            this.Controls.SetChildIndex(this.lblCurrentFlow, 0);
            this.Controls.SetChildIndex(this.lblMax, 0);
            this.Controls.SetChildIndex(this.lblMin, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.lblCurrentText, 0);
            this.Controls.SetChildIndex(this.label5, 0);
            this.Controls.SetChildIndex(this.label6, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            ((System.ComponentModel.ISupportInitialize)(this.numericValuePicker)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCurrentPercent;
        private System.Windows.Forms.NumericUpDown numericValuePicker;
        private System.Windows.Forms.Label lblCurrentFlow;
        private System.Windows.Forms.Label lblMax;
        private System.Windows.Forms.Label lblMin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblCurrentText;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}
