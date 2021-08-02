using IronOcr;
using NewSanofi.ClassHelper;
using NewSanofi.Models;
using NewSanofi.UserControls;
using NewSanofi.Windows;
using SimpleTCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Panel = System.Windows.Forms.Panel;

namespace NewSanofi.ViewModel
{
    public  class MainViewModel : BaseViewModel
    {
        #region Field
        private DatabaseInfo DatabaseInfoServer;
        PictureBox pictureBox;
        PictureBox pictureBoxPreview;
        Bitmap picwgraphic,picNGraphic;
        Graphics graphicsPic;
        Region region;
        List<System.Drawing.Point> polyPoints=new List<System.Drawing.Point>();
        MainWindow mw;
        List<string> PostIdList = new List<string>();
        private string _PostCode = "";
        public string PostCode
        {
            get => _PostCode; set
            {
                _PostCode = value;
                OnPropertyChanged();
            }
        }

        private string _TransText = "";
        public string TransText
        {
            get => _TransText; set
            {
                _TransText = value;
                OnPropertyChanged();
            }
        }

        private string _OCRText = "";
        public string OCRText
        {
            get => _OCRText; set
            {
                _OCRText = value;
                OnPropertyChanged();
            }
        }

        private string _TitleApp = "Chức Năng";
        public string TitleApp
        {
            get => _TitleApp; set
            {
                _TitleApp = value;
                OnPropertyChanged();
            }
        }

        private string _ColorText = "";
        public string ColorText
        {
            get => _ColorText; set
            {
                _ColorText = value;
                OnPropertyChanged();
            }
        }

        private string _StatusText = "";
        public string StatusText
        {
            get => _StatusText; set
            {
                _StatusText = value;
                OnPropertyChanged();
            }
        }

        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
    

        private int _CounterText = 0;
        public int CounterText
        {
            get => _CounterText; set
            {
                _CounterText = value;
                OnPropertyChanged();
            }
        }


        private string _PostCodeLock = "";
        public string PostCodeLock
        {
            get => _PostCodeLock; set
            {
                _PostCodeLock = value;
                OnPropertyChanged();
            }
        }

        private string _ItemCode = "";
        public string ItemCode
        {
            get => _ItemCode; set
            {
                _ItemCode = value;
                OnPropertyChanged();
            }
        }


        private string _BatchCode = "";
        public string BatchCode
        {
            get => _BatchCode; set
            {
                _BatchCode = value;
                OnPropertyChanged();
            }
        }


        private int _ToolSelectedIndex = -1;
        public int ToolSelectedIndex
        {
            get => _ToolSelectedIndex; set
            {
                _ToolSelectedIndex = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<string> _ToolList = new ObservableCollection<string>();
        public ObservableCollection<string> ToolList
        {
            get => _ToolList; set
            {
                _ToolList = value;
                OnPropertyChanged();
            }
        }
        string[] inputTextList;

        private string _InputText = "";
        public string InputText
        {
            get => _InputText; set
            {
                _InputText = value;
                OnPropertyChanged();
            }
        }


        private string _OutputText = "";
        public string OutputText
        {
            get => _OutputText; set
            {
                _OutputText = value;
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

        private string _ContentStatus = "";
        public string ContentStatus
        {
            get => _ContentStatus; set
            {
                _ContentStatus = value;

                OnPropertyChanged();
            }
        }

        private ObservableCollection<TabItem> _TabControler;
        public ObservableCollection<TabItem> TabControler
        {
            get => _TabControler; set
            {
                _TabControler = value;

                OnPropertyChanged();
            }
        }
                
        private WindowState _WindowsState =  WindowState.Normal;
        public WindowState WindowsState
        {
            get => _WindowsState; set
            {
                _WindowsState = value;

                OnPropertyChanged();
            }
        }

        private int _SelectedNumberCache = -1;
        public int SelectedNumberCache
        {
            get => _SelectedNumberCache; set
            {
                _SelectedNumberCache = value;
                MainWindow.numCache = value;
                OnPropertyChanged();
            }
        }

        private int _TypeIndex = -1;
        public int TypeIndex
        {
            get => _TypeIndex; set
            {
                _TypeIndex = value;
                OnPropertyChanged();
            }
        }

        private int _NumberChar = 0;
        public int NumberChar
        {
            get => _NumberChar; set
            {
                _NumberChar = value;
                MainWindow.numChar = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Command

        public ICommand LoadedWindowCommand { get; set; }
        public ICommand TranslateCommand { get; set; }

        public ICommand ColorDetectCommand { get; set; }

        public ICommand SaveImageCommand { get; set; }

        public ICommand OCRCommand { get; set; }

        public ICommand ExecuteCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public ICommand AddAffairCommand { get; set; }
        public ICommand UpdateAffairCommand { get; set; }
        

        public ICommand LockCommand { get; set; }

        public ICommand AutolockCommand { get; set; }
        public ICommand AffairExecuteCommand { get; set; }

        public ICommand UnlockCommand { get; set; }

        public ICommand CloseCommand { get; set; }
        public ICommand HideCommand { get; set; }

        public ICommand ConnectCommand { get; set; }

        public ICommand ExportCommand { get; set; }
        public ICommand ExportNTCommand { get; set; }
        public ICommand ExportQTCommand { get; set; }
        public ICommand GetWeightCommand { get; set; }

        public ICommand FolderChooseCommand { get; set; }

        public ICommand SelectToolChangedCommand { get; set; }


        #endregion

        #region Construction Method
        public MainViewModel()
        {
            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                mw = (p as MainWindow);
                WindowsState = WindowState.Maximized;
                LoadPictureBox();
                LoadDatabaseInfos();
                (mw.ImageLoaderUC.ViewModel as ImageLoaderViewModel).mwm = this;


                ToolList.Add("Affair");

                LoadPostIdList();
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0,30, 0);
                //dispatcherTimer.Start();
            });

            TranslateCommand = new  RelayCommand<object>((p) => {
                if (OCRText == "")
                    return false;
                return true; }, (p) => {

                TransExecute();

            });

            ColorDetectCommand = new RelayCommand<object>((p) => {
                if (polyPoints.Count != 2)
                    return false;
                return true;
            }, (p) => {

                ColorDetectExecute(polyPoints[0], polyPoints[1]);
                polyPoints.Clear();
            });

            SaveImageCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                pictureBox.Image.Save(@"C:\Users\Admin\Desktop\huy.jpg", ImageFormat.Jpeg);
            });



            OCRCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                string text0 = RunPython(StatusText);
                string text1 = text0.Replace("\r\n", " ").Replace('|','I');
                OCRText = text1;
            });





            AffairExecuteCommand = new RelayCommand<object>((p) => {
                if (PostCodeLock == "")
                    return false;
                else
                {
                    if ((ItemCode == "" && BatchCode == ""))
                        return false;
                    else
                        return true;
                }
            }, (p) => {

                if(PostCodeLock!= "670100"&& PostCodeLock != "672630" && PostCodeLock != "672720" && PostCodeLock != "672830" && PostCodeLock != "673340" && PostCodeLock != "673820"
                && PostCodeLock != "674320" && PostCodeLock != "674740" && PostCodeLock != "675230" && PostCodeLock != "675560" && PostCodeLock != "675830" && PostCodeLock != "676030")
                {
                    ManualDialog md = new ManualDialog();
                    md.ManualShow(1, "Lỗi", "Sai Mã Bưu Cục, Chỉ Được Chọn Bưu Cục Khai Thác");
                    return;
                }

                    DatabaseInfoServer = new DatabaseInfo();
                    GetInforServerByPostCode(PostCodeLock);

                if (ItemCode != "")
                {
                    if (!ItemCode.ToLower().Contains("vn"))
                    {
                        ManualDialog md = new ManualDialog();
                        md.ManualShow(1, "Lỗi", "Mã Bưu Gửi Thiếu VN");
                        return;
                    }
                    AffairExecute(DatabaseInfoServer);
                }
                else
                {
                    List<string> ls = GetItemCode(DatabaseInfoServer, BatchCode);
                    foreach (var x in ls)
                    {
                        AffairExecute(DatabaseInfoServer, x);
                    }
                }

                    System.Windows.MessageBox.Show("Xong Nha");

                });


            //SelectToolChangedCommand = new RelayCommand<object>((p) => {
            //    if (ToolSelectedIndex == -1)
            //        return false;
            //    return true; }, (p) => {
            //        switch (ToolSelectedIndex)
            //        {
            //            case 0:
            //                mw.lockgrid.Visibility = Visibility.Visible;
            //                mw.AffairGrid.Visibility = Visibility.Hidden;
            //                break;
            //            case 1:

            //                mw.lockgrid.Visibility = Visibility.Hidden;
            //                mw.AffairGrid.Visibility = Visibility.Visible;

            //                break;
            //        }

            //    });

            ExecuteCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                DatabaseInfoServer = new DatabaseInfo();
                inputTextList = null;
                string resultList = "";
                GetInforServerByPostCode(PostCode);
                ListItemExecute();
                if (inputTextList != null)
                {
                    foreach (string s in inputTextList)
                    {
                        string result = GETName(DatabaseInfoServer, s.Trim(), TypeIndex);
                        if (result == "")
                        {
                            //System.Windows.MessageBox.Show("không có dữ liệu đối với mã bưu gửi: " + s);
                            resultList += "\n";
                        }
                        else
                        {
                            resultList += result + "\n";
                        }
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Please Check Input Text");
                }
                OutputText = resultList;
                System.Windows.MessageBox.Show("DONE");
            });



            DeleteCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                DatabaseInfoServer = new DatabaseInfo();
                GetInforServerByPostCode(PostCode);
                inputTextList = null;
                ListItemExecute();
                if (inputTextList != null)
                {
                    DeleteItem(DatabaseInfoServer, inputTextList);
                }
                System.Windows.MessageBox.Show("DONE");
            });

            AddAffairCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                DatabaseInfoServer = new DatabaseInfo();
                GetInforServerByPostCode(PostCode);
                inputTextList = null;
                ListItemExecute();
                if (inputTextList != null)
                {
                    AddAffair(DatabaseInfoServer, inputTextList);
                }
                System.Windows.MessageBox.Show("DONE");
            });

            UpdateAffairCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                DatabaseInfoServer = new DatabaseInfo();
                GetInforServerByPostCode(PostCode);
                inputTextList = null;
                ListItemExecute();
                if (inputTextList != null)
                {
                    UpdateAffair(DatabaseInfoServer, inputTextList);
                }
                System.Windows.MessageBox.Show("DONE");
            });


            ConnectCommand = new RelayCommand<object>((p) => { return true; }, (p) => {


                GetMayChu();
            });

            LockCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                DatabaseInfoServer = new DatabaseInfo();
                bool fal = false;
                GetInforServerByPostCode(PostCodeLock);
                try
                {
                    LockPost(DatabaseInfoServer);
                }
                catch
                {
                    fal = true;
                }
                //if (fal)
                //    System.Windows.MessageBox.Show("FAIL");
                //else
                //    System.Windows.MessageBox.Show("DONE");

            });

            AutolockCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                //if (DateTime.Now.Hour == 9|| DateTime.Now.Hour == 10|| DateTime.Now.Hour == 11|| DateTime.Now.Hour == 12|| DateTime.Now.Hour == 13|| DateTime.Now.Hour == 14||
                //DateTime.Now.Hour == 15|| DateTime.Now.Hour == 16|| DateTime.Now.Hour == 17)
                //{
                //    foreach (var item in PostIdList)
                //    {
                //        LockCommand.Execute(null);
                //    }
                //}
                foreach (var item in PostIdList)
                {
                    PostCodeLock = item;
                    LockCommand.Execute(null);
                }
                CounterText++;

            });

            UnlockCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                DatabaseInfoServer = new DatabaseInfo();
                GetInforServerByPostCode(PostCodeLock);
                bool fal = false;
                try
                {
                    UnLockPost(DatabaseInfoServer);
                }
                catch
                {
                    fal = true;
                }

            });




            CloseCommand = new RelayCommand<object>((p) => {
                return true; }, (p) => {

                string s = "Do You Want To Close";
                if (MessageWindow.ShowMessage(s) == MessageBoxResult.Yes)
                {
                        SaveDatabaseInfos(MainWindow.DatabaseInfos);
                        (mw.ImageLoaderUC.ViewModel).SaveFolderPath();
                     (p as Window).Close();
                }

            });




            HideCommand = new RelayCommand<object>((p) => {
                
                return true; }, (p) => {
                
                    (p as Window).WindowState = WindowState.Minimized;
            });

           
        }

        private void ColorDetectExecute(System.Drawing.Point a, System.Drawing.Point b)
        {

             Bitmap default_image = new Bitmap(pictureBox.Image);
            var colora = default_image.GetPixel(a.X, a.Y);
            string col1 = colora.G+","+ colora.R + "," + colora.B;
            var colorb = default_image.GetPixel(b.X, b.Y);
            string col2 = colorb.G + "," + colorb.R + "," + colorb.B;
            ColorText = string.Format("Color 1: {0}, Color 2: {1}", col1, col2);
        }

        private void TransExecute()
        {
            string url = String.Format
            ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
             "en", "vi", Uri.EscapeUriString(OCRText));
            HttpClient httpClient = new HttpClient();
            string result = httpClient.GetStringAsync(url).Result;
            var jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);
            var translationItems = jsonData[0];
            string translation = "";
            foreach (object item in translationItems)
            {
                IEnumerable translationLineObject = item as IEnumerable;
                IEnumerator translationLineString = translationLineObject.GetEnumerator();
                translationLineString.MoveNext();
                translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
            }
            if (translation.Length > 1) { translation = translation.Substring(1); };
            TransText= translation;
        }

        private void LoadPictureBox()
        {
            Panel panel = new System.Windows.Forms.Panel();
            panel.AutoScroll = true;
            panel.Dock = DockStyle.Fill;
            pictureBox = new PictureBox();
            pictureBox.MouseClick += PictureBox_MouseClick;
            pictureBox.MouseEnter += PictureBox_MouseEnter;
            pictureBox.MouseLeave += PictureBox_MouseLeave;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            panel.Controls.Add(pictureBox);
            (mw.ImageLoaderUC.ViewModel as ImageLoaderViewModel).pictureBox = pictureBox;
            mw.FormHost.Child = panel;

            pictureBoxPreview = new PictureBox();
            mw.PreviewHost.Child = pictureBoxPreview;
        }

       

        private void PictureBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ColorText = (e.Location.X.ToString() + ',' + e.Location.Y.ToString());
            //var color = GetColor(e.Location);
            //ColorText = RGBConverter(color);

        }

        private System.Drawing.Color GetColor(System.Drawing.Point point)
        {
                using (var bitmap = new Bitmap(1, 1))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(point, new System.Drawing.Point(0, 0), new System.Drawing.Size(1, 1));
                    }
                    return bitmap.GetPixel(0, 0);
                }
            
        }

        private String RGBConverter(System.Drawing.Color c)
        {
            String rtn = String.Empty;
            try
            {
                rtn = "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
            }
            catch (Exception ex)
            {
                //doing nothing
            }

            return rtn;
        }

        public void ChangeImageExecute()
        {
            picNGraphic = new Bitmap(pictureBox.Image);
            Bitmap test = new Bitmap(pictureBox.Width, pictureBox.Height);
            graphicsPic = Graphics.FromImage(test);
            
                graphicsPic.DrawImage(pictureBox.Image, new System.Drawing.Point(0, 0));
                pictureBox.Image = test;
            
        }

        private void PictureBox_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ColorText = (e.Location.X.ToString() + ',' + e.Location.Y.ToString());
                drawPoint(e.Location.X, e.Location.Y);
                polyPoints.Add(e.Location);
                pictureBox.Update();
                pictureBox.Refresh();
            }
            if (e.Button == MouseButtons.Middle)
            {
               // Graphics g = Graphics.FromHwnd(pictureBox.Handle);
               
                GraphicsPath path = new GraphicsPath();
                path.AddPolygon(polyPoints.ToArray());
                region = new Region(path);
                System.Drawing.Pen pen = Pens.Red;
                graphicsPic.DrawPath(pen, path);
                
                graphicsPic.FillRegion(new SolidBrush(System.Drawing.Color.White), region);
                graphicsPic.DrawString("hello", new Font(new System.Drawing.FontFamily("Arial"), 16, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(System.Drawing.Color.Black),region.GetBounds(graphicsPic));
                //graphicsPic.Clear(System.Drawing.Color.Red);
                //MessageBox.Show("clear");
                pictureBox.Update();
                pictureBox.Refresh();
                var cloneRect=GetRecRegion(polyPoints);
                picwgraphic = new Bitmap(pictureBox.Image);
                
                System.Drawing.Imaging.PixelFormat format = picwgraphic.PixelFormat;
                Bitmap cloneBitmap = picNGraphic.Clone(cloneRect, format);
                pictureBoxPreview.Image = cloneBitmap;
                polyPoints.Clear();
                SaveImageToTemp(cloneBitmap);

                // graphicsPic.Clear(System.Drawing.Color.Red);
                
            }
        }

        private void SaveImageToTemp(Bitmap cloneBitmap)
        {
            string result = Path.GetTempPath();
            string fileName = "IMageName.jpg";
            string imagePath = result + fileName;
            cloneBitmap.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            StatusText = imagePath;
        }

        private string RunPython(string args)
        {
            var Ocr = new IronTesseract();
            string result = "";
            using (var Input = new OcrInput(args))
            {
                var Result = Ocr.Read(Input);
                result = Result.Text;
            }
            return result;
        }

        private Rectangle GetRecRegion(List<System.Drawing.Point> polyPoints)
        {
            Rectangle rectangle = new Rectangle();
            List<int> xdimension = new List<int>();
            List<int> ydimension = new List<int>();
            foreach (var point in polyPoints)
            {
                xdimension.Add(point.X);
                ydimension.Add(point.Y);
            }
            var minx = xdimension.Min();
            var maxx = xdimension.Max();
            var miny = ydimension.Min();
            var maxy = ydimension.Max();
            rectangle.X = minx;
            rectangle.Y = miny;
            rectangle.Width = maxx - minx;
            rectangle.Height = maxy - miny;
            return rectangle;
        }

        public void drawPoint(int x, int y)
        {
            // g = Graphics.FromHwnd(pictureBox.Handle);
            SolidBrush brush = new SolidBrush(System.Drawing.Color.Red);
            System.Drawing.Point dPoint = new System.Drawing.Point(x, y);
            Rectangle rect = new Rectangle(dPoint, new System.Drawing.Size(4, 4));
            graphicsPic.FillRectangle(brush, rect);
            
        }

        private void PictureBox_MouseLeave(object sender, EventArgs e)
        {
            //mw.Cursor = System.Windows.Input.Cursors.Arrow;
            //TitleApp = "bye";
        }

        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            //pictureBox.Cursor = new System.Windows.Forms.Cursor("Data/Pen.ico");
        }

        private void AddAffair(DatabaseInfo di, string[] ItemCode)
        {
            string connetionString = string.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3}", di.ServerName, di.DatabaseName, di.UserName, di.Password);
            foreach (string itemtext in ItemCode)
            {
                string sql = string.Format("INSERT INTO TraceItem (TraceIndex, ItemCode, POSCode, Status,TraceDate,StatusDesc,TransferMachine,TransferUser) VALUES(3, '{0}', {1}, 3, '2021-{2}-{3} 20:41:09.460', 'BCCP.CT.', 'DESKTOP-T0T77PO', 'huy'); ",itemtext, PostCode,DateTime.Now.Month,DateTime.Now.Day);

                using (SqlConnection conn = new SqlConnection(connetionString))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                    conn.Close();

                }
            }
        }

        private void UpdateAffair(DatabaseInfo di, string[] ItemCode)
        {
            string connetionString = string.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3}", di.ServerName, di.DatabaseName, di.UserName, di.Password);
            foreach (string itemtext in ItemCode)
            {
                string sql = string.Format("update TraceItem set POSCode = {0} where ItemCode ='{1}' and Status = 3  ",  PostCode, itemtext);

                using (SqlConnection conn = new SqlConnection(connetionString))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                    conn.Close();

                }
            }
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 17)
            {
                foreach (var item in PostIdList)
                {
                    LockCommand.Execute(null);
                }

                CounterText++;

            }

        }

        private void AffairExecute(DatabaseInfo di)
        {

            string connetionString = string.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3}", di.ServerName, di.DatabaseName, di.UserName, di.Password);

            string sql = string.Format ("update Affair set status = 1 where ItemCode = '{0}'",ItemCode);



            using (SqlConnection conn = new SqlConnection(connetionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch { }
                conn.Close();


            }

        }

        private void AffairExecute(DatabaseInfo di,string itemcode1)
        {

            string connetionString = string.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3}", di.ServerName, di.DatabaseName, di.UserName, di.Password);

            string sql = string.Format("update Affair set status = 1 where ItemCode = '{0}'", itemcode1);



            using (SqlConnection conn = new SqlConnection(connetionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch { }
                conn.Close();


            }

        }

        private void UnLockPost(DatabaseInfo di)
        {
            string connetionString = string.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3}", di.ServerName, di.DatabaseName, di.UserName, di.Password);

            string sql = "INSERT [dbo].[ItemType] ([ItemTypeCode], [ItemTypeName], [Description], [OrderIndex], [Type]) VALUES (N'BK', N'Bưu kiện', N'Bưu kiện', 11, 0)" +
                "INSERT [dbo].[ServiceItemType] ([ServiceCode], [ItemTypeCode], [DomesticMaximumWeight], [InternationMaximumWeight], [DomesticMinimumFreight], [InternationMinimumWeight], [DomesticMinimumWeight], [AllowsQuick]) VALUES (N'C', N'BK', 50000, 50000, 0, 0, 0, 1)" +
                "INSERT [dbo].[RangeServiceItemType] ([RangeCode], [ServiceCode], [ItemTypeCode]) VALUES (N'QT', N'C', N'BK')" +
                "INSERT [dbo].[RangeServiceItemType] ([RangeCode], [ServiceCode], [ItemTypeCode]) VALUES (N'TN', N'C', N'BK')"+
                "INSERT [dbo].[ItemType] ([ItemTypeCode], [ItemTypeName], [Description], [OrderIndex], [Type]) VALUES (N'BE', N'TMĐT Economy ', N'TMĐT Economy ', 12, 0)"+
                "INSERT [dbo].[ServiceItemType] ([ServiceCode], [ItemTypeCode], [DomesticMaximumWeight], [InternationMaximumWeight], [DomesticMinimumFreight], [InternationMinimumWeight], [DomesticMinimumWeight], [AllowsQuick]) VALUES (N'C', N'BE', 100000000, 100000000, 1, 1, 1, NULL)"+
                "INSERT [dbo].[ServiceItemType] ([ServiceCode], [ItemTypeCode], [DomesticMaximumWeight], [InternationMaximumWeight], [DomesticMinimumFreight], [InternationMinimumWeight], [DomesticMinimumWeight], [AllowsQuick]) VALUES (N'P', N'BE', 500000000, 500000000, 0, 0, 0, NULL)"+
                "INSERT [dbo].[RangeServiceItemType] ([RangeCode], [ServiceCode], [ItemTypeCode]) VALUES (N'TN', N'C', N'BE')"+
                "INSERT [dbo].[RangeServiceItemType] ([RangeCode], [ServiceCode], [ItemTypeCode]) VALUES (N'TN', N'P', N'BE')";
                


            using (SqlConnection conn = new SqlConnection(connetionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                    conn.Close();
                

            }

        }


        

        private void LockPost(DatabaseInfo di)
        {
            string connetionString = string.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3}", di.ServerName, di.DatabaseName, di.UserName, di.Password);

            string sql = "delete FROM[ServiceItemType] where itemtypecode = 'BK'" + "\n" +
                "INSERT[dbo].[ServiceItemType]([ServiceCode], [ItemTypeCode], [DomesticMaximumWeight], [InternationMaximumWeight], [DomesticMinimumFreight], [InternationMinimumWeight], [DomesticMinimumWeight], [AllowsQuick]) VALUES(N'C', N'BK', 50000, 50000, 0, 0, 0, 1)" + "\n" +
                "delete FROM[ServiceItemType] where itemtypecode = 'BE'" + "\n" +
                "INSERT [dbo].[ServiceItemType] ([ServiceCode], [ItemTypeCode], [DomesticMaximumWeight], [InternationMaximumWeight], [DomesticMinimumFreight], [InternationMinimumWeight], [DomesticMinimumWeight], [AllowsQuick]) VALUES (N'C', N'BE', 100000000, 100000000, 1, 1, 1, NULL)" + "\n" +
                "INSERT [dbo].[ServiceItemType] ([ServiceCode], [ItemTypeCode], [DomesticMaximumWeight], [InternationMaximumWeight], [DomesticMinimumFreight], [InternationMinimumWeight], [DomesticMinimumWeight], [AllowsQuick]) VALUES (N'P', N'BE', 500000000, 500000000, 0, 0, 0, NULL)";

            using (SqlConnection conn = new SqlConnection(connetionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch { }
                conn.Close();

            }
        }

        private void GetInforServerByPostCode(string postCode)
        {
            DatabaseInfoServer = GetDatabaseInfo("", postCode);
        }

        private void ListItemExecute()
        {
            string text = InputText;
            inputTextList = text.Split(new String[] { Environment.NewLine }, StringSplitOptions.None);
            
        }

        private void SaveDatabaseInfos(List<DatabaseInfo> list)
        {
            List<string> ls = new List<string>();
            string row = "";
            foreach(var item in list)
            {
                
                row = item.DatabaseName + "-"+ item.District + "-"+ item.Password + "-"+ item.ServerName + "-"+ item.UserName + "-" + item.Id;
                ls.Add(row);
            }

            TextFileProcess.WriteFile("DatabaseInfos", ls);
        }
        private void LoadDatabaseInfos()
        {
            try
            {
                
                List<string> ls = TextFileProcess.ReadFile("DatabaseInfos");
                foreach(var item in ls)
                {
                    string[] str = item.Split('-');
                    DatabaseInfo databaseInfo = new DatabaseInfo { DatabaseName = str[0], District = str[1], Password = str[2], ServerName = str[3], UserName = str[4],Id=str[5] };
                    MainWindow.DatabaseInfos.Add(databaseInfo);
                }
            }
            catch { }
        }

     
       

        private void SaveFile()
        {
            List<string> ls = new List<string>();
            //ls.Add(IPAddress);
            //ls.Add(Port);
            //ls.Add(Speed);
            TextFileProcess.WriteFile("config", ls);
        }

        private void LoadFile()
        {
            //List<string> ls = TextFileProcess.ReadFile("config");
            //IPAddress = ls[0];
            //Port = ls[1];
            //Speed = ls[2];
        }


        private DataTable ExportToDataTable(string server, string database, string username, string password, int ems)
        {
            ///ems: 1-LT
            ///2-NT
            ///3-QT

            DataTable dt = new DataTable();
            string command = "";
            switch (ems)
            {
                case 1:
                 command = string.Format(" SELECT  ItemCode,	AcceptancePOSCode,	BatchCode,	CountryCode, POSCode, Note, ItemTypeCode,SendingTime,	Weight,	TotalFreight,  MainFreight,	SubFreight,	RemainingFreight,	WeightConvert,	FuelSurchargeFreight,  FarRegionFreight,	OtherFreight,	TotalFreightVAT,	RemainingFreightVAT  FROM  Item  where Month(SendingTime) = {0} and Year(SendingTime)= {1}  and servicecode = 'E' and left(AcceptancePOSCode, 2)= '67'  and left(POScode, 2)!= '67'  and(countryCode is null or countrycode = 'VN')", DateTime.Now.Month - 1 , DateTime.Now.Year);
                    break;
                case 2:
                    command = string.Format(" SELECT  ItemCode,	AcceptancePOSCode,	BatchCode,	CountryCode, POSCode, Note, ItemTypeCode,SendingTime,	Weight,	TotalFreight,  MainFreight,	SubFreight,	RemainingFreight,	WeightConvert,	FuelSurchargeFreight,  FarRegionFreight,	OtherFreight,	TotalFreightVAT,	RemainingFreightVAT  FROM  Item  where Month(SendingTime) = {0} and Year(SendingTime)= {1}  and servicecode = 'E' and left(AcceptancePOSCode, 2)= '67'  and left(POScode, 2)= '67'  and(countryCode is null or countrycode = 'VN')", DateTime.Now.Month - 1, DateTime.Now.Year);
                    break;

                case 3:
                    command = string.Format(" SELECT  ItemCode,	AcceptancePOSCode,	BatchCode,	CountryCode, POSCode, Note, ItemTypeCode,SendingTime,	Weight,	TotalFreight,  MainFreight,	SubFreight,	RemainingFreight,	WeightConvert,	FuelSurchargeFreight,  FarRegionFreight,	OtherFreight,	TotalFreightVAT,	RemainingFreightVAT  FROM  Item  where Month(SendingTime) = {0} and Year(SendingTime)= {1}  and servicecode = 'E' and left(AcceptancePOSCode, 2)= '67'  and left(POScode, 2)!= '67'  and(countrycode != 'VN')", DateTime.Now.Month - 1, DateTime.Now.Year);
                    break;
        }
            string constr = string.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3}", server, database, username, password);
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(command))
                {
                    cmd.CommandTimeout = 900;
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        dt.Load(sdr);
                    }
                    con.Close();
                }
            }
            
            return dt;
        }

     
       













        #endregion

        #region  Event Handler


        #endregion

        #region Method

        void GetMayChu()
        {
            List<string> listbc = TextFileProcess.ReadFile("maychu");
            MainWindow.DatabaseInfos = new List<DatabaseInfo>();
            foreach (var item in listbc)
            {
                var bc = item.Split('-');
                var name = bc[0];
                var ip = bc[1];
                DatabaseInfo database = GetDatabaseInfo(name, ip);
                MainWindow.DatabaseInfos.Add(database);
            }
            ManualDialog manualDialog = new ManualDialog();
            manualDialog.ManualShow(0, "Status", "Done");
        }

        private DatabaseInfo GetDatabaseInfo(string name, string ip)
        {
            DatabaseInfo databaseInfo = new DatabaseInfo();
            databaseInfo.District = name;
            databaseInfo.Id = ip;
            string connetionString;
            SqlConnection cnn;
            connetionString = @"Data Source=10.61.200.9;Initial Catalog=db_BCCP67;User ID=sa;Password=ems";
            cnn = new SqlConnection(connetionString);
            cnn.Open();


            SqlCommand command;
            SqlDataReader dataReader;
            string sql = "";
            sql = string.Format("select MayChuSQL, TenCSDL, TenDangNhap, MatKhau from tblMayChuBCCP where mabc = '{0}'",ip);
            command = new SqlCommand(sql, cnn);
            dataReader = command.ExecuteReader();
            
            while (dataReader.Read())
            {
                databaseInfo.ServerName = dataReader.GetValue(0).ToString();
                databaseInfo.DatabaseName = dataReader.GetValue(1).ToString();
                databaseInfo.UserName = dataReader.GetValue(2).ToString();
                databaseInfo.Password = dataReader.GetValue(3).ToString();
            }
            dataReader.Close();
            cnn.Close();
            return databaseInfo;
        }


        private List<string> GetItemCode(DatabaseInfo di,string batchcode)
        {
            List<string> result = new List<string>();
            string connetionString = string.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3}", di.ServerName, di.DatabaseName, di.UserName, di.Password);

            string sql = string.Format("SELECT ItemCode FROM Item where BatchCode = '{0}'", batchcode);
            SqlCommand command;
            SqlDataReader dataReader;
            SqlConnection cnn;
            cnn = new SqlConnection(connetionString);
            command = new SqlCommand(sql, cnn);
            if (cnn.State == ConnectionState.Closed)
                cnn.Open();
            dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                result.Add(dataReader.GetValue(0).ToString());
            }
            dataReader.Close();
            cnn.Close();


            return result;

        }





        private string getWeight(DatabaseInfo di,int month)
        {
            SqlConnection cnn;
            string connetionString = string.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3}",di.ServerName,di.DatabaseName,di.UserName,di.Password);
            cnn = new SqlConnection(connetionString);
            cnn.Open();


            var monthpast = month - 1;



            SqlCommand command;
            SqlDataReader dataReader;
            string sql = "";
            sql = string.Format(" select count (*) as Tui, sum (Weight) as KhoiLuong FROM[dbo].[PostBag] where Year > 2020{0}00 and Year < 2020{1}00 and ToPOSCode <> '{2}' and FromPoSCode <> '{2}' and  Weight < 1000", ConvertMonthToString(monthpast),ConvertMonthToString(month),di.Id);
            command = new SqlCommand(sql, cnn);
            dataReader = command.ExecuteReader();
            int bagvalue1=0;
            int weightvalue1=0; 

            while (dataReader.Read())
            {
                bagvalue1 = dataReader.GetValue(0).ToString() != "" ? int.Parse(dataReader.GetValue(0).ToString()) : 0;
                weightvalue1 = dataReader.GetValue(1).ToString() != "" ? (int)float.Parse(dataReader.GetValue(1).ToString()) : 0;
            }
            dataReader.Close();

            sql = string.Format("select count (*) as Tui, sum (Weight)/1000 as KhoiLuong FROM[dbo].[PostBag] where Year > 2020{0}00 and Year < 2020{1}00 and ToPOSCode <> '{2}' and FromPoSCode <> '{2}' and  Weight > 1000", ConvertMonthToString(monthpast), ConvertMonthToString(month),di.Id);
            command = new SqlCommand(sql, cnn);
            dataReader = command.ExecuteReader();
            int bagvalue2=0;
            int weightvalue2=0;
            while (dataReader.Read())
            {
                bagvalue2 = dataReader.GetValue(0).ToString() != "" ? int.Parse(dataReader.GetValue(0).ToString()):0;

               
                weightvalue2 = dataReader.GetValue(1).ToString()!=""? (int)float.Parse(dataReader.GetValue(1).ToString()):0;
            }

            dataReader.Close();
            cnn.Close();
            string result = string.Format("{0}: {1} túi kiện/ {2} kg",di.District,(bagvalue1+bagvalue2).ToString(),(weightvalue1+weightvalue2).ToString());
            return result;
        }

        private string ConvertMonthToString(int month)
        {
            if (month < 10)
            {
                return string.Format("0{0}", month.ToString());
            }
            else
            {
                return month.ToString();
            }
        }

        private void DeleteItem(DatabaseInfo di, string[] ItemCode)
        {
            string connetionString = string.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3}", di.ServerName, di.DatabaseName, di.UserName, di.Password);
            foreach (string itemtext in ItemCode)
            {
                string sql = string.Format("delete FROM Item where itemcode = '{0}'", itemtext);
            
            using (SqlConnection conn = new SqlConnection(connetionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                    conn.Close();
                
            }
            }
        }

        private string GETName(DatabaseInfo di, string ItemCode, int item)
        {
            SqlConnection cnn;
            string connetionString = string.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3}", di.ServerName, di.DatabaseName, di.UserName, di.Password);
            cnn = new SqlConnection(connetionString);
            cnn.Open();

            SqlCommand command;
            SqlDataReader dataReader;
            string sql = "";
            string itemtext = "";
            switch (item)
            {
                case 0:
                    itemtext = "SenderFullname";
                    break;
                    case 1:
                    itemtext = "SenderAddress";
                        break;
                case 2:
                    itemtext = "ReceiverFullname";
                    break;
                case 3:
                    itemtext = "ReceiverAddress";
                    break;
                case 4:
                    itemtext = "SendingContent";
                    break;
                case 5:
                    itemtext = "Weight";
                    break;


            }
            sql = string.Format("SELECT {0} FROM Item where itemcode = '{1}'", itemtext, ItemCode);
            command = new SqlCommand(sql, cnn);
            dataReader = command.ExecuteReader();
            string result = "";
            

            while (dataReader.Read())
            {
                result = dataReader.GetValue(0).ToString();
                if (result.Contains('-'))
                    result = result.Replace('-', ' ');
                if (result.Contains('_'))
                    result = result.Replace('_', ' ');
                if (result.Contains("\r\n"))
                {
                    result = result.Remove(result.Length - 2, 2);
                }
            }
            dataReader.Close();

            
            return result;
        }
        private void LoadPostIdList()
        {
            try
            {
                PostIdList = ClassHelper.TextFileProcess.ReadFile("mabc");
            }
            catch { }
        }







        #endregion
    }

}
