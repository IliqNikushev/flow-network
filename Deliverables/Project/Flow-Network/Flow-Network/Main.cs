﻿using System;
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
    public partial class Main : Form
    {
        enum ActiveToolType
        {
            Pump,
            Splitter,
            Merger,
            AdjustableSplitter,
            Select,
            Delete,
            Sink,
            Pipe,
            None

        }

        ActiveToolType ActiveTool = ActiveToolType.None;


        PictureBox iconBelowCursor;

        List<Element> elements = new List<Element>();

        public Main()
        {
            InitializeComponent();

            Resources.PumpIcon = this.pictureBox2.Image;
            Resources.SinkIcon = this.pictureBox3.Image;
            Resources.MergerIcon = this.pictureBox6.Image;
            Resources.SplitterIcon = this.pictureBox4.Image;
            Resources.AdjSplitterIcon = this.pictureBox5.Image;
            iconBelowCursor = new PictureBox();
            iconBelowCursor.Width = 24;
            iconBelowCursor.Height = 24;
            iconBelowCursor.BackColor = Color.AliceBlue;
            iconBelowCursor.Visible = false;
            Controls.Add(iconBelowCursor);

            Point mousePosition = new Point(0,0);
            plDraw.MouseMove += (x, y)
                => mousePosition = y.Location;

            plDraw.Click += (x,y) =>
                {
                    if (ActiveTool == ActiveToolType.None) return;

                    if (this.elements.Where(q =>
                        {
                            Point position = mousePosition;

                            if (q.X - q.PictureBox.Width <= position.X && q.X + q.PictureBox.Width >= position.X)
                                if (q.Y - q.PictureBox.Height <= position.Y && q.Y + q.PictureBox.Height >= position.Y)
                                    return true;
                            return false;
                        }
                        ).Any())
                        return;

                    Element toAdd = null;

                    if (ActiveTool == ActiveToolType.Pump)
                    {
                        toAdd = new Pump();
                    }
                    else if (ActiveTool == ActiveToolType.Sink)
                    {
                        toAdd = new Sink();
                    }
                    else if (ActiveTool == ActiveToolType.Splitter)
                    {
                        toAdd = new Splitter();
                    }
                    else if (ActiveTool == ActiveToolType.AdjustableSplitter)
                    {
                        toAdd = new AdjustableSplitter();
                    }
                    else if (ActiveTool == ActiveToolType.Merger)
                    {
                        toAdd = new Merger();
                    }
                    if(toAdd != null)
                    {
                        toAdd.PictureBox.Width = 32;
                        toAdd.PictureBox.Height = 32;

                        toAdd.PictureBox.MouseMove += (q, qq) =>
                        {
                            mousePosition = toAdd.PictureBox.Location;
                            mousePosition.Offset(plDraw.Location);


                            Point point = mousePosition;
                            point.X += qq.X;
                            point.Y += qq.Y;
                            point.Offset(16, 16);

                            iconBelowCursor.Location = point;
                        };

                        toAdd.X = mousePosition.X; // x,y = mouse position
                        toAdd.Y = mousePosition.Y;

                        this.plDraw.Controls.Add(toAdd.PictureBox);
                        elements.Add(toAdd);
                    }

                };
                
        }
        

        //private Point MouseDownLocation;

        //private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == System.Windows.Forms.MouseButtons.Left)
        //    {
        //        MouseDownLocation = e.Location;
        //    }
        //}

        //private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        //{

        //    if (e.Button == System.Windows.Forms.MouseButtons.Left)
        //    {
        //        pictureBox1.Left = e.X + pictureBox1.Left - MouseDownLocation.X;
        //        pictureBox1.Top = e.Y + pictureBox1.Top - MouseDownLocation.Y;
        //    }
        //}
        
        
        PictureBox currentActive;
        protected void onClickHandler(object sender, EventArgs e)
        {
            PictureBox s = (PictureBox)sender;
            if (s == null)
                return;
            if (currentActive != null)
            {
                currentActive.BackColor = Color.AliceBlue;
                
            } 
            currentActive = s;
            if (currentActive == pictureBox2)
                ActiveTool = ActiveToolType.Pump;
            else if (currentActive == pictureBox3)
                ActiveTool = ActiveToolType.Sink;
            else if (currentActive == pictureBox4)
                ActiveTool = ActiveToolType.Splitter;
            else if (currentActive == pictureBox5)
                ActiveTool = ActiveToolType.AdjustableSplitter;
            else if (currentActive == pictureBox6)
                ActiveTool = ActiveToolType.Merger;
            else if (currentActive == pictureBox7)
                ActiveTool = ActiveToolType.Pipe;
            else if (currentActive == pictureBox8)
                ActiveTool = ActiveToolType.Delete;
            else
                ActiveTool = ActiveToolType.None;
            s.BackColor = Color.Gold;
        }

        private void plDraw_MouseMove(object sender, MouseEventArgs e)
        {
            
            if(currentActive==null)
            {
                iconBelowCursor.Visible = false;
                iconBelowCursor.BackColor = Color.Bisque;
            }
            else
            {
                iconBelowCursor.Visible = true;
                Point point = e.Location;
                point.Offset(plDraw.Location);
                point.Offset(16, 16);
                this.iconBelowCursor.Location = point;
                iconBelowCursor.BackColor = Color.Gold;
                iconBelowCursor.BringToFront();
            }
        }


    }
}

