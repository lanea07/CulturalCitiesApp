using System;
using Android.Content;
using Android.Preferences;

namespace CulturalCitiesApp
{
    public class AppPreferences
    {
        private ISharedPreferences nameSharedPrefs;
        private ISharedPreferencesEditor namePrefsEditor; //Declare Context,Prefrences name and Editor name
        private Context mContext;
        private static String PREFERENCE_ACCESS_KEY = "UserId"; //Value Access Key Name
        public static String NAME = "NAME"; //Value Variable Name
        public AppPreferences(Context context)
        {
            this.mContext = context;
            nameSharedPrefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
            namePrefsEditor = nameSharedPrefs.Edit();
        }
        public void saveAccessKey(string key) // Save data Values
        {
            namePrefsEditor.PutString(PREFERENCE_ACCESS_KEY, key);
            namePrefsEditor.Commit();
        }
        public string getAccessKey() // Return Get the Value
        {
            return nameSharedPrefs.GetString(PREFERENCE_ACCESS_KEY, "");
        }
    }
}