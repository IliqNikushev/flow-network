namespace Flow_Network
{
    partial class Main
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbSink = new System.Windows.Forms.Label();
            this.lbPump = new System.Windows.Forms.Label();
            this.lbSelect = new System.Windows.Forms.Label();
            this.tbInFlow = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.plDraw = new System.Windows.Forms.Panel();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.undoButton = new System.Windows.Forms.Button();
            this.redoButton = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.numberActionsToUndoLbl = new System.Windows.Forms.Label();
            this.lastActionToUndoLbl = new System.Windows.Forms.Label();
            this.lastActionUndone = new System.Windows.Forms.Label();
            this.numberActionsRedone = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.btnNew = new System.Windows.Forms.Button();
            this.nudMaxFlow = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.tbOutFlow = new System.Windows.Forms.TextBox();
            this.nudSafetyLimit = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.infoBox = new System.Windows.Forms.PictureBox();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.pbMerger = new System.Windows.Forms.PictureBox();
            this.pbAdjSplitter = new System.Windows.Forms.PictureBox();
            this.pbSplitter = new System.Windows.Forms.PictureBox();
            this.pbPipe = new System.Windows.Forms.PictureBox();
            this.pbSink = new System.Windows.Forms.PictureBox();
            this.pbPump = new System.Windows.Forms.PictureBox();
            this.pbDelete = new System.Windows.Forms.PictureBox();
            this.pbSelect = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxFlow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSafetyLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMerger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAdjSplitter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSplitter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPipe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSink)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPump)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSelect)).BeginInit();
            this.SuspendLayout();
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(82, 487);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 14);
            this.label10.TabIndex = 19;
            this.label10.Text = "Delete";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(82, 419);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 14);
            this.label9.TabIndex = 18;
            this.label9.Text = "Pipe";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(82, 362);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 14);
            this.label8.TabIndex = 17;
            this.label8.Text = "Merger";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(82, 302);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 14);
            this.label7.TabIndex = 16;
            this.label7.Text = "Adj. splitter";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(82, 237);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 14);
            this.label6.TabIndex = 15;
            this.label6.Text = "Splitter";
            // 
            // lbSink
            // 
            this.lbSink.AutoSize = true;
            this.lbSink.Location = new System.Drawing.Point(82, 180);
            this.lbSink.Name = "lbSink";
            this.lbSink.Size = new System.Drawing.Size(27, 14);
            this.lbSink.TabIndex = 14;
            this.lbSink.Text = "Sink";
            // 
            // lbPump
            // 
            this.lbPump.AutoSize = true;
            this.lbPump.Location = new System.Drawing.Point(82, 121);
            this.lbPump.Name = "lbPump";
            this.lbPump.Size = new System.Drawing.Size(33, 14);
            this.lbPump.TabIndex = 13;
            this.lbPump.Text = "Pump";
            // 
            // lbSelect
            // 
            this.lbSelect.AutoSize = true;
            this.lbSelect.Location = new System.Drawing.Point(82, 61);
            this.lbSelect.Name = "lbSelect";
            this.lbSelect.Size = new System.Drawing.Size(57, 14);
            this.lbSelect.TabIndex = 12;
            this.lbSelect.Text = "Select/Edit";
            // 
            // tbInFlow
            // 
            this.tbInFlow.Enabled = false;
            this.tbInFlow.Location = new System.Drawing.Point(90, 533);
            this.tbInFlow.Name = "tbInFlow";
            this.tbInFlow.ReadOnly = true;
            this.tbInFlow.Size = new System.Drawing.Size(127, 20);
            this.tbInFlow.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 579);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Max in flow";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 536);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current in flow";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.Location = new System.Drawing.Point(12, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 25);
            this.label11.TabIndex = 2;
            this.label11.Text = "Toolbox";
            // 
            // plDraw
            // 
            this.plDraw.BackColor = System.Drawing.Color.OldLace;
            this.plDraw.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.plDraw.Location = new System.Drawing.Point(223, 43);
            this.plDraw.Name = "plDraw";
            this.plDraw.Size = new System.Drawing.Size(815, 556);
            this.plDraw.TabIndex = 2;
            // 
            // btnLoad
            // 
            this.btnLoad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnLoad.Location = new System.Drawing.Point(376, 5);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(76, 31);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "LOAD";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(301, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 31);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "SAVE";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // undoButton
            // 
            this.undoButton.Location = new System.Drawing.Point(480, 1);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(75, 23);
            this.undoButton.TabIndex = 4;
            this.undoButton.Text = "Undo";
            this.undoButton.UseVisualStyleBackColor = true;
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // redoButton
            // 
            this.redoButton.Location = new System.Drawing.Point(480, 21);
            this.redoButton.Name = "redoButton";
            this.redoButton.Size = new System.Drawing.Size(75, 23);
            this.redoButton.TabIndex = 20;
            this.redoButton.Text = "Redo";
            this.redoButton.UseVisualStyleBackColor = true;
            this.redoButton.Click += new System.EventHandler(this.redoButton_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(558, 5);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 14);
            this.label12.TabIndex = 21;
            this.label12.Text = "items to undo";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(662, 5);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(84, 14);
            this.label13.TabIndex = 22;
            this.label13.Text = "Last thing done:";
            // 
            // numberActionsToUndoLbl
            // 
            this.numberActionsToUndoLbl.AutoSize = true;
            this.numberActionsToUndoLbl.Location = new System.Drawing.Point(627, 5);
            this.numberActionsToUndoLbl.Name = "numberActionsToUndoLbl";
            this.numberActionsToUndoLbl.Size = new System.Drawing.Size(31, 14);
            this.numberActionsToUndoLbl.TabIndex = 23;
            this.numberActionsToUndoLbl.Text = "####";
            // 
            // lastActionToUndoLbl
            // 
            this.lastActionToUndoLbl.AutoSize = true;
            this.lastActionToUndoLbl.Location = new System.Drawing.Point(752, 5);
            this.lastActionToUndoLbl.Name = "lastActionToUndoLbl";
            this.lastActionToUndoLbl.Size = new System.Drawing.Size(43, 14);
            this.lastActionToUndoLbl.TabIndex = 24;
            this.lastActionToUndoLbl.Text = "............";
            // 
            // lastActionUndone
            // 
            this.lastActionUndone.AutoSize = true;
            this.lastActionUndone.Location = new System.Drawing.Point(752, 24);
            this.lastActionUndone.Name = "lastActionUndone";
            this.lastActionUndone.Size = new System.Drawing.Size(43, 14);
            this.lastActionUndone.TabIndex = 28;
            this.lastActionUndone.Text = "............";
            // 
            // numberActionsRedone
            // 
            this.numberActionsRedone.AutoSize = true;
            this.numberActionsRedone.Location = new System.Drawing.Point(627, 24);
            this.numberActionsRedone.Name = "numberActionsRedone";
            this.numberActionsRedone.Size = new System.Drawing.Size(31, 14);
            this.numberActionsRedone.TabIndex = 27;
            this.numberActionsRedone.Text = "####";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(662, 24);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(84, 14);
            this.label16.TabIndex = 26;
            this.label16.Text = "Last thing done:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(558, 24);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(69, 14);
            this.label17.TabIndex = 25;
            this.label17.Text = "items to redo";
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(223, 5);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(72, 31);
            this.btnNew.TabIndex = 29;
            this.btnNew.Text = "NEW";
            this.btnNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNew.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // nudMaxFlow
            // 
            this.nudMaxFlow.DecimalPlaces = 2;
            this.nudMaxFlow.Location = new System.Drawing.Point(90, 577);
            this.nudMaxFlow.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudMaxFlow.Name = "nudMaxFlow";
            this.nudMaxFlow.Size = new System.Drawing.Size(127, 20);
            this.nudMaxFlow.TabIndex = 31;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 556);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 14);
            this.label3.TabIndex = 32;
            this.label3.Text = "Total out flow";
            // 
            // tbOutFlow
            // 
            this.tbOutFlow.Enabled = false;
            this.tbOutFlow.Location = new System.Drawing.Point(90, 556);
            this.tbOutFlow.Name = "tbOutFlow";
            this.tbOutFlow.ReadOnly = true;
            this.tbOutFlow.Size = new System.Drawing.Size(127, 20);
            this.tbOutFlow.TabIndex = 33;
            // 
            // nudSafetyLimit
            // 
            this.nudSafetyLimit.DecimalPlaces = 2;
            this.nudSafetyLimit.Location = new System.Drawing.Point(90, 603);
            this.nudSafetyLimit.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudSafetyLimit.Name = "nudSafetyLimit";
            this.nudSafetyLimit.Size = new System.Drawing.Size(127, 20);
            this.nudSafetyLimit.TabIndex = 35;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 605);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 14);
            this.label4.TabIndex = 34;
            this.label4.Text = "Pipe safety limit";
            // 
            // infoBox
            // 
            this.infoBox.Image = global::Flow_Network.Properties.Resources.Information_icon;
            this.infoBox.Location = new System.Drawing.Point(169, 8);
            this.infoBox.Name = "infoBox";
            this.infoBox.Size = new System.Drawing.Size(48, 48);
            this.infoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.infoBox.TabIndex = 36;
            this.infoBox.TabStop = false;
            this.infoBox.Click += new System.EventHandler(this.infoBox_Click);
            // 
            // pictureBox11
            // 
            this.pictureBox11.Enabled = false;
            this.pictureBox11.Image = global::Flow_Network.Properties.Resources.save;
            this.pictureBox11.Location = new System.Drawing.Point(227, 8);
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.Size = new System.Drawing.Size(33, 24);
            this.pictureBox11.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox11.TabIndex = 30;
            this.pictureBox11.TabStop = false;
            // 
            // pictureBox10
            // 
            this.pictureBox10.Enabled = false;
            this.pictureBox10.Image = global::Flow_Network.Properties.Resources.save;
            this.pictureBox10.Location = new System.Drawing.Point(301, 8);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(33, 24);
            this.pictureBox10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox10.TabIndex = 3;
            this.pictureBox10.TabStop = false;
            // 
            // pictureBox9
            // 
            this.pictureBox9.Enabled = false;
            this.pictureBox9.Image = global::Flow_Network.Properties.Resources.load;
            this.pictureBox9.Location = new System.Drawing.Point(380, 8);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(33, 24);
            this.pictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox9.TabIndex = 2;
            this.pictureBox9.TabStop = false;
            // 
            // pbMerger
            // 
            this.pbMerger.BackColor = System.Drawing.Color.AliceBlue;
            this.pbMerger.Image = ((System.Drawing.Image)(resources.GetObject("pbMerger.Image")));
            this.pbMerger.Location = new System.Drawing.Point(17, 343);
            this.pbMerger.Name = "pbMerger";
            this.pbMerger.Size = new System.Drawing.Size(59, 54);
            this.pbMerger.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbMerger.TabIndex = 9;
            this.pbMerger.TabStop = false;
            this.pbMerger.Click += new System.EventHandler(this.pboxToolClick);
            // 
            // pbAdjSplitter
            // 
            this.pbAdjSplitter.BackColor = System.Drawing.Color.AliceBlue;
            this.pbAdjSplitter.Image = global::Flow_Network.Properties.Resources.adjustableSplitter;
            this.pbAdjSplitter.Location = new System.Drawing.Point(17, 283);
            this.pbAdjSplitter.Name = "pbAdjSplitter";
            this.pbAdjSplitter.Size = new System.Drawing.Size(59, 54);
            this.pbAdjSplitter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbAdjSplitter.TabIndex = 8;
            this.pbAdjSplitter.TabStop = false;
            this.pbAdjSplitter.Click += new System.EventHandler(this.pboxToolClick);
            // 
            // pbSplitter
            // 
            this.pbSplitter.BackColor = System.Drawing.Color.AliceBlue;
            this.pbSplitter.Image = ((System.Drawing.Image)(resources.GetObject("pbSplitter.Image")));
            this.pbSplitter.Location = new System.Drawing.Point(17, 223);
            this.pbSplitter.Name = "pbSplitter";
            this.pbSplitter.Size = new System.Drawing.Size(59, 54);
            this.pbSplitter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbSplitter.TabIndex = 7;
            this.pbSplitter.TabStop = false;
            this.pbSplitter.Click += new System.EventHandler(this.pboxToolClick);
            // 
            // pbPipe
            // 
            this.pbPipe.BackColor = System.Drawing.Color.AliceBlue;
            this.pbPipe.Image = global::Flow_Network.Properties.Resources.connect;
            this.pbPipe.Location = new System.Drawing.Point(17, 403);
            this.pbPipe.Name = "pbPipe";
            this.pbPipe.Size = new System.Drawing.Size(59, 54);
            this.pbPipe.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbPipe.TabIndex = 10;
            this.pbPipe.TabStop = false;
            this.pbPipe.Click += new System.EventHandler(this.pboxToolClick);
            // 
            // pbSink
            // 
            this.pbSink.BackColor = System.Drawing.Color.AliceBlue;
            this.pbSink.Image = ((System.Drawing.Image)(resources.GetObject("pbSink.Image")));
            this.pbSink.Location = new System.Drawing.Point(17, 163);
            this.pbSink.Name = "pbSink";
            this.pbSink.Size = new System.Drawing.Size(59, 54);
            this.pbSink.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbSink.TabIndex = 6;
            this.pbSink.TabStop = false;
            this.pbSink.Click += new System.EventHandler(this.pboxToolClick);
            // 
            // pbPump
            // 
            this.pbPump.BackColor = System.Drawing.Color.AliceBlue;
            this.pbPump.Image = ((System.Drawing.Image)(resources.GetObject("pbPump.Image")));
            this.pbPump.Location = new System.Drawing.Point(17, 103);
            this.pbPump.Name = "pbPump";
            this.pbPump.Size = new System.Drawing.Size(59, 54);
            this.pbPump.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbPump.TabIndex = 5;
            this.pbPump.TabStop = false;
            this.pbPump.Click += new System.EventHandler(this.pboxToolClick);
            // 
            // pbDelete
            // 
            this.pbDelete.BackColor = System.Drawing.Color.AliceBlue;
            this.pbDelete.Image = global::Flow_Network.Properties.Resources.close;
            this.pbDelete.Location = new System.Drawing.Point(17, 463);
            this.pbDelete.Name = "pbDelete";
            this.pbDelete.Size = new System.Drawing.Size(59, 59);
            this.pbDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbDelete.TabIndex = 11;
            this.pbDelete.TabStop = false;
            this.pbDelete.Click += new System.EventHandler(this.pboxToolClick);
            // 
            // pbSelect
            // 
            this.pbSelect.BackColor = System.Drawing.Color.AliceBlue;
            this.pbSelect.Image = global::Flow_Network.Properties.Resources.cursor;
            this.pbSelect.Location = new System.Drawing.Point(17, 43);
            this.pbSelect.Name = "pbSelect";
            this.pbSelect.Size = new System.Drawing.Size(59, 54);
            this.pbSelect.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbSelect.TabIndex = 4;
            this.pbSelect.TabStop = false;
            this.pbSelect.Click += new System.EventHandler(this.pboxToolClick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1084, 626);
            this.Controls.Add(this.infoBox);
            this.Controls.Add(this.nudSafetyLimit);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbOutFlow);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudMaxFlow);
            this.Controls.Add(this.pictureBox11);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.pictureBox10);
            this.Controls.Add(this.lastActionUndone);
            this.Controls.Add(this.pictureBox9);
            this.Controls.Add(this.numberActionsRedone);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.lastActionToUndoLbl);
            this.Controls.Add(this.numberActionsToUndoLbl);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.redoButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.plDraw);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.pbMerger);
            this.Controls.Add(this.pbAdjSplitter);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.pbSplitter);
            this.Controls.Add(this.pbPipe);
            this.Controls.Add(this.pbSink);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.pbPump);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbDelete);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pbSelect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbSelect);
            this.Controls.Add(this.lbSink);
            this.Controls.Add(this.tbInFlow);
            this.Controls.Add(this.lbPump);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "Main";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxFlow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSafetyLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMerger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAdjSplitter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSplitter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPipe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSink)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPump)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSelect)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbInFlow;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbDelete;
        private System.Windows.Forms.PictureBox pbPipe;
        private System.Windows.Forms.PictureBox pbMerger;
        private System.Windows.Forms.PictureBox pbAdjSplitter;
        private System.Windows.Forms.PictureBox pbSplitter;
        private System.Windows.Forms.PictureBox pbSink;
        private System.Windows.Forms.PictureBox pbPump;
        private System.Windows.Forms.PictureBox pbSelect;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbSink;
        private System.Windows.Forms.Label lbPump;
        private System.Windows.Forms.Label lbSelect;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel plDraw;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.PictureBox pictureBox10;
        private System.Windows.Forms.PictureBox pictureBox9;
        private System.Windows.Forms.Button undoButton;
        private System.Windows.Forms.Button redoButton;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label numberActionsToUndoLbl;
        private System.Windows.Forms.Label lastActionToUndoLbl;
        private System.Windows.Forms.Label lastActionUndone;
        private System.Windows.Forms.Label numberActionsRedone;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.PictureBox pictureBox11;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.NumericUpDown nudMaxFlow;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbOutFlow;
        private System.Windows.Forms.NumericUpDown nudSafetyLimit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox infoBox;
    }
}

