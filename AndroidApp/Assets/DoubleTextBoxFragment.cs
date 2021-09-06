using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using AndroidX.Fragment.App;
using AndroidX.AppCompat.App;
using Android.App;

namespace AndroidApp.Assets
{
    public class DoubleTextBoxFragment : AndroidX.Fragment.App.DialogFragment
    {
        private View view;
        public delegate void OnPositive(EditText title_field,
        EditText desc_field);
        public delegate void OnDismissDelegate();
        public event OnPositive onpositive;
        public event OnDismissDelegate dismiss;
        public DoubleTextBoxFragment( OnPositive positive)
        {
            onpositive += positive;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreateDialog(savedInstanceState);
            AndroidX.AppCompat.App.AlertDialog.Builder builder = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
            view = Activity.LayoutInflater.Inflate(Resource.Layout.dialog_wordenter, null);
            builder.SetView(view).SetTitle("単語を登録")
                .SetNegativeButton(Resource.String.dialog_cancel, Cancel)
                .SetPositiveButton(Resource.String.dialog_register, (a, b) => { onpositive.Invoke(view.FindViewById<EditText>(Resource.Id.title_textfield),
                     view.FindViewById<EditText>(Resource.Id.description_textfield)); });

            return builder.Create();
        }
        public void Cancel(object s, DialogClickEventArgs args)
        {
            Dismiss();
        }
        public override void OnDismiss(IDialogInterface dialog)
        {
            dismiss?.Invoke();
            base.OnDismiss(dialog);
        }
    }
}