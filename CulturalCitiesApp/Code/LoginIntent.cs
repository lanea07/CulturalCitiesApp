using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Simple.OData.Client;

namespace CulturalCitiesApp
{
    [Activity(Label = "LoginIntent")]
    public class LoginIntent : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login_Intent);

            Button loginButton = FindViewById<Button>(Resource.Id.btnLogin);
            Button registerButton = FindViewById<Button>(Resource.Id.btnRegister);
            TextView txtUsername = FindViewById<TextView>(Resource.Id.txtUsername);
            TextView txtPassword = FindViewById<TextView>(Resource.Id.txtPassword);
            string url = "http://192.168.0.111/culturalcities/webservice/";
            //string url = "http://192.168.0.111:44322/webservice/";

            loginButton.Click += async (sender, args) =>
            {
                try
                {
                    var clienteHTTP = new ODataClient(url);
                    if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text)) throw new Exception("Username or Password cannot be blank or just spaces...");
                    var user = await clienteHTTP.FindEntriesAsync("tblCustomers?$filter=username eq '" + txtUsername.Text.Trim() + "'");
                    if (user.Count() != 1)
                    {
                        Toast.MakeText(this, "Fallo la autenticación o el usuario es incorrecto", ToastLength.Long).Show();
                        return;
                    }
                    foreach (var usuario in user)
                    {
                        if (usuario["password"].ToString().Trim() == txtPassword.Text.Trim())
                        {
                            var userPreferences = await clienteHTTP.FindEntriesAsync("tblCustomerPreferences?" +
                                "$select=tblPreferenceValue/preference_name,preference_value&" +
                                "$filter=customer_id+eq+" + usuario["customer_id"] + "&" +
                                "$expand=tblPreferenceValue");
                            var prefNameDict = new Dictionary<string, object>();
                            var prefName = "";
                            var prefValue = "";
                            foreach (var preference in userPreferences)
                            {
                                prefValue = preference["preference_value"].ToString();
                                prefNameDict = (Dictionary<string, object>)preference["tblPreferenceValue"];
                                prefName = prefNameDict["preference_name"].ToString();
                            }


                            Context mContext = Android.App.Application.Context;
                            AppPreferences ap = new AppPreferences(mContext);
                            ap.saveAccessKey(txtUsername.Text);

                            //Usuario correcto
                            Toast.MakeText(this, "Login succesful", ToastLength.Long).Show();
                            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                        }
                        else
                        {
                            //Usuario incorrecto
                            Toast.MakeText(this, "Login failed", ToastLength.Long).Show();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };

            registerButton.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(CustomerRegistration_tblCustomer));
                StartActivity(intent);
            };

            txtPassword.EditorAction += (sender, args) =>
            {
                if (args.ActionId == ImeAction.Send)
                {
                    loginButton.PerformClick();
                }
                else
                {
                    args.Handled = false;
                }
            };
        }
    }
}