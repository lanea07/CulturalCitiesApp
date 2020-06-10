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
        List<string> currentGenresID = new List<string>();
        List<string> currentGenresName = new List<string>();
        ListView lv;
        List<string> allGenresID = new List<string>();
        List<string> allGenresName = new List<string>();

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

            //EVENTOS

            genresSelector.BeforeTextChanged += (sender, args) =>
            {
                progressbar.Visibility = ViewStates.Visible;
            };            

            genresSelector.TextChanged += async (sender, args) =>
            {
                var currentGenres = await clienteHTTP.FindEntriesAsync("tblCustomerPreferences?$filter=customer_id eq 2");
                allGenresID.Clear();
                allGenresName.Clear();
                var genres = await clienteHTTP.FindEntriesAsync("tblGenres?$filter=substringof('"+args.Text+"', name)");
                foreach (var genre in genres)
                {
                    allGenresID.Add(genre["genre_id"].ToString());
                    allGenresName.Add(genre["name"].ToString());
                }
                adapter = new ArrayAdapter<String>(this, Resource.Layout.list_item, allGenresName);
                genresSelector.Adapter = adapter;
                progressbar.Visibility = ViewStates.Invisible;
            };

            genresSelector.ItemClick += (sender, args) =>
            {
                Toast.MakeText(this, genresSelector.Text, ToastLength.Long).Show();
            };
        }

        protected async override void OnStart()
        {
            base.OnStart();
            ODataClient clienteHTTP = new ODataClient(url);
            currentGenresID = new List<string>();
            currentGenresName = new List<string>();
            lv = FindViewById<ListView>(Resource.Id.generos);
            ArrayAdapter<string> adaptador;
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
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }
    }
}