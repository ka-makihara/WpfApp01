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

            ICollection<string> paths = _scriptEngine.GetSearchPaths();
            // IronPython をインストールしていないので(?)得られるパスは'.'のみであった
            // よって、アプリ側でパスを追加してやる必要がある
            paths.Add("C:\\Users\\makih\\source\\repos\\IronPython.2.7.10\\Lib");
            paths.Add("C:\\Users\\makih\\source\repos\\IronPython.2.7.10\\net45\\DLLs");
            paths.Add("C:\\Users\\makih\\source\repos\\IronPython.2.7.10\\Lib\\site-packages");
            paths.Add(AppDomain.CurrentDomain.BaseDirectory + "scripts");
            paths.Add(AppDomain.CurrentDomain.BaseDirectory + "programs");
            _scriptEngine.SetSearchPaths(paths);

            /*
            _scriptFiles.Add(new ScriptFile { Id = 1, Name = "aa", Description = "msg", Path = "C:\\" });
            _scriptFiles.Add(new ScriptFile { Id = 2, Name = "bb", Description = "msg", Path = "C:\\" });
            _scriptFiles.Add(new ScriptFile { Id = 3, Name = "cc", Description = "msg", Path = "C:\\" });
            _scriptFiles.Add(new ScriptFile { Id = 4, Name = "dd", Description = "msg", Path = "C:\\" });
            */
            //GetScriptFiles(AppDomain.CurrentDomain.BaseDirectory + "scripts");

            scriptFile.ItemsSource = _scriptFiles;
            
            updateVersion.ItemsSource = _updateVerInfo;
            curVersion.ItemsSource = _curVerInfo;
        }
        private void GetScriptFiles(string path)
        {
            //指定のモジュールを読み込んで、モジュール内の関数を実行
            var scope = Python.ImportModule(_scriptEngine, "ftpToolUtils");

            scope.SetVariable("filePath", path);

            string cmd = "getScriptFiles()";

            _scriptEngine.Execute(cmd, scope);

            var sec = scope.GetVariable<IList<string>>("result");
            foreach (string fn in sec)
            {
                _scriptFiles.Add(new ScriptFile { Id = 0, Name = fn, Description = "", Path = fn });
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new WpfApp1.MessageBoxEx();
            dlg.Message = "テストメッセージ";

            dlg.Left = this.Left + 50;
            dlg.Top = this.Top + 50;
            //dlg.Background = Brushes.Wheat;
            dlg.Button = MessageBoxButton.YesNoCancel;
            dlg.Image = MessageBoxImage.Warning;
            dlg.Result = MessageBoxResult.No;
            dlg.ShowDialog();

            MessageBoxResult result = dlg.Result;
        }

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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "UpdateCommon.inf";
            string scrFileName = AppDomain.CurrentDomain.BaseDirectory + "test.py";

            StringBuilder result = new StringBuilder();

            UpdateCommonInfo inf = new UpdateCommonInfo();

            result.AppendFormat("[Palette]");
            result.Append(Environment.NewLine);

            inf.Path = GetIniValue(iniFileName, string.Format("Palette"), GetName(() => inf.Path));
            inf.ID = GetIniValue(iniFileName, string.Format("Palette"), GetName(() => inf.ID));

            // IronPython 2.7.10 を使用する
            ScriptSource src = _scriptEngine.CreateScriptSourceFromFile(scrFileName);

            _scriptScope.SetVariable("glo", 123);          //数値を渡してみる
            _scriptScope.SetVariable("f_data", 12.345);    //浮動小数点
            _scriptScope.SetVariable("pyStr", "ss");       //文字列

            try {
                src.Execute(_scriptScope);
            }
            catch(Exception err)
            {
                string msg = _scriptEngine.GetService<ExceptionOperations>().FormatException(err);
                MessageBox.Show(msg, "実行エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //var ret = scope.GetVariable<string>("pyresult");    //python の変数をうけとってみる

            //文字のリストを受け取る場合は
            var ret = _scriptScope.GetVariable<IList<string>>("pyresult");    //python の変数をうけとってみる
            foreach (string m in ret)
            {
                Console.WriteLine(m);
            }

            //ロードしたスクリプトの関数をよんでみる
            //pe.Execute("test_func(100)", scope);          //数値
            _scriptEngine.Execute("test_func(\"C:/aa/bb.txt\")", _scriptScope);   //文字列　"\"は上手く渡せないかも

            //指定のモジュールを読み込んで、モジュール内の関数を実行
            var sc = Python.ImportModule(_scriptEngine, "sample");
            string cmd = "get_iniFile_sections(\"" + iniFileName + "\")";
            string cmd2 = cmd.Replace("\\", "/");
            _scriptEngine.Execute(cmd2, sc);
            var sec = sc.GetVariable<IList<string>>("result");
            foreach (string m in sec)
            {
                string ver = GetIniValue(iniFileName, string.Format(m), GetName(() => inf.Version));
                _updateVerInfo.Add(new VerInfo { Name = m, Version = ver });
                Console.WriteLine(m);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var ww = new InputIP();
            ww.ShowDialog();
            string ip = ww.IPtextBox.Text;
        }
    }
}
