using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.App
{
    public static class TalonApi
    {
        private static Process _apiProcess;

        public static void Start()
        {
            try
            {
                _apiProcess = new Process();

                string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                _apiProcess.StartInfo.UseShellExecute = false;
                _apiProcess.StartInfo.CreateNoWindow = true;
                _apiProcess.StartInfo.FileName = $"{System.IO.Path.GetDirectoryName(strExeFilePath)}/bin/Debug/net6.0/ProjectTalon.Api.exe";
                _apiProcess.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void Stop()
        {
            _apiProcess.Kill();
        }
    }
}
