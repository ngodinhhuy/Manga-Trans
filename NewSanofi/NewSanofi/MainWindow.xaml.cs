using NewSanofi.Models;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace NewSanofi
{
    /// <summary>
    /// `Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Field
        public static string currentDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        public static List<string> lsText = new List<string>();

        public static DateTime datecheck;
        public static int numCache=-1;
        public static int numChar = 0;
        public static List<DatabaseInfo> DatabaseInfos = new List<DatabaseInfo>();
        #endregion

        #region Construction
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                LoadNumCache();
            }
            catch { }
           
        }
        #endregion


        #region Method
        public static bool IsConnected(Socket client)
        {
            bool blockingState = client.Blocking;

            try
            {
                byte[] tmp = new byte[1];

                client.Blocking = false;
                client.Send(tmp, 0, 0);
                return true;
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK
                if (e.NativeErrorCode.Equals(10035))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                client.Blocking = blockingState;
            }
        }
        public static FrameworkElement GetTabItemParent(FrameworkElement p)
        {
            FrameworkElement parent = p;

            while (parent.Parent != null)
            {
                if (parent.Parent is TabItem)
                    return parent.Parent as FrameworkElement;
                parent = parent.Parent as FrameworkElement;
            }

            return parent;
        }

        private void LoadNumCache()
        {
            List<string> ls = ClassHelper.TextFileProcess.ReadFile("NumCache");
            numCache = int.Parse(ls[0].ToString());
            numChar = int.Parse(ls[1].ToLower());
        }


        #endregion

    }


}
