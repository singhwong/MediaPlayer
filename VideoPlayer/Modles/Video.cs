using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace VideoPlayer.Modles
{
    public class Video
    {
        public string Title { get; set; }
        public string Video_Path { get; set; }
        public BitmapImage Cover { get; set; }
        public StorageFile VideoFile { get; set; }
        public int id { get; set; }
        public SolidColorBrush ForeGround { get; set; }
        public Color Video_Color { get; set; }
        public IRandomAccessStream Video_Stream { get; set; }
        public string Video_Duration { get; set; }
        public TimeSpan Duration { get; set; }
        //public TimeSpan Progress_duration { get; set; }
        public double History_progress { get; set; }
        public bool IsSelected { get; set; }
        //public int Progress_num { get; set; }
    }
    //main_video.History_progress = History_Progress.GetHistroyProgress(main_mediaElement.Position, main_video.Duration);
    public class History_Progress
    {
        public static void GetHistroyProgress(Video progress_vieo,MediaElement media)
        {
            //progress_vieo.Progress_duration = media.Position;
            progress_vieo.History_progress = (media.Position / progress_vieo.Duration) * 100;
        }

        //public static void GetCurrentPosition(double value,MediaElement media,Video video)
        //{
        //    media.Position = (value / 100) * video.Duration;
        //}
        //private static DispatcherTimer tmr = new DispatcherTimer();
        //public static void SetProgressTimerTick()
        //{
        //    tmr.Interval = TimeSpan.FromSeconds(0.1);
        //    tmr.Tick += OnTimerTick;
        //    tmr.Start();
        //}

        //private static void OnTimerTick(object sender,object args)
        //{
            
        //}

        //private void SetCurrentProgressValue(Video progress_video,MediaElement media,TimeS)
        //{
        //    progress_video.History_progress = History_Progress.GetHistroyProgress();
        //}
    }
}
