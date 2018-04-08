using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace RizoChange2
{
    class Program
    {
        //  フィールドパラメータ

        static LogWrite lg = null;

        static void Main(string[] args)
        {
            //  引数をセット
            ArgsParam ap = new ArgsParam(args);

            using (Mutex mutex = new Mutex(false, "RizoChange"))
            {
                //  多重禁止
                if (!mutex.WaitOne(0, false)) { return; }

                //  カレントディレクトリ
                Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                //  ログ準備
                lg = new LogWrite(ap.LogFile);
                lg.WriteLine("開始");

                //  ヘルプ表示
                if (ap.ViewHelp || !ap.Runnable)
                {
                    Help.View();
                    //return;
                }

                //  モニタ情報情報を取得
                MonitorRegistry mr = new MonitorRegistry();
                mr.GetConnectedMonitor();
                mr.CheckRegistryKey();
                mr.LatestRegistryKey();

                Console.WriteLine(mr.LatestMonitor.DeviceID);




                lg.WriteLine("終了");
            }
            Console.ReadLine();
        }

        



    }
}
