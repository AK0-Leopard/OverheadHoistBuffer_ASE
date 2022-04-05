using System;
using System.IO.Ports;
using System.Media;


namespace WebApplication1
{
    public class WeatherForecast
    {
        public SoundPlayer mcs;
        public SoundPlayer alarm;
        public SoundPlayer agv1;
        public SoundPlayer agv2;
        public SoundPlayer agv3;
        public SoundPlayer agv4;
        public SoundPlayer agv5;
        public SoundPlayer agv6;
        public SoundPlayer agv7;
        public SoundPlayer agv8;
        public SoundPlayer agv9;
        public SoundPlayer agv_dis;
        public SoundPlayer line1_dis;
        public SoundPlayer line2_dis;
        public SoundPlayer line3_dis;
        public SoundPlayer loop_dis;
        public SoundPlayer mcsCommandTimeOut;
        public SoundPlayer oht_error_happend;
        public SoundPlayer ohcv_bcrcode_error;
        public SoundPlayer agv_st_bcrcode_error;
        public string passReportVh = "";




        public WeatherForecast(string passReporVh)
        {
            this.passReportVh = passReporVh;
            var executablePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            mcs = new SoundPlayer();
            alarm = new SoundPlayer();
            agv1 = new SoundPlayer();
            agv2 = new SoundPlayer();
            agv3 = new SoundPlayer();
            agv4 = new SoundPlayer();
            agv5 = new SoundPlayer();
            agv6 = new SoundPlayer();
            agv7 = new SoundPlayer();
            agv8 = new SoundPlayer();
            agv9 = new SoundPlayer();
            agv_dis = new SoundPlayer();
            line1_dis = new SoundPlayer();
            line2_dis = new SoundPlayer();
            line3_dis = new SoundPlayer();
            loop_dis = new SoundPlayer();
            mcsCommandTimeOut = new SoundPlayer();
            oht_error_happend = new SoundPlayer();
            ohcv_bcrcode_error = new SoundPlayer();
            agv_st_bcrcode_error = new SoundPlayer();


            mcs.SoundLocation = executablePath + @"mcs.wav";
            alarm.SoundLocation = executablePath + @"alarm.wav";
            agv1.SoundLocation = executablePath + @"1많Ŧ.wav";
            agv2.SoundLocation = executablePath + @"2많Ŧ.wav";
            agv3.SoundLocation = executablePath + @"3많Ŧ.wav";
            agv4.SoundLocation = executablePath + @"4많Ŧ.wav";
            agv5.SoundLocation = executablePath + @"5많Ŧ.wav";
            agv6.SoundLocation = executablePath + @"6많Ŧ.wav";
            agv7.SoundLocation = executablePath + @"7많Ŧ.wav";
            agv8.SoundLocation = executablePath + @"8많Ŧ.wav";
            agv9.SoundLocation = executablePath + @"9많Ŧ.wav";
            agv_dis.SoundLocation = executablePath + @"agv_dis.wav";
            line1_dis.SoundLocation = executablePath + @"line1_dis.wav";
            line2_dis.SoundLocation = executablePath + @"line2_dis.wav";
            line3_dis.SoundLocation = executablePath + @"line3_dis.wav";
            loop_dis.SoundLocation = executablePath + @"loop_dis.wav";
            mcsCommandTimeOut.SoundLocation = executablePath + @"realcst_timeout.wav";
            oht_error_happend.SoundLocation = executablePath + @"OHT_Alarm.wav";
            ohcv_bcrcode_error.SoundLocation = executablePath + @"OHCV_BARCODE_ERROR.wav";
            agv_st_bcrcode_error.SoundLocation = executablePath + @"AGV_ST_BARCODE_ERROR.wav";




            mcs.LoadAsync();
            alarm.LoadAsync();
            agv1.LoadAsync();
            agv2.LoadAsync();
            agv3.LoadAsync();
            agv4.LoadAsync();
            agv5.LoadAsync();
            agv6.LoadAsync();
            agv7.LoadAsync();
            agv8.LoadAsync();
            agv9.LoadAsync();
            agv_dis.LoadAsync();
            line1_dis.LoadAsync();
            line2_dis.LoadAsync();
            line3_dis.LoadAsync();
            loop_dis.LoadAsync();
            mcsCommandTimeOut.LoadAsync();
            oht_error_happend.LoadAsync();
            ohcv_bcrcode_error.LoadAsync();
            agv_st_bcrcode_error.LoadAsync();
        }
    }
}
