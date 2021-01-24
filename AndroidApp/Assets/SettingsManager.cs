
namespace AndroidApp.Assets
{
    class SettingsManager : 
    {
        //private Activity activity;
        //private Button button_removeanswer;
        //private Button button_erashow;
        //private Button button_marubatsuanim;
        //private Button button_deletealldata;
        //public SettingsManager(Button erashow, Button removeanswer, Button marubatsuanim, Button deleteall)
        //{
        //    this.activity = Platform.CurrentActivity;
        //    button_deletealldata = deleteall;
        //    button_erashow = erashow;
        //    button_marubatsuanim = marubatsuanim;
        //    button_removeanswer = removeanswer;
        //}

        //private void OnClick_DeleteAllData()
        //{
        //    DialogComponents.ShowWarning("全データ削除",activity.GetString(Resource.String.dialog_deletealldata),activity,REmove,(a,v)=> { },
        //        activity.GetString(Resource.String.dialog_delete), activity.GetString(Resource.String.dialog_cancel));
        //}

        //public void OnClick_Erashow()
        //{

        //}
    }
    public enum Eras
    {
        WesternCalender,
        JapaneseCalender,        
        JucheCaleneder
    }
}