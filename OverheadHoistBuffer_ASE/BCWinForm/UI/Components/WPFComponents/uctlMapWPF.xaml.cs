using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace com.mirle.ibg3k0.bc.winform.UI.Components.WPFComponents
{
    /// <summary>
    /// uctlMapWPF.xaml 的互動邏輯
    /// </summary>
    public partial class uctlMapWPF : UserControl
    {

        Brush Brush_Black;
        Brush Brush_Yellow;
        Brush Brush_LawnGreen;
        Brush Brush_Orange;
        Brush Brush_Cyan;

        PathEnhance pathEnhance = new PathEnhance();

        List<Address> addresses;
        List<Section> sections;

        public uctlMapWPF()
        {
            InitializeComponent();

            BrushConverter brushConverter = new BrushConverter();
            Brush_Black = brushConverter.ConvertFromString("Black") as Brush;
            Brush_Yellow = brushConverter.ConvertFromString("Yellow") as Brush;
            Brush_LawnGreen = brushConverter.ConvertFromString("LawnGreen") as Brush;
            Brush_Orange = brushConverter.ConvertFromString("Orange") as Brush;
            Brush_Cyan = brushConverter.ConvertFromString("Cyan") as Brush;
        }

        public void Start(App.BCApplication _bcApp)
        {
            initialObj(_bcApp);
            initialPath();
            initialVhEvent(_bcApp);

        }
        string event_id = string.Empty;
        private void initialVhEvent(BCApplication _bcApp)
        {
            sc.AVEHICLE vh_1 = _bcApp.SCApplication.VehicleBLL.cache.getVhByID("B7_OHBLOOP_CR1");
            event_id = this.Name;
            vh_1.addEventHandler(event_id
                                , nameof(vh_1.VhPositionChangeEvent)
                                , (s1, e1) =>
                                {
                                    updateVehiclePosition_vh1(vh1_position, s1 as sc.AVEHICLE);
                                });
            //vh_1.addEventHandler(event_id
            //        , nameof(vh_1.VhStatusChangeEvent)
            //        , (s1, e1) =>
            //        {

            //        });
            //updateVehicleStatus_vh1(vh1, vh_1);

            sc.AVEHICLE vh_2 = _bcApp.SCApplication.VehicleBLL.cache.getVhByID("B7_OHBLOOP_CR2");
            event_id = this.Name;
            vh_2.addEventHandler(event_id
                                , nameof(vh_2.VhPositionChangeEvent)
                                , (s1, e1) =>
                                {
                                    updateVehiclePosition_vh1(vh2_position, s1 as sc.AVEHICLE);
                                });
            //updateVehicleStatus_vh1(vh2, vh_2);


        }

        private void updateVehiclePosition_vh1(TranslateTransform transfer, sc.AVEHICLE vh)
        {
            Adapter.Invoke((obj) =>
            {
                transfer.X = ((vh.X_Axis - min_x + pathEnhance.StrokeThickness) * PathEnhance.Scale) - Vh1_Circle.RadiusX;
                transfer.Y = ((vh.Y_Axis - min_y + pathEnhance.StrokeThickness) * PathEnhance.Scale) - Vh1_Circle.RadiusY;
            }, null);

        }
        private void updateVehicleStatus_vh1(Path path_vh, sc.AVEHICLE vh)
        {
            if (!vh.isTcpIpConnect)
            {
                path_vh.Fill = Brush_Black;
            }
            else
            {
                switch (vh.MODE_STATUS)
                {
                    case VHModeStatus.InitialPowerOff:
                    case VHModeStatus.Manual:
                        break;
                    case VHModeStatus.InitialPowerOn:
                        break;
                    case VHModeStatus.AutoLocal:
                        break;
                    case VHModeStatus.AutoMts:
                        break;
                    case VHModeStatus.AutoMtl:
                        break;
                    case VHModeStatus.AutoRemote:
                        break;
                    default:
                        break;
                }
            }
        }


        double min_x = 0;
        double min_y = 0;

        private void initialObj(App.BCApplication _bcApp)
        {
            addresses = loadAddresss(_bcApp);
            sections = loadASection(_bcApp);
            min_x = addresses.Min(address => address.X);
            min_y = addresses.Min(address => address.Y);

        }
        private void initialPath()
        {

            foreach (var adr in addresses)
            {
                pathEnhance.AddEllipse(adr.Point);
            }

            foreach (var sec in sections)
            {
                pathEnhance.AddLineSegment(sec.StartAddress.Point, sec.EndAddress.Point);
            }

            VehicleTrack.Width = pathEnhance.Path.Data.Bounds.Width * PathEnhance.Scale;
            VehicleTrack.Height = pathEnhance.Path.Data.Bounds.Height * PathEnhance.Scale;
            VehicleTrack.Children.Add(pathEnhance.Path);
        }

        public List<Address> loadAddresss(BCApplication bcApp)
        {
            try
            {
                DataTable dt = bcApp.SCApplication.OHxCConfig.Tables["AADDRESS"];
                var query = from c in dt.AsEnumerable()
                            select new Address(c.Field<string>("Id"), startToDouble(c.Field<string>("PositionX")), startToDouble(c.Field<string>("PositionY")));
                return query.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<Section> loadASection(BCApplication bcApp)
        {
            try
            {
                DataTable dt = bcApp.SCApplication.OHxCConfig.Tables["ASECTION"];
                var query = from c in dt.AsEnumerable()
                            select new Section(addresses, c.Field<string>("Id"), c.Field<string>("FromAddress"), c.Field<string>("ToAddress"));
                return query.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private double startToDouble(string value)
        {
            double.TryParse(value, out double result);
            return result;
        }
        class PathEnhance
        {
            public event MouseButtonEventHandler MouseDown;
            public static double Scale = 0.03;
            public Path Path { get; private set; } = new Path();
            GeometryGroup geometryGroup = new GeometryGroup();
            public Brush Stroke
            {
                get { return Path.Stroke; }
                set { Path.Stroke = value; }
            }
            public double StrokeThickness { get; set; } = 100;
            public PathEnhance()
            {
                Path.Cursor = Cursors.Hand;
                Path.AllowDrop = true;
                Path.Stroke = Brushes.Black;
                Path.StrokeThickness = StrokeThickness;
                Path.MouseDown += Path_MouseDown;
                Path.MouseMove += Path_MouseMove;
                Path.Data = geometryGroup;
                Path.Stretch = Stretch.Uniform;
                Path.RenderTransform = new ScaleTransform(Scale, Scale);
            }
            public void AddEllipse(Point point)
            {
                EllipseGeometry ellipseGeometry = new EllipseGeometry(point, 100, 100);
                geometryGroup.Children.Add(ellipseGeometry);
            }
            public void AddLineSegment(Point startPoint, Point endPoint)
            {
                LineSegment lineSegment = new LineSegment(endPoint, true);
                PathFigure pathFigure = new PathFigure(startPoint, new LineSegment[] { lineSegment }, false);
                PathGeometry pathGeometry = new PathGeometry(new PathFigure[] { pathFigure });
                geometryGroup.Children.Add(pathGeometry);
            }

            private void Path_MouseMove(object sender, MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    //Point newPos = e.GetPosition(inputElement);
                    //var a = newPos - _pos;
                    //OffsetVector += a;
                    //_pos = newPos;
                }
            }

            private Point _pos = default(Point);
            private void Path_MouseDown(object sender, MouseButtonEventArgs e)
            {
                //if (e.LeftButton == MouseButtonState.Pressed)
                //{
                //    _pos = e.GetPosition(inputElement);//按下時記錄位置
                //}
                MouseDown?.Invoke(this, e);
            }


        }

    }

    public class Address
    {
        public string ID;
        public double X, Y;
        public Point Point { get; private set; }
        public Address() { }
        public Address(string id, double x, double y)
        {
            ID = id;
            X = x;
            Y = y;
            Point = new Point(x, y);
        }
    }
    public class Section
    {
        public string ID;
        public string StartArd, EndArd;

        public Address StartAddress;
        public Address EndAddress;

        public Section(List<Address> adrs, string id, string startAdr, string endAdr)
        {
            ID = id;
            StartArd = startAdr;
            EndArd = endAdr;
            StartAddress = adrs.Where(adr => adr.ID.Trim() == startAdr.Trim()).FirstOrDefault();
            EndAddress = adrs.Where(adr => adr.ID.Trim() == endAdr.Trim()).FirstOrDefault();
        }
        public Section(string id, Address startAdr, Address endAdr)
        {
            ID = id;
            StartAddress = startAdr;
            EndAddress = endAdr;

        }
    }
}
