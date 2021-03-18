using Android.Content;
using AndroidX.AppCompat.App;
using System;

namespace AndroidApp.Assets
{
    internal class DialogComponents
    {

        public static void ShowWarning(string title, string message, Context builder)
        {
            AlertDialog.Builder a = new AlertDialog.Builder(builder);
            a.SetTitle(title);
            a.SetMessage(message);
            a.SetPositiveButton("OK", (sb, f) => { });
            a.Show();
        }
        public static void ShowWarning(string title, string message, Context builder, EventHandler<DialogClickEventArgs> positive, EventHandler<DialogClickEventArgs> negative, string positivemessage, string negativemessage)
        {
            AlertDialog.Builder a = new AlertDialog.Builder(builder);
            a.SetTitle(title);
            a.SetMessage(message);
            a.SetPositiveButton(positivemessage, positive);
            a.SetNegativeButton(negativemessage, negative);
            a.Show();
        }
    }
}