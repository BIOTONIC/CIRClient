using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace CIRClient
{
    /// <summary>
    /// HistoryView.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryView : UserControl
    {
        public static RoutedEvent FirstPageEvent;
        public static RoutedEvent PreviousPageEvent;
        public static RoutedEvent NextPageEvent;
        public static RoutedEvent LastPageEvent;

        public static readonly DependencyProperty PagesTotalProperty;
        public static readonly DependencyProperty PageIndexProperty;

        // 为了禁止输入框出现& 先判断Shift键按下了没有
        private bool isShiftDown;

        // 总记录数
        public int PagesTotal
        {
            get { return (int)GetValue(PagesTotalProperty); }
            set { SetValue(PagesTotalProperty, value); }
        }

        // 当前页码
        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        // 构造器
        public HistoryView()
        {
            InitializeComponent();
        }

        // 静态代码块
        static HistoryView()
        {
            // 为绑定的数据注册
            PagesTotalProperty = DependencyProperty.Register("PagesTotal", typeof(string),
            typeof(HistoryView), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnPagesTotalChanged)));
            PageIndexProperty = DependencyProperty.Register("PageIndex", typeof(string),
            typeof(HistoryView), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnPageIndexChanged)));
        }

        // 绑定数据属性改变的回调函数
        public static void OnPagesTotalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HistoryView p = d as HistoryView;

            if (p != null)
            {
                Run PagesTotal = (Run)p.FindName("pagesTotal");
                PagesTotal.Text = (string)e.NewValue;
            }
        }

        public static void OnPageIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HistoryView p = d as HistoryView;

            if (p != null)
            {
                Run PageIndex = (Run)p.FindName("pageIndex");
                PageIndex.Text = (string)e.NewValue;
            }
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
