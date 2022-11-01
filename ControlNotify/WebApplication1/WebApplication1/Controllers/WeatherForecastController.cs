using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        static int come_count = 0;


        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {

            _logger = logger;

        }

        static object lock_obj = new object();
        [HttpGet("{id}")]
        public String Get(string id, [FromServices] WeatherForecast service, [FromServices] SerialPortService serialPort)

        {

            //SpeechSynthesizer synth = new SpeechSynthesizer();

            // Configure the audio output.   
            //synth.SetOutputToDefaultAudioDevice();

            // Speak a string.  
            //synth.Speak("This example demonstrates a basic use of Speech Synthesizer");
            lock (lock_obj)
            {
                if (!service.passReportVh.Contains(id))
                    serialPort.openPort();
                switch (id)
                {
                    case "0":
                        _logger.LogInformation("有MCS新命令");
                        service.mcs.PlaySync();
                        break;
                    case "1":
                        _logger.LogInformation("#1車有新命令");
                        service.agv1.PlaySync();
                        break;
                    case "2":
                        _logger.LogInformation("#2車有新命令");
                        service.agv2.PlaySync();
                        break;
                    case "3":
                        _logger.LogInformation("#3車有新命令");
                        service.agv3.PlaySync();
                        break;
                    case "4":
                        _logger.LogInformation("#4車有新命令");
                        service.agv4.PlaySync();
                        break;
                    case "5":
                        _logger.LogInformation("#5車有新命令");
                        service.agv5.PlaySync();
                        break;
                    case "6":
                        _logger.LogInformation("#6車有新命令");
                        service.agv6.PlaySync();
                        break;
                    case "7":
                        _logger.LogInformation("#7車有新命令");
                        service.agv7.PlaySync();
                        break;
                    case "8":
                        _logger.LogInformation("#8車有新命令");
                        service.agv8.PlaySync();
                        break;
                    case "9":
                        _logger.LogInformation("#9車有新命令");
                        service.agv9.PlaySync();
                        break;
                    case "10":
                        _logger.LogInformation("MCS 執行逾時");
                        service.mcsCommandTimeOut.PlaySync();
                        break;
                    case "96":
                        _logger.LogInformation("AGV BCR Read fail超過次數");
                        service.agv_st_bcrcode_error.PlaySync();
                        break;
                    case "97":
                        _logger.LogInformation("OHCV BCR Read fail超過次數");

                        service.ohcv_bcrcode_error.PlaySync();
                        break;
                    case "98":
                        _logger.LogInformation("OHt 有發生Alarm");
                        service.oht_error_happend.PlaySync();
                        break;
                    case "99":
                        _logger.LogInformation("AGV 有發生Alarm");
                        service.alarm.PlaySync();
                        break;
                    case "agv_dis":
                        _logger.LogInformation("AGV Disconnect");
                        service.agv_dis.PlaySync();
                        break;
                    case "line1_dis":
                        _logger.LogInformation("line1 Disconnect");
                        service.line1_dis.PlaySync();
                        break;
                    case "line2_dis":
                        _logger.LogInformation("line2 Disconnect");
                        service.line2_dis.PlaySync();
                        break;
                    case "line3_dis":
                        _logger.LogInformation("line3 Disconnect");
                        service.line3_dis.PlaySync();
                        break;
                    case "loop_dis":
                        _logger.LogInformation("line4 Disconnect");
                        service.loop_dis.PlaySync();
                        break;
                    case "line1_VehicleHasCMDNoAction":
                        _logger.LogInformation("Line1 vehicle has command but took no action for long time.");
                        service.line1_VehicleHasCMDNoAction.PlaySync();
                        break;
                    case "line2_VehicleHasCMDNoAction":
                        _logger.LogInformation("Line2 vehicle has command but took no action for long time.");
                        service.line2_VehicleHasCMDNoAction.PlaySync();
                        break;
                    case "line3_VehicleHasCMDNoAction":
                        _logger.LogInformation("Line3 vehicle has command but took no action for long time.");
                        service.line3_VehicleHasCMDNoAction.PlaySync();
                        break;
                    case "loop_VehicleHasCMDNoAction":
                        _logger.LogInformation("Loop vehicle has command but took no action for long time.");
                        service.loop_VehicleHasCMDNoAction.PlaySync();
                        break;
                    case "AGV_Safety_Sensor":
                        _logger.LogInformation("AGV detected obstacle with safety sensor.");
                        service.agv_safetySensor.PlaySync();
                        break;
                    case "AGV_StopChargeFail":
                        _logger.LogInformation("AGV stop charge fail.");
                        service.agv_stop_charge_fail.PlaySync();
                        break;
                    case "ChargeStatusIsAbnormal":
                        _logger.LogInformation("Charge status is abnormal.");
                        service.charge_status_abnormal.PlaySync();
                        break;
                }
                if (!service.passReportVh.Contains(id))
                    serialPort.closePort();
            }
            return "ok";
        }
    }
}
