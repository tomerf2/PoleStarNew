using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
//using SDKTemplate;
using Windows.Devices.Sms;

namespace PoleStar.Utils
{
    class SMS
    {

        private static SmsDevice2 device;
        

        public static async void sendSMS(String to, String text)
        {
            if (device == null)
            {
                try
                {
                    //DialogBox.ShowOk("Notification", "Getting default SMS device ...");
                    device = SmsDevice2.GetDefault();
                }
                catch (Exception ex)
                {
                    //rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
                    DialogBox.ShowOk("Error", ex.Message);
                    return;
                }
        }

            string msgStr = "";
            if (device != null)
            {
                try
                {
                    // Create a text message - set the entered destination number and message text.
                    SmsTextMessage2 msg = new SmsTextMessage2();
                    msg.To = to;
                    msg.Body = text;

                    // Send the message asynchronously
                    //rootPage.NotifyUser("Sending message ...", NotifyType.StatusMessage);
                    SmsSendMessageResult result = await device.SendMessageAndGetResultAsync(msg);

                    if (result.IsSuccessful)
                    {
                        msgStr = "";
                        msgStr += "Text message sent, cellularClass: " + result.CellularClass.ToString();
                        IReadOnlyList<Int32> messageReferenceNumbers = result.MessageReferenceNumbers;

                        for (int i = 0; i < messageReferenceNumbers.Count; i++)
                        {
                            msgStr += "\n\t\tMessageReferenceNumber[" + i.ToString() + "]: " + messageReferenceNumbers[i].ToString();
                        }
                        DialogBox.ShowOk("Success", msgStr);//change??
                    }
                    else
                    {
                        msgStr = "";
                        msgStr += "ModemErrorCode: " + result.ModemErrorCode.ToString();
                        msgStr += "\nIsErrorTransient: " + result.IsErrorTransient.ToString();
                        if (result.ModemErrorCode == SmsModemErrorCode.MessagingNetworkError)
                        {
                            msgStr += "\n\tNetworkCauseCode: " + result.NetworkCauseCode.ToString();

                            if (result.CellularClass == CellularClass.Cdma)
                            {
                                msgStr += "\n\tTransportFailureCause: " + result.TransportFailureCause.ToString();
                            }
                            DialogBox.ShowOk("Error", msgStr);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DialogBox.ShowOk("Error", ex.Message);
                }
            }
            else
            {
                DialogBox.ShowOk("Error", "Failed to find SMS device");
            }
        }
        
    }
}

