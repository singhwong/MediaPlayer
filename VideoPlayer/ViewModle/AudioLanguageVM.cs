using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace VideoPlayer.ViewModle
{
    class AudioLanguageVM
    {
        public static List<string> SetAudioLanguage(MediaElement media)
        {
            //bool wasLanguageSet = false;
            List<string> audioLanguage_list = new List<string>();
            for (int index = 0; index < media.AudioStreamCount; index++)
            {
                //if (media.GetAudioStreamLanguage(index) == "en")
                //{
                //media.AudioStreamIndex = index;
                //wasLanguageSet = true;
                //}
                string str = media.GetAudioStreamLanguage(index).ToString();
                if (str == "")
                {
                    str = "默认";
                }
                audioLanguage_list.Add(str);
            }

            //return wasLanguageSet;
            return audioLanguage_list;
        }
    }
}
