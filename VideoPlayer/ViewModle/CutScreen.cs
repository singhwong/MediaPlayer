using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using VideoPlayer.Modles;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace VideoPlayer.ViewModle
{
    class CutScreen
    {
        private static ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
        private static string saveSucceed_str = resourceLoader.GetString("saveSucceed_str");
        private static string openAdress_str = resourceLoader.GetString("openAdress_str");
        private static string editPhoto_str = resourceLoader.GetString("editPhoto_str");
        private static string closeMessage_str = resourceLoader.GetString("CloseMessageDialog_str");
        public static async Task<StorageFile> GetScreen(Video video,MediaElement mediaElement)
        {
            string desiredName = video.VideoFile.DisplayName + ".jpg";
            StorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync("M-Player", CreationCollisionOption.OpenIfExists);
            var saveFile = await folder.CreateFileAsync(desiredName, CreationCollisionOption.GenerateUniqueName);
            try
            {
               
                RenderTargetBitmap bitmap = new RenderTargetBitmap();
                await bitmap.RenderAsync(mediaElement);
                var pixelBuffer = await bitmap.GetPixelsAsync();
                using (var fileStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Ignore,
                         (uint)bitmap.PixelWidth,
                         (uint)bitmap.PixelHeight,
                         DisplayInformation.GetForCurrentView().LogicalDpi,
                         DisplayInformation.GetForCurrentView().LogicalDpi,
                         pixelBuffer.ToArray());
                    await encoder.FlushAsync();
                }
                //MessageDialog message = new MessageDialog(saveSucceed_str + saveFile.Path);
                //message.Commands.Add(new UICommand(openAdress_str, async (command) =>
                //{
                //    bool result = await Windows.System.Launcher.LaunchFolderAsync(folder);
                //}));
                //message.Commands.Add(new UICommand(editPhoto_str, async (command) =>
                //{
                //    bool result = await Windows.System.Launcher.LaunchFileAsync(saveFile);
                //}));
                //message.Commands.Add(new UICommand(closeMessage_str, (command) =>
                //{
                //}));
                //message.DefaultCommandIndex = 2;
                //await message.ShowAsync();
                
            }
            catch
            {
            }
            return saveFile;
        }
    }
}
