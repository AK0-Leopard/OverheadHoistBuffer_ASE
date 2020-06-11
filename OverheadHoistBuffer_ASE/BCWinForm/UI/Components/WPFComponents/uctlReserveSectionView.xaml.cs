using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace com.mirle.ibg3k0.bc.winform.UI.Components.WPFComponents
{
    /// <summary>
    /// uctlReserveSectionView.xaml 的互動邏輯
    /// </summary>
    public partial class uctlReserveSectionView : UserControl, INotifyPropertyChanged
    {
        App.BCApplication bcApp = null;
        sc.App.SCApplication scApp = null;
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public uctlReserveSectionView()
        {
            InitializeComponent();
            DataContext = this;
            refresh_sw.Start();
        }

        #region Methods
        DispatcherTimer _timer = new DispatcherTimer();
        public void Start(App.BCApplication _bcApp)
        {
            bcApp = _bcApp;
            scApp = bcApp.SCApplication;
            //宣告Timer

            //設定呼叫間隔時間為30ms
            _timer.Interval = TimeSpan.FromMilliseconds(3000);

            //加入callback function
            _timer.Tick += _timer_Tick;

            //開始
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _timer = null;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            RefreshReserveSectionInfo();
        }
        #endregion Methods


        #region Display
        private BitmapSource _mapBitmapSource;


        public virtual BitmapSource MapBitmapSource
        {
            get
            {
                return _mapBitmapSource;
            }
            set
            {
                if (_mapBitmapSource != value)
                {
                    _mapBitmapSource = value;
                    OnPropertyChanged();
                }
            }
        }

        System.Diagnostics.Stopwatch refresh_sw { get; set; } = new System.Diagnostics.Stopwatch();
        public void RefreshReserveSectionInfo()
        {
            if (refresh_sw.ElapsedMilliseconds < 1000) return;
            refresh_sw.Restart();
            scApp.ReserveBLL.DrawAllReserveSectionInfo();
            var Bitmap = scApp.ReserveBLL.GetCurrentReserveInfoMap();
            MapBitmapSource = Bitmap;

            //Dispatcher.Invoke(() =>
            //{
            //});
        }

        #endregion

        private void ToolBar_Click(object sender, RoutedEventArgs e)
        {
            RefreshReserveSectionInfo();
        }
    }
}
