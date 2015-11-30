using System;
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

        List<Connection.Path> paths = new List<Connection.Path>();

        PictureBox iconBelowCursor;

        public static List<Element> AllElements = new List<Element>();

        Element PathStart;
        Element PathEnd;

        bool changed = false;

        public Main()
        {
            InitializeComponent();

            plDraw.Paint += (x, y) =>
            {
                foreach (Connection.Path path in this.paths)
                {
                    Point previous = path.From;
                    foreach (Point point in path.PathPoints)
                    {
                        y.Graphics.DrawLine(Pens.Black, previous, point);
                        previous = point;
                    }
                }
            };

            Resources.PumpIcon = this.pictureBox2.Image;
            Resources.SinkIcon = this.pictureBox3.Image;
            Resources.MergerIcon = this.pictureBox6.Image;
            Resources.SplitterIcon = this.pictureBox4.Image;
            Resources.AdjSplitterIcon = this.pictureBox5.Image;
            iconBelowCursor = new PictureBox();
            iconBelowCursor.Width = 16;
            iconBelowCursor.Height = 16;
            iconBelowCursor.BackColor = Color.AliceBlue;
            iconBelowCursor.Visible = false;
            Controls.Add(iconBelowCursor);

            Point mousePosition = new Point(0,0);
            plDraw.MouseMove += (x, y)
                =>
                {
                    mousePosition = y.Location;
                    if (ActiveTool == ActiveToolType.None)
                    {
                        iconBelowCursor.Visible = false;
                        iconBelowCursor.BackColor = Color.Bisque;
                    }
                    else
                    {
                        iconBelowCursor.Visible = true;
                        Point point = y.Location;
                        point.Offset(plDraw.Location);
                        point.Offset(16, 16);
                        this.iconBelowCursor.Location = point;
                        if(HasCollision(mousePosition))
                        {
                            iconBelowCursor.BackColor = Color.Red;
                        }
                        else iconBelowCursor.BackColor = Color.Green;
                        iconBelowCursor.BringToFront();
                    }
                };

            plDraw.Click += (x,y) =>
                {
                    if (ActiveTool == ActiveToolType.None) return;

                    if (ActiveTool == ActiveToolType.Delete)
                    {
                        return;
                    }

                    if (ActiveTool == ActiveToolType.Pipe)
                    {
                        Element hovered = FindCollisionElement(mousePosition);
                        if (hovered == null) return;

                        if (PathStart == null) PathStart = hovered;
                        else PathEnd = hovered;

                        if (PathStart != null && PathEnd != null)
                        {
                            Connection.Path result = new Connection.Path(new Connection(new Point(), PathStart),
                                new Connection(new Point(), PathEnd));

                            result.OnAdjusted += () =>
                                {
                                    paths.Add(result);
                                    plDraw.Invalidate();
                                };

                            result.Adjust();

                            PathStart = null;
                            PathEnd = null;
                        }

                        return;
                    }

                    if (ActiveTool == ActiveToolType.Select)
                    {
                        return;
                    }

                    if(HasCollision(mousePosition))
                    {
                        return;
                    }

                    Element toAdd = null;

                    if (ActiveTool == ActiveToolType.Pump)
                    {
                        toAdd = new Pump();
                        toAdd.PictureBox.Width = 32;
                        toAdd.PictureBox.Height = 32;
                    }
                    else if (ActiveTool == ActiveToolType.Sink)
                    {
                        toAdd = new Sink();
                        toAdd.PictureBox.Width = 50;
                        toAdd.PictureBox.Height = 50;
                    }
                    else if (ActiveTool == ActiveToolType.Splitter)
                    {
                        toAdd = new Splitter();
                        toAdd.PictureBox.Width = 100;
                        toAdd.PictureBox.Height = 100;
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
                        toAdd.PictureBox.Enabled = false;
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
                        AllElements.Add(toAdd);
                        iconBelowCursor.BackColor = Color.Red;
                    }

                };
                
        }

        private Element FindCollisionElement(Point mousePosition)
        {
            return AllElements.FirstOrDefault(q =>
            {
                Point position = mousePosition;

                if (q.X - q.PictureBox.Width <= position.X && q.X + q.PictureBox.Width >= position.X)
                    if (q.Y - q.PictureBox.Height <= position.Y && q.Y + q.PictureBox.Height >= position.Y)
                        return true;
                return false;
            });
        }

        private bool HasCollision(Point mousePosition)
        {
            return FindCollisionElement(mousePosition) != null;
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



    }
}

