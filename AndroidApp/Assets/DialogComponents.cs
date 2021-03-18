using System;
using Android.Content;
using Android.Views;
using AndroidX.AppCompat.App;

namespace AndroidApp.Assets
{
    class DialogComponents
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