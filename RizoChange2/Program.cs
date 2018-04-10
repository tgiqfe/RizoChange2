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
                    return;
                }
                
                //  レジストリのモニタ情報を確認
                MonitorRegistry mr = new MonitorRegistry();
                mr.CheckRegMonitor();

                //  解像度変更
                lg.WriteLine("ディスプレイ番号：" + string.Join(", ", ap.TargetDisplays));
                lg.WriteLine("横幅 (X)：" + ap.ResolutionX.ToString());
                lg.WriteLine("高さ (Y)：" + ap.ResolutionY.ToString());
                mr.ChangeRegResolution(ap.TargetDisplays, ap.ResolutionX, ap.ResolutionY);

                //  ディスプレイ再起動
                if (mr.IsChanged && ap.DisplayReload)
                {
                    lg.WriteLine("ディスプレイ再起動");
                    new ChangeStatePNPDevice("Display").Reload();
                }

                //  終了
                lg.WriteLine("終了");
            }
        }
    }
}
