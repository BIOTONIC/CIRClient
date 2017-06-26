using System.Windows;
using System.IO;
using Microsoft.Win32;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace CIRClient
{
    public partial class MainWindow
    {
        // 消息提示的窗体
        private PopupWindow popupWindow;

        // 消息提示语句
        private string response;

        // 为了禁止输入框出现& 先判断Shift键按下了没有
        private bool isShiftDown;

        // 文件一和二的完整路径和名字
        private string file1FullName;
        private string file2FullName;

        // 文件一和二的数据大小
        private long file1ByteSize;
        private long file2ByteSize;

        // 历史记录是否更新过了
        private bool isHistoryUpdated = false;

        public MainWindow()
        {
            InitializeComponent();

            // 刚到主界面弹窗提示登录成功
            popupWindow = new PopupWindow("登录成功", 1000);
            popupWindow.Show();
        }

        public MainWindow(string name)
        {
            // 登陆成功会获得厂家名 加到标题上来
            if (name == null)
                name = "Test";
            InitializeComponent();
            Title1.Text = "CIR文件上传客户端:" + name;
            Title2.Text = "CIR文件上传客户端:" + name;

            popupWindow = new PopupWindow("登录成功", 1000);
            popupWindow.Show();
        }

        // 输入框禁止特殊字符
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // 同时按下Shift和7才算按下&键
            if(e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                isShiftDown = true;
            }
            if( isShiftDown && e.Key == Key.D7)
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
            if(e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                isShiftDown = false;
            }         
        }

        // 选择文件一
        private void File1Select_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                // SafeFileName 只有文件名 不包含路径
                string safeFileName = dlg.SafeFileName;
                File1NameBox.Text = safeFileName;

                // 完整的包含绝对路径的文件名
                file1FullName = dlg.FileName;
                FileInfo file = new FileInfo(file1FullName);
                file1ByteSize = file.Length;
                File1SizeBox.Text = FileHelper.ConvertBytes(file.Length);

                File1HashBox.Text = FileHelper.CalcFileMD5(dlg.FileName);
            }
        }

        // 选择文件二
        private void File2Select_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string safeFileName = dlg.SafeFileName;
                File2NameBox.Text = safeFileName;

                file2FullName = dlg.FileName;
                FileInfo file = new FileInfo(file2FullName);
                file2ByteSize = file.Length;
                File2SizeBox.Text = FileHelper.ConvertBytes(file.Length);

                File2HashBox.Text = FileHelper.CalcFileMD5(dlg.FileName);
            }
        }

        // 上传文件按钮点击事件
        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            // 收集填写的数据
            string person = PersonBox.Text;
            string type = TypeBox.Text;
            string version = VerBox.Text;
            string desc = DescBox.Text;
            string file1Name = File1NameBox.Text;
            string file1Size = file1ByteSize.ToString();
            string file2Size = file2ByteSize.ToString();
            string file1Hash = File1HashBox.Text;
            string file2Name = File2NameBox.Text;
            string file2Hash = File2HashBox.Text;

            // 校验数据填写是否完整
            if (person == "" || type == "" || version == "" || desc == "" || file1Name == "" || file1Size == "" || file1Hash == "" || file2Name == "" || file2Size == "" || file2Hash == "")
            {
                popupWindow = new PopupWindow("请填写完整数据", 1000);
                popupWindow.Show();
                return;
            }

            // 发送文件信息-----------------------------------------------------
            string fileInfoMsg = "fileInfo/person:" + person + "&type:" + type + "&version:" + version + "&desc:" + desc + "&file1Name:"
            + file1Name + "&file1Size:" + file1Size + "&file1Hash:" + file1Hash + "&file2Name:" + file2Name + "&file2Size:" + file2Size + "&file2Hash:" + file2Hash;

            try
            {
                SyncClient.SendBytes(FileHelper.getBytesOfString(fileInfoMsg));

                response = SyncClient.ReceiveString();
            }
            catch (Exception)
            {
                popupWindow = new PopupWindow("无法连接到管理服务器", 1000);
                popupWindow.Show();
                ReturnToLogin();
                return;
            }

            if (response == "exist")
            {
                popupWindow = new PopupWindow("数据版本已经存在", 1000);
                popupWindow.Show();
                return;
            }
            else if (response != "success")
            {
                popupWindow = new PopupWindow("文件信息上传失败", 1000);
                popupWindow.Show();
                return;
            }
            else
            {
                popupWindow = new PopupWindow("文件信息上传完毕", 1000);
                popupWindow.Show();
            }


            // 上传文件一-------------------------------------------------------
            try
            {
                SyncClient.SendBytes(FileHelper.getBytesOfStringAndFile("file1/content:", file1FullName));
                response = SyncClient.ReceiveString();
            }
            catch (Exception)
            {
                popupWindow = new PopupWindow("无法连接到管理服务器", 1000);
                popupWindow.Show();
                ReturnToLogin();
                return;
            }

            if (response != "success")
            {
                popupWindow = new PopupWindow("文件1上传失败", 1000);
                popupWindow.Show();
                return;
            }
            else
            {
                popupWindow = new PopupWindow("文件1上传完毕", 1000);
                popupWindow.Show();
            }

            // 上传文件二-------------------------------------------------------
            try
            {
                SyncClient.SendBytes(FileHelper.getBytesOfStringAndFile("file2/content:", file2FullName));
                response = SyncClient.ReceiveString();
            }
            catch (Exception)
            {
                popupWindow = new PopupWindow("无法连接到管理服务器", 1000);
                popupWindow.Show();
                ReturnToLogin();
                return;
            }

            if (response != "success")
            {
                popupWindow = new PopupWindow("文件2上传失败", 1000);
                popupWindow.Show();
                return;
            }
            else
            {
                popupWindow = new PopupWindow("文件2上传完毕", 1000);
                popupWindow.Show();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                // 如果切换到了历史记录显示的Tab
                if (TabControl.SelectedIndex == 1)
                {
                    // 且当前没有更新过
                    if (!isHistoryUpdated)
                    {
                        // TODO 在这里做判断 刚点开历史记录标签就查询返回一次结果
                        // 当然 下次点开这个tab要不要查询也是一个问题
                        // 可能会导致请求过多的问题
                    }

                }
            }
        }

        private void ReturnToLogin()
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}
