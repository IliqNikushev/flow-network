using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Flow_Network
{
    /// <summary>
    /// Flow network main form
    /// </summary>
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
        /// <summary>
        /// All elements within the network
        /// </summary>
        public static List<Element> AllElements = new List<Element>();
        /// <summary>
        /// All paths within the network
        /// </summary>
        public static List<ConnectionZone.Path> AllPaths = new List<ConnectionZone.Path>();

        private ActiveToolType ActiveTool = ActiveToolType.None;

        private PictureBox iconBelowCursor;

        private ConnectionZone PathStart;
        private ConnectionZone PathEnd;

        private Element dragElement;
        private PictureBox oldDragElementPlaceholder;
        private Point dragStart;
        private Point mousePosition = new Point(0, 0);
        PictureBox currentActiveToolPbox;
        ConnectionZone.Path pathToDelete;

        /// <summary>
        /// Initializes the resources based on the present images
        /// Subscribes for the Undo and Redo events of the UndoStack
        /// Subscribes the draw panel for the different events
        /// </summary>

        public Main()
        {
            InitializeComponent();

            oldDragElementPlaceholder = new PictureBox();
            oldDragElementPlaceholder.Height = Element.DefaultSize.Y;
            oldDragElementPlaceholder.SizeMode = PictureBoxSizeMode.StretchImage;
            oldDragElementPlaceholder.Width = Element.DefaultSize.X;
            oldDragElementPlaceholder.BorderStyle = BorderStyle.FixedSingle;
            oldDragElementPlaceholder.Visible = false;

            plDraw.Controls.Add(oldDragElementPlaceholder);
            
            Resources.PumpIcon = this.pbPump.Image;
            Resources.SinkIcon = Properties.Resources.sinkRescaled;
            Resources.MergerIcon = Properties.Resources.merger1;
            Resources.SplitterIcon = Properties.Resources.splitter1;
            Resources.AdjSplitterIcon = this.pbAdjSplitter.Image;
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
            plDraw.Paint += plDraw_Redraw;
            plDraw.Click += plDraw_HandleClick;

            UndoStack.OnUndoAltered += (numberLeft, lastAction) =>
            {
                numberActionsToUndoLbl.Text = numberLeft.ToString();
                if (lastAction == null)
                    lastActionToUndoLbl.Text = "";
                else
                    lastActionToUndoLbl.Text = lastAction.ToString();

                if (lastAction is UndoableActions.RemoveConnectionAction || lastAction is UndoableActions.AddConnectionAction)
                    plDraw.Invalidate();
            };

            UndoStack.OnRedoAltered += (numberLeft, lastAction) =>
            {
                numberActionsRedone.Text = numberLeft.ToString();
                if (lastAction == null)
                    lastActionUndone.Text = "";
                else
                    lastActionUndone.Text = lastAction.ToString();

                if (lastAction is UndoableActions.RemoveConnectionAction || lastAction is UndoableActions.AddConnectionAction)
                    plDraw.Invalidate();
            };
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
            if (currentActiveToolPbox == pbSelect)
                ActiveTool = ActiveToolType.Select;
            else if (currentActiveToolPbox == pbPump)
                ActiveTool = ActiveToolType.Pump;
            else if (currentActiveToolPbox == pbSink)
                ActiveTool = ActiveToolType.Sink;
            else if (currentActiveToolPbox == pbSplitter)
                ActiveTool = ActiveToolType.Splitter;
            else if (currentActiveToolPbox == pbAdjSplitter)
                ActiveTool = ActiveToolType.AdjustableSplitter;
            else if (currentActiveToolPbox == pbMerger)
                ActiveTool = ActiveToolType.Merger;
            else if (currentActiveToolPbox == pbPipe)
                ActiveTool = ActiveToolType.Pipe;
            else if (currentActiveToolPbox == pbDelete)
                ActiveTool = ActiveToolType.Delete;
            else
                ActiveTool = ActiveToolType.None;
            clickedPbox.BackColor = Color.Gold;
            plDraw.Invalidate();
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
            Element e = FindElementUnder(mousePosition);
            if (e != null)
                RemoveElement(e);
        }

        void HandleCreateElementToolClick()
        {
            if (HasCollision(mousePosition))
            {
                return;
            }

            Element elementToAdd = null;

            if (ActiveTool == ActiveToolType.Pump)
            {
                elementToAdd = new PumpElement();

            }
            else if (ActiveTool == ActiveToolType.Sink)
            {
                elementToAdd = new SinkElement();
            }
            else if (ActiveTool == ActiveToolType.Splitter)
            {
                elementToAdd = new SplitterElement();
            }
            else if (ActiveTool == ActiveToolType.AdjustableSplitter)
            {
                elementToAdd = new AdjustableSplitter();
            }
            else if (ActiveTool == ActiveToolType.Merger)
            {
                elementToAdd = new MergerElement();
            }
            if (elementToAdd != null)
            {
                AddElement(elementToAdd, mousePosition);
            }
            else
                throw new ArgumentException("Unknown element " + ActiveTool);
        }

        void HandleConnectionToolClick()
        {
            //TO DO: if hovered is added to path and if the element is already added as start or end ... end method
            ConnectionZone hovered = FindConnectionZoneUnder(mousePosition);
            if (hovered == null) return;

            if (PathStart == null) PathStart = hovered;
            else PathEnd = hovered;
            
            if (PathStart != null && PathEnd != null)
            {
                if (PathStart.Parent == PathEnd.Parent)
                {
                    PathStart = null;
                    PathStart = null;
                    return;
                }
                else if(PathEnd.Parent is PumpElement)
                {
                    PathEnd = null;
                    return;
                }
                else if (PathStart.Parent is SinkElement)
                {
                    PathStart = null;
                    return;
                }
                ConnectionZone.Path result = new ConnectionZone.Path(PathStart, PathEnd);

                result.OnCreated += () =>
                {
                    result.Add();
                    plDraw.Invalidate();
                };

                result.OnAdjusted += () =>
                {
                    plDraw.Invalidate();
                };

                result.Adjust();

                UndoStack.AddAction(new UndoableActions.AddConnectionAction(result));

                PathStart = null;
                PathEnd = null;
            }

        }
        #endregion
        #region drag
        void plDraw_HandleStopDrag(object sender, MouseEventArgs e)
        {
            if (HasCollision(mousePosition))
            {
                RevertDrag();
            }
            oldDragElementPlaceholder.Visible = false;
            dragElement = null;
            plDraw.Cursor = Cursors.Arrow;
        }

        void plDraw_HandleStartDrag(object sender, MouseEventArgs e)
        {
            if (ActiveTool == ActiveToolType.Select)
            {
                if (dragElement == null)
                {
                    dragElement = FindElementUnder(mousePosition);
                    if (dragElement != null)
                    {
                        dragStart = dragElement.Location;
                        oldDragElementPlaceholder.Visible = true;
                        oldDragElementPlaceholder.Location = dragStart;
                        oldDragElementPlaceholder.Image = dragElement.Icon;
                    }
                }
                else
                {
                    dragElement.Location = mousePosition;
                }
            }
        }

        void plDraw_MoveDragElement(object sender, MouseEventArgs e)
        {
            if (dragElement == null) return;
            if (dragElement.Location != e.Location)
            {
                dragElement.Location = e.Location;
                RefreshConnections();
            }
        }

        private void RevertDrag()
        {
            if (dragElement == null) return;

            dragElement.Location = dragStart;
            oldDragElementPlaceholder.Visible = false;
            dragElement = null;

            RefreshConnections();

        }

        #endregion

        void plDraw_HandleDynamicIcon(object sender, MouseEventArgs evnt)
        {
            if (ActiveTool == ActiveToolType.Select)
            {
                Element e = FindElementUnder(mousePosition);
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
            else if (ActiveTool == ActiveToolType.Delete)
            {
                iconBelowCursor.Visible = false;
                if (HasCollision(mousePosition))
                {
                    this.Cursor = System.Windows.Forms.Cursors.No;
                }
                else
                    this.Cursor = Cursors.Arrow;

                ConnectionZone.Path previousCollisionPath = pathToDelete;
                //todo check if works
                pathToDelete = FindPathUnder(mousePosition);
                if (previousCollisionPath != pathToDelete)
                    plDraw.Invalidate();
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

        void plDraw_Redraw(object sender, PaintEventArgs e)
        {
            foreach (var item in AllElements)
            {
                e.Graphics.DrawImage(item.Icon, item.Location.X, item.Location.Y, item.Width, item.Height);
                if (ActiveTool == ActiveToolType.Pipe)
                    foreach (var con in item.ConnectionZones)
                    {
                        //if connection is taken make red, if connection is empty green, if connection is in use yellow
                        //if mouse is on top - mark active
                        e.Graphics.DrawImage(Properties.Resources.toggled, con.Location.X, con.Location.Y, con.Width, con.Height);
                    }
            }

            foreach (ConnectionZone.Path path in new List<ConnectionZone.Path>(AllPaths))
            {
                Pen pen = Pens.Black;

                if (pathToDelete == path)
                    pen = new Pen(Color.Red, 3);
                else
                    pen = Pens.Black;

                Point previous = path.From;
                foreach (Point point in path.PathPoints)
                {
                    e.Graphics.DrawLine(pen, previous, point);
                    previous = point;
                }
            }
        }
        private bool LineIntersectsAt(Point a, Point b, Point mouse)
        {
            Point crossH1 = new Point(mouse.X, mouse.Y - 1);
            Point crossH2 = new Point(mouse.X, mouse.Y + 1);

            Point crossV1 = new Point(mouse.X - 1, mouse.Y);
            Point crossV2 = new Point(mouse.X + 1, mouse.Y);

            return (Collision.Intersects(a, b, crossH1, crossH2) || Collision.Intersects(a, b, crossV1, crossV2));
        }

        #region AddElement Remove

        void RemoveElement(Element e)
        {
            AllElements.Remove(e);
            UndoStack.AddAction(new UndoableActions.RemoveElementAction(e));

            RefreshConnections(e);
        }

        void AddElement(Element e, Point position)
        {
            e.X = position.X;
            e.Y = position.Y;

            AllElements.Add(e);

            UndoStack.AddAction(new UndoableActions.AddElementAction(e));

            RefreshConnections(e);
        }

        private void RefreshConnections(Element e = null)
        {
            if (e == null)
                foreach (Element item in AllElements)
                    item.RefreshConnections();
            else
                foreach (Element item in AllElements)
                    if (item == e)
                        continue;
                    else
                        item.RefreshConnections();

            plDraw.Invalidate();
        }

        void AddElement<T>(Point position) where T : Element
        {
            Element e = Activator.CreateInstance<T>();
            AddElement(e, position);
        }
        #endregion

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
            {
                RevertDrag();
                return;
            }

            RightClickOptions options = ~RightClickOptions.Remove;
            rightClickMousePosition = mousePosition;

            if (HasCollision(rightClickMousePosition))
            {
                Element e = FindElementUnder(rightClickMousePosition);
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
                    Element e = FindElementUnder(rightClickMousePosition);
                    if (e == null) return;
                    else
                    {
                        RemoveElement(e);
                    }
                }).Name = "remove";
                rightClickPanel.AddButton("Add Pump", 20, (x, y) => { AddElement<PumpElement>(rightClickMousePosition); });
                rightClickPanel.AddButton("Add Sink", 40, (x, y) => { AddElement<SinkElement>(rightClickMousePosition); });
                rightClickPanel.AddButton("Add Splitter", 60, (x, y) => { AddElement<SplitterElement>(rightClickMousePosition); });
                rightClickPanel.AddButton("Add Adjustable", 80, (x, y) => { AddElement<AdjustableSplitter>(rightClickMousePosition); });
                rightClickPanel.AddButton("Add Merger", 100, (x, y) => { AddElement<MergerElement>(rightClickMousePosition); });
                rightClickPanel.AddButton("Cancel", 120).Name = "cancel";

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

            rightClickPanel.Controls.Find("cancel", false)[0].Enabled = true;

            rightClickPanel.Location = rightClickMousePosition;
            rightClickPanel.Visible = true;
            rightClickPanel.BringToFront();
            rightClickPanel.BackColor = Color.FromArgb(255, 157, 157, 157);
        }
        #endregion
        #region collision,element detection
        private Element FindElementUnder(Point mousePosition)
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
        private ConnectionZone.Path FindPathUnder(Point mousePosition)
        {
            foreach (ConnectionZone.Path path in AllPaths)
            {
                for (int i = 0; i < path.PathPoints.Count - 1; i++)
                {
                    Point lineStart = path.PathPoints[i];
                    Point lineEnd = path.PathPoints[i + 1];
                    if (LineIntersectsAt(lineStart, lineEnd, mousePosition))
                        return path;
                }
            }
            return null;
        }

        private ConnectionZone FindConnectionZoneUnder(Point mousePosition)
        {
            foreach (var item in AllElements)
            {
                foreach (var q in item.ConnectionZones)
                {
                    if (q.Location.X <= mousePosition.X && q.Location.X + q.Width >= mousePosition.X)
                        if (q.Location.Y <= mousePosition.Y && q.Location.Y + q.Height >= mousePosition.Y)
                            return q;
                }
            }
            return null;
        }

        private Element FindCollisionForPlacementOfElementUnder(Point mousePosition)
        {
            return AllElements.FirstOrDefault(q =>
            {
                if (q == dragElement) return false;
                Point position = mousePosition;

                if (q.X - q.Width <= position.X && q.X + q.Width >= position.X)
                    if (q.Y - q.Height <= position.Y && q.Y + q.Height >= position.Y)
                        return true;
                return false;
            });
        }

        private bool HasCollision(Point mousePosition)
        {
            return FindCollisionForPlacementOfElementUnder(mousePosition) != null;
        }
        #endregion

        private void undoButton_Click(object sender, EventArgs e)
        {
            UndoStack.Undo();
        }

        private void redoButton_Click(object sender, EventArgs e)
        {
            UndoStack.Redo();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "D:\\";
            openFileDialog.Filter = "pipeline file(*.pipelane)|*.pipelane";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string nextLine;
                while ((nextLine = sr.ReadLine()) != null)
                {
                    Console.WriteLine(nextLine);
                    
                    {

                    }
                } 
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = "D:\\";
            sfd.Filter = "pipeline file(*.pipelane)|*.pipelane";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = sfd.OpenFile()) != null)
                {
                    using (StreamWriter sw = new StreamWriter(myStream))
                    {
                        foreach (var item in AllElements)
                        {
                            string x = item.Location.X.ToString();
                            string y = item.Location.Y.ToString();
                            sw.WriteLine(item + "," + x +","+ y);
                            // following is the connection need to be corrected
                            foreach (var con in item.Connections)
                            {
                                sw.WriteLine("line" + "," + x + "," + y);
                            }
                            // does not work the right way, dont know about how connection is working
                        }
                    }
                    myStream.Close();
                    MessageBox.Show("Saved");
                }

            }
        }
    }
}

        static class Extentions
        {
            public static Button AddButton(this Panel panel, string text, int top, EventHandler onClick = null)
            {
                Button button = new Button();
                button.Text = text;
                button.Width = panel.Width;
                button.Height = 20;
                button.Top = top;
                if (onClick != null)
                    button.Click += onClick;

                panel.Controls.Add(button);

                return button;
            }
        }

