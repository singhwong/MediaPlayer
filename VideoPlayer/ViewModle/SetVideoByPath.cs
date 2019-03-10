using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace VideoPlayer.Modles
{
    class SetVideoByPath
    {
        public static Video GetVideoByStream(ObservableCollection<Video> video, string path)
        {
            Video video_value = new Video();
            foreach (var item in video)
            {
                if (item.Video_Path == path)
                {
                    video_value = item;
                }
            }
            return video_value;
        }
    }
}
