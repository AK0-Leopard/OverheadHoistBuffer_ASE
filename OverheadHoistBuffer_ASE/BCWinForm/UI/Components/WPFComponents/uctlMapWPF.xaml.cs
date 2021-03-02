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
        List<VhObj> vhObjs = new List<VhObj>();

        PathEnhance pathEnhance = new PathEnhance();
        PathEnhance guidePath = null;

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
            initialVhObject(_bcApp);
            initialVhEvent(_bcApp);

        }

        private void initialVhObject(App.BCApplication _bcApp)
        {
            List<sc.AVEHICLE> vhs = _bcApp.SCApplication.VehicleBLL.cache.loadVhs();
            foreach (var vh in vhs)
            {
                var vh_obj = new VhObj(vh);
                vhObjs.Add(vh_obj);
                VehicleTrack.Children.Add(vh_obj.ellipse);
                VehicleTrack.Children.Add(vh_obj.nodeText);
                vh_obj.EntryMonitorMode += Vh_obj_EntryMonitorMode;
            }
        }

        private void Vh_obj_EntryMonitorMode(object sender, EventArgs e)
        {
            guidePath.RemoveAllGeometry();
            var vh = sender as VhObj;
            if (vh == null) return;
            var sec_objs = sections.Where(sec => vh.GetGuideSections().Contains(sec.ID));
            foreach (var sec in sec_objs)
            {
                guidePath.AddLineSegment(sec.StartAddress.Point, sec.EndAddress.Point);
            }
        }

        string event_id = string.Empty;
        private void initialVhEvent(BCApplication _bcApp)
        {
            //_bcApp.TestGuideSectionSearch += _bcApp_TestGuideSectionSearch;
            //sc.AVEHICLE vh_1 = _bcApp.SCApplication.VehicleBLL.cache.getVhByID("B7_OHBLOOP_CR1");
            //event_id = this.Name;
            //vh_1.addEventHandler(event_id
            //                    , nameof(vh_1.VhPositionChangeEvent)
            //                    , (s1, e1) =>
            //                    {
            //                        updateVehiclePosition_vh1(vh1_position, s1 as sc.AVEHICLE);
            //                    });
            ////vh_1.addEventHandler(event_id
            ////        , nameof(vh_1.VhStatusChangeEvent)
            ////        , (s1, e1) =>
            ////        {

            ////        });
            ////updateVehicleStatus_vh1(vh1, vh_1);

            //sc.AVEHICLE vh_2 = _bcApp.SCApplication.VehicleBLL.cache.getVhByID("B7_OHBLOOP_CR2");
            //event_id = this.Name;
            //vh_2.addEventHandler(event_id
            //                    , nameof(vh_2.VhPositionChangeEvent)
            //                    , (s1, e1) =>
            //                    {
            //                        updateVehiclePosition_vh1(vh2_position, s1 as sc.AVEHICLE);
            //                    });
            ////updateVehicleStatus_vh1(vh2, vh_2);
        }

        private void _bcApp_TestGuideSectionSearch(object sender, string[] e)
        {
            guidePath.RemoveAllGeometry();
            var sec_objs = sections.Where(sec => e.Contains(sec.ID));
            foreach (var sec in sec_objs)
            {
                guidePath.AddLineSegment(sec.StartAddress.Point, sec.EndAddress.Point);
            }
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


        static double min_x = 0;
        static double min_y = 0;

        private void initialObj(App.BCApplication _bcApp)
        {
            addresses = loadAddresss(_bcApp);
            sections = loadASection(_bcApp);
            min_x = addresses.Min(address => address.X);
            min_y = addresses.Min(address => address.Y);

        }
        private void initialPath()
        {
            VehicleTrack.Children.Clear();
            guidePath = new PathEnhance(Brushes.Yellow);
            VehicleTrack.Children.Add(guidePath.Path);
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

        class VhObj
        {
            public event EventHandler EntryMonitorMode;

            public Ellipse ellipse { private set; get; } = null;
            public TextBlock nodeText { private set; get; } = null;
            private sc.AVEHICLE vh = null;
            public VhObj(sc.AVEHICLE _vh)
            {
                vh = _vh;
                ellipse = GetEllipse(vh.Num);
                nodeText = GetTextBlock(vh.Num);
                ellipse.MouseDown += Ellipse_MouseDown;
                updateVehicleStatus(vh);
                initialVhEvent();

            }

            private void initialVhEvent()
            {
                string event_id = $"WPF_UI_{this.vh.VEHICLE_ID}";
                vh.addEventHandler(event_id
                                    , nameof(vh.VhPositionChangeEvent)
                                    , (s1, e1) =>
                                    {
                                        updateVehiclePosition(s1 as sc.AVEHICLE);
                                    });
                vh.addEventHandler(event_id
                                    , nameof(vh.VhStatusChangeEvent)
                                    , (s1, e1) =>
                                    {
                                        updateVehicleStatus(s1 as sc.AVEHICLE);
                                    });
                vh.addEventHandler(event_id
                    , nameof(vh.isTcpIpConnect)
                    , (s1, e1) =>
                    {
                        updateVehicleStatus(s1 as sc.AVEHICLE);
                    });
            }
            private void updateVehiclePosition(sc.AVEHICLE vh)
            {
                double X = (((vh.X_Axis)) * PathEnhance.Scale) - (ellipse.ActualWidth / 2) + 5;
                double Y = (((vh.Y_Axis)) * PathEnhance.Scale) - (ellipse.ActualHeight / 2) + 5;

                //double X = ((vh.X_Axis) * PathEnhance.Scale);
                //double Y = ((vh.Y_Axis) * PathEnhance.Scale);
                if (X == 0 && Y == 0)
                {
                    X = vh.Num * 30;
                }

                Adapter.Invoke((obj) =>
                {
                    setPosition(X, Y);
                }, null);

            }
            private void updateVehicleStatus(sc.AVEHICLE vh)
            {
                if (!vh.isTcpIpConnect)
                {
                    setColor(Brushes.Black);
                }
                else
                {
                    switch (vh.MODE_STATUS)
                    {
                        case VHModeStatus.InitialPowerOff:
                        case VHModeStatus.Manual:
                            setColor(Brushes.Orange);
                            break;
                        case VHModeStatus.InitialPowerOn:
                            break;
                        case VHModeStatus.AutoLocal:
                            setColor(Brushes.Blue);
                            break;
                        case VHModeStatus.AutoMts:
                            setColor(Brushes.Gold);
                            break;
                        case VHModeStatus.AutoMtl:
                            setColor(Brushes.Gold);
                            break;
                        case VHModeStatus.AutoRemote:
                            setColor(Brushes.Green);
                            break;
                        default:
                            setColor(Brushes.Orange);
                            break;
                    }
                }
            }


            private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
            {
                EntryMonitorMode?.Invoke(this, EventArgs.Empty);
            }


            private Ellipse GetEllipse(int vhNum)
            {
                // Create a red Ellipse.
                Ellipse myEllipse = new Ellipse();

                myEllipse.Fill = Brushes.Yellow;

                // Set the width and height of the Ellipse.
                myEllipse.Width = 30;
                myEllipse.Height = 30;
                myEllipse.RenderTransform = new TranslateTransform(vhNum * 30, 0);
                return myEllipse;
            }
            private TextBlock GetTextBlock(int vhNum)
            {
                TextBlock nodeText = new TextBlock(new Run(vhNum.ToString()) { Foreground = Brushes.White });
                //nodeText.Text = vhNum.ToString();
                nodeText.HorizontalAlignment = HorizontalAlignment.Center;
                nodeText.VerticalAlignment = VerticalAlignment.Center;
                nodeText.TextAlignment = TextAlignment.Center;
                nodeText.RenderTransform = new TranslateTransform(vhNum * 30, 0);
                return nodeText;
            }

            public void setPosition(double x, double y)
            {
                var ellipse_translate = ellipse.RenderTransform as TranslateTransform;
                if (ellipse_translate == null) return;
                var textblock_translate = nodeText.RenderTransform as TranslateTransform;
                if (textblock_translate == null) return;

                ellipse_translate.X = x;
                ellipse_translate.Y = y;
                textblock_translate.X = x;
                textblock_translate.Y = y;
            }
            public void setColor(Brush brush)
            {
                Adapter.Invoke((obj) =>
                {
                    ellipse.Fill = brush;
                }, null);

            }
            public string[] GetGuideSections()
            {
                if (vh.WillPassSectionID == null)
                    return new string[0];
                return vh.WillPassSectionID?.ToArray();
            }
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
            public PathEnhance(SolidColorBrush solidColor)
            {
                Path.Cursor = Cursors.Hand;
                Path.AllowDrop = true;
                Path.Stroke = solidColor;
                Path.StrokeThickness = 300;
                //Path.MouseDown += Path_MouseDown;
                //Path.MouseMove += Path_MouseMove;
                Path.Data = geometryGroup;
                Path.Stretch = Stretch.None;
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
            public void RemoveAllGeometry()
            {
                geometryGroup.Children.Clear();
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

        private void VehicleTrack_MouseDown(object sender, MouseButtonEventArgs e)
        {
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
