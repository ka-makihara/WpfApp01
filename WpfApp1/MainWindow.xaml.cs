using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using System.Runtime.InteropServices;
using System.Linq.Expressions;
using System.IO;

using IronPython.Runtime;
using IronPython.Hosting;
using IronPython.Modules;
using Microsoft.Scripting.Hosting;


namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<ScriptFile> _scriptFiles = new ObservableCollection<ScriptFile>();
        private ObservableCollection<VerInfo> _updateVerInfo = new ObservableCollection<VerInfo>();
        private ObservableCollection<VerInfo> _curVerInfo = new ObservableCollection<VerInfo>();

        private static ScriptEngine _scriptEngine = Python.CreateEngine();
        private static ScriptScope _scriptScope = _scriptEngine.CreateScope();
        private static ScriptScope _builtinScope = _scriptEngine.GetBuiltinModule();

        [DllImport("kernel32.dll")]
            private static extern int GetPrivateProfileString(
                string lpApplicationName,
                string lpKeyName,
                string lpDefault,
                StringBuilder lpReturnString,
                int nSize,
                string lpFileName);

        public MainWindow()
        {
            InitializeComponent();

            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            ICollection<string> paths = _scriptEngine.GetSearchPaths();
            // IronPython をインストールしていないので(?)得られるパスは'.'のみであった
            // よって、アプリ側でパスを追加してやる必要がある
            paths.Add(userPath + "\\source\\repos\\WpfApp01\\IronPython.2.7.10\\Lib");
            paths.Add(userPath + "\\source\\repos\\WpfApp01\\IronPython.2.7.10\\net45\\DLLs");
            paths.Add(userPath + "\\source\\repos\\WpfApp01\\IronPython.2.7.10\\Lib\\site-packages");
            paths.Add(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\scripts");
            paths.Add(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\programs");
            _scriptEngine.SetSearchPaths(paths);

            //自分のクラスの関数を呼び出せるようにしてみる
            // この方法で、例えば　python 内で ftpTool.xxxx('msg') などで xxxx の関数呼び出しができる 
            _scriptScope.SetVariable("ftpTool", this);

            //BuiltinModule のスコープに値を設定しておくと、import したモジュール内でも ftpTool.xxx として関数呼び出しができる
            _scriptEngine.GetBuiltinModule().SetVariable("ftpTool", this);

            GetScriptFiles(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\programs");

            scriptFile.ItemsSource = _scriptFiles;
            updateVersion.ItemsSource = _updateVerInfo;
            curVersion.ItemsSource = _curVerInfo;
        }

        //
        // 指定したフォルダ下にあるスクリプトファイルの一覧を取得してスクリプト選択用のコンボボックスに設定
        //
        private void GetScriptFiles(string path)
        {
            //指定のモジュールを読み込んで、モジュール内の関数を実行
            var scope = Python.ImportModule(_scriptEngine, "ftpToolUtils");

            //scope.SetVariable("__filePath", path);  //変数を設定してデータを渡す方法

            string cmd = "getScriptFiles('" + path + "')"; //関数の引数で渡す

            // ※パスの引き渡しがうまくいかないので、前もってパスを変換する必要がある
            //    例えば、"\\bin などとあると、どうも\b が最初に解釈されてしまい
            //    間違ったパスが指定された、というようなエラーを起こす。
            _scriptEngine.Execute(cmd.Replace("\\","/"), scope);

            var sec = scope.GetVariable<IList<string>>("result");
            foreach (string fn in sec) {
                _scriptFiles.Add(new ScriptFile { Id = 0, Name = System.IO.Path.GetFileNameWithoutExtension(fn), Description = "", Path = fn });
            }
        }
/*
        private void Script_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = scriptFile.SelectedItem as ScriptFile;
            var dlg = new WpfApp1.MessageBoxEx();
            var sb = new StringBuilder();

            sb.AppendLine(item.Name);
            sb.AppendLine(item.Description);

            dlg.Message = sb.ToString();

            dlg.Button = MessageBoxButton.YesNoCancel;
            dlg.Image = MessageBoxImage.Warning;
            dlg.Result = MessageBoxResult.No;
            dlg.ShowDialog();
        }
*/
        public static string GetName<T>(Expression<Func<T>> e)
        {
            var member = (MemberExpression)e.Body;
            return member.Member.Name;
        }

        public string GetIniValue(string path, string section, string key)
        {
            StringBuilder sb = new StringBuilder(256);
            GetPrivateProfileString(section, key, string.Empty, sb, sb.Capacity, path);

            return sb.ToString();
        }
        public void ConsoleOut(string s)
        {
            Console.WriteLine(s);
        }

        private void FtpToolMsgBox(string msg)
        {
            var dlg = new MessageBoxEx();
            dlg.Message = msg;

            dlg.Left = this.Left + 50;
            dlg.Top = this.Top + 30;
            dlg.Button = MessageBoxButton.OK;
            dlg.Image = MessageBoxImage.Error;
            dlg.Result = MessageBoxResult.OK;
            dlg.ShowDialog();
        }

        private void ExecScriptFile(string file)
        {
            ScriptSource src = _scriptEngine.CreateScriptSourceFromFile(file);

            try {
                src.Execute(_scriptScope);
            }
            catch(Exception err)
            {
                string msg = _scriptEngine.GetService<ExceptionOperations>().FormatException(err);
                //MessageBox.Show(msg, "実行エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                FtpToolMsgBox(msg);
            }
        }

        private ScriptScope ExecScriptModule(string moduleName, string cmd)
        {
            var scope = Python.ImportModule(_scriptEngine, moduleName);
            scope.SetVariable("ftpTool", this);

            try {
                _scriptEngine.Execute(cmd.Replace("\\", "/"), scope);
            }
            catch(Exception err)
            {
                string msg = _scriptEngine.GetService<ExceptionOperations>().FormatException(err);
                FtpToolMsgBox(msg);
            }
            return scope;
        }

        private void python_test()
        {
            _scriptScope.SetVariable("glo", 123);          //数値を渡してみる
            _scriptScope.SetVariable("f_data", 12.345);    //浮動小数点
            _scriptScope.SetVariable("pyStr", "ss");       //文字列
            _scriptScope.SetVariable("ftpToolExe", this);

            //スクリプトファイルの実行
            ExecScriptFile(AppDomain.CurrentDomain.BaseDirectory + "test.py");

            //var ret = scope.GetVariable<string>("pyresult");    //python の変数をうけとってみる

            //文字のリストを受け取る場合は
            var ret = _scriptScope.GetVariable<IList<string>>("pyresult");    //python の変数をうけとってみる
            foreach (string m in ret)
            {
                Console.WriteLine(m);
            }

            //ロードしたスクリプトの関数をよんでみる
            //pe.Execute("test_func(100)", scope);          //数値
            _scriptEngine.Execute("test_func('C:/aa/bb.txt')", _scriptScope);   //文字列
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "UpdateCommon.inf";

            //指定のモジュールを読み込んで、モジュール内の関数を実行
            string cmd = "get_iniFile_sections('" + iniFileName + "')";

            ScriptScope scope = ExecScriptModule("ftpToolUtils",cmd);

            var sec = scope.GetVariable<IList<string>>("result");

            foreach (string m in sec)
            {
                UpdateCommonInfo inf = new UpdateCommonInfo();

                string ver = GetIniValue(iniFileName, string.Format(m), GetName(() => inf.Version));
                _updateVerInfo.Add(new VerInfo { Name = m, Version = ver });
                Console.WriteLine(m);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            /*
            var ww = new InputIP();
            ww.ShowDialog();
            string ip = ww.IPtextBox.Text;
            */
            DateTime dateTime = DateTime.Now;
            infoView.AppendText(dateTime + "\n");
            infoView.Select(infoView.Text.Length, 0);
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "UpdateCommon_tmp.inf";
            string ip = ipAddr.Text;

            string cmd = "connect('" + ip + "','" + iniFileName + "')";
            ScriptScope scope = ExecScriptModule("ftpToolUtils", cmd);

            //var ftp = scope.GetVariable<Object>("result");
            var sec = scope.GetVariable<IList<string>>("result");

            foreach (string m in sec)
            {
                UpdateCommonInfo inf = new UpdateCommonInfo();

                string ver = GetIniValue(iniFileName, string.Format(m), GetName(() => inf.Version));
                _curVerInfo.Add(new VerInfo { Name = m, Version = ver });
            }
        }

        private void scriptFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScriptFile sf = scriptFile.SelectedItem as ScriptFile;

            var scope = Python.ImportModule(_scriptEngine, "ftpToolUtils");
            string cmd = "getFuncList('" + sf.Path + "')";
            _scriptEngine.Execute(cmd.Replace("\\", "/"), scope);
            var list = scope.GetVariable<IList<string>>("result");

            stepBtnArea.Children.Clear();

            foreach( string nn in list)
            {
                Button b1 = new Button();
                b1.Content = nn;
                b1.Name = nn;
                b1.Margin=new Thickness(10,5,10,5);
                b1.Height = 35;
                b1.FontSize = 18;
                if (stepBtnArea.Children.Count > 0){
                    //最初のボタン以外はDisableにする
                    //b1.IsEnabled = false;
                }
                b1.Click += (ss, ee) => BtnEvent(ss);
                stepBtnArea.Children.Add(b1);
            }
        }
        private void BtnEvent(object sender)
        {
            ScriptFile sf = scriptFile.SelectedItem as ScriptFile;

            string cmd = ((Button)sender).Name + "('" + ipAddr.Text + "')";

            ScriptScope scope = ExecScriptModule(sf.Name, cmd);
        }
    }
}
