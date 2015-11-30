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

        public static List<Element> AllElements = new List<Element>();
        public static List<ConnectionZone.Path> AllPaths = new List<ConnectionZone.Path>();

        private ActiveToolType ActiveTool = ActiveToolType.None;
       
        private PictureBox iconBelowCursor;

        private Element PathStart;
        private Element PathEnd;

        private Element dragElement;
        private Point dragStart;

        private Point mousePosition = new Point(0, 0);

        PictureBox currentActiveToolPbox;

        public Main()
        {
            InitializeComponent();

            plDraw.Paint += plDraw_DrawPaths;

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

            plDraw.MouseMove += plDraw_HandleDynamicIcon;
            plDraw.MouseMove += plDraw_MoveDragElement;
            plDraw.MouseDown += plDraw_HandleStartDrag;
            plDraw.MouseUp += plDraw_HandleStopDrag;

            plDraw.Click += plDraw_HandleClick;
        }

        protected void pboxToolClick(object sender, EventArgs e)
        {
            PictureBox clickedPbox = (PictureBox)sender;
            if (clickedPbox == null)
                return;
            if (currentActiveToolPbox != null)
            {
                currentActiveToolPbox.BackColor = Color.AliceBlue;
            }
            currentActiveToolPbox = clickedPbox;
            if (currentActiveToolPbox == pictureBox1)
                ActiveTool = ActiveToolType.Select;
            else if (currentActiveToolPbox == pictureBox2)
                ActiveTool = ActiveToolType.Pump;
            else if (currentActiveToolPbox == pictureBox3)
                ActiveTool = ActiveToolType.Sink;
            else if (currentActiveToolPbox == pictureBox4)
                ActiveTool = ActiveToolType.Splitter;
            else if (currentActiveToolPbox == pictureBox5)
                ActiveTool = ActiveToolType.AdjustableSplitter;
            else if (currentActiveToolPbox == pictureBox6)
                ActiveTool = ActiveToolType.Merger;
            else if (currentActiveToolPbox == pictureBox7)
                ActiveTool = ActiveToolType.Pipe;
            else if (currentActiveToolPbox == pictureBox8)
                ActiveTool = ActiveToolType.Delete;
            else
                ActiveTool = ActiveToolType.None;
            clickedPbox.BackColor = Color.Gold;
        }

        void plDraw_HandleClick(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Right)
            {
                HandleRightClick();
                return;
            }

            switch (ActiveTool)
            {
                case ActiveToolType.Pump:
                case ActiveToolType.Sink:
                case ActiveToolType.Splitter:
                case ActiveToolType.AdjustableSplitter:
                case ActiveToolType.Merger:
                    HandleCreateElementToolClick(); return;
                case ActiveToolType.Pipe: HandleConnectionToolClick(); return;
                case ActiveToolType.Delete: HandleDeleteToolClick(); return;
                case ActiveToolType.Select: HandleSelectToolClick(); return;
                case ActiveToolType.None: return;
                default: throw new ArgumentException("Unknown tool " + ActiveTool);
            }
        }
        #region tools
        void HandleSelectToolClick()
        {

        }

        void HandleDeleteToolClick()
        {

        }

        void HandleCreateElementToolClick()
        {
            if (HasCollision(mousePosition))
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
            if (toAdd != null)
            {
                toAdd.PictureBox.Enabled = false;

                toAdd.X = mousePosition.X;
                toAdd.Y = mousePosition.Y;

                this.plDraw.Controls.Add(toAdd.PictureBox);
                AllElements.Add(toAdd);
                iconBelowCursor.BackColor = Color.Red;
            }
            else
                throw new ArgumentException("Unknown element " + ActiveTool);
        }

        void HandleConnectionToolClick()
        {
            Element hovered = FindCollisionUnder(mousePosition);
            if (hovered == null) return;

            if (PathStart == null) PathStart = hovered;
            else PathEnd = hovered;

            if (PathStart != null && PathEnd != null)
            {
                ConnectionZone.Path result = new ConnectionZone.Path(new ConnectionZone(new Point(), PathStart),
                    new ConnectionZone(new Point(), PathEnd));

                result.OnAdjusted += () =>
                {
                    AllPaths.Add(result);
                    plDraw.Invalidate();
                };

                result.Adjust();

                PathStart = null;
                PathEnd = null;
            }

        }
        #endregion
        #region drag
        void plDraw_HandleStopDrag(object sender, MouseEventArgs e)
        {
            if (HasCollision(mousePosition)) return;
            if (dragElement != null)
            {
                dragElement = null;
                plDraw.Cursor = Cursors.Arrow;
            }
        }

        void plDraw_HandleStartDrag(object sender, MouseEventArgs e)
        {
            if (ActiveTool == ActiveToolType.Select)
            {
                if (dragElement == null)
                {
                    dragElement = FindCollisionUnder(mousePosition);
                    if (dragElement != null)
                        dragStart = dragElement.PictureBox.Location;
                }
                else
                {
                    dragElement.PictureBox.Location = mousePosition;
                }
            }
        }

        void plDraw_MoveDragElement(object sender, MouseEventArgs e)
        {
            if (dragElement == null) return;
            dragElement.PictureBox.Location = e.Location;
        }
        #endregion
        void plDraw_HandleDynamicIcon(object sender, MouseEventArgs evnt)
        {
            if (ActiveTool == ActiveToolType.Select)
            {
                Element e = FindCollisionUnder(mousePosition);
                if (e != null || dragElement != null)
                {
                    this.Cursor = Cursors.SizeAll;
                }
                else
                    this.Cursor = Cursors.Arrow;
            }

            mousePosition = evnt.Location;
            if (ActiveTool == ActiveToolType.None)
            {
                iconBelowCursor.Visible = false;
                iconBelowCursor.BackColor = Color.Bisque;
            }
            else
            {
                iconBelowCursor.Visible = true;
                Point point = evnt.Location;
                point.Offset(plDraw.Location);
                point.Offset(16, 16);
                this.iconBelowCursor.Location = point;
                if (HasCollision(mousePosition))
                {
                    iconBelowCursor.BackColor = Color.Red;
                }
                else iconBelowCursor.BackColor = Color.Green;
                iconBelowCursor.BringToFront();
            }
        }

        void plDraw_DrawPaths(object sender, PaintEventArgs e)
        {
            foreach (ConnectionZone.Path path in AllPaths)
            {
                Point previous = path.From;
                foreach (Point point in path.PathPoints)
                {
                    e.Graphics.DrawLine(Pens.Black, previous, point);
                    previous = point;
                }
            }
        }
        #region rightClick
        [Flags]
        enum RightClickOptions
        {
            Sink,
            Pump,
            Splitter,
            Merger,
            Adjustable,
            Remove,
            Cancel
        }

        Panel rightClickPanel;
        Point rightClickMousePosition = new Point();

        private void HandleRightClick()
        {
            if (ActiveTool == ActiveToolType.Select)
                if (dragElement != null)
                {
                    dragElement.PictureBox.Location = dragStart;
                    dragElement = null;

                    return;
                }

            RightClickOptions options = ~RightClickOptions.Remove;
            rightClickMousePosition = mousePosition;

            if (HasCollision(rightClickMousePosition))
            {
                Element e = FindCollisionUnder(rightClickMousePosition);
                if (e != null)
                    options = RightClickOptions.Remove;
            }

            if (rightClickPanel == null)
            {
                rightClickPanel = new Panel();
                plDraw.Controls.Add(rightClickPanel);

                rightClickPanel.Width = 100;
                rightClickPanel.Height = 140;

                rightClickPanel.AddButton("Remove", 0, (x, y) =>
                {
                    Element e = FindCollisionUnder(rightClickMousePosition);
                    if (e == null) return;
                    else
                    {
                        AllElements.Remove(e);
                        plDraw.Controls.Remove(e.PictureBox);
                    }
                }).Name = "remove";
                rightClickPanel.AddButton("Add Pump", 20, (x, y) => { });
                rightClickPanel.AddButton("Add Sink", 40, (x, y) => { });
                rightClickPanel.AddButton("Add Splitter", 60, (x, y) => { });
                rightClickPanel.AddButton("Add Adjustable", 80, (x, y) => { });
                rightClickPanel.AddButton("Add Merger", 100, (x, y) => { });
                rightClickPanel.AddButton("Cancel", 120);

                foreach (var item in rightClickPanel.Controls)
                {
                    if (item is Button)
                    {
                        (item as Button).Click += (x, y) => rightClickPanel.Visible = false;
                    }
                }
            }
            foreach (var item in rightClickPanel.Controls)
            {
                if (item is Button)
                {
                    (item as Button).Enabled = true;
                }
            }
            if (!options.HasFlag(RightClickOptions.Remove))
                rightClickPanel.Controls.Find("remove", false)[0].Enabled = false;
            else
            {
                foreach (var item in rightClickPanel.Controls)
                {
                    if (item is Button)
                    {
                        (item as Button).Enabled = false;
                    }
                }
                rightClickPanel.Controls.Find("remove", false)[0].Enabled = true;
            }

            rightClickPanel.Location = rightClickMousePosition;
            rightClickPanel.Visible = true;
            rightClickPanel.BringToFront();
            rightClickPanel.BackColor = Color.FromArgb(255, 157, 157, 157);
        }
        #endregion
        #region collision,element detection
        private Element FindCollisionUnder(Point mousePosition)
        {
            return AllElements.FirstOrDefault(q =>
                {
                    if (q == dragElement) return false;

                    if (q.X <= mousePosition.X && q.X + q.Width >= mousePosition.X)
                        if (q.Y <= mousePosition.Y && q.Y + q.Height >= mousePosition.Y)
                            return true;
                    return false;
                });
        }

        private Element FindCollisionElement(Point mousePosition)
        {
            return AllElements.FirstOrDefault(q =>
            {
                if (q == dragElement) return false;
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
        #endregion
    }
}

static class E
{
    public static Button AddButton(this Panel panel, string text, int top, EventHandler onClick = null)
    {
        Button button = new Button();
        button.Text = text;
        button.Width = panel.Width;
        button.Height = 20;
        button.Top = top;
        if(onClick != null)
            button.Click += onClick;

        panel.Controls.Add(button);

        return button;
    }
}

