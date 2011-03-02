using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Core;

namespace WorldMap
{
    public class GeocodeLayer
    {
        private Map myMap;
        private MapLayer layer;

        public GeocodeLayer(Map _myMap)
        {
            myMap = _myMap;
            layer = new MapLayer();
            myMap.Children.Add(layer);
        }

        private string ExtractNumber( PlatformServices.Address address)
        {
            // Get street address ... works for most US addresses.
            int blankAt = address.AddressLine.IndexOf(' ');
            if (blankAt > 0)
            {
                return address.AddressLine.Substring(0, blankAt);
            }
            else
            {
                return string.Empty;
            }
        }

        // Point inside box that is approximately (2*side) meters per side
        private const int side = 5;
        private LocationRect BoxIt(Location loc)
        {
            Point pt = myMap.LocationToViewportPoint(loc);
            double mpp = MercatorUtility.ZoomToScale(new Size(512, 512), myMap.ZoomLevel, loc);
            double offset = side / mpp;
            pt.X -= offset;
            pt.Y -= offset;
            Location nw = myMap.ViewportPointToLocation(pt);
            pt.X += (2 * offset);
            pt.Y += (2 * offset);
            Location se = myMap.ViewportPointToLocation(pt);
            LocationRect rect = new LocationRect(
                new Location(nw.Latitude, se.Longitude),
                new Location(se.Latitude, nw.Longitude));
            return rect;
        }

        public Location AddResult(PlatformServices.GeocodeResult result)
        {
            MapShapeBase shape;
            Location loc = new Location(result.Locations[0].Latitude, result.Locations[0].Longitude);
            LocationRect rect = BoxIt(loc);
            if (0 == string.CompareOrdinal(result.Locations[0].CalculationMethod, "Interpolation"))
            {
                // Draw plus sign at interpolated point.
                MapPolyline polyline = new MapPolyline();
                double midLat = (rect.Southeast.Latitude + rect.Northwest.Latitude) / 2;
                double midLon = (rect.Southeast.Longitude + rect.Northwest.Longitude) / 2;
                Location center = new Location(midLat, midLon);
                polyline.Locations = new LocationCollection();
                polyline.Locations.Add(center);
                polyline.Locations.Add(new Location(rect.Northwest.Latitude, midLon));
                polyline.Locations.Add(center);
                polyline.Locations.Add(new Location(midLat, rect.Northwest.Longitude));
                polyline.Locations.Add(center);
                polyline.Locations.Add(new Location(rect.Southeast.Latitude, midLon));
                polyline.Locations.Add(center);
                polyline.Locations.Add(new Location(midLat, rect.Southeast.Longitude));
                polyline.Locations.Add(center);
                polyline.Stroke = new SolidColorBrush(Colors.Red);
                polyline.StrokeThickness = 1.0;
                polyline.StrokeEndLineCap = PenLineCap.Round;
                shape = polyline;
            }
            else
            {
                // Parcel or rooftop... show colored box.
                MapPolygon polygon = new MapPolygon();
                if (0 == string.CompareOrdinal(result.Locations[0].CalculationMethod, "Parcel"))
                {
                    // Blue box for Parcel geocode.
                    polygon.Stroke = new SolidColorBrush(Colors.White);
                    polygon.Fill = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    // Red box for rooftop geocode.
                    polygon.Stroke = new SolidColorBrush(Colors.White);
                    polygon.Fill = new SolidColorBrush(Colors.Red);
                }
                polygon.StrokeThickness = 1.0;
                polygon.Opacity = .6;
                polygon.Locations = new LocationCollection();
                polygon.Locations.Add(rect.Northwest);
                polygon.Locations.Add(rect.Northeast);
                polygon.Locations.Add(rect.Southeast);
                polygon.Locations.Add(rect.Southwest);
                shape = polygon;
            }

            // Store address with shape in
            shape.Tag = result.DisplayName;
            // Add a tool tip to display the full address.
            ToolTipService.SetToolTip(shape, result.DisplayName);
            // Add shape to the layer
            layer.Children.Add(shape);

            // Put the street address on the map.
            string addressNumber = ExtractNumber(result.Address);
            if (!string.IsNullOrEmpty(addressNumber))
            {
                TextBlock tb = new TextBlock();
                tb.Text = addressNumber;
                tb.FontFamily = new FontFamily("Times New Roman");
                tb.FontSize = 12.0;
                tb.HorizontalAlignment = HorizontalAlignment.Left;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.Foreground = new SolidColorBrush(Colors.White);
                // Set tool tip on text since it's above the shape.
                ToolTipService.SetToolTip(tb, result.DisplayName);
                tb.Tag = result.DisplayName;
                layer.AddChild(tb, loc, PositionOrigin.Center);
            }

            return loc;
        }
    }
}
