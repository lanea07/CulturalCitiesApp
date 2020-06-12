using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using Simple.OData.Client;

namespace CulturalCitiesApp
{

    [Activity(Label = "UserPreferences")]
    public class UserPreferences : Activity
    {

        private Context mContext;
        private AppPreferences ap;
        private Dictionary<string, string> key;
        static private string url = "http://192.168.0.111/culturalcities/webservice/";
        private List<string> currentGenresID = new List<string>();
        private List<string> currentGenresName = new List<string>();
        private ListView lv;
        private List<string> allGenresID = new List<string>();
        private List<string> allGenresName = new List<string>();
        private Button btnSavePreferences;
        private ArrayAdapter<string> adaptador;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            //DECLARACIONES Y ASIGNACIONES

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.preferences_intent);

            AutoCompleteTextView genresSelector = FindViewById<AutoCompleteTextView>(Resource.Id.buscadorGeneros);
            ODataClient clienteHTTP = new ODataClient(url);

            mContext = Android.App.Application.Context;
            ap = new AppPreferences(mContext);
            key = ap.getAccessKey();

            ArrayAdapter<String> adapter;
            ProgressBar progressbar = FindViewById<ProgressBar>(Resource.Id.genresSearchProgresBar);
            btnSavePreferences = FindViewById<Button>(Resource.Id.btnSavePreferences);

            //EVENTOS

            genresSelector.BeforeTextChanged += (sender, args) =>
            {
                progressbar.Visibility = ViewStates.Visible;
            };

            genresSelector.TextChanged += async (sender, args) =>
            {
                try
                {
                    var currentGenres = await clienteHTTP.FindEntriesAsync("tblCustomerPreferences?$filter=customer_id eq 2");
                    allGenresID.Clear();
                    allGenresName.Clear();
                    var genres = await clienteHTTP.FindEntriesAsync("tblGenres?$filter=substringof('" + args.Text + "', name)");
                    foreach (var genre in genres)
                    {
                        allGenresID.Add(genre["genre_id"].ToString());
                        allGenresName.Add(genre["name"].ToString());
                    }
                    adapter = new ArrayAdapter<String>(this, Resource.Layout.list_item, allGenresName);
                    genresSelector.Adapter = adapter;
                    progressbar.Visibility = ViewStates.Invisible;
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };

            genresSelector.ItemClick += (sender, args) =>
            {
                Toast.MakeText(this, genresSelector.Text, ToastLength.Long).Show();
                currentGenresID.Add(allGenresID[allGenresName.FindIndex(x => x.Equals(genresSelector.Text))]);
                currentGenresName.Add(genresSelector.Text);
                adaptador.Add(genresSelector.Text);
                genresSelector.Text = "";
            };

            btnSavePreferences.Click += async (sender, args) =>
            {
                string preferences = string.Join(",", currentGenresID.ToArray());
                var registry = new
                {
                    customer_id = key["userID"],
                    preference_id = 9,
                    preference_value = preferences,
                    //create_time = Convert.ToDateTime(DateTime.Now),
                    update_time = Convert.ToDateTime(DateTime.Now)
                };
                try
                {
                    var result = await clienteHTTP.For("tblCustomerPreferences").Key(new { customer_id = int.Parse(key["userID"]), preference_id = 9 }).Set(registry).UpdateEntryAsync();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
                
            };

        }

        protected async override void OnStart()
        {
            base.OnStart();
            ODataClient clienteHTTP = new ODataClient(url);
            currentGenresID = new List<string>();
            currentGenresName = new List<string>();
            lv = FindViewById<ListView>(Resource.Id.generos);
            List<string> genresArray = new List<string>();
            string filterString;
            try
            {
                var currentGenres = await clienteHTTP.FindEntriesAsync("tblCustomerPreferences?$select=preference_value&$filter=customer_id eq " + key["userID"] + " and preference_id eq 9");
                foreach (var currentGenre in currentGenres)
                {
                    genresArray = currentGenre["preference_value"].ToString().Split(",").ToList();
                    filterString = string.Join(" or genre_id eq ", genresArray);
                    var genres = await clienteHTTP.FindEntriesAsync("tblGenres?$filter=genre_id eq " + filterString);
                    foreach (var genre in genres)
                    {
                        currentGenresID.Add(genre["genre_id"].ToString());
                        currentGenresName.Add(genre["name"].ToString());
                    }
                }
                adaptador = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, currentGenresName);
                lv.Adapter = adaptador;
                btnSavePreferences.Enabled = true;

                lv.ItemClick += (sender, args) =>
                {
                    var item = lv.GetItemAtPosition(args.Position);
                    var pos = allGenresName.FindIndex(x => x.Equals(item.ToString()));
                    if (pos > 0)
                    {
                        var itm = allGenresID[pos].ToString();
                        allGenresID.RemoveAll(x => x.Equals(itm.ToString()));
                        allGenresName.RemoveAll(x => x.Equals(itm.ToString()));
                        currentGenresID.Remove(itm);
                        currentGenresName.Remove(lv.GetItemAtPosition(args.Position).ToString());
                        adaptador.Remove(lv.GetItemAtPosition(args.Position).ToString());
                    }
                };
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }
    }
}