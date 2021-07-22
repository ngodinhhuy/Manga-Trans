using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSanofi
{
    public class User : INotifyPropertyChanged
    {
        private static User user;
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
        
        private User() { }
        public static User UserInstance
        {
            get
            {
                if (user == null)
                {
                    user = new User();
                }
                return user;
            }
        }
        //public User(bool defaultvalue)
        //{
        //    if(defaultvalue==true)
        //    {
        //        Name = "Anonymous";
        //        Type = "Monitor";
        //    }
        //}

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public string _Name = "Anonymous";

      
        public string _Password = "";
        public string Password
        {
            get { return _Password; }
            set { _Password = value; NotifyPropertyChanged("Password"); }
        }
        public string ID;
    }


    
}
