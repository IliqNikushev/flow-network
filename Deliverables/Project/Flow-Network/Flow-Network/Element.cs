﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace Flow_Network
{
    public class Element
    {
        public static List<Element> AllElements { get { return Main.AllElements; } }

        public Point Center { get { return new Point(this.X + this.Width / 2, this.Y + this.Height / 2); } }
        public Point A { get { return new Point(this.X, this.Y + this.Height); } }
        public Point B { get { return new Point(this.X + this.Width, this.Y + this.Height); } }
        public Point C { get { return new Point(this.X + this.Width, this.Y); } }
        public Point D { get { return new Point(this.X, this.Y); } }

        public int Width { get { return this.PictureBox.Width; } }
        public int Height { get { return this.PictureBox.Height; } }

        public System.Drawing.Image Icon { get { return Resources.Icon(this); } }
        public System.Windows.Forms.PictureBox PictureBox { get; private set; }

        public Element()
        {
            this.PictureBox = new System.Windows.Forms.PictureBox();
            this.PictureBox.Image = this.Icon;
            this.PictureBox.Width = 32;
            this.PictureBox.Height = 32;
        }
        public int X
        {
            get
            {
                return this.PictureBox.Left; // total left
            }
            set
            {
                this.PictureBox.Left = value;
            }
        }

        public int Y
        {
            get
            {
                return this.PictureBox.Top;
            }
            set
            {
                this.PictureBox.Top = value;
            }
        }

        //public @delegate OnFlowAltered
        //{
        //    get
        //    {
        //        throw new System.NotImplementedException();
        //    }
        //    set
        //    {
        //    }
        //}
    }
}
