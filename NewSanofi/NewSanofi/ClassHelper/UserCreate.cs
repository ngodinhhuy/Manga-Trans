using NewSanofi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSanofi
{
    public class UserCreate : BaseViewModel
    {
        private int _Id = 0;
        public int Id
        {
            get => _Id; set
            {
                _Id = value;

                OnPropertyChanged();
            }
        }

        private string _UserName = "";
        public string UserName
        {
            get => _UserName; set
            {
                _UserName = value;

                OnPropertyChanged();
            }
        }

        private string _UserType = "";
        public string UserType
        {
            get => _UserType; set
            {
                _UserType = value;

                OnPropertyChanged();
            }
        }

        private DateTime _DateTimeCreated;
        public DateTime DateTimeCreated
        {
            get => _DateTimeCreated; set
            {
                _DateTimeCreated = value;
                DateCreated = DateTimeCreated.ToString("hh:mm:ss dd/MM/yyyy");
                OnPropertyChanged();
            }
        }

        private string _DateCreated;
        public string DateCreated
        {
            get => _DateCreated; set
            {
                _DateCreated = value;

                OnPropertyChanged();
            }
        }

    }
}
