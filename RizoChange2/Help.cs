using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

//  ヘルプ表示用クラス
namespace RizoChange2
{
    class Help
    {
        public Help() { }

        public static void View()
        {
            string appName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine($@"
使用法 {appName} / X <横幅> /Y <高さ> [/N <ディスプレイ番号>]
              [/R] [/L <ログファイル>] [/?]

オプション：
  /X <横幅>             ディスプレイ解像度の横幅
  /Y <高さ>             ディスプレイ解像度の高さ
  /N <ディスプレイ番号> 設定する対象のディスプレイ番号。複数指定の場合は、00,01,02 ・・・
                        未指定の場合、プライマリディスプレイのみを設定
  /R                    解像度設定後、ディスプレイデバイスを再起動
  /L <ログファイル>     ログ出力先
  /?                    ヘルプを表示

例：
  {appName} /X 1600 /Y 900
  - 接続している全ディスプレイの解像度を1600×900に設定。
    次回再起動or再度ログオン後に反映。

  {appName} /X 1920 /Y 1080 /N 00 /R
  - プライマリディスプレイのみ、1920×1080に設定。
    ディスプレイデバイスを再起動する為、一瞬の暗転後に反映
");
        }
    }
}
