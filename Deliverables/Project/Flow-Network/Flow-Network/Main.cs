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
        private PathMidPointDrawable dragMidPoint;
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
            Resources.Initialize();
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
            plDraw.MouseMove += plDraw_HandleMouseMove;
            plDraw.MouseDown += plDraw_HandleStartDrag;
            plDraw.MouseUp += plDraw_HandleStopDrag;
            plDraw.Paint += plDraw_Redraw;
            plDraw.MouseEnter += plDraw_HandleGainFocus;
            plDraw.MouseLeave += plDraw_HandleLoseFocus;

            UndoStack.OnUndoAltered += (numberLeft, lastAction) =>
            {
                numberActionsToUndoLbl.Text = numberLeft.ToString();
                if (lastAction == null)
                    lastActionToUndoLbl.Text = "";
                else
                    lastActionToUndoLbl.Text = lastAction.ToString();

                if (lastAction is UndoableActions.AddConnectionAction)
                {
                    UndoableActions.AddConnectionAction action = lastAction as UndoableActions.AddConnectionAction;

                    if (action is UndoableActions.RemoveConnectionAction)
                    {
                        plDraw.Invalidate();
                    }
                    else
                    {
                        if (action.Connection.IsNew) return;
                        action.Connection.Draw(this.plDrawGraphics, this.plDraw.BackColor);
                        action.Connection.To.Draw(this.plDrawGraphics, this.plDraw.BackColor);
                        action.Connection.From.Draw(this.plDrawGraphics, this.plDraw.BackColor);
                    }
                }

                else if (lastAction is UndoableActions.AddElementAction)
                {
                    UndoableActions.AddElementAction action = lastAction as UndoableActions.AddElementAction;

                    if (action is UndoableActions.RemoveElementAction)
                    {
                        plDraw.Invalidate();
                    }
                    else
                    {
                        action.Element.Draw(this.plDrawGraphics, this.plDraw.BackColor);
                        RefreshConnections();
                    }
                }

                else if (lastAction is UndoableActions.MoveElementAction)
                {
                    RefreshConnections();
                    plDraw.Invalidate();
                }
            };

            UndoStack.OnRedoAltered += (numberLeft, lastAction) =>
            {
                numberActionsRedone.Text = numberLeft.ToString();
                if (lastAction == null)
                    lastActionUndone.Text = "";
                else
                    lastActionUndone.Text = lastAction.ToString();

                if (lastAction is UndoableActions.AddConnectionAction)
                {
                    UndoableActions.AddConnectionAction action = lastAction as UndoableActions.AddConnectionAction;

                    if (action is UndoableActions.RemoveConnectionAction)
                    {
                        action.Connection.Draw(this.plDrawGraphics, this.plDraw.BackColor);
                    }
                    else
                    {
                        plDraw.Invalidate();
                    }
                }

                else if (lastAction is UndoableActions.AddElementAction)
                {
                    UndoableActions.AddElementAction action = lastAction as UndoableActions.AddElementAction;

                    if (action is UndoableActions.RemoveElementAction)
                    {
                        action.Element.Draw(this.plDrawGraphics, this.plDraw.BackColor);
                        RefreshConnections();
                    }
                    else
                    {
                        plDraw.Invalidate();
                    }
                }

                else if (lastAction is UndoableActions.MoveElementAction)
                {
                    RefreshConnections();
                    plDraw.Invalidate();
                }
            };
        }

        void plDraw_HandleLoseFocus(object sender, EventArgs e)
        {
            iconBelowCursor.Visible = false;
        }

        void plDraw_HandleGainFocus(object sender, EventArgs e)
        {
            if (ActiveTool != ActiveToolType.Delete)
                iconBelowCursor.Visible = true;
        }

        void plDraw_HandleMouseMove(object sender, MouseEventArgs e)
        {
            plDraw_HandleHover(sender, e);
            plDraw_HandleDynamicIcon(sender, e);
            if (dragElement != null)
            {
                plDraw_HandleMoveDragElement(sender, e);
            }
            else if (dragMidPoint != null)
            {
                plDraw_HandleMoveDragMidPoint(sender, e);
            }
        }

        Drawable lastHovered = null;

        void plDraw_HandleHover(object sender, MouseEventArgs e)
        {
            Drawable hovered = null;
            DrawState state = DrawState.None;
            if (ActiveTool == ActiveToolType.Pipe)
                hovered = FindConnectionZoneUnder(mousePosition);
            else
                if (hovered == null)
                    hovered = FindElementUnder(mousePosition);
            if (hovered == null)
                hovered = FindPathUnder(mousePosition);

            if (hovered == lastHovered)
                return;

            if (lastHovered != hovered)
            {
                if (lastHovered != null)
                {
                    if (lastHovered is ConnectionZone)
                    {
                        ConnectionZone z = lastHovered as ConnectionZone;
                        if (z == PathStart)
                            z.DrawState = DrawState.Active;
                        else if (z.IsConnected || z.FlowIsSameAs(PathStart))
                            z.DrawState = DrawState.Blocking;
                        else
                            z.DrawState = DrawState.Normal;
                    }
                    else
                        if (lastHovered is ConnectionZone.Path)
                            lastHovered.DrawState = DrawState.Normal;
                        else if (lastHovered is Element)
                            lastHovered.DrawState = DrawState.Normal;
                        else
                            lastHovered.DrawState = lastHovered.LastState;
                    lastHovered.Draw(plDrawGraphics, plDraw.BackColor);
                }

                lastHovered = hovered;
            }

            if (hovered == null) return;

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
            else if (hovered is ConnectionZone.Path)
            {
                if (ActiveTool == ActiveToolType.Delete)
                    state = DrawState.Delete;
                else if (ActiveTool == ActiveToolType.Pipe)
                    state = DrawState.Hovered;
                else if (ActiveTool == ActiveToolType.Select)
                {
                    ConnectionZone.Path path = hovered as ConnectionZone.Path;
                    PathMidPointDrawable closest = path.GetClosestMidPointTo(mousePosition);
                    if (closest != null)
                    {
                        plDraw.Cursor = Cursors.SizeAll;
                        state = DrawState.Hovered;
                    }
                    else
                        plDraw.Cursor = Cursors.Arrow;
                }
            }
            if (state == DrawState.None)
                state = DrawState.Normal;

            hovered.DrawState = state;
            hovered.Draw(plDrawGraphics, plDraw.BackColor);
            if (ActiveTool == ActiveToolType.Pipe)
                if (hovered is Element)
                {
                    foreach (ConnectionZone zone in (hovered as Element).ConnectionZones)
                    {
                        zone.Draw(plDrawGraphics, plDraw.BackColor);
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
            ActiveToolType previousTool = ActiveTool;
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
            if (previousTool == ActiveTool) return;
            if (previousTool != ActiveToolType.Pipe && ActiveTool == ActiveToolType.Pipe)
                foreach (Element el in AllElements)
                {
                    foreach (ConnectionZone zone in el.ConnectionZones)
                    {
                        zone.Draw(plDrawGraphics, plDraw.BackColor);
                    }
                }
            else if (previousTool == ActiveToolType.Pipe && ActiveTool != ActiveToolType.Pipe)
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
        }

        void HandleEdit(Drawable d)
        {
            if (d is ConnectionZone.Path)
                ShowEditPath(d as ConnectionZone.Path);
            else if (d is PumpElement)
                ShowEditPump(d as PumpElement);
            else if (d is AdjustableSplitter)
                ShowEditAdjustableSplitter(d as AdjustableSplitter);
        }

        void ShowEditPath(ConnectionZone.Path path)
        {
            foreach (PathMidPointDrawable midPoint in path.UserDefinedMidPoints)
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

        void ShowEditPump(PumpElement pump)
        {

        }

        void ShowEditAdjustableSplitter(AdjustableSplitter splitter)
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
            if (HasElementForPlacementUnder(mousePosition))
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
            if (hovered == null)
            {
                Point intersection;
                ConnectionZone.Path path = FindPathUnder(mousePosition, out intersection);
                if (path != null)
                {
                    PathMidPointDrawable midPoint = path.GetClosestMidPointTo(mousePosition);
                    if (midPoint == null)
                    {
                        path.AddUserMidPoint(intersection);
                    }
                }
                return;
            }

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
                PathStart.Draw(plDrawGraphics, plDraw.BackColor);
                PathEnd.Draw(plDrawGraphics, plDraw.BackColor);
                result.OnCreated += () =>
                {
                    result.Draw(plDrawGraphics, plDraw.BackColor);
                };

                result.OnBeforeAdjusted += () =>
                {
                    result.DrawClear(plDrawGraphics, plDraw.BackColor);
                };

                result.OnAdjusted += () =>
                {
                    result.Draw(plDrawGraphics, plDraw.BackColor);
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
                    if(redraw)
                        zone.Draw(plDrawGraphics, plDraw.BackColor);
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
                        zone.Draw(plDrawGraphics, plDraw.BackColor);
                }
            }
        }
        #endregion
        #region drag
        void plDraw_HandleStopDrag(object sender, MouseEventArgs e)
        {
            if (dragElement == null) return;
            if (HasElementForPlacementUnder(dragElement.Location))
            {
                RevertDrag();
            }
            else
            {
                UndoStack.AddAction(new UndoableActions.MoveElementAction(dragElement, dragStart, dragElement.Location));
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
                else
                {
                    ConnectionZone.Path path = FindPathUnder(mousePosition);
                    if (path != null)
                    {
                        PathMidPointDrawable point = path.GetClosestMidPointTo(mousePosition);
                        if (point != null)
                        {
                            
                        }
                    }
                }
            }
            else
            {
                dragElement.Location = mousePosition;
            }
        }

        void plDraw_HandleMoveDragElement(object sender, MouseEventArgs e)
        {
            if (dragElement == null) return;
            Point location = e.Location;
            if (location.X < 0)
                location.X = 0;
            else if (location.X > plDraw.Width - Element.DefaultSize.X)
                location.X = plDraw.Width - Element.DefaultSize.X;
            if (location.Y < 0)
                location.Y = 0;
            else if (location.Y > plDraw.Height - Element.DefaultSize.Y)
                location.Y = plDraw.Height - Element.DefaultSize.Y;
            
            if (dragElement.Location != location)
            {
                dragElement.DrawClear(plDrawGraphics, plDraw.BackColor);
                dragElement.Location = location;

                foreach (Element q in FindCollisionsForPlacementOfElementUnder(dragElement.Location))
                {
                    if (q == dragElement) continue;
                    q.Draw(plDrawGraphics, plDraw.BackColor);
                }

                dragElement.OnlyDraw(plDrawGraphics, plDraw.BackColor);

                RefreshDragElementPathCollisions();
                RefreshConnections();
            }
        }

        void plDraw_HandleMoveDragMidPoint(object sender, MouseEventArgs e)
        {
            if (dragMidPoint == null) return;
            Point location = e.Location;
            if (location.X < 0)
                location.X = 0;
            else if (location.X > plDraw.Width - Element.DefaultSize.X)
                location.X = plDraw.Width - Element.DefaultSize.X;
            if (location.Y < 0)
                location.Y = 0;
            else if (location.Y > plDraw.Height - Element.DefaultSize.Y)
                location.Y = plDraw.Height - Element.DefaultSize.Y;

            if (dragMidPoint.Location != location)
            {
                Point oldLocation = dragMidPoint.Location;
                dragMidPoint.DrawClear(plDrawGraphics, plDraw.BackColor);
                dragMidPoint.Location = location;

                foreach (Element q in FindCollisionsForPlacementOfElementUnder(dragMidPoint.Location))
                {
                    dragMidPoint.Location = oldLocation;
                    q.Draw(plDrawGraphics, plDraw.BackColor);
                }

                dragMidPoint.OnlyDraw(plDrawGraphics, plDraw.BackColor);

                RefreshDragMidPointPathCollisions();
                RefreshConnections();
            }
        }

        private void RefreshDragMidPointPathCollisions()
        {
            dragMidPoint.Path.Adjust();
        }

        private void RefreshDragElementPathCollisions()
        {
            if (dragElement.Connections.Count() == 0) return;
            foreach (Element element in AllElements)
            {
                if (element == dragElement) continue;
                bool redraw = false;
                foreach (ConnectionZone.Path path in dragElement.Connections)
                {
                    Point previous = path.From;

                    foreach (Point point in path.PathPoints)
                    {
                        if (point == previous) continue;
                        if (
                            Collision.Intersects(previous, point, element.A, element.B) ||
                            Collision.Intersects(previous, point, element.B, element.C) ||
                            Collision.Intersects(previous, point, element.C, element.D) ||
                            Collision.Intersects(previous, point, element.D, element.A)
                            )
                        {
                            element.Draw(plDrawGraphics, plDraw.BackColor);
                            break;
                        }
                        previous = point;
                    }
                    if (redraw) break;
                }
            }
        }

        private void RevertDrag()
        {
            if (dragMidPoint != null)
            {
                dragMidPoint.DrawClear(plDrawGraphics, plDraw.BackColor);
                dragMidPoint.Path.DrawClear(plDrawGraphics, plDraw.BackColor);

                foreach (Element q in FindCollisionsForPlacementOfElementUnder(dragElement.Location))
                {
                    if (q == dragElement) continue;
                    q.Draw(plDrawGraphics, plDraw.BackColor);
                }

                dragMidPoint.Location = dragStart;
                oldDragElementPlaceholder.Visible = false;
                dragMidPoint = null;
                //plDraw.Invalidate();
                RefreshConnections();
            }
            else if (dragElement != null)
            {
                dragElement.DrawClear(plDrawGraphics, plDraw.BackColor);
                foreach (ConnectionZone.Path path in dragElement.Connections)
                {
                    path.DrawClear(plDrawGraphics, plDraw.BackColor);
                }

                foreach (Element q in FindCollisionsForPlacementOfElementUnder(dragElement.Location))
                {
                    if (q == dragElement) continue;
                    q.Draw(plDrawGraphics, plDraw.BackColor);
                }

                dragElement.Location = dragStart;
                oldDragElementPlaceholder.Visible = false;
                dragElement = null;
                //plDraw.Invalidate();
                RefreshConnections();
            }
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
                {
                    ConnectionZone.Path path = FindPathUnder(mousePosition);
                    bool foundPoint = false;
                    if (path != null)
                        if (path.GetClosestMidPointTo(mousePosition) != null)
                            foundPoint = true;
                    if(!foundPoint)
                        this.Cursor = Cursors.Arrow;
                }
            }

            mousePosition = evnt.Location;
            if (mousePosition.X < 0)
                mousePosition.X = 0;
            else if (mousePosition.X > plDraw.Width - iconBelowCursor.Width)
                mousePosition.X = plDraw.Width - iconBelowCursor.Height;
            if (mousePosition.Y < 0)
                mousePosition.Y = 0;
            else if (mousePosition.Y > plDraw.Height - iconBelowCursor.Height)
                mousePosition.Y = plDraw.Height - iconBelowCursor.Height;

            if (ActiveTool == ActiveToolType.None)
            {
                iconBelowCursor.Visible = false;
                iconBelowCursor.BackColor = Color.Bisque;
            }
            else if (ActiveTool == ActiveToolType.Delete)
            {
                iconBelowCursor.Visible = false;
                if (HasElementForPlacementUnder(mousePosition))
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
                Point point = mousePosition;
                point.Offset(plDraw.Location);
                point.Offset(16, 16);
                this.iconBelowCursor.Location = point;

                IEnumerable<Element> collisionsForPlacement = FindCollisionsForPlacementOfElementUnder(mousePosition);
                if (collisionsForPlacement.Any())
                {
                    iconBelowCursor.BackColor = Color.Red;
                    foreach (Element collision in collisionsForPlacement)
                    {
                        if (collision.DrawState == DrawState.Blocking) continue;
                        collision.DrawState = DrawState.Blocking;
                        collision.Draw(plDrawGraphics, plDraw.BackColor);
                    }
                }
                else
                {
                    iconBelowCursor.BackColor = Color.Green;
                }

                foreach (Element collision in lastCollisionsForPlacement)
                {
                    if (collisionsForPlacement.Contains(collision)) continue;
                    if (collision.DrawState == DrawState.Normal) continue;
                    collision.DrawState = DrawState.Normal;
                    collision.Draw(plDrawGraphics, plDraw.BackColor);
                }
                lastCollisionsForPlacement = collisionsForPlacement;

                iconBelowCursor.BringToFront();
            }
        }

        private IEnumerable<Element> lastCollisionsForPlacement = new Element[0];

        void plDraw_Redraw(object sender, PaintEventArgs e)
        {
            foreach (var item in AllElements)
            {
                item.Draw(e.Graphics, plDraw.BackColor);
                if (ActiveTool == ActiveToolType.Pipe)
                    foreach (var con in item.ConnectionZones)
                    {
                        con.Draw(e.Graphics, plDraw.BackColor);
                    }
            }

            foreach (ConnectionZone.Path path in new List<ConnectionZone.Path>(AllPaths))
            {
                path.Draw(e.Graphics, plDraw.BackColor);
            }
        }

        private bool LineIntersectsAt(Point a, Point b, Point mouse, out Point intersection)
        {
            int crossLength = 2;

            Point crossH1 = new Point(mouse.X, mouse.Y - crossLength);
            Point crossH2 = new Point(mouse.X, mouse.Y + crossLength);
            Point diagTL = new Point(mouse.X - crossLength, mouse.Y - crossLength);
            Point diagBR = new Point(mouse.X + crossLength, mouse.Y + crossLength);

            Point diagBL = new Point(mouse.X - crossLength, mouse.Y + crossLength);
            Point diagTR = new Point(mouse.X + crossLength, mouse.Y - crossLength);

            Point crossV1 = new Point(mouse.X - crossLength, mouse.Y);
            Point crossV2 = new Point(mouse.X + crossLength, mouse.Y);

            return (Collision.Intersects(a, b, crossH1, crossH2, out intersection) || Collision.Intersects(a, b, crossV1, crossV2, out intersection) ||
                    Collision.Intersects(a, b, diagTL, diagBR, out intersection) || Collision.Intersects(a, b, diagBL, diagTR, out intersection));
        }

        private bool LineIntersectsAt(Point a, Point b, Point mouse, int lineWidth = 1)
        {
            Point p;
            return LineIntersectsAt(a, b, mouse, out p);
        }

        #region AddElement Remove

        void RemoveElement(Element e)
        {
            UndoStack.AddAction(new UndoableActions.RemoveElementAction(e));

            if (lastHovered == e) lastHovered = null;
        }

        void AddElement(Element e, Point position)
        {
            e.X = position.X;
            e.Y = position.Y;

            UndoStack.AddAction(new UndoableActions.AddElementAction(e));
        }

        static object refreshLock = new object();

        private void RefreshConnections(Element e = null)
        {
            foreach (Element element in AllElements)
            {
                if (e != null)
                    if (e == element) continue;

                foreach (ConnectionZone.Path p in element.Connections)
                {
                    ConnectionZone.Path path = p;
                    List<Point> current = new List<Point>(path.PreviousPointsToGoThrough);
                    path.Adjust(false, onDone: () =>
                    {
                        this.Invoke(new Action(() =>
                        {
                            path.To.Parent.DrawClear(plDrawGraphics, plDraw.BackColor);
                            path.To.Parent.Draw(plDrawGraphics, plDraw.BackColor);
                            path.From.Parent.DrawClear(plDrawGraphics, plDraw.BackColor);
                            path.From.Parent.Draw(plDrawGraphics, plDraw.BackColor);
                            List<Point> newPoints = new List<Point>(path.PreviousPointsToGoThrough);
                            path.PreviousPointsToGoThrough = current;
                            path.DrawClear(plDrawGraphics, plDraw.BackColor);
                            path.PreviousPointsToGoThrough = newPoints;
                            path.DrawClear(plDrawGraphics, plDraw.BackColor);
                            path.Draw(plDrawGraphics, plDraw.BackColor);

                            foreach (ConnectionZone.Path otherPath in AllPaths)
                            {
                                if (otherPath == path) continue;
                                if (Collision.Intersects(path.From, path.To, otherPath.From, otherPath.To))
                                {
                                    otherPath.Draw(plDrawGraphics, plDraw.BackColor);
                                }
                            }
                            List<Point> points = new List<Point>(path.PathPoints);
                            foreach (Element el in AllElements)
                            {
                                foreach (Point pp in points)
                                {
                                    if (pp == el.A || pp == el.B || pp == el.C || pp == el.D)
                                    {
                                        el.OnlyDraw(plDrawGraphics, plDraw.BackColor);
                                        break;
                                    }
                                }
                            }
                        }
                        ));
                    });
                }
            }
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
            }

            if (ActiveTool == ActiveToolType.Pipe)
            {
                if (PathStart != null)
                {
                    ResetBlockedForSameFlowZones(true);
                    PathStart.DrawState = DrawState.Normal;
                }
                
                PathStart = null;
                return;
            }

            RightClickOptions options = ~RightClickOptions.Remove;
            rightClickMousePosition = mousePosition;

            if (HasElementForPlacementUnder(rightClickMousePosition))
            {
                Element e = FindElementUnder(rightClickMousePosition);
                if (e != null)
                {
                    options = RightClickOptions.Remove;
                    if (e is AdjustableSplitter || e is PumpElement)
                        options |= RightClickOptions.Edit;
                }
            }
            else if (FindPathUnder(rightClickMousePosition) != null)
            {
                HandleEdit(FindPathUnder(rightClickMousePosition));
                return;
            }

            if (rightClickPanel == null)
            {
                rightClickPanel = new Panel();
                rightClickPanel.LostFocus += (x, y) => rightClickPanel.Visible = false;
                plDraw.Controls.Add(rightClickPanel);

                rightClickPanel.Width = 100;

                rightClickPanel.AddButton("Edit", (x, y) => 
                {
                    HandleEdit(FindElementUnder(rightClickMousePosition));
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
                    if (LineIntersectsAt(lineStart, lineEnd, mousePosition, path.Width))
                        return path;
                }
            }
            return null;
        }

        private ConnectionZone.Path FindPathUnder(Point mousePosition, out Point intersection)
        {
            intersection = new Point(-1, -1);
            foreach (ConnectionZone.Path path in AllPaths)
            {
                for (int i = 0; i < path.PathPoints.Count - 1; i++)
                {
                    Point lineStart = path.PathPoints[i];
                    Point lineEnd = path.PathPoints[i + 1];
                    if (LineIntersectsAt(lineStart, lineEnd, mousePosition, out intersection))
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

        private IEnumerable<Element> FindCollisionsForPlacementOfElementUnder(Point mousePosition)
        {
            return AllElements.Where(q =>
            {
                if (q == dragElement) return false;
                Point position = mousePosition;

                if (q.X - q.Width <= position.X && q.X + q.Width >= position.X)
                    if (q.Y - q.Height <= position.Y && q.Y + q.Height >= position.Y)
                        return true;
                return false;
            });
        }

        private bool HasElementForPlacementUnder(Point mousePosition)
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

