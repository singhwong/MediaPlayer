using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Services.Store;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace VideoPlayer.ViewModle
{
    public class SetFeedBackClass
    {
        private static SolidColorBrush cornFlowerBlue = new SolidColorBrush(Colors.CornflowerBlue);
        private static SolidColorBrush white = new SolidColorBrush(Colors.White);
        //public static async void SetFeedBack()
        //{
        //    //var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
        //    //await launcher.LaunchAsync();
        //}

        public static async Task ComposeEmail(Contact recipient)//设置邮件方法
        {
            var emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
            //emailMessage.Body = messageBody;
            var email = new ContactEmail()
            {
                Address = "singhwongwxg@hotmail.com",
                Description = "FeedBack",
            };
            //var email = recipient.Emails.FirstOrDefault<Windows.ApplicationModel.Contacts.ContactEmail>();
            if (email != null)
            {
                var emailRecipient = new Windows.ApplicationModel.Email.EmailRecipient(email.Address);
                emailMessage.To.Add(emailRecipient);
                // emailMessage.Subject = subject;
            }

            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }      

        public static async Task<bool> ShowRatingReviewDialog()
        {
            StoreSendRequestResult result = await StoreRequestHelper.SendRequestAsync(
                StoreContext.GetDefault(), 16, String.Empty);

            if (result.ExtendedError == null)
            {
                JObject jsonObject = JObject.Parse(result.Response);
                if (jsonObject.SelectToken("status").ToString() == "success")
                {
                    // The customer rated or reviewed the app.
                    return true;
                }
            }

            // There was an error with the request, or the customer chose not to
            // rate or review the app.
            return false;
        }
    }
}
