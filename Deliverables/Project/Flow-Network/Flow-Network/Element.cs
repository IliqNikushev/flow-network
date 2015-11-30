using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flow_Network
{
    public class Element
    {
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
