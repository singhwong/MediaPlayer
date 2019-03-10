using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VideoPlayer.Modle;
using VideoPlayer.Modles;
using Windows.Storage;

namespace VideoPlayer.ViewModle
{
    public class SaveProgressVM
    {
        public static void SaveData(List<Progress> list,string path)
        {
            var writer = new FileStream(path,FileMode.Create);
            var ser = new DataContractSerializer(typeof(List<Progress>));
            ser.WriteObject(writer,list);
            writer.Dispose();
        }

        public static List<Progress> ReadData(string path)
        {
            var objectProgress = new List<Progress>();
            var fs = new FileStream(path, FileMode.Open);
            var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            var ser = new DataContractSerializer(typeof(List<Progress>));
            objectProgress = ser.ReadObject(reader, true) as List<Progress>;
            reader.Dispose();
            fs.Dispose();
            return objectProgress;
        }

        public static void ReadProgressData(Video video, List<Progress> list,string path)
        {
            if (File.Exists(path))
            {
                list = SaveProgressVM.ReadData(path);
                foreach (var progress in list)
                {
                    if (progress.Path == video.Video_Path)
                    {
                        video.History_progress = progress.Value;
                    }
                }
            }
        }
    }  
}
