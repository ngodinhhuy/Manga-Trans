using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSanofi.ClassHelper
{
    public class SizeStatus : INotifyPropertyChanged
    {
        private static SizeStatus sizeStatus;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        

        private SizeStatus() { }
        public static SizeStatus SizeStatusInstance
        {
            get
            {
                if (sizeStatus == null)
                {
                    sizeStatus = new SizeStatus();
                }
                return sizeStatus;
            }
        }

        
        public int FolderSize
        {
            get { return _FolderSize; }
            set
            {
                _FolderSize = value;
                NotifyPropertyChanged("FolderSize");
            }
        }
        public int _FolderSize = 0;

        public int MaximumFolderSize
        {
            get { return _MaximumFolderSize; }
            set
            {
                _MaximumFolderSize = value;
                NotifyPropertyChanged("MaximumFolderSize");
            }
        }
        public int _MaximumFolderSize = 0;

        public long DiskSize
        {
            get { return _DiskSize; }
            set
            {
                _DiskSize = value;
                NotifyPropertyChanged("DiskSize");
            }
        }
        public long _DiskSize = 0;

        public long MaxDiskSize
        {
            get { return _MaxDiskSize; }
            set
            {
                _MaxDiskSize = value;
                NotifyPropertyChanged("MaxDiskSize");
            }
        }
        public long _MaxDiskSize = 0;

    }
}
