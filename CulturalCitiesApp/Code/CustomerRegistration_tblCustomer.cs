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
using Java.Util;
using Newtonsoft.Json;
using Simple.OData.Client;

namespace CulturalCitiesApp
{
    [Activity(Label = "Activity1")]
    public class CustomerRegistration_tblCustomer : Activity
    {
        private static TextView CustomerBirthDate;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.customer_registration_tblCustomer);

            string url = "http://192.168.0.111/culturalcities/webservice/";
            //string url = "http://192.168.0.111:44322/webservice/";
            var clienteHTTP = new ODataClient(url);
            Button btnRegister = FindViewById<Button>(Resource.Id.btnRegister);
            List<string> countryID = new List<string>();
            List<string> countryName = new List<string>();
            List<string> cityID = new List<string>();
            List<string> cityName = new List<string>();

            EditText txtCustomerID = FindViewById<EditText>(Resource.Id.txtCustomerId);
            EditText txtCustomerFirstName = FindViewById<EditText>(Resource.Id.txtCustomerFirstName);
            EditText txtCustomerLastName = FindViewById<EditText>(Resource.Id.txtCustomerLastName);
            EditText txtCustomerBirth = FindViewById<EditText>(Resource.Id.txtCustomerBirth);
            EditText txtCustomerUserName = FindViewById<EditText>(Resource.Id.txtCustomerUserName);
            EditText txtCustomerPassWord = FindViewById<EditText>(Resource.Id.txtCustomerPassWord);
            EditText txtCustomerReTypePassWord = FindViewById<EditText>(Resource.Id.txtCustomerReTypePassWord);
            EditText txtCustomerEmail = FindViewById<EditText>(Resource.Id.txtCustomerEmail);
            Spinner countrySpinner = FindViewById<Spinner>(Resource.Id.spnCountry);
            Spinner citySpinner = FindViewById<Spinner>(Resource.Id.spnCity);

            CustomerBirthDate = FindViewById<TextView>(Resource.Id.txtCustomerBirth);
            CustomerBirthDate.Click += (sender, e) => {
                DateTime today = DateTime.Today;
                DatePickerDialog dialog = new DatePickerDialog(this, OnDateSet, today.Year, today.Month - 1, today.Day);
                dialog.DatePicker.MinDate = today.Millisecond;
                dialog.Show();
            };

            try
            {
                var countries = await clienteHTTP.FindEntriesAsync("tblCountries");
                foreach (var country in countries)
                {
                    countryID.Add(country["country_id"].ToString());
                    countryName.Add(country["name"].ToString());
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            var countryAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, countryName);
            countrySpinner.Adapter = countryAdapter;

            countrySpinner.ItemSelected += async (sender, args) =>
            {
                try
                {
                    var cities = await clienteHTTP.FindEntriesAsync("tblCities?$filter=country_id eq " + int.Parse(countryID[args.Position]));
                    foreach (var city in cities)
                    {
                        cityID.Clear();
                        cityName.Clear();
                        cityID.Add(city["city_id"].ToString());
                        cityName.Add(city["name"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
                var cityAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, cityName);
                citySpinner.Adapter = cityAdapter;
            };

            btnRegister.Click += async (sender, args) =>
            {
                try
                {
                    var registry = new
                    {
                        customer_id = 0,
                        personal_identification = int.Parse(txtCustomerID.Text),
                        first_name = txtCustomerFirstName.Text,
                        last_name = txtCustomerLastName.Text,
                        birth_date = Convert.ToDateTime(txtCustomerBirth.Text),
                        username = txtCustomerUserName.Text,
                        password = txtCustomerPassWord.Text,
                        email = txtCustomerEmail.Text,
                        city_id = cityID[citySpinner.SelectedItemPosition],
                        create_time = Convert.ToDateTime(DateTime.Now),
                        update_time = Convert.ToDateTime(DateTime.Now)
                    };
                    var result = await clienteHTTP.For("tblCustomers").Set(registry).InsertEntryAsync();
                    if (result.Count() == 0)
                    {
                        //Falló la inserción
                        Toast.MakeText(this, "Registro fallido", ToastLength.Long).Show();
                    }
                    else
                    {
                        var preferences = await clienteHTTP.FindEntriesAsync("tblPreferenceValues");
                        foreach (var preference in preferences)
                        {
                            switch (preference["preference_name"])
                            {
                                case "max_items_per_page":
                                    await clienteHTTP.For("tblCustomerPreferences").Set(new
                                    {
                                        customer_id = result["customer_id"],
                                        preference_id = preference["preference_id"],
                                        preference_value = "20",
                                        update_time = Convert.ToDateTime(DateTime.Now)
                                    }).InsertEntryAsync();
                                    break;
                                case "mobile_alerts_enabled":
                                    await clienteHTTP.For("tblCustomerPreferences").Set(new
                                    {
                                        customer_id = result["customer_id"],
                                        preference_id = preference["preference_id"],
                                        preference_value = "0",
                                        update_time = Convert.ToDateTime(DateTime.Now)
                                    }).InsertEntryAsync();
                                    break;
                                case "preferred_genres":
                                    break;
                            }
                        }
                        Toast.MakeText(this, "Registro exitoso", ToastLength.Long).Show();
                    }

                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }

            };
        }

        void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            CustomerBirthDate.Text = e.Date.ToShortDateString();
        }
    }
}