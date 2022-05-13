﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

using BombServerEmu_MNR.Src.Log;
using BombServerEmu_MNR.Src.DataTypes;
using BombServerEmu_MNR.Src.Services;

namespace BombServerEmu_MNR.Src
{
    class Program
    {
        public static List<BombService> services = new List<BombService>();

        static void Main(string[] args)
        {
            Logging.OpenLogFile();
            Logging.RealLog(typeof(Program), "BombServer  Copyright (C) 2021  derole\n" +
                "This program comes with ABSOLUTELY NO WARRANTY! This is free software, and you are welcome to redistribute it under certain conditions\n", LogType.Info);
            CheckArgs(args);
            //SetCipherSuite();
            if (!CheckCerts()) {
                Logging.Log(typeof(Program), "Failed to find a certificate in the Certs folder!", LogType.Error);
                Console.ReadKey();
                return;
            }
            services.Add(new Directory("192.168.1.196", 10501).service);

            services.Add(new Matchmaking("192.168.1.196", 10510).service);
            services.Add(new GameManager("192.168.1.196", 10505).service);
            services.Add(new GameBrowser("192.168.1.196", 10412).service);

            services.Add(new TextComm("192.168.1.196", 10513).service);
            services.Add(new PlayGroup("192.168.1.196", 10514).service);
            services.Add(new Stats("192.168.1.196", 10515).service);

            //services.Add(new Directory("192.168.1.196", 11501).service);

            //services.Add(new Matchmaking("192.168.1.196", 11510).service);
            //services.Add(new GameManager("192.168.1.196", 11511).service);
            //services.Add(new GameBrowser("192.168.1.196", 11512).service);

            //services.Add(new TextComm("192.168.1.196", 11513).service);
            //services.Add(new PlayGroup("192.168.1.196", 11514).service);
            //services.Add(new Stats("192.168.1.196", 11515).service);
        }

        static bool CheckCerts()
        {
            try
            {
                Logging.Log(typeof(Program), System.IO.Path.GetFullPath(@"Certs\output.pfx"), LogType.Debug);
                if (!System.IO.File.Exists(@"Data\Certs\output.pfx"))
                {
                    var proc = Process.Start(@"Data\Scripts\GenCert.bat");
                    proc.WaitForExit();
                    if (System.IO.File.Exists(@"C:\Program Files\OpenSSL-Win64\bin\certs\output.pfx"))
                    {
                        System.IO.File.Move(@"C:\Program Files\OpenSSL-Win64\bin\certs\output.pfx", @"Data\Certs\output.pfx");
                        System.IO.Directory.Delete(@"C:\Program Files\OpenSSL-Win64\bin\certs", true);
                        return true;
                    }
                    return false;
                }
                return true;
            } catch { return false; }
        }

        static void SetCipherSuite()
        {
            var proc = Process.Start("PowerShell", string.Format("\"{0}\"", System.IO.Path.GetFullPath(@"Data\Scripts\SetCipherSuite.ps1")));
            proc.WaitForExit();
        }

        static void CheckArgs(string[] args)
        {
            for (int i=0; i<args.Length; i++) {
                switch (args[i]) {
                    case "-loglevel":
                        Logging.logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), args[i + 1]);
                        break;
                }
            }
        }
    }
}
