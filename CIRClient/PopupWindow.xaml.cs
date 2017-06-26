using System;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace CIRClient
{
    // 弹出消息 定时关闭
    public partial class PopupWindow : Window
    {
        // 定时器
        private Timer timer;

        public PopupWindow(string msg, int time)
        {
            InitializeComponent();
            // 显示消息内容
            MsgText.Text = msg;

            // 计时
            timer = new Timer(time);
            timer.Elapsed += new ElapsedEventHandler(timer_elapsed);
            timer.Start();
        }

        private void timer_elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate ()
            {
                Close();
            });
        }
    }
}
