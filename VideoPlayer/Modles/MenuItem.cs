using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace VideoPlayer.Modle
{
    public class MenuItem : INotifyPropertyChanged
    {
        public FontFamily FontFamily { get; set; }
        public string Icon { get; set; }
        public string Label { get; set; }
        public int index { get; set; }
        public SolidColorBrush ColorBrush{ get; set; }
        public Visibility selected;
        public Visibility Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                this.OnPropertyChanged("Selected");
            }
        }
        //public LeftListViewData(string icon,string text)
        //{
        //    Icon = icon;
        //    Text = text;
        //}
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SetMenuItemData
    {
        private static ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
        //private static string openFile_str = resourceLoader.GetString("openFile_str");
        private static string videoList_str = resourceLoader.GetString("videoFolder_str");
        private static string addFolder_str = resourceLoader.GetString("addFolder_str");
        private static string clearVideos_str = resourceLoader.GetString("ClearAll_str");
        private static string theme_str = resourceLoader.GetString("Theme_str");
        private static string setting_str = resourceLoader.GetString("setting_str");
        private static string transcode_str = resourceLoader.GetString("transcode_str");
        private static SolidColorBrush lightGray = new SolidColorBrush(Colors.LightGray);
        public ObservableCollection<MenuItem> GetMenuItemData = new ObservableCollection<MenuItem>(
            new[] {

                //new MenuItem()
                //{
                //    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                //    Icon = "\xE8FD",
                //    Label = "M-Player",
                //    index = 0,
                //    //ColorBrush = lightGray,
                //    selected = Visibility.Collapsed
                //},
                // new MenuItem()
                //{
                //    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                //    Icon = "\xE8E5",
                //    Label = openFile_str,
                //    index = 0,
                //    //ColorBrush = lightGray,
                //    selected = Visibility.Collapsed
                //},
                  new MenuItem()
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Icon = "\xE72C",
                    Label = videoList_str,
                    index = 1,
                    //ColorBrush = lightGray,
                    selected = Visibility.Collapsed
                },
                   new MenuItem()
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Icon = "\xE8B7",
                    Label = addFolder_str,
                    index = 2,
                    //ColorBrush = lightGray,
                    selected = Visibility.Collapsed
                },
                //    new MenuItem()
                //{
                //    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                //    Icon = "\xE74D",
                //    Label = clearVideos_str,
                //    index = 3,
                //    //ColorBrush = lightGray,
                //    selected = Visibility.Collapsed
                //},
                    new MenuItem()
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Icon = "\xE793",
                    Label = theme_str,
                    index = 4,
                    //ColorBrush = lightGray,
                    selected = Visibility.Collapsed
                },
                        
                });
        public List<MenuItem> GetSettingMneuData = new List<MenuItem>
            (
            new[]
            {
                new MenuItem()
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Icon = "\xE8AB",
                    Label = transcode_str,
                    index = 0,
                    //ColorBrush = lightGray,
                    selected = Visibility.Collapsed
                },
                // new MenuItem()
                //{
                //    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                //    Icon = "\xEB9F",
                //    Label = "截图",
                //    index = 1,
                //    //ColorBrush = lightGray,
                //    selected = Visibility.Collapsed
                //},
                 new MenuItem()
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Icon = "\uE713",
                    Label = setting_str,
                    index = 2,
                    //ColorBrush = lightGray,
                    selected = Visibility.Collapsed
                },
            }
            );

    }
}
