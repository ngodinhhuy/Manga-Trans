using NewSanofi.ClassHelper;
using NewSanofi.UserControls;
using NewSanofi.Windows;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NewSanofi.ViewModel
{
    public class SubControlViewModel : BaseViewModel
    {

        #region Field
        SubControl subControl;
        StackPanel rowStackPanel;
        SimpleTcpClient client;
        public TabItem mainTab;
        private string _IPAddress = "";
        public string IPAddress
        {
            get => _IPAddress; set
            {
                _IPAddress = value;
                ((mainTab.Header as StackPanel).Children[0] as TextBlock).Text =value;
                OnPropertyChanged();
            }
        }

        private Visibility _PlaybackVisible = Visibility.Hidden;
        public Visibility PlaybackVisible
        {
            get => _PlaybackVisible; set
            {
                _PlaybackVisible = value;

                OnPropertyChanged();
            }
        }

        private List<string> _codeList = new List<string>();

        private string _Port = "";
        public string Port
        {
            get => _Port; set
            {
                _Port = value;

                OnPropertyChanged();
            }
        }

        int counterNotReset = 0;

        private int _CounterValue = 0;
        public int CounterValue
        {
            get => _CounterValue; set
            {
                _CounterValue = value;
                CounterText = value.ToString();
                OnPropertyChanged();
            }
        }

        private int _RowStart = 0;
        public int RowStart
        {
            get => _RowStart; set
            {
                _RowStart = value;
                OnPropertyChanged();
            }
        }

        private string _CounterText = "0";
        public string CounterText
        {
            get => _CounterText; set
            {
                _CounterText = value;

                OnPropertyChanged();
            }
        }

        private bool _StatusConnect = false;
        public bool StatusConnect
        {
            get => _StatusConnect; set
            {
                _StatusConnect = value;
                if (StatusConnect == true)
                {
                    StatusString = "Disconnect";
                    
                }
                else
                {
                    StatusString = "Connect";
                    StatusRun = false;
                    firsttime = false;
                }
                OnPropertyChanged();
            }
        }

        private bool _StatusRun = false;
        public bool StatusRun
        {
            get => _StatusRun; set
            {
                _StatusRun = value;
                if (value)
                {
                    RunString = "Stop";
                }
                else
                {
                    RunString = "Run";
                }
                OnPropertyChanged();
            }
        }

        private string _RunString = "Run";
        public string RunString
        {
            get => _RunString; set
            {
                _RunString = value;

                OnPropertyChanged();
            }
        }


        private string _StatusString = "Connect";
        public string StatusString
        {
            get => _StatusString; set
            {
                _StatusString = value;

                OnPropertyChanged();
            }
        }

        private string _Speed = "";
        public string Speed
        {
            get => _Speed; set
            {
                _Speed = value;

                OnPropertyChanged();
            }
        }



        private string _ContentPrint = "";
        public string ContentPrint
        {
            get => _ContentPrint; set
            {
                _ContentPrint = value;

                OnPropertyChanged();
            }
        }

        private string _ExcelPath = "";
        public string ExcelPath
        {
            get => _ExcelPath; set
            {
                _ExcelPath = value;
                OnPropertyChanged();
            }
        }

        private string _ViewStatus = "Show View";
        public string ViewStatus
        {
            get => _ViewStatus; set
            {
                _ViewStatus = value;

                OnPropertyChanged();
            }
        }

        private string _ResponseMessage = "";
        public string ResponseMessage
        {
            get => _ResponseMessage; set
            {
                _ResponseMessage = value;

                OnPropertyChanged();
            }
        }

        private WindowState _WindowsState = WindowState.Normal;
        public WindowState WindowsState
        {
            get => _WindowsState; set
            {
                _WindowsState = value;

                OnPropertyChanged();
            }
        }
        bool done = false;
        bool firsttime = false;
        int i = 0;
        byte[] utf8String;
        #endregion


        #region Command

        public ICommand LoadedWindowCommand { get; set; }
        public ICommand SetCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand RunCommand { get; set; }
        public ICommand ResetCommand { get; set; }

        public ICommand InportExcelCommand { get; set; }
        public ICommand HideCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }

        #endregion

        #region Construction Method
        public SubControlViewModel()
        {
            ResetCommand = new RelayCommand<object>((p) => { return true; }, (p) => {

                CounterValue = 0;

            });
            SetCommand = new RelayCommand<object>((p) => {
                if (_codeList.Count == 0)
                    return false;
                return true; }, (p) => {
                    if (RowStart < _codeList.Count)
                    {
                        i = RowStart;
                        rowStackPanel.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        ManualDialog md = new ManualDialog();
                        md.ManualShow(1, "Set Row Start", "Can't Set, Row Start Value Larger than Code List Count");
                    }
            });
            InportExcelCommand = new RelayCommand<object>((p) => { return true; }, (p) => {

                System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
                
                openFile.RestoreDirectory = true;
                if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (openFile.FileName.Split('.')[1] == "xlsx" || openFile.FileName.Split('.')[1] == "xls")
                    {

                        Task.Run(new Action(() =>
                        {
                            try
                            {
                                _codeList = ImportExcel.import_start(openFile.FileName);
                            }
                            catch { }
                        })).ContinueWith(_ =>
                        {
                            App.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                ManualDialog md = new ManualDialog();
                                if (_codeList.Count > 0)
                                {
                                    md.ManualShow(0, IPAddress + "_Load File Excel", "Done, Number Of File:" + _codeList.Count.ToString());
                                    i = 0;
                                    counterNotReset = 0;
                                    CounterValue = 0;
                                    ExcelPath = openFile.FileName;
                                }
                                else
                                {
                                    md.ManualShow(1, IPAddress + "_Load File Excel", "Fail");
                                }
                            }));
                        });
                    }
                    else if(openFile.FileName.Split('.')[1] == "txt")
                    {
                        Task.Run(new Action(() =>
                        {
                            try
                            {
                                _codeList = TextFileProcess.ReadFileExtent(openFile.FileName);
                            }
                            catch { }
                        })).ContinueWith(_ =>
                        {
                            App.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                ManualDialog md = new ManualDialog();
                                if (_codeList.Count > 0)
                                {
                                    md.ManualShow(0, IPAddress + "_Load File Text", "Done, Number Of File:" + _codeList.Count.ToString());
                                    i = 0;
                                    counterNotReset = 0;
                                    CounterValue = 0;
                                    ExcelPath = openFile.FileName;
                                }
                                else
                                {
                                    md.ManualShow(1, IPAddress + "_Load File Text", "Fail");
                                }
                            }));
                        });
                    }
                    else
                    {
                        ManualDialog md = new ManualDialog();
                        md.ManualShow(1, "Import File", "Error, Can't Define File Type");
                    }
                }
            });


            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                if (checkonetime)
                {

                    rowStackPanel = (p as SubControl).StartRowSP;
                    rowStackPanel.Visibility = Visibility.Hidden;
                    LoadExcel();
                    checkonetime = false;
                }
            });

            ConfirmCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                if (!StatusConnect)
                {
                    try
                    {
                        Connect(IPAddress, Port);
                        StatusConnect = true;
                    }
                    catch (Exception e)
                    {
                        ResponseMessage = e.Message;
                        StatusConnect = false;
                    }
                }
                else
                {
                    client?.Disconnect();
                    StatusConnect = false;
                }

            });

            RunCommand = new RelayCommand<object>((p) => {
                if (!StatusConnect)
                    return false;
                return true;
            }, (p) => {
                if(_codeList.Count==0)
                {
                    ManualDialog md = new ManualDialog();
                    md.ManualShow(1, "Run Process", "Please Import Excel File");
                    return;
                }
                if (rowStackPanel.Visibility == Visibility.Visible)
                {
                    ManualDialog md = new ManualDialog();
                    md.ManualShow(1, "Run Process", "Please Set Row Start Value");
                    return;
                }
                if (MainWindow.numCache == -1||MainWindow.numChar==0)
                {
                    ManualDialog md = new ManualDialog();
                    md.ManualShow(1, "Run Process", "Please Choose Number Insert To Cache Or Set Number Of Character");
                }
                else
                {
                    int realCache = ((MainWindow.numCache + 1) * 5);
                    //int realCache = 1;
                    if (!StatusRun)
                    {
                        StatusRun = true;
                        if (!firsttime)
                        {
                            firsttime = true;
                            for (int j = 0; j < realCache; j++)
                            {
                                WriteData(_codeList[i]);
                                i++;
                                if (i >= _codeList.Count)
                                {
                                    MessageBox.Show("You Have Push All Codes");
                                    break;
                                }
                                //Thread.Sleep(100);
                                //RunExcute();
                            }
                            MessageBox.Show("done");
                        }
                        else
                        {
                            RunExcute();
                        }
                    }
                    else
                    {
                        StatusRun = false;
                    }
                }

            });

            CloseCommand = new RelayCommand<object>((p) => {
                return true;
            }, (p) => {

                string s = "Do You Want To Close";
                if (MessageWindow.ShowMessage(s) == MessageBoxResult.Yes)
                {

                    SaveFile();
                    (p as Window).Close();
                }

            });



            HideCommand = new RelayCommand<object>((p) => { return true; }, (p) => {

                (p as Window).WindowState = WindowState.Minimized;

            });
        }

        public void CloseAll()
        {
            if (ExcelPath != "" && _codeList.Count > 0 && counterNotReset < _codeList.Count)
            {
                TextFileProcess.WriteFile(IPAddress + "_excelPath", new List<string> { ExcelPath, CounterText,counterNotReset.ToString()});
            }
        }

        private void LoadExcel()
        {
            try
            {


                List<string> ls = TextFileProcess.ReadFile(IPAddress + "_excelPath");
                if (ls.Count != 0)
                {

                    string s =IPAddress+ "_A Processing Found, Do You Want To Continue?";
                    if (MessageWindow.ShowMessage(s) == MessageBoxResult.Yes)
                    {

                        ExcelPath = ls[0];
                        CounterValue = int.Parse(ls[1]);
                        i = int.Parse(ls[2]);
                        if (ExcelPath.Split('.')[1] == "xlsx" || ExcelPath.Split('.')[1] == "xls")
                        {
                            Task.Run(new Action(() =>
                            {
                                try
                                {
                                    _codeList = ImportExcel.import_start(ExcelPath);
                                }
                                catch { }
                            })).ContinueWith(_ =>
                            {
                                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    ManualDialog md = new ManualDialog();
                                    if (_codeList.Count > 0)
                                    {
                                        md.ManualShow(0, IPAddress + "_Load File Excel", "Done, Number Of File:" + _codeList.Count.ToString());
                                    }
                                    else
                                    {
                                        md.ManualShow(1, IPAddress + "_Load File Excel", "Fail");
                                    }
                                }));
                            });
                        }
                        else if (ExcelPath.Split('.')[1] == "txt")
                        {
                            Task.Run(new Action(() =>
                            {
                                try
                                {
                                    _codeList = TextFileProcess.ReadFileExtent(ExcelPath);
                                }
                                catch { }
                            })).ContinueWith(_ =>
                            {
                                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    ManualDialog md = new ManualDialog();
                                    if (_codeList.Count > 0)
                                    {
                                        md.ManualShow(0, IPAddress + "_Load File Text", "Done, Number Of File:" + _codeList.Count.ToString());
                                    }
                                    else
                                    {
                                        md.ManualShow(1, IPAddress + "_Load File Text", "Fail");
                                    }
                                }));
                            });
                        }
                    }
                    else
                    {
                        rowStackPanel.Visibility = Visibility.Visible;
                    }
                    TextFileProcess.DeleteFile(IPAddress + "_excelPath");
                }
            }
            catch { }
        }
        private void RunExcute()
        {
            if (StatusRun)
            {
                try
                {
                    if (i >= _codeList.Count)
                    {
                        return;
                    }
                    if (done)
                    {

                        done = false;
                        WriteData(_codeList[i]);
                        i++;

                    }

                    

                }
                catch (Exception e)
                {
                    ResponseMessage = e.Message;

                    StatusConnect = false;
                }
            }
        }

        private void SaveFile()
        {
            List<string> ls = new List<string>();
            ls.Add(IPAddress);
            ls.Add(Port);
            ls.Add(Speed);
            TextFileProcess.WriteFile("config", ls);
        }

        private void LoadFile()
        {
            List<string> ls = TextFileProcess.ReadFile("config");
            IPAddress = ls[0];
            Port = ls[1];
            Speed = ls[2];
        }

        private void CheckDate()
        {
            List<string> ls = TextFileProcess.ReadFile("DeadTime");
            MainWindow.datecheck = DateTime.Parse(ls[0]);
        }

        private void WriteDate()
        {
            MainWindow.datecheck = DateTime.Now;
            List<string> ls = new List<string>();
            ls.Add(MainWindow.datecheck.ToLongDateString());
            TextFileProcess.WriteFile("DeadTime", ls);
        }











        #endregion

        #region  Event Handler


        #endregion

        #region Method

        //void LoadTextFile()
        //{
        //    MainWindow.lsText = TextFileProcess.ReadFile("CODES");
        //}



        string Connect(string ip, string port)
        {

            string connect = "connect fail";
            client = new SimpleTcpClient();
            client.StringEncoder = Encoding.ASCII;
            client.Connect(ip, int.Parse(port));
            client.DataReceived += Client_DataReceived;

            connect = ($"Connected to {ip}:{port}");
            return connect;
        }
        int kt = 0;
        private bool checkonetime=true;

        void Client_DataReceived(object sender, Message e)
        {
           
            if (e.MessageString == "\u001b\u000f")
            {
                done = true;
                CounterValue++;
                counterNotReset++;
                if (StatusRun)
                {
                    RunExcute();
                }
                //if (kt == 2)
                //{
                //    for (int j = 0; j < 2; j++)
                //    {
                //        CounterValue++;
                //        RunExcute();
                //    }
                //    kt = 0;
                //}
                //else
                //{
                //    CounterValue++;
                //    kt++;
                //}

            }
            else
            {
                utf8String = Encoding.UTF8.GetBytes(e.MessageString);
                ResponseMessage = BitConverter.ToString(utf8String);
            }
            
        }

        void WriteData(string data)
        {
            var numbyte = 7 + MainWindow.numChar;
            var b = new byte[numbyte];
            //LEN 2 bytes
            b[0] = 27; // 1B
            b[1] = 02; // 02

            b[2] = 29; // 1D

            b[3] = (byte)MainWindow.numChar; // data length
            b[4] = 00;

            //byte data
            for(int i = 0; i < MainWindow.numChar; i++)
            {
                b[i + 5] = (byte)data[i];
            }
            //b[5] = (byte)data[0];
            //b[6] = (byte)data[1];
            //b[7] = (byte)data[2];
            //b[8] = (byte)data[3];
            //b[9] = (byte)data[4];
            //b[10] = (byte)data[5];
            //b[11] = (byte)data[6];
            //b[12] = (byte)data[7];

            b[numbyte-2] = 27; // 1B
            b[numbyte-1] = 03; //ETX

            client.Write(b);
            ContentPrint = ($"{data}");
        }



        #endregion


    }

}
