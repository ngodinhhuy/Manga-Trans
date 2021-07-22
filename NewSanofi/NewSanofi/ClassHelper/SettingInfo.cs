using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSanofi.ClassHelper
{
    public class SettingInfo : INotifyPropertyChanged
    {
        private static SettingInfo settingInfo;
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler typechanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
                NotifyPropertyChanged("Type");
                if (typechanged != null)
                {
                    typechanged(this, new PropertyChangedEventArgs(value));
                }
            }
        }
        public string _Type = "Operator";

        private SettingInfo() { }
        public static SettingInfo SettingInstance
        {
            get
            {
                if (settingInfo == null)
                {
                    settingInfo = new SettingInfo();
                }
                return settingInfo;
            }
        }

        public int RecordModeIndex
        {
            get { return _RecordModeIndex; }
            set
            {
                _RecordModeIndex = value;
                NotifyPropertyChanged("RecordModeIndex");
            }
        }
        public int _RecordModeIndex =2;

        public string WathCell
        {
            get { return _WathCell; }
            set
            {
                _WathCell = value;
                NotifyPropertyChanged("WathCell");
            }
        }
        public string _WathCell = "A117";

        public string UserCell
        {
            get { return _UserCell; }
            set
            {
                _UserCell = value;
                NotifyPropertyChanged("UserCell");
            }
        }
        public string _UserCell = "E177";

        public string IpAddress
        {
            get { return _IpAddress; }
            set
            {
                _IpAddress = value;
                NotifyPropertyChanged("IpAddress");
            }
        }
        public string _IpAddress = "192.168.1.2";


        public string RecordFolderPath
        {
            get { return _RecordFolderPath; }
            set
            {
                _RecordFolderPath = value;
                NotifyPropertyChanged("RecordFolderPath");
            }
        }
        public string _RecordFolderPath = "";

        public string PlaybackFolderPath
        {
            get { return _PlaybackFolderPath; }
            set
            {
                _PlaybackFolderPath = value;
                NotifyPropertyChanged("PlaybackFolderPath");
            }
        }
        public string _PlaybackFolderPath = "";

        public bool RecordCheck
        {
            get { return _RecordCheck; }
            set
            {
                _RecordCheck = value;
                NotifyPropertyChanged("RecordCheck");
            }
        }
        public bool _RecordCheck = false;

        public bool UserCellCheck
        {
            get { return _UserCellCheck; }
            set
            {
                _UserCellCheck = value;
                NotifyPropertyChanged("UserCellCheck");
            }
        }
        public bool _UserCellCheck = false;

        public int LogoutTime
        {
            get { return _LogoutTime; }
            set
            {
                _LogoutTime = value;
                NotifyPropertyChanged("LogoutTime");
            }
        }
        public int _LogoutTime = 2;

        public bool FullScreen
        {
            get { return _FullScreen; }
            set
            {
                _FullScreen = value;
                NotifyPropertyChanged("FullScreen");
            }
        }
        public bool _FullScreen = false;

       


    }
}
