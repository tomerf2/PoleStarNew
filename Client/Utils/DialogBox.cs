﻿using System;
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

        public async static Task<Windows.UI.Popups.IUICommand> ShowYesNo(string caption, string text, string yesBtnText, string noBtnText)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(text, caption);

            dialog.Commands.Add(new Windows.UI.Popups.UICommand(yesBtnText) { Id = 0 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand(noBtnText) { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            return await dialog.ShowAsync();
        }
    }
}
