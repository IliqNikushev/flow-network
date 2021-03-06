﻿using System;
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
        public enum ActiveToolType
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

        public float FlowCapacity { get { return (float)this.nudMaxFlow.Value; } set { this.nudMaxFlow.Value = (decimal)value; } }
        public float InFlow { get { return float.Parse(this.tbInFlow.Text); } set { this.tbInFlow.Text = value.ToString(); } }
        public float OutFlow { get { return float.Parse(this.tbOutFlow.Text); } set { this.tbOutFlow.Text = value.ToString(); } }

        public Main()
        {
            this.pumpEditPopup.OnFlowAltered += () =>
                {
                    RefreshPipeline();
                    if (InFlow > FlowCapacity)
                    {
                        float delta = FlowCapacity - InFlow;
                        if (this.pumpEditPopup.Pump.Flow - delta >= 0)
                            this.pumpEditPopup.AdjustFlow(delta);
                        else
                        {
                            //todo something wrong with
                        }
                    }
                };
            this.adjustableSplitterEditPopup.OnFlowAltered += () => RefreshPipeline();
            Resources.Initialize();

            InitializeComponent();

            foreach (Control control in this.Controls)
            {
                if (control != plDraw)
                {
                    control.Click += (x, y) => plDraw_HandleLoseFocus(x, y);
                    control.MouseMove += (x, y) =>
                        {
                            mousePosition = new Point(-1, -1);
                            plDraw_HandleLoseFocus(this, null);
                        };
                }
            }

            this.nudMaxFlow.ValueChanged += (x, y) => RefreshPipeline();
            this.nudSafetyLimit.ValueChanged += (x, y) => RefreshPipeline();

            this.nudMaxFlow.Maximum = decimal.MaxValue;
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
            this.MouseMove += (x, y) =>
            {
                mousePosition = new Point(-1, -1);
                plDraw_HandleLoseFocus(this, null);
            };

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

                    RefreshFlow();

                    if (action is UndoableActions.RemoveConnectionAction)
                    {
                        plDraw.Invalidate();
                    }
                    else
                    {
                        if (action.Connection.IsNew) return;
                        action.Connection.To.Draw(this.plDrawGraphics, this.plDraw.BackColor);
                        action.Connection.From.Draw(this.plDrawGraphics, this.plDraw.BackColor);
                        action.Connection.Draw(this.plDrawGraphics, this.plDraw.BackColor);
                    }
                }

                else if (lastAction is UndoableActions.AddElementAction)
                {
                    UndoableActions.AddElementAction action = lastAction as UndoableActions.AddElementAction;

                    RefreshPipeline();

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
                else if (lastAction is UndoableActions.AddMidPointAction)
                    plDraw.Invalidate();

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

                    RefreshPipeline();

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

                    RefreshPipeline();

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
                else if (lastAction is UndoableActions.AddMidPointAction)
                    plDraw.Invalidate();

                else if (lastAction is UndoableActions.MoveElementAction)
                {
                    RefreshConnections();
                    plDraw.Invalidate();
                }
            };
        }

        private void RefreshPipeline()
        {
            RefreshFlow();
            RefreshConnections();
        }


        void plDraw_HandleLoseFocus(object sender, EventArgs e)
        {
            if (sender == plDraw)
            {
                if(mousePosition.X != -1)
                    return;
            }
            iconBelowCursor.Visible = false;
            if (rightClickPanel != null)
                rightClickPanel.Visible = false;
            if (adjustableSplitterEditPopup != null)
                adjustableSplitterEditPopup.Visible = false;
            if (pumpEditPopup != null)
                pumpEditPopup.Visible = false;
            if (pipeEditPopup != null)
                pipeEditPopup.Visible = false;
        }

        void plDraw_HandleGainFocus(object sender, EventArgs e)
        {
            if (ActiveTool != ActiveToolType.Delete)
                iconBelowCursor.Visible = true;
        }

        void plDraw_HandleMouseMove(object sender, MouseEventArgs e)
        {
            mousePosition = e.Location;
            if (mousePosition.X < 0)
                mousePosition.X = 0;
            else if (mousePosition.X > plDraw.Width - iconBelowCursor.Width)
                mousePosition.X = plDraw.Width - iconBelowCursor.Height;
            if (mousePosition.Y < 0)
                mousePosition.Y = 0;
            else if (mousePosition.Y > plDraw.Height - iconBelowCursor.Height)
                mousePosition.Y = plDraw.Height - iconBelowCursor.Height;

            plDraw_HandleHover(sender, e);
            plDraw_HandleDynamicIcon(sender, e);
            if (dragElement != null)
            {
                plDraw_HandleMoveDragElement(sender, e);
                plDraw.Invalidate();
            }
            else if (dragMidPoint != null)
            {
                plDraw_HandleMoveDragMidPoint(sender, e);
                plDraw.Invalidate();
            }
        }

        Drawable lastHovered = null;
        Drawable currentHovered = null;

        void plDraw_HandleHover(object sender, MouseEventArgs e)
        {
            if (dragMidPoint != null)
            {
                dragMidPoint.Path.DrawState = DrawState.Hovered;
                return;
            }

            currentHovered = null;
            DrawState state = DrawState.None;
            if (ActiveTool == ActiveToolType.Pipe)
                currentHovered = FindConnectionZoneUnder(mousePosition);
            else
                if (currentHovered == null)
                    currentHovered = FindElementUnder(mousePosition);
            if (currentHovered == null)
                currentHovered = FindPathUnder(mousePosition);

            if (currentHovered is ConnectionZone.Path)
            {
                ConnectionZone.Path path = currentHovered as ConnectionZone.Path;
                PathMidPointDrawable closest = path.GetClosestMidPointTo(mousePosition);
                if (closest != null)
                {
                    currentHovered = closest;
                }
            }

            if (currentHovered == lastHovered)
                return;

            if (lastHovered != null)
            {
                if (lastHovered is ConnectionZone)
                {
                    ConnectionZone z = lastHovered as ConnectionZone;
                    if (z == PathStart)
                        z.DrawState = DrawState.Active;
                    else if (z.IsConnected || z.FlowIsSameAs(PathStart) || (PathStart != null && ZoneParentIsUsedInPath(z)))
                        z.DrawState = DrawState.Blocking;
                    else
                        z.DrawState = DrawState.Normal;
                }
                else
                    if (lastHovered is ConnectionZone.Path)
                    {
                        ConnectionZone.Path path = lastHovered as ConnectionZone.Path;
                        if (path.Flow > path.MaxFlow)
                            path.DrawState = DrawState.Blocking;
                        else if (path.Flow > (float)nudSafetyLimit.Value)
                            path.DrawState = DrawState.Delete;
                        else
                            lastHovered.DrawState = DrawState.Normal;
                    }
                    else if (lastHovered is Element)
                        lastHovered.DrawState = DrawState.Normal;
                    else
                        lastHovered.DrawState = lastHovered.LastState;
                lastHovered.Draw(plDrawGraphics, plDraw.BackColor);
            }

            lastHovered = currentHovered;

            if (currentHovered == null) return;

            if (currentHovered is ConnectionZone)
            {
                if (currentHovered == PathStart)
                    state = DrawState.Active;
                else if ((currentHovered as ConnectionZone).IsConnected)
                    state = DrawState.Blocking;
                else if (PathStart != null)
                {
                    if (PathStart.FlowIsSameAs(currentHovered as ConnectionZone))
                        state = DrawState.Blocking;
                    else
                        state = DrawState.Hovered;
                }
                else
                    state = DrawState.Hovered;
            }
            else if (currentHovered is Element)
            {
                if (ActiveTool == ActiveToolType.Delete)
                    state = DrawState.Delete;
                else if (ActiveTool == ActiveToolType.Select)
                    if (dragElement == null)
                        state = DrawState.Hovered;
                    else
                        state = DrawState.Blocking;
                else
                    state = DrawState.Blocking;
            }
            else if (currentHovered is ConnectionZone.Path)
            {
                ConnectionZone.Path path = currentHovered as ConnectionZone.Path;
                if (path.Flow > path.MaxFlow)
                    state = DrawState.Blocking;
                else if (path.Flow > (float)nudSafetyLimit.Value)
                    state = DrawState.Delete;
                else if (ActiveTool == ActiveToolType.Delete)
                    state = DrawState.Delete;
                else if (ActiveTool == ActiveToolType.Pipe)
                    state = DrawState.Hovered;
                else if (ActiveTool == ActiveToolType.Select)
                    state = DrawState.Normal;
            }
            else if (currentHovered is PathMidPointDrawable)
            {
                if (ActiveTool == ActiveToolType.Delete)
                    state = DrawState.Delete;
                else if (ActiveTool == ActiveToolType.Pipe)
                    state = DrawState.Blocking;
                else if (ActiveTool == ActiveToolType.Select)
                {
                    plDraw.Cursor = Cursors.SizeAll;
                    state = DrawState.Hovered;
                }
                if (ActiveTool != ActiveToolType.Select)
                    plDraw.Cursor = Cursors.Arrow;
            }
            if (state == DrawState.None)
                state = DrawState.Normal;

            currentHovered.DrawState = state;
            currentHovered.Draw(plDrawGraphics, plDraw.BackColor);
            if (ActiveTool == ActiveToolType.Pipe)
                if (currentHovered is Element)
                {
                    foreach (ConnectionZone zone in (currentHovered as Element).ConnectionZones)
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
            if (infoForm != null)
                SetInfo();
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
            if (activePopup != null)
                if (activePopup.Visible)
                    activePopup.Visible = false;

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

        private CustomComponents.PipeEditPopup pipeEditPopup = new CustomComponents.PipeEditPopup(null);
        private CustomComponents.PumpEditPopup pumpEditPopup = new CustomComponents.PumpEditPopup(null);
        private CustomComponents.AdjustableSplitterEditPopup adjustableSplitterEditPopup = new CustomComponents.AdjustableSplitterEditPopup(null);

        private CustomComponents.EditPopup activePopup;

        void HandleSelectToolClick()
        {
            if (dragElement != null) return;
        }

        void HandleEdit(Drawable d)
        {
            if (d is ConnectionZone.Path)
                ShowEditPopup(d, pipeEditPopup);
            else if (d is PumpElement)
                ShowEditPopup(d, pumpEditPopup);
            else if (d is AdjustableSplitter)
                ShowEditPopup(d, adjustableSplitterEditPopup);
        }

        void ShowEditPopup(Object value, CustomComponents.EditPopup popup)
        {
            if (activePopup != null)
                if (activePopup != popup)
                    activePopup.Value = null;

            activePopup = popup;
            activePopup.Value = value;

            activePopup.Visible = true;

            activePopup.Location = mousePosition;

            if (activePopup.Parent != this)
                this.plDraw.Controls.Add(activePopup);
        }

        void HandleDeleteToolClick()
        {
            if (currentHovered == null) return;
            else if (currentHovered is Element)
                RemoveElement(currentHovered as Element);
            else if (currentHovered is ConnectionZone.Path)
                RemoveConnection(currentHovered as ConnectionZone.Path);
            else if (currentHovered is PathMidPointDrawable)
                RemoveMidPoint(currentHovered as PathMidPointDrawable);
            else if (currentHovered is ConnectionZone) return;
            else
                throw new NotImplementedException("Unknown hovered, " + currentHovered.GetType().Name);
            if (lastHovered == currentHovered) lastHovered = null;
            currentHovered = null;
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

        bool ElementIsUsedInPath(Element element, HashSet<Element> processed)
        {
            if (processed.Contains(element)) return true;
            processed.Add(element);
            foreach (ConnectionZone zone in element.ConnectionZones)
            {
                if (zone.IsOutFlow)
                    if (zone.ConnectedZone != null)
                        if (ElementIsUsedInPath(zone.ConnectedZone.Parent, processed))
                            return true;
            }
            return false;
        }

        bool ZoneParentIsUsedInPath(ConnectionZone zone)
        {
            return ElementIsUsedInPath(zone.Parent, new HashSet<Element>(new Element[] { PathStart.Parent }));
        }

        bool PathResultsInCirular()
        {
            return ZoneParentIsUsedInPath(PathEnd);
        }

        void HandlePipeToolClick()
        {
            ConnectionZone hovered = currentHovered as ConnectionZone;
            if (hovered == null)
            {
                if (currentHovered is PathMidPointDrawable) return;

                ConnectionZone.Path path = currentHovered as ConnectionZone.Path;
                if (path != null)
                {
                    PathMidPointDrawable midPoint = path.GetClosestMidPointTo(mousePosition);
                    if (midPoint == null)
                    {
                        int index = path.AddUserMidPoint(mousePosition);
                        UndoStack.AddAction(new UndoableActions.AddMidPointAction(path.UserDefinedMidPoints[index]));
                    }
                }
                return;
            }

            if (PathStart != null) if (PathStart.Parent == hovered.Parent) return;
            if (hovered.IsConnected) return;

            if (PathStart == null) PathStart = hovered;
            else PathEnd = hovered;

            if (PathStart != null)
                SetBlockedForSameFlowZones(true);

            if (PathEnd == null)
            {
                return;
            }
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
            if (PathResultsInCirular())
            {
                PathEnd = null;
                return;
            }

            if (PathEnd.IsOutFlow)
            {
                PathEnd = PathStart;
                PathStart = hovered;
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
                    if (redraw)
                        zone.Draw(plDrawGraphics, plDraw.BackColor);
                }
            }
        }

        private void SetBlockedForSameFlowZones(bool redraw)
        {
            foreach (Element e in AllElements)
            {
                bool isUsed = ElementIsUsedInPath(e, new HashSet<Element>(new Element[] { PathStart.Parent }));
                foreach (ConnectionZone zone in e.ConnectionZones)
                {
                    if (zone == PathStart) continue;
                    if (isUsed || zone.FlowIsSameAs(PathStart))
                        zone.DrawState = DrawState.Blocking;
                    if (redraw)
                        zone.Draw(plDrawGraphics, plDraw.BackColor);
                }
            }
        }
        #endregion
        #region drag

        void HandleStopDrag(IconDrawable drawable)
        {
            if (drawable == null) return;
            if (FindElementUnder(mousePosition) != null || (HasElementForPlacementUnder(mousePosition) && drawable is Element) ||
                (drawable is Element && AllPaths.Where(x => x.UserDefinedMidPoints.Where(y => dragElement.Contains(y)).Any()).Any()))
            {
                RevertDrag();
            }
            else
            {
                UndoStack.AddAction(new UndoableActions.MoveElementAction(drawable, dragStart, mousePosition));
            }
            oldDragElementPlaceholder.Visible = false;
            if (drawable is Element) dragElement = null;
            else if (drawable is PathMidPointDrawable) dragMidPoint = null;
            plDraw.Cursor = Cursors.Arrow;
        }

        void plDraw_HandleStopDrag(object sender, MouseEventArgs e)
        {
            if (dragElement != null)
                HandleStopDrag(dragElement);
            else if (dragMidPoint != null)
                HandleStopDrag(dragMidPoint);
        }

        void plDraw_HandleStartDrag(object sender, MouseEventArgs e)
        {
            if (ActiveTool != ActiveToolType.Select)
                return;
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            if (dragElement == null && dragMidPoint == null)
            {
                dragElement = currentHovered as Element;
                if (dragElement != null)
                {
                    dragStart = dragElement.Location;
                    oldDragElementPlaceholder.Visible = true;
                    oldDragElementPlaceholder.Location = dragStart;
                    oldDragElementPlaceholder.Image = dragElement.Icon;
                }
                else
                {
                    PathMidPointDrawable point = currentHovered as PathMidPointDrawable;
                    if (point != null)
                    {
                        dragStart = point.Location;
                        dragMidPoint = point;
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
            Point location = mousePosition;

            if (dragElement.Location != location)
            {
                dragElement.Location = location;

                RefreshConnections();
            }
        }

        Point oldMidPointLocation = new Point(0, 0);

        void plDraw_HandleMoveDragMidPoint(object sender, MouseEventArgs e)
        {
            if (dragMidPoint == null) return;
            Point location = mousePosition;

            if (dragMidPoint.Location != location)
            {
                dragMidPoint.Location = location;

                Element found = FindElementUnder(location);
                if (found != null)
                {
                    dragMidPoint.Location = oldMidPointLocation;
                }

                RefreshDragMidPointPathCollisions();
                RefreshConnections();

                oldMidPointLocation = location;
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
                dragMidPoint.Location = dragStart;
                oldDragElementPlaceholder.Visible = false;

                dragMidPoint.Path.Adjust(onDone: () => plDraw.Invalidate());
                dragMidPoint = null;
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
            if (ActiveTool == ActiveToolType.Pipe)
            {
                this.iconBelowCursor.Visible = false;
                this.Cursor = Cursors.Arrow;
                return;
            }
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
                    if (!foundPoint)
                        this.Cursor = Cursors.Arrow;
                }
            }

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
                bool checkForCollisions = ActiveTool != ActiveToolType.Delete && ActiveTool != ActiveToolType.Pipe;

                IEnumerable<Element> collisionsForPlacement = new Element[0];
                if (checkForCollisions)
                {
                    if (dragMidPoint != null)
                        collisionsForPlacement = new Element[] { FindElementUnder(mousePosition) }.Where(x => x != null);
                    else
                        if (ActiveTool == ActiveToolType.Select && dragElement != null)
                            collisionsForPlacement = FindCollisionsForPlacementOfElementUnder(mousePosition);
                        else if (ActiveTool != ActiveToolType.Select)
                            collisionsForPlacement = FindCollisionsForPlacementOfElementUnder(mousePosition);
                }
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
            foreach (Element element in AllElements)
            {
                if (element == dragElement)
                    continue;
                else
                    element.Draw(e.Graphics, plDraw.BackColor);
                if (ActiveTool == ActiveToolType.Pipe)
                    foreach (ConnectionZone con in element.ConnectionZones)
                    {
                        con.Draw(e.Graphics, plDraw.BackColor);
                    }
            }
            if (dragElement != null)
                dragElement.OnlyDraw(e.Graphics, plDraw.BackColor);
            foreach (ConnectionZone.Path path in new List<ConnectionZone.Path>(AllPaths))
            {
                path.Draw(e.Graphics, plDraw.BackColor);
            }
        }

        private bool LineIntersectsAt(Point a, Point b, Point mouse, int lineWidth = 1)
        {
            Point p;
            return LineIntersectsAt(a, b, mouse, out p);
        }

        private bool LineIntersectsAt(Point a, Point b, Point mouse, out Point p, int lineWidth = 1)
        {
            return Collision.PointIsOnLine(a, b, mouse, out p);
        }

        #region AddElement Remove

        void RemoveElement(Element e)
        {
            if (e == lastHovered) lastHovered = null;
            if (e == currentHovered) currentHovered = null;

            UndoStack.AddAction(new UndoableActions.RemoveElementAction(e));
        }

        void RemoveConnection(ConnectionZone.Path e)
        {
            if (e == lastHovered) lastHovered = null;
            if (e == currentHovered) currentHovered = null;
            UndoStack.AddAction(new UndoableActions.RemoveConnectionAction(e));
        }

        void RemoveMidPoint(PathMidPointDrawable e)
        {
            if (e == lastHovered) lastHovered = null;
            if (e == currentHovered) currentHovered = null;
            UndoStack.AddAction(new UndoableActions.RemoveMidPointAction(e));
        }

        void AddElement(Element e, Point position)
        {
            e.X = position.X;
            e.Y = position.Y;

            UndoStack.AddAction(new UndoableActions.AddElementAction(e));
        }

        private void RefreshFlow()
        {
            InFlow = 0;
            OutFlow = 0;
            float inFlow = 0;
            float outFlow = 0;
            foreach (Element e in AllElements)
                if (e is PumpElement)
                    inFlow += (e as PumpElement).Flow;
                else if (e is SinkElement)
                    outFlow += (e as SinkElement).Flow;
            InFlow = inFlow;
            OutFlow = outFlow;

            foreach (ConnectionZone.Path path in AllPaths)
            {
                if (path.Flow > path.MaxFlow)
                {
                    path.DrawState = DrawState.Blocking;
                }
                else if (path.Flow > (float)nudSafetyLimit.Value)
                {
                    path.DrawState = DrawState.Delete;
                }
            }
            //todo conflict with hover?
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

            Element e = FindElementUnder(rightClickMousePosition);
            if (e != null)
            {
                options = RightClickOptions.Remove;
                if (e is AdjustableSplitter || e is PumpElement)
                    options |= RightClickOptions.Edit;
            }
            else if (FindPathUnder(rightClickMousePosition) != null)
            {
                HandleEdit(FindPathUnder(rightClickMousePosition));
                return;
            }

            if (rightClickPanel == null)
            {
                rightClickPanel = new Panel();
                plDraw.Controls.Add(rightClickPanel);

                rightClickPanel.Width = 100;

                rightClickPanel.AddButton("Edit", (x, y) =>
                {
                    HandleEdit(FindElementUnder(rightClickMousePosition));
                }).Name = "edit";

                rightClickPanel.AddButton("Remove", (x, y) =>
                {
                    Element el = FindElementUnder(rightClickMousePosition);
                    if (el == null) return;
                    else
                    {
                        RemoveElement(el);
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

            if (!options.HasFlag(RightClickOptions.Remove))
            {
                if (HasElementForPlacementUnder(rightClickMousePosition))
                {
                    rightClickPanel.Visible = false;
                    return;
                }
            }

            if (options.HasFlag(RightClickOptions.Edit))
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
            PromptForSave();
            ClearPipeline();
        }

        void PromptForSave()
        {
            if (UndoStack.CanUndo)
            {
                MessageBoxButtons button = MessageBoxButtons.YesNoCancel;
                DialogResult dr = MessageBox.Show("anything need to be saved?", "new", button);
                if (dr == DialogResult.Yes)
                {
                    btnSave_Click(null, null);
                }
            }
        }

        void ClearPipeline()
        {
            AllPaths.Clear();
            AllElements.Clear();
            plDraw.Invalidate();
            UndoStack.Clear();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            PromptForSave();
            ClearPipeline();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            openFileDialog.Filter = "pipeline file(*.pipelane)|*.pipelane";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string nextLine;
                string[] lineSplit;

                while ((nextLine = sr.ReadLine()) != null)
                {
                    Element load = null;
                    Console.WriteLine(nextLine);
                    {
                        lineSplit = nextLine.Split(',');

                        if (lineSplit[0] == typeof(SinkElement).Name)
                        {
                            load = new SinkElement();
                        }
                        else if (lineSplit[0] == typeof(PumpElement).Name)
                        {
                            load = new PumpElement();
                            (load as PumpElement).Flow = int.Parse(lineSplit[3]);
                        }
                        else if (lineSplit[0] == typeof(MergerElement).Name)
                        {
                            load = new MergerElement();
                        }
                        else if (lineSplit[0] == typeof(SplitterElement).Name)
                        {
                            load = new SplitterElement();
                        }
                        else if (lineSplit[0] == typeof(AdjustableSplitter).Name)
                        {
                            load = new AdjustableSplitter();
                        }
                        else if (lineSplit[0] == typeof(ConnectionZone.Path).Name)
                        {
                            Point from = new Point(int.Parse(lineSplit[1]), int.Parse(lineSplit[2]));
                            Point to = new Point(int.Parse(lineSplit[3]), int.Parse(lineSplit[4]));
                            ConnectionZone From = FindConnectionZoneUnder(from);
                            ConnectionZone To = FindConnectionZoneUnder(to);
                            ConnectionZone.Path pathload = new ConnectionZone.Path(From, To);

                            pathload.OnCreated += () =>
                            {
                                pathload.Draw(plDrawGraphics, plDraw.BackColor);
                            };

                            pathload.OnBeforeAdjusted += () =>
                            {
                                pathload.DrawClear(plDrawGraphics, plDraw.BackColor);
                            };

                            pathload.OnAdjusted += () =>
                            {
                                pathload.Draw(plDrawGraphics, plDraw.BackColor);
                            };

                            pathload.MaxFlow = int.Parse(lineSplit[5]);
                            pathload.AddToSystem();
                            for (int i = 6; i < lineSplit.Count() - 1; i = i + 2)
                            {
                                PathMidPointDrawable midpoint = new PathMidPointDrawable(int.Parse(lineSplit[i]), int.Parse(lineSplit[i + 1]), pathload);
                                pathload.UserDefinedMidPoints.Add(midpoint);
                            }
                            pathload.Adjust();
                            continue;
                        }
                        else if (lineSplit[0] == "Max flow")
                        {
                            this.FlowCapacity = float.Parse(lineSplit[1]);
                            continue;
                        }
                        else if (lineSplit[0] == "Safety Limit")
                        {
                            this.nudSafetyLimit.Value = (decimal)float.Parse(lineSplit[1]);
                            continue;
                        }
                        load.X = int.Parse(lineSplit[1]);
                        load.Y = int.Parse(lineSplit[2]);
                        AllElements.Add(load);
                    }
                }
                RefreshPipeline();
                plDraw.Invalidate();
                fs.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
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
                            if (item is PumpElement)
                            {
                                string flow = (item as PumpElement).Flow.ToString();
                                sw.WriteLine((item.GetType().Name + "," + x + "," + y + "," + flow));
                            }
                            else sw.WriteLine((item.GetType().Name + "," + x + "," + y));
                        }
                        foreach (var connection in AllPaths)
                        {
                            string a = connection.From.Location.X.ToString();
                            string b = connection.From.Location.Y.ToString();
                            string c = connection.To.Location.X.ToString();
                            string d = connection.To.Location.Y.ToString();
                            string maxflow = connection.MaxFlow.ToString();
                            string text = connection.GetType().Name + "," + a + "," + b + "," + c + "," + d + "," + maxflow;
                            foreach (var midpoint in connection.UserDefinedMidPoints)
                            {
                                string mid_x = midpoint.X.ToString();
                                string mid_y = midpoint.Y.ToString();
                                text += "," + mid_x + "," + mid_y;
                            }
                            sw.WriteLine(text);
                        }
                        string flowcapacity = this.FlowCapacity.ToString();
                        sw.WriteLine("Max flow" + "," + flowcapacity);
                        sw.WriteLine("Safety Limit, " + nudSafetyLimit.Value);
                    }
                }
                myStream.Close();
                MessageBox.Show("Saved");
            }
        }

        InfoForm infoForm;

        private void SetInfo()
        {
            if (infoForm == null) infoForm = new InfoForm();
            else if (infoForm.IsDisposed) infoForm = new InfoForm();
            infoForm.SetTool(ActiveTool);
        }

        private void infoBox_Click(object sender, EventArgs e)
        {
            SetInfo();
            infoForm.Show();
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
            if (c != null)
                button.Top += c.Height;
        }
        if (onClick != null)
            button.Click += onClick;

        panel.Controls.Add(button);

        return button;
    }
}