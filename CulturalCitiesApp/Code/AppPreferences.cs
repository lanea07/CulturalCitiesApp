using System;
using System.Collections;
using System.Collections.Generic;
using Android.Content;
using Android.Preferences;

namespace CulturalCitiesApp
{
    public class AppPreferences
    {
        private ISharedPreferences nameSharedPrefs;
        private ISharedPreferencesEditor namePrefsEditor; //Declare Context,Prefrences name and Editor name
        private Context mContext;
        private static String PREFERENCE_USERNAME= "Username"; //Value Access Key Name
        private static String PREFERENCE_USERID = "UserID"; //Value Access Key Name
        public static String NAME = "NAME"; //Value Variable Name
        public AppPreferences(Context context)
        {
            this.mContext = context;
            nameSharedPrefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
            namePrefsEditor = nameSharedPrefs.Edit();
        }
        public void saveAccessKey(Dictionary<string, string> prefObject) // Save data Values
        {
            namePrefsEditor.PutString(PREFERENCE_USERNAME, prefObject["username"]);
            namePrefsEditor.PutString(PREFERENCE_USERID, prefObject["userid"]);
            namePrefsEditor.Commit();
        }
        public Dictionary<string, string> getAccessKey() // Return Get the Value
        {
            return new Dictionary<string, string>
            {
                { "userID", nameSharedPrefs.GetString(PREFERENCE_USERID, "") },
                { "userName", nameSharedPrefs.GetString(PREFERENCE_USERNAME, "") }
            };
        }
    }
}