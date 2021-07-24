using NewSanofi.ClassHelper;
using NewSanofi.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace NewSanofi.ViewModel
{
    public class ImageLoaderViewModel : BaseViewModel
    {

        #region Field
        ImageLoader imageLoader;
        public PictureBox pictureBox;
        string[] filePaths;
        string CurrentFolder = "";

        private ObservableCollection<string> _ImageData = new ObservableCollection<string>();
        public ObservableCollection<string> ImageData
        {
            get => _ImageData; set
            {
                _ImageData = value;
                OnPropertyChanged();
            }
        }

        private int _ImageIndexSelected = 0;
        public int ImageIndexSelected
        {
            get => _ImageIndexSelected; set
            {
                _ImageIndexSelected = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #region Command

        public ICommand LoadedWindowCommand { get; set; }
        public ICommand ImageSelectChangedCommand { get; set; }
        public ICommand AddImageFolderCommand { get; set; }

        #endregion

        #region Construction Method
        public ImageLoaderViewModel()
        {

            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                imageLoader = (p as ImageLoader);
                LoadFolderPath();
                if (CurrentFolder != "")
                {
                    LoadFolderImage(CurrentFolder);
                }
            });

            ImageSelectChangedCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if ( ImageIndexSelected!=-1)
                {
                    pictureBox.Image = Image.FromFile(ImageData[ImageIndexSelected]);
                }

            });

            AddImageFolderCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        CurrentFolder = fbd.SelectedPath;
                        LoadFolderImage(CurrentFolder);
                    }
                }
            });

        }
        #endregion

        #region event handler
        #endregion

        #region Method

        private void LoadFolderImage(string path)
        {
            ImageData = new ObservableCollection<string>(ExtensionMethod.GetFilesByExtensions(new DirectoryInfo(path), new[] { ".jpg", ".jpeg", ".jpe", ".jfif", ".png" }).Select(x => x.FullName));
        }


        public void SaveFolderPath()
        {
            TextFileProcess.WriteFile("ImageFolderPath", new List<string> { CurrentFolder });
        }

        private void LoadFolderPath()
        {
            try
            {
                CurrentFolder = TextFileProcess.ReadFile("ImageFolderPath")[0];
            }
            catch { }
        }


        #endregion
    }

    public static class ExtensionMethod
    {

        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension));
        }

    }
}
