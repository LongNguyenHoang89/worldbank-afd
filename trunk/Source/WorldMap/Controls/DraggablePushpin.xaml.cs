/*
 * Refer to 
 * http://pietschsoft.com/post/2010/05/30/Draggable-Pushpins-using-Bing-Maps-Silverlight-Control.aspx
 */

using System;
using System.Windows.Input;
using Microsoft.Maps.MapControl;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace WorldMap
{
    public partial class DraggablePushpin : Pushpin
    {
        private bool isDragging = false;

        EventHandler<MapMouseDragEventArgs> ParentMapMousePanHandler;
        MouseButtonEventHandler ParentMapMouseLeftButtonUpHandler;
        MouseEventHandler ParentMapMouseMoveHandler;

        private DateTime _lastMouseDownTime;

        private MapLayer _mapLayer;


        /// <summary>
        /// Event after releasing the pushpin
        /// </summary>
        public event EventHandler Pinned;

        /// <summary>
        /// Event after clicking the pushpin
        /// </summary>
        public event EventHandler Clicked;

        /// <summary>
        /// Title for tooltip
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Tooltip details
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Check if the pushpin is on the map
        /// </summary>
        public bool IsOnMap { get; set; }

        public DraggablePushpin()
            : base()
        {
        }

        public DraggablePushpin(MapLayer map)
            : base()
        {
            Random ra = new Random();
            SolidColorBrush randomColor = new SolidColorBrush(Color.FromArgb(255, (byte)ra.Next(0, 255), (byte)ra.Next(0, 255), (byte)ra.Next(0, 255)));

            this.Background = randomColor;
            this._mapLayer = map;
            this.IsOnMap = false;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this._lastMouseDownTime = DateTime.Now;

            // Check if the Map Event Handlers have been created/attached to the Map
            // If not, then attach them. This is done in the "Pushpin.OnMouseLeftButtonDown"
            // event because we don't know when the Pushpin is added to a Map or MapLayer, but
            // we do konw that when this event is fired the Pushpin will already have been added.

            var parentLayer = _mapLayer as MapLayer;

            if (parentLayer != null)
            {
                var parentMap = parentLayer.ParentMap;
                if (parentMap != null)
                {
                    if (this.ParentMapMousePanHandler == null)
                    {
                        this.ParentMapMousePanHandler = new EventHandler<MapMouseDragEventArgs>(ParentMap_MousePan);
                        parentMap.MousePan += this.ParentMapMousePanHandler;
                    }
                    if (this.ParentMapMouseLeftButtonUpHandler == null)
                    {
                        this.ParentMapMouseLeftButtonUpHandler = new MouseButtonEventHandler(ParentMap_MouseLeftButtonUp);
                        parentMap.MouseLeftButtonUp += this.ParentMapMouseLeftButtonUpHandler;
                    }
                    if (this.ParentMapMouseMoveHandler == null)
                    {
                        this.ParentMapMouseMoveHandler = new MouseEventHandler(ParentMap_MouseMove);
                        parentMap.MouseMove += this.ParentMapMouseMoveHandler;
                    }
                }
            }

            // Enable Dragging
            this.isDragging = true;
            base.OnMouseLeftButtonDown(e);

        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            TimeSpan span = DateTime.Now - _lastMouseDownTime;
            if (span.Milliseconds < 300)
            {
                if (this.Clicked != null)
                {
                    Clicked(this, e);
                }
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.Cursor = Cursors.Arrow;
        }

        #region "Mouse Event Handler Methods"

        void ParentMap_MousePan(object sender, MapMouseDragEventArgs e)
        {
            // If the Pushpin is being dragged, specify that the Map's MousePan
            // event is handled. This is to suppress the Panning of the Map that
            // is done when the mouse drags the map.
            if (this.isDragging)
            {
                e.Handled = true;
            }
        }

        void ParentMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Left Mouse Button released, stop dragging the Pushpin
            if (this.isDragging)
            {
                this.isDragging = false;
                if (this.Pinned != null)
                {
                    Pinned(this, e);
                }
            }
        }

        void ParentMap_MouseMove(object sender, MouseEventArgs e)
        {
            var map = sender as Microsoft.Maps.MapControl.Map;
            // Check if the user is currently dragging the Pushpin
            if (this.isDragging)
            {
                // If so, the Move the Pushpin to where the Mouse is.
                var mouseMapPosition = e.GetPosition(map);
                var mouseGeocode = map.ViewportPointToLocation(mouseMapPosition);
                this.Location = mouseGeocode;
            }
        }
        #endregion

    }
}
