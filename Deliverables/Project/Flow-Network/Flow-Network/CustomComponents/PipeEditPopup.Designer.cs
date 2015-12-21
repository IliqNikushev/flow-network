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
            this.lblMin = new System.Windows.Forms.Label();
            this.lblCurrentPercent = new System.Windows.Forms.Label();
            this.numericValuePicker = new System.Windows.Forms.NumericUpDown();
            this.lblCurrentFlow = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblCurrentText = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericValuePicker)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMin
            // 
            this.lblMin.AutoSize = true;
            this.lblMin.Location = new System.Drawing.Point(0, 0);
            this.lblMin.Name = "lblMin";
            this.lblMin.Size = new System.Drawing.Size(13, 13);
            this.lblMin.TabIndex = 0;
            this.lblMin.Text = "0";
            // 
            // lblCurrentPercent
            // 
            this.lblCurrentPercent.AutoSize = true;
            this.lblCurrentPercent.Location = new System.Drawing.Point(22, 52);
            this.lblCurrentPercent.Name = "lblCurrentPercent";
            this.lblCurrentPercent.Size = new System.Drawing.Size(36, 13);
            this.lblCurrentPercent.TabIndex = 1;
            this.lblCurrentPercent.Text = "###%";
            // 
            // numericValuePicker
            // 
            this.numericValuePicker.Location = new System.Drawing.Point(3, 127);
            this.numericValuePicker.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericValuePicker.Name = "numericValuePicker";
            this.numericValuePicker.Size = new System.Drawing.Size(44, 20);
            this.numericValuePicker.TabIndex = 2;
            // 
            // lblCurrentFlow
            // 
            this.lblCurrentFlow.AutoSize = true;
            this.lblCurrentFlow.Location = new System.Drawing.Point(21, 39);
            this.lblCurrentFlow.Name = "lblCurrentFlow";
            this.lblCurrentFlow.Size = new System.Drawing.Size(35, 13);
            this.lblCurrentFlow.TabIndex = 3;
            this.lblCurrentFlow.Text = "####";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Max";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Min";
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(54, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(2, 112);
            this.label3.TabIndex = 6;
            // 
            // lblCurrentText
            // 
            this.lblCurrentText.AutoSize = true;
            this.lblCurrentText.Location = new System.Drawing.Point(56, 61);
            this.lblCurrentText.Name = "lblCurrentText";
            this.lblCurrentText.Size = new System.Drawing.Size(41, 13);
            this.lblCurrentText.TabIndex = 7;
            this.lblCurrentText.Text = "Current";
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(48, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 2);
            this.label5.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(48, 15);
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
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblCurrentFlow);
            this.Controls.Add(this.numericValuePicker);
            this.Controls.Add(this.lblCurrentPercent);
            this.Controls.Add(this.lblMin);
            this.Name = "PipeEditPopup";
            this.Size = new System.Drawing.Size(99, 148);
            ((System.ComponentModel.ISupportInitialize)(this.numericValuePicker)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMin;
        private System.Windows.Forms.Label lblCurrentPercent;
        private System.Windows.Forms.NumericUpDown numericValuePicker;
        private System.Windows.Forms.Label lblCurrentFlow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblCurrentText;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}
