using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace CIRClient
{
    public class HistoryViewModel : ViewModel
    {
        // 每次查询服务器获得的首条记录的索引号
        private int _startIndex = 0;

        // 查询得到的总记录 区别于存到list并显示的个数
        private int _recordsTotal = 0;

        // 消息提示文本
        private string _response = "";

        // 消息提示框
        private PopupWindow _popupWindow;

        // 存储HistoryModel的列表
        private List<HistoryModel> _historyList;

        // 绑定查询的上传人框
        private string _searchPerson;

        public string SearchPerson
        {
            get { return _searchPerson; }
            set
            {
                if (_searchPerson != value)
                {
                    _searchPerson = value;
                }
            }
        }

        // 绑定查询的设备类型框
        private string _searchDevType;

        public string SearchDevType
        {
            get { return _searchDevType; }
            set
            {
                if (_searchDevType != value)
                {
                    _searchDevType = value;
                }
            }
        }

        // 绑定查询的起始时间
        private string _searchStartTime;

        public string SearchStartTime
        {
            get { return _searchStartTime; }
            set
            {
                if (_searchStartTime != value)
                {
                    _searchStartTime = value;
                }
            }
        }

        // 绑定查询的截止时间
        private string _searchEndTime;

        public string SearchEndTime
        {
            get { return _searchEndTime; }
            set
            {
                if (_searchEndTime != value)
                {
                    _searchEndTime = value;
                }
                if (value == "")
                {
                    _searchEndTime = "截止时间";
                }
            }
        }

        // 绑定搜索按钮
        public ICommand SearchCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    // 点击搜索按钮的具体处理 写在回调函数里
                    CommunicateWithServer();
                });
            }
        }

        //绑定首页按钮
        public ICommand FirstPageCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    PageIndex = 1;
                    CommunicateWithServer();
                });
            }
        }

        // 绑定上一页按钮
        public ICommand PreviousPageCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (PageIndex == 1)
                    {
                        return;
                    }
                    PageIndex--;
                    CommunicateWithServer();
                });
            }
        }

        // 绑定下一页按钮
        public ICommand NextPageCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (PageIndex == PagesTotal)
                    {
                        return;
                    }
                    PageIndex++;
                    CommunicateWithServer();
                });
            }
        }

        // 绑定末页按钮
        public ICommand LastPageCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    PageIndex = PagesTotal;
                    CommunicateWithServer();
                });
            }
        }

        // 绑定每页存放记录数量
        private int _pageSize;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }

            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    OnPropertyChanged("PageSize");
                }
            }
        }

        // 绑定当前页的索引号
        private int _pageIndex;

        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }

            set
            {
                if (_pageIndex != value)
                {
                    _pageIndex = value;
                    OnPropertyChanged("PageIndex");
                }
            }
        }

        // 绑定总页数
        private int _pagesTotal;

        public int PagesTotal
        {
            get
            {
                return _pagesTotal;
            }

            set
            {
                if (_pagesTotal != value)
                {
                    _pagesTotal = value;
                    OnPropertyChanged("PagesTotal");
                }
            }
        }

        // 绑定到前端的history列表
        private ObservableCollection<HistoryModel> _historySource;

        public ObservableCollection<HistoryModel> HistorySource
        {
            get
            {
                return _historySource;
            }

            set
            {
                if (_historySource != value)
                {
                    _historySource = value;
                    OnPropertyChanged("HistorySource");
                }
            }
        }

        // 构造器
        public HistoryViewModel()
        {
            _pageIndex = 0;
            _pageSize = 10;

            _historyList = new List<HistoryModel>();
            _historySource = new ObservableCollection<HistoryModel>();
        }

        // 根据输入框输入的参数和页数生成历史纪录查询语句
        private string GenerateSearchString()
        {
            // 计算出要向服务器查询的首条数据的索引号
            _startIndex = (PageIndex - 1) * PageSize;
            if (_startIndex < 0)
            {
                _startIndex = 0;
                PageIndex = 1;
            }

            // 基本的查询参数
            string searchStr = "records/type:upload&numPerPage:" + _pageSize + "&start:" + _startIndex;
            // 更具输入框添加的查询参数
            if (_searchPerson != "" && _searchPerson != null)
            {
                searchStr += "&person:";
                searchStr += _searchPerson;
            }
            if (_searchDevType != "" && _searchDevType != null)
            {
                searchStr += "&devType:";
                searchStr += _searchDevType;
            }
            if (_searchStartTime != "" && _searchStartTime != null)
            {
                // 时间原本是"2017/6/22"类型的 要转成"2017-6-22"
                string startTime = _searchStartTime.Replace('/', '-');
                searchStr += "&startTime:";
                searchStr += startTime;
            }
            if (_searchEndTime != "" && _searchEndTime != null)
            {
                string endTime = _searchEndTime.Replace('/', '-');
                searchStr += "&endTime:";
                searchStr += endTime;
            }
            Console.WriteLine(searchStr);
            return searchStr;
        }

        // 解析服务器传回来的字段信息 并把记录存储到_historyList
        private void ParseRepsonseFromServer(string response)
        {
            try
            {
                // 返回的字符串是这样的
                // "total:1000&cur:0&content:[time:2017-05-03;person:小王;devType:类型一][...]"
                // 先按&分隔
                string[] param = response.Split('&');

                // 获得总查询记录
                _recordsTotal = int.Parse(param[0].Split(':')[1]);

                // 计算总页数
                PagesTotal = (_recordsTotal - 1) / PageSize + 1;

                // 总共0页当前也是0页
                if (PagesTotal == 0)
                {
                    PageIndex = 0;
                }

                // 当前返回数量 <= numPerPage 其实这个值不需要 因为具体返回数量还是看后面的记录量
                int curNum = int.Parse(param[1].Split(':')[1]);

                // 获取 content的value "[time:2017-05-03;person:小王;devType:类型一][...]"
                string content = response.Split('&')[2].Substring(8);

                // 匹配方括号之间的内容
                Regex rx = new Regex(@"\[(.*?)\]");
                Match m = rx.Match(content);

                // 向historyList添加记录前先清空
                _historyList.Clear();

                // 循环匹配记录
                while (m.Success)
                {
                    Console.WriteLine(m.Value);
                    // m.Value 
                    // 去掉左右两端的方括号 "[type:123;value:123]"
                    string record = m.Value.Substring(1, m.Value.Length - 2);
                    // record "type:123;value:123"
                    string[] columns = record.Split(';');

                    string time = "";
                    string person = "";
                    string devType = "";
                    string version = "";
                    string desc = "";
                    string file1 = "";
                    string file2 = "";
                    int isSuccess = 0;

                    // 把每一个column对应的值记录下来
                    for (int i = 0; i < columns.Length; i++)
                    {
                        // pair "type" "123"
                        string[] pair = columns[i].Split(':');
                        switch (pair[0])
                        {
                            case "time":
                                time = pair[1];
                                // 时间显示从 2017/4/15 13/20/20 变成 2017/4/15 13:20:20的格式
                                string[] parts = time.Split(' ');
                                string second = parts[1].Replace('/', ':');
                                time = parts[0] + ' ' + second;
                                break;
                            case "person":
                                person = pair[1];
                                break;
                            case "devType":
                                devType = pair[1];
                                break;
                            case "version":
                                version = pair[1];
                                break;
                            case "desc":
                                desc = pair[1];
                                break;
                            case "file1":
                                file1 = pair[1];
                                break;
                            case "file2":
                                file2 = pair[1];
                                break;
                            case "isSuccess":
                                isSuccess = int.Parse(pair[1]);
                                break;
                            default:
                                break;
                        }
                    }

                    // 实例化一个Model并加入到historyList
                    HistoryModel hm = new HistoryModel(time, person, devType, version, desc, file1, file2, isSuccess);
                    _historyList.Add(hm);

                    // 匹配下一个
                    m = m.NextMatch();
                }

                // 再转存到_historySource中
                _historySource.Clear();
                _historySource.AddRange(_historyList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // 负责与服务器的具体连接
        private void CommunicateWithServer()
        {
            try
            {
                SyncClient.SendBytes(FileHelper.getBytesOfString(GenerateSearchString()));
                
                _response = SyncClient.ReceiveString();
            }
            catch (Exception)
            {
                _popupWindow = new PopupWindow("无法连接到管理服务器", 1000);
                _popupWindow.Show();

                // TODO 怎么在这里关闭MainWindow 回到LoginWindow呢
                // MainWindow.ReturnToLogin();
                return;
            }

            try
            {
                // 从服务器得到数据并存储到_historyList
                ParseRepsonseFromServer(_response);
            }
            catch (Exception)
            {
                _popupWindow = new PopupWindow("从管理服务器获取数据错误", 1000);
                _popupWindow.Show();

                // TODO 怎么在这里关闭MainWindow 回到LoginWindow呢
                // MainWindow.ReturnToLogin();
                return;
            }
        }
    }
}
