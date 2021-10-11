
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using VehicleControl_Viewer.UI.Components;

namespace VehicleControl_Viewer.frm_Mainfrom
{
    /// <summary>
    /// uc_AGC_MCS_Mainform.xaml 的互動邏輯
    /// </summary>
    public partial class uctlMapWPFNew : UserControl
    {
        #region 全域變數
        private static System.Windows.Threading.DispatcherTimer positionUpdat3Timer;
        private DataSet ohxcConfig = null;
        public DataSet OHxCConfig { get { return ohxcConfig; } }
        List<Address> addresses;
        List<Section> sections;
        List<VehicleInfo> vehicleInfos;
        ShapeCollection RailsCollection = null;
        ShapeCollection DisableRailsCollection = null;
        ShapeCollection TestGuideRailsCollection = null;
        object lock_guide_info_refresh = new object();
        Dictionary<VehicleInfo, ShapeCollection> GuideRailCollection = null;
        GridLength gl80 = new GridLength(80, GridUnitType.Pixel);
        GridLength gl250 = new GridLength(250, GridUnitType.Pixel);
        com.mirle.ibg3k0.bc.winform.App.BCApplication bcApp;
        #endregion 全域變數


        public uctlMapWPFNew()
        {
            InitializeComponent();
            ohxcConfig = new DataSet();
            RailsCollection = new ShapeCollection();
            DisableRailsCollection = new ShapeCollection();
            TestGuideRailsCollection = new ShapeCollection();
            GuideRailCollection = new Dictionary<VehicleInfo, ShapeCollection>();

        }
        public void Start(com.mirle.ibg3k0.bc.winform.App.BCApplication _bcApp)
        {
            bcApp = _bcApp;
            initialObj();
            initialPath();
            initialVhNew();
            initialTimer();

            registeredEvent();
        }


        private void registeredEvent()
        {
            RailsCollection.AddressSelected += RailsCollection_AddressSelected;
        }

        bool isSourceSelected = false;
        private void RailsCollection_AddressSelected(object sender, EventArgs e)
        {

        }

        private void ObjCacheManager_RailStatusChanged(object sender, EventArgs e)
        {
            refreshRailStatus();
        }



        private void refreshRailStatus()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DisableRailsCollection.shapes.ForEach(s => VehicleTrack.Children.Remove(s));
                DisableRailsCollection.shapes.Clear();
            }));
        }

        private void initialTimer()
        {
            positionUpdat3Timer = new System.Windows.Threading.DispatcherTimer();
            positionUpdat3Timer.Tick += new EventHandler(timeCycle);
            positionUpdat3Timer.Interval = new TimeSpan(0, 0, 0, 2);
            positionUpdat3Timer.Start();
        }
        public void timeCycle(object sender, EventArgs e)
        {
            vehicleInfos.ForEach(vh =>
            {
                vh.resreshPosition();
                vh.resreshStatus();
            });
        }

        private void initialObj()
        {
            addresses = loadAddresss();
            sections = loadASection();
        }
        public List<Address> loadAddresss()
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

        public List<Section> loadASection()
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


        private void initialVhNew()
        {
            vehicleInfos = new List<VehicleInfo>();
            var vhs = bcApp.SCApplication.VehicleBLL.cache.loadVhs();
            foreach (var vh in vhs)
            {
                VehicleInfo vh_info = new VehicleInfo(vh);

                Storyboard.SetTarget(vh_info.doubleAnimation_x, vh_info.vhPresenter);
                Storyboard.SetTarget(vh_info.doubleAnimation_y, vh_info.vhPresenter);
                Storyboard.SetTargetProperty(vh_info.doubleAnimation_x, new PropertyPath("RenderTransform.(TranslateTransform.X)"));
                Storyboard.SetTargetProperty(vh_info.doubleAnimation_y, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));

                vehicleInfos.Add(vh_info);
                VehicleTrack.Children.Add(vh_info.vhPresenter);
                vh_info.vhPresenter.MouseDoubleClick += VhPresenter_MouseDoubleClick;

                Canvas.SetZIndex(vh_info.vhPresenter, int.MaxValue);
            }
        }

        private void VhPresenter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var content = sender as ContentControl;
            var vh_info = content.Content as VehicleInfo;
            lock (lock_guide_info_refresh)
            {
                SolidColorBrush Brush = new SolidColorBrush();
                Brush.Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0x00);

                resertGuideReail();
                var guide_rail = new ShapeCollection();
                var will_pass_section = vh_info.getWillPassSection();
                if (will_pass_section == null || will_pass_section.Count == 0) return;
                foreach (string sec in will_pass_section)
                {
                    var sec_obj = sections.Where(s => com.mirle.ibg3k0.sc.Common.SCUtility.isMatche(s.ID, sec)).FirstOrDefault();
                    guide_rail.AddLineSegment(this, sec, new Point(sec_obj.StartAddress.Point.X * 1, sec_obj.StartAddress.Point.Y),
                                               new Point(sec_obj.EndAddress.Point.X * 1, sec_obj.EndAddress.Point.Y), Brush, will_pass_section.Last() == sec);
                }
                GuideRailCollection.Add(vh_info, guide_rail);
                guide_rail.shapes.ForEach(s => VehicleTrack.Children.Add(s));
            }
        }

        private void resertGuideReail()
        {
            if (GuideRailCollection == null || GuideRailCollection.Count() == 0)
            {
                return;
            }
            foreach (var guide_info in GuideRailCollection)
            {
                guide_info.Value.shapes.ForEach(s => VehicleTrack.Children.Remove(s));
            }
            GuideRailCollection.Clear();
        }

        private void initialPath()
        {
            double max_x = 0;
            double max_y = 0;


            foreach (var sec in sections)
            {
                var start_adr_obj = sec.StartAddress;
                var end_adr_obj = sec.EndAddress;
                if (start_adr_obj == null || end_adr_obj == null) continue;
                RailsCollection.AddLineSegment(this, sec.ID, new Point(sec.StartAddress.Point.X * 1, sec.StartAddress.Point.Y),
                                           new Point(sec.EndAddress.Point.X * 1, sec.EndAddress.Point.Y));
            }
            foreach (var adr in addresses)
            {
                double t_x = (adr.Point.X);
                double t_y = (adr.Point.Y);
                RailsCollection.AddEllipse(this, adr, new Point(t_x, t_y));
                if (t_x > max_x)
                    max_x = t_x;
                if (t_y > max_y)
                    max_y = t_y;
                Console.WriteLine($"adr id:{adr.ID},x:{t_x}");
            }
            VehicleTrack.Width = max_x;
            VehicleTrack.Height = max_y;
            //VehicleTrack.RenderTransform = new ScaleTransform(PathEnhance.Scale, PathEnhance.Scale);
            foreach (var s in RailsCollection.shapes)
            {
                VehicleTrack.Children.Add(s);
                if (s is Ellipse)
                {
                    Canvas.SetZIndex(s, int.MaxValue - 1);
                }
            }
        }


        public void Start()
        {
            try
            {

            }
            catch (Exception ex)
            {
            }
        }


        object setTestGuideTail_Sync = new object();
        public void setTestGideRail(IEnumerable<string> guideSection)
        {
            lock (setTestGuideTail_Sync)
            {
                SolidColorBrush Brush = new SolidColorBrush();
                Brush.Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0x00);

                foreach (var s in TestGuideRailsCollection.shapes)
                {
                    VehicleTrack.Children.Remove(s);
                }
                TestGuideRailsCollection.shapes.Clear();
                foreach (var sec_id in guideSection)
                {
                    var sec_obj = sections.Where(sec => sec.ID == sec_id.Trim()).FirstOrDefault();
                    if (sec_obj == null)
                    {

                        continue;
                    }
                    TestGuideRailsCollection.AddLineSegment(this, sec_id, new Point(sec_obj.StartAddress.Point.X * 1, sec_obj.StartAddress.Point.Y),
                                                          new Point(sec_obj.EndAddress.Point.X * 1, sec_obj.EndAddress.Point.Y), Brush, false);
                }
                foreach (var s in TestGuideRailsCollection.shapes)
                {
                    VehicleTrack.Children.Add(s);
                }
            }
        }

        class ShapeCollection
        {
            public EventHandler AddressSelected;
            SolidColorBrush mySolidColorBrush_ForPoint = new SolidColorBrush();
            SolidColorBrush mySolidColorBrush_ForRail = new SolidColorBrush();
            public List<Shape> shapes = null;
            public ShapeCollection()
            {
                //mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
                //mySolidColorBrush.Color = Color.FromArgb(int.Parse("FF0080FF ", NumberStyles.AllowHexSpecifier));
                mySolidColorBrush_ForRail.Color = Color.FromArgb(0xFF, 0, 0x80, 0xFF);
                mySolidColorBrush_ForPoint.Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                shapes = new List<Shape>();
            }
            public void AddEllipse(FrameworkElement frameworkElement, Address adr, Point point)
            {
                Ellipse myEllipse = new Ellipse();
                myEllipse.Fill = mySolidColorBrush_ForPoint;
                myEllipse.StrokeThickness = 10;
                myEllipse.Stroke = mySolidColorBrush_ForRail;
                myEllipse.Width = 250;
                myEllipse.Height = 250;
                double left = point.X - (myEllipse.Width / 2); double top = point.Y - (myEllipse.Height / 2);
                myEllipse.Margin = new Thickness(left, top, 0, 0);
                myEllipse.Cursor = Cursors.Hand;
                myEllipse.MouseDown += MyEllipse_MouseDown;
                myEllipse.Tag = adr;
                var t = new ToolTip();
                //t.Style = (Style)frameworkElement.FindResource("MaterialDesignToolTip");
                ToolTipService.SetInitialShowDelay(t, 0);
                t.Content = adr.ID;
                myEllipse.ToolTip = t;

                shapes.Add(myEllipse);
            }

            private void MyEllipse_MouseDown(object sender, MouseButtonEventArgs e)
            {
                AddressSelected?.Invoke(sender, EventArgs.Empty);
            }

            public void AddLineSegment(FrameworkElement frameworkElement, string secID, Point startPoint, Point endPoint)
            {
                Line myLine = new Line();
                //myLine.Stroke = System.Windows.Media.Brushes.Black;
                myLine.Stroke = mySolidColorBrush_ForRail;
                myLine.X1 = startPoint.X;
                myLine.X2 = endPoint.X;
                myLine.Y1 = startPoint.Y;
                myLine.Y2 = endPoint.Y;
                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 200;
                var t = new ToolTip();
                ToolTipService.SetInitialShowDelay(t, 0);
                t.Content = secID;
                //t.Style = (Style)frameworkElement.FindResource("MaterialDesignToolTip");
                myLine.ToolTip = t;
                shapes.Add(myLine);
            }
            public void AddLineSegment(FrameworkElement frameworkElement, string secID, Point startPoint, Point endPoint, SolidColorBrush brush, bool isFinal)
            {
                Line myLine = new Line();
                //myLine.Stroke = System.Windows.Media.Brushes.Black;
                myLine.Stroke = brush;
                myLine.X1 = startPoint.X;
                myLine.X2 = endPoint.X;
                myLine.Y1 = startPoint.Y;
                myLine.Y2 = endPoint.Y;
                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 200;
                var t = new ToolTip();
                ToolTipService.SetInitialShowDelay(t, 0);
                t.Content = secID;
                //t.Style = (Style)frameworkElement.FindResource("MaterialDesignToolTip");
                myLine.ToolTip = t;

                if (isFinal)
                    myLine.StrokeEndLineCap = PenLineCap.Triangle;


                shapes.Add(myLine);
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
                if (StartAddress == null)
                    Console.WriteLine($"adr id:{startAdr} not exist");

                EndAddress = adrs.Where(adr => adr.ID.Trim() == endAdr.Trim()).FirstOrDefault();
                if (EndAddress == null)
                    Console.WriteLine($"adr id:{endAdr} not exist");
            }
            public Section(string id, Address startAdr, Address endAdr)
            {
                ID = id;
                StartAddress = startAdr;
                EndAddress = endAdr;

            }
        }
        public class Address
        {
            public string ID { get; private set; }
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
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void VehicleTrack_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            lock (lock_guide_info_refresh)
            {
                resertGuideReail();
            }
        }

    }
}
