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
        /// 
        Graphics plDrawGraphics;

        public Main()
        {
            InitializeComponent();
            this.plDrawGraphics = this.plDraw.CreateGraphics();
            oldDragElementPlaceholder = new PictureBox();
            oldDragElementPlaceholder.Height = Element.DefaultSize.Y;
            oldDragElementPlaceholder.SizeMode = PictureBoxSizeMode.StretchImage;
            oldDragElementPlaceholder.Width = Element.DefaultSize.X;
            oldDragElementPlaceholder.BorderStyle = BorderStyle.FixedSingle;
            oldDragElementPlaceholder.Visible = false;

            plDraw.Controls.Add(oldDragElementPlaceholder);
            
            iconBelowCursor = new PictureBox();
            iconBelowCursor.Width = 16;
            iconBelowCursor.Height = 16;
            iconBelowCursor.BackColor = Color.AliceBlue;
            iconBelowCursor.Visible = false;
            Controls.Add(iconBelowCursor);

            plDraw.Click += plDraw_HandleClick;
            plDraw.MouseMove += plDraw_MouseMove;
            plDraw.MouseDown += plDraw_HandleStartDrag;
            plDraw.MouseUp += plDraw_HandleStopDrag;
            plDraw.Paint += plDraw_Redraw;
            

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

        void plDraw_MouseMove(object sender, MouseEventArgs e)
        {
            plDraw_HandleHover(sender, e);
            plDraw_HandleDynamicIcon(sender, e);
            if (dragElement != null)
            {
                plDraw_MoveDragElement(sender, e);
            }
        }

        Drawable lastHoveredDrawable = null;

        ConnectionZone lastHovered;
        ConnectionZone lastHoveredConnected;
        void plDraw_HandleHover(object sender, MouseEventArgs e)
        {
            Drawable hovered = null;
            DrawState state = DrawState.None;
            if (ActiveTool == ActiveToolType.Pipe)
                hovered = FindConnectionZoneUnder(mousePosition);
            else
                if (hovered == null)
                    hovered = FindElementUnder(mousePosition);
            if(hovered == null)
                hovered = FindPathUnder(mousePosition);
            if(hovered == null)
            {
                if (lastHoveredDrawable != null)
                {
                    if (lastHoveredDrawable is ConnectionZone)
                    {
                        ConnectionZone z = lastHoveredDrawable as ConnectionZone;
                        if (z == PathStart)
                            z.DrawState = DrawState.Active;
                        else if (z.IsConnected || z.FlowIsSameAs(PathStart))
                            z.DrawState = DrawState.Blocking;
                        else
                            z.DrawState = DrawState.Normal;
                    }
                    else
                        lastHoveredDrawable.DrawState = lastHoveredDrawable.LastState;
                    lastHoveredDrawable.Draw(plDrawGraphics);
                }
                
                lastHoveredDrawable = null;
                return;
            }
            if (hovered is ConnectionZone)
            {
                if (hovered == PathStart)
                    state = DrawState.Active;
                else if ((hovered as ConnectionZone).IsConnected)
                    state = DrawState.Blocking;
                else if (PathStart != null)
                {
                    if (PathStart.FlowIsSameAs(hovered as ConnectionZone))
                        state = DrawState.Blocking;
                    else
                        state = DrawState.Hovered;
                }
                else
                    state = DrawState.Hovered;
            }
            else if (hovered is Element)
            {
                if (ActiveTool == ActiveToolType.Delete)
                    state = DrawState.Delete;
                else if (ActiveTool == ActiveToolType.Select)
                    state = DrawState.Hovered;
                else
                    state = DrawState.Blocking;
            }
            if (state == DrawState.None)
               state = DrawState.Normal;

            hovered.DrawState = state;
            hovered.Draw(plDrawGraphics);
            if(ActiveTool == ActiveToolType.Pipe)
                if (hovered is Element)
                {
                    foreach (ConnectionZone zone in (hovered as Element).ConnectionZones)
                    {
                        zone.Draw(plDrawGraphics);
                    }
                }
            lastHoveredDrawable = hovered;
                //HandleHoverOverZone(zone);
            //Element element = FindElementUnder(mousePosition);
            //if (element != null)// if current element == previous -> nothing happens, else previous is unhovered


        }

        void HandleHoverOverZone(ConnectionZone hovered)
        {
            if (hovered != null)
            {
                lastHovered = hovered;
                if (hovered.DrawState == DrawState.Blocking)
                {
                    lastHoveredConnected = hovered;
                }
                hovered.DrawState = DrawState.Hovered;
                plDraw.Invalidate();
            }
            else if (hovered != lastHovered)
            {
                lastHovered.DrawState = DrawState.Normal;
                if (lastHoveredConnected != null)
                {
                    lastHoveredConnected.DrawState = DrawState.Blocking;
                }
            }
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
            if (rightClickPanel != null)
                if (rightClickPanel.Visible)
                    rightClickPanel.Visible = false;

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
                case ActiveToolType.Pipe: HandlePipeToolClick(); return;
                case ActiveToolType.Delete: HandleDeleteToolClick(); return;
                case ActiveToolType.Select: HandleSelectToolClick(); return;
                case ActiveToolType.None: return;
                default: throw new ArgumentException("Unknown tool " + ActiveTool);
            }
        }
        #region tools

        private CustomComponents.PipeEditPopup pipeEditPopup;

        void HandleSelectToolClick()
        {
            if (dragElement != null) return;

            HandleEdit();
        }

        void HandleEdit()
        {
            ConnectionZone.Path path = FindPathUnder(mousePosition);
            if (path != null)
            {
                ShowEditPath(path);
                return;
            }

        }

        void ShowEditPath(ConnectionZone.Path path)
        {
            foreach (Point midPoint in path.UserDefinedMidPoints)
            {
                //if mousePosition is within midpoint => DRAG midpoint
            }
            if (pipeEditPopup == null)
            {
                pipeEditPopup = new CustomComponents.PipeEditPopup(path);

                this.plDraw.Controls.Add(pipeEditPopup);
            }

            pipeEditPopup.Location = mousePosition;
        }

        void ShowEditPump()
        {
        }

        void ShowEditAdjustableSplitter()
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
        

        void HandlePipeToolClick()
        {
            //TO DO: end if cycle;
            ConnectionZone hovered = FindConnectionZoneUnder(mousePosition);
            if (hovered == null) return;

            if (PathStart != null) if (PathStart.Parent == hovered.Parent) return;
            if (hovered.IsConnected) return;

            if (PathStart == null) PathStart = hovered;
            else PathEnd = hovered;
            if (PathStart != null && PathEnd == null)
            {
                PathStart.DrawState = DrawState.Active;
                SetBlockedForSameFlowZones(true);
            }
            if (PathStart != null && PathEnd != null)
            {
                if (PathStart.FlowIsSameAs(PathEnd))
                {
                    PathEnd = null;
                    return;
                }
                if (PathStart.Parent == PathEnd.Parent)
                {
                    PathEnd = null;
                    return;
                }
                ResetBlockedForSameFlowZones(true);

                PathStart.DrawState = DrawState.Blocking;
                PathEnd.DrawState = DrawState.Blocking;
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

        private void ResetBlockedForSameFlowZones(bool redraw)
        {
            foreach (Element e in AllElements)
            {
                foreach (ConnectionZone zone in e.ConnectionZones)
                {
                    if (zone.IsConnected)
                        zone.DrawState = DrawState.Blocking;
                    else
                        zone.DrawState = DrawState.Normal;
                    zone.Draw(plDrawGraphics);
                }
            }
        }

        private void SetBlockedForSameFlowZones(bool redraw)
        {
            foreach (Element e in AllElements)
            {
                foreach (ConnectionZone zone in e.ConnectionZones)
                {
                    if (zone == PathStart) continue;
                    if (zone.FlowIsSameAs(PathStart))
                        zone.DrawState = DrawState.Blocking;
                    if(redraw)
                        zone.Draw(plDrawGraphics);
                }
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
            if (ActiveTool != ActiveToolType.Select)
                return;
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
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
                        con.Draw(e.Graphics);
                        //if connection is taken make red, if connection is empty green, if connection is in use yellow
                        //if mouse is on top - mark active

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
            Edit,
            Cancel
        }

        Panel rightClickPanel;
        Point rightClickMousePosition = new Point();

        private void HandleRightClick()
        {
            if (ActiveTool == ActiveToolType.Select)
            {
                RevertDrag();
                HandleEdit();
                return;
            }

            if (ActiveTool == ActiveToolType.Pipe)
            {
                if (PathStart != null)
                {
                    ResetBlockedForSameFlowZones(false);
                    PathStart.DrawState = DrawState.Normal;
                }
                
                PathStart = null;
                return;
            }

            RightClickOptions options = ~RightClickOptions.Remove;
            rightClickMousePosition = mousePosition;

            if (HasCollision(rightClickMousePosition))
            {
                Element e = FindElementUnder(rightClickMousePosition);
                if (e != null)
                {
                    options = RightClickOptions.Remove;
                    if (e is AdjustableSplitter || e is PumpElement)
                        options |= RightClickOptions.Edit;
                }
            }

            if (rightClickPanel == null)
            {
                rightClickPanel = new Panel();
                rightClickPanel.LostFocus += (x, y) => rightClickPanel.Visible = false;
                plDraw.Controls.Add(rightClickPanel);

                rightClickPanel.Width = 100;

                rightClickPanel.AddButton("Edit", (x, y) => 
                {
                    HandleEdit();
                }).Name = "edit";

                rightClickPanel.AddButton("Remove", (x, y) =>
                {
                    Element e = FindElementUnder(rightClickMousePosition);
                    if (e == null) return;
                    else
                    {
                        RemoveElement(e);
                    }
                }).Name = "remove";
                rightClickPanel.AddButton("Add Pump", (x, y) => { AddElement<PumpElement>(rightClickMousePosition); });
                rightClickPanel.AddButton("Add Sink", (x, y) => { AddElement<SinkElement>(rightClickMousePosition); });
                rightClickPanel.AddButton("Add Splitter", (x, y) => { AddElement<SplitterElement>(rightClickMousePosition); });
                rightClickPanel.AddButton("Add Adjustable", (x, y) => { AddElement<AdjustableSplitter>(rightClickMousePosition); });
                rightClickPanel.AddButton("Add Merger", (x, y) => { AddElement<MergerElement>(rightClickMousePosition); });
                rightClickPanel.AddButton("Cancel").Name = "cancel";

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
            if (options.HasFlag(RightClickOptions.Remove))
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
            else
                rightClickPanel.Controls.Find("remove", false)[0].Enabled = false;

            if(options.HasFlag(RightClickOptions.Edit))
                rightClickPanel.Controls.Find("edit", false)[0].Enabled = true;
            else
                rightClickPanel.Controls.Find("edit", false)[0].Enabled = false;
            rightClickPanel.Controls.Find("cancel", false)[0].Enabled = true;

            int current = 0;
            foreach (var item in rightClickPanel.Controls)
            {
                Button i = item as Button;
                if (i != null)
                {
                    if (i.Enabled)
                    {
                        i.Top = current;
                        current += i.Height;
                        i.Visible = true;
                    }
                    else
                        i.Visible = false;
                }
            }

            rightClickPanel.Height = current;

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
            MessageBoxButtons button = MessageBoxButtons.YesNoCancel;
            DialogResult dr = MessageBox.Show("anything need to be saved?", "new", button);
            if (dr == DialogResult.Yes)
            {
                btnSave_Click(sender, e);
            }
            else if (dr == DialogResult.No)
            {
                AllPaths.Clear();
                AllElements.Clear();
                plDraw.Invalidate();
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            AllPaths.Clear();
            AllElements.Clear();
            plDraw.Invalidate();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "pipeline file(*.pipelane)|*.pipelane";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string nextLine;
                string[] a;
                Element l = null;
                while ((nextLine = sr.ReadLine()) != null)
                {
                    Console.WriteLine(nextLine);
                    { 
                        a= nextLine.Split(',');
                        if (a[0] == typeof(SinkElement).Name)
                        {
                            l = new SinkElement();
                        }
                        if (a[0] == typeof(PumpElement).Name)
                        {
                            l = new PumpElement();
                        } 
                        if (a[0] == typeof(MergerElement).Name)
                        {
                            l = new MergerElement();
                        }
                        if (a[0] == typeof(SplitterElement).Name)
                        {
                            l = new SplitterElement();
                        }
                        if (a[0] == typeof(AdjustableSplitter).Name)
                        {
                            l = new AdjustableSplitter();
                        }
                        if (a[0] == typeof(ConnectionZone.Path).Name)
                        {
                            Point from = new Point(int.Parse(a[1]), int.Parse(a[2]));
                            Point to = new Point(int.Parse(a[3]), int.Parse(a[4]));
                            ConnectionZone f = new ConnectionZone(from, l, true);
                            ConnectionZone t = new ConnectionZone(to, l, false);
                            ConnectionZone.Path p = new ConnectionZone.Path(f, t);
                            AllPaths.Add(p);
                        }
                        AllElements.Add(l);
                        l.X = int.Parse(a[1]);
                        l.Y = int.Parse(a[2]);
                    }
                }
                plDraw.Invalidate();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
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
                            sw.WriteLine((item.GetType().Name + "," + x +","+ y));
                        }
                            foreach (var con in AllPaths)
                            {
                                string a = con.From.Location.X.ToString();
                                string b = con.From.Location.Y.ToString();
                                string c = con.To.Location.X.ToString();
                                string d = con.To.Location.Y.ToString();
                                sw.WriteLine(con.GetType().Name + "," + a + "," + b + "," + c +","+ d);
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
            public static Button AddButton(this Panel panel, string text, EventHandler onClick = null)
            {
                Button button = new Button();
                button.Text = text;
                button.Width = panel.Width;
                button.Height = 20;
                foreach (var item in panel.Controls)
                {
                    Control c = item as Control;
                    if(c != null)
                        button.Top += c.Height;
                }
                if (onClick != null)
                    button.Click += onClick;

                panel.Controls.Add(button);

                return button;
            }
        }

