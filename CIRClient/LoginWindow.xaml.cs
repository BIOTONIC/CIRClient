using System;
using System.Windows;
using System.Threading;
using System.Windows.Input;

namespace CIRClient
{
    public partial class LoginWindow
    {
        private PopupWindow popupWindow;
        private string response;

        // 管理服务器地址和端口号
        public static string serverAddr;
        public static int serverPort;

        // 为了禁止输入框出现& 先判断Shift键按下了没有
        private bool isShiftDown;

        // 第三方RSA代码
        //private static RSAHelper.RSAKey keyPair;

        public LoginWindow()
        {
            InitializeComponent();
            ////开辟新线程
            Thread th = new Thread(init);
            th.Start();
        }

        // 连接服务器 发送公钥
        private void init()
        {
            // 从配置文件里读取管理服务器的地址和端口号
            // TODO 之后应该是先连接归属服务器 在连接管理服务器
            serverAddr = FileHelper.GetValueFromConf("client.conf", "homeserveraddr");
            serverPort = int.Parse(FileHelper.GetValueFromConf("client.conf", "homeserverport"));

            try
            {
                SyncClient.StartClient(serverAddr, serverPort);
            }
            catch (Exception)
            {

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    popupWindow = new PopupWindow("无法连接到管理服务器", 1000);
                    popupWindow.Show();
                }));
            }

            //生成key pair
            // keyPair = RSAHelper.GetRASKey();
        }

        private void Login_click(object sender, RoutedEventArgs e)
        {
            //获取用户名密码
            string user = NameTextBox.Text;
            string pass = FileHelper.CalcStringMD5(PasswordBox.Password);
            string userMsg = "user/username:" + user + "&password:" + pass;

            ////私钥加密
            //string enLoginValue = RSAHelper.EncryptString(loginValue, keyPair.PrivateKey);

            try
            {
                SyncClient.SendBytes(FileHelper.getBytesOfString(userMsg));

                response = SyncClient.ReceiveString();

                if (response == "failure")
                {
                    popupWindow = new PopupWindow("用户名密码不正确", 1000);
                    popupWindow.Show();
                    return;
                }
                else if (response != "" && response != "success")
                {
                    MainWindow mainWindow = new MainWindow(response);
                    mainWindow.Show();
                    Close();
                }
            }
            catch (Exception)
            {
                popupWindow = new PopupWindow("无法连接到管理服务器", 1000);
                popupWindow.Show();

                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                Close();
            }
        }

        private void Cancel_click(Object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = "";
            PasswordBox.Password = "";
        }

        // 输入框禁止特殊字符
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // 同时按下Shift和7才算按下&键
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                isShiftDown = true;
            }
            if (isShiftDown && e.Key == Key.D7)
            {
                e.Handled = true;
            }
            // 冒号 || 分号 || 左方括号 || 右方括号
            if (e.Key == Key.Separator || e.Key == Key.OemSemicolon || e.Key == Key.OemOpenBrackets || e.Key == Key.OemCloseBrackets || e.Key == Key.Add)
            {
                e.Handled = true;
            }
        }

        // 输入框禁止特殊字符
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                isShiftDown = false;
            }
        }
    }
}