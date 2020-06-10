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

        
        protected override void OnCreate(Bundle savedInstanceState)
        {

            //DECLARACIONES Y ASIGNACIONES

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.preferences_intent);

            string url = "http://192.168.0.111/culturalcities/webservice/";
            AutoCompleteTextView genresSelector = FindViewById<AutoCompleteTextView>(Resource.Id.buscadorGeneros);
            ODataClient clienteHTTP = new ODataClient(url);
            
            List<string> allGenresID = new List<string>();
            List<string> allGenresName = new List<string>();
            List<string> currentGenresID = new List<string>();
            List<string> currentGenresName = new List<string>();

            Context mContext = Android.App.Application.Context;
            AppPreferences ap = new AppPreferences(mContext);
            Dictionary<string,string> key = ap.getAccessKey();

            ArrayAdapter<String> adapter;

            ListView lv = FindViewById<ListView>(Resource.Id.generos);
            List<string> listaAdaptador = new List<string>();
            listaAdaptador.Add("Prueba");
            ArrayAdapter<string> adaptador = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, listaAdaptador);
            lv.Adapter = adaptador;


            //EVENTOS

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
            };

            genresSelector.ItemClick += (sender, args) =>
            {
                Toast.MakeText(this, genresSelector.Text, ToastLength.Long).Show();
            };
        }

        static string[] COUNTRIES = new string[] {
            "Afghanistan", "Albania", "Algeria", "American Samoa", "Andorra",
            "Angola", "Anguilla", "Antarctica", "Antigua and Barbuda", "Argentina",
            "Armenia", "Aruba", "Australia", "Austria", "Azerbaijan",
            "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium",
            "Belize", "Benin", "Bermuda", "Bhutan", "Bolivia",
            "Bosnia and Herzegovina", "Botswana", "Bouvet Island", "Brazil", "British Indian Ocean Territory",
            "British Virgin Islands", "Brunei", "Bulgaria", "Burkina Faso", "Burundi",
            "Cote d'Ivoire", "Cambodia", "Cameroon", "Canada", "Cape Verde",
            "Cayman Islands", "Central African Republic", "Chad", "Chile", "China",
            "Christmas Island", "Cocos (Keeling) Islands", "Colombia", "Comoros", "Congo",
            "Cook Islands", "Costa Rica", "Croatia", "Cuba", "Cyprus", "Czech Republic",
            "Democratic Republic of the Congo", "Denmark", "Djibouti", "Dominica", "Dominican Republic",
            "East Timor", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Eritrea",
            "Estonia", "Ethiopia", "Faeroe Islands", "Falkland Islands", "Fiji", "Finland",
            "Former Yugoslav Republic of Macedonia", "France", "French Guiana", "French Polynesia",
            "French Southern Territories", "Gabon", "Georgia", "Germany", "Ghana", "Gibraltar",
            "Greece", "Greenland", "Grenada", "Guadeloupe", "Guam", "Guatemala", "Guinea", "Guinea-Bissau",
            "Guyana", "Haiti", "Heard Island and McDonald Islands", "Honduras", "Hong Kong", "Hungary",
            "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Israel", "Italy", "Jamaica",
            "Japan", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kuwait", "Kyrgyzstan", "Laos",
            "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg",
            "Macau", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Marshall Islands",
            "Martinique", "Mauritania", "Mauritius", "Mayotte", "Mexico", "Micronesia", "Moldova",
            "Monaco", "Mongolia", "Montserrat", "Morocco", "Mozambique", "Myanmar", "Namibia",
            "Nauru", "Nepal", "Netherlands", "Netherlands Antilles", "New Caledonia", "New Zealand",
            "Nicaragua", "Niger", "Nigeria", "Niue", "Norfolk Island", "North Korea", "Northern Marianas",
            "Norway", "Oman", "Pakistan", "Palau", "Panama", "Papua New Guinea", "Paraguay", "Peru",
            "Philippines", "Pitcairn Islands", "Poland", "Portugal", "Puerto Rico", "Qatar",
            "Reunion", "Romania", "Russia", "Rwanda", "Sqo Tome and Principe", "Saint Helena",
            "Saint Kitts and Nevis", "Saint Lucia", "Saint Pierre and Miquelon",
            "Saint Vincent and the Grenadines", "Samoa", "San Marino", "Saudi Arabia", "Senegal",
            "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "Solomon Islands",
            "Somalia", "South Africa", "South Georgia and the South Sandwich Islands", "South Korea",
            "Spain", "Sri Lanka", "Sudan", "Suriname", "Svalbard and Jan Mayen", "Swaziland", "Sweden",
            "Switzerland", "Syria", "Taiwan", "Tajikistan", "Tanzania", "Thailand", "The Bahamas",
            "The Gambia", "Togo", "Tokelau", "Tonga", "Trinidad and Tobago", "Tunisia", "Turkey",
            "Turkmenistan", "Turks and Caicos Islands", "Tuvalu", "Virgin Islands", "Uganda",
            "Ukraine", "United Arab Emirates", "United Kingdom",
            "United States", "United States Minor Outlying Islands", "Uruguay", "Uzbekistan",
            "Vanuatu", "Vatican City", "Venezuela", "Vietnam", "Wallis and Futuna", "Western Sahara",
            "Yemen", "Yugoslavia", "Zambia", "Zimbabwe"
        };
    }
}