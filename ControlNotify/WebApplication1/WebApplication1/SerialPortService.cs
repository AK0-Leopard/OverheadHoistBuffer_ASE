using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class SerialPortService
    {
        String portName = "COM3";
        Int32 BaudRate = 115200;
        int sleepTime = 1000;

        public SerialPort serialPort1;
        public SerialPortService(String pn, int st)
        {
            string[] ports = SerialPort.GetPortNames();
            this.sleepTime = st;
            Console.WriteLine("The following serial ports were found:");

            // Display each port name to the console.
            foreach (string port in ports)
            {
                Console.WriteLine(port);
            }
            this.portName = pn;
            serialPort1 = new SerialPort(portName);
            serialPort1.BaudRate = Convert.ToInt32(BaudRate);
            serialPort1.DataBits = 8;
            serialPort1.Parity = System.IO.Ports.Parity.None;
            serialPort1.StopBits = System.IO.Ports.StopBits.One;
        }

        public void openPort()
        {
            try
            {
                //首先判斷串口是否開啓
                if (serialPort1.IsOpen)
                {
                    serialPort1.Write("o");
                    Thread.Sleep(sleepTime);

                }
                else
                {
                    serialPort1.Open();     //打開串口
                    serialPort1.Write("o");
                    Thread.Sleep(sleepTime);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public void closePort()
        {
            try
            {
                //首先判斷串口是否開啓
                if (serialPort1.IsOpen)
                {
                    serialPort1.Write("f");
                    Thread.Sleep(sleepTime);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }

    }
}
