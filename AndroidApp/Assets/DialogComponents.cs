using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AndroidApp.Assets
{
    class DialogComponents
    {
        private static Android.Support.V7.App.AlertDialog.Builder builder;

        public static void ShowWarning(string title, string message, Context builder)
        {           
            Android.Support.V7.App.AlertDialog.Builder a = new Android.Support.V7.App.AlertDialog.Builder(builder);
            a.SetTitle(title);
            a.SetMessage(message);
            a.SetPositiveButton("OK", (sb, f) => { });
            a.Show();
        }
        public static void ShowWarning(string title, string message, Context builder, EventHandler<DialogClickEventArgs> positive, EventHandler<DialogClickEventArgs> negative, string positivemessage, string negativemessage)
        {
            Android.Support.V7.App.AlertDialog.Builder a = new Android.Support.V7.App.AlertDialog.Builder(builder);
            a.SetTitle(title);
            a.SetMessage(message);
            a.SetPositiveButton(positivemessage, positive);
            a.SetNegativeButton(negativemessage, negative);
            a.Show();
        }
    }
}