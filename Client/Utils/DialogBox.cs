using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoleStar.Utils
{
    public class DialogBox
    {
        public async static void ShowOk(string caption, string text)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(text, caption);

            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Ok") { Id = 0 });
            dialog.CancelCommandIndex = 0;

            var result = await dialog.ShowAsync();
        }

        /*public async static void ShowYesNo(string caption, string text)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(text, caption);

            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            //var btn = sender as Button;
            //btn.Content = $"Result: {result.Label} ({result.Id})";
        }*/
    }
}
