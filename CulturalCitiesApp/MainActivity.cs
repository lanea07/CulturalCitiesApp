using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Android.Nfc;
using System;
using Android.Views.InputMethods;

namespace CulturalCitiesApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            Button loginButton = FindViewById<Button>(Resource.Id.btnLogin);
            Button registerButton = FindViewById<Button>(Resource.Id.btnRegister);
            TextView txtUsername = FindViewById<TextView>(Resource.Id.txtUsername);
            TextView txtPassword = FindViewById<TextView>(Resource.Id.txtPassword);
            string loginServiceUrl = "http://192.168.0.111/culturalcities/webservice/tblCustomers/Login";

            loginButton.Click += async (sender, args) =>
            {
                try
                {
                    ClienteHTTP cliente = new ClienteHTTP();
                    if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text)) throw new Exception("Username or Password cannot be blank or just spaces...");
                    var resultado = await cliente.Login<CustomerLoginInfo>(new CustomerLoginInfo() { username = txtUsername.Text, password = txtPassword.Text },loginServiceUrl);
                    switch (cliente.codigoHTTP)
                    {
                        case 200:
                            throw new Exception("Login succesful");
                            break;
                        case 404:
                            throw new Exception("Login failed");
                            break;
                        default:
                            throw new Exception("Unknown error");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };

            registerButton.Click += (sender, args) =>
            {
                Toast.MakeText(this, "Under construction", ToastLength.Short).Show();
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
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    public class ClienteHTTP
    {
        public int codigoHTTP { get; set; }

        public ClienteHTTP()
        {
            codigoHTTP = 200;
        }

        public async Task<T> Login<T>(CustomerLoginInfo customerLoginInfo, string url)
        {
            HttpClient cliente = new HttpClient();
            var json = JsonConvert.SerializeObject(customerLoginInfo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            response = await cliente.PostAsync(url, content);
            json = await response.Content.ReadAsStringAsync();
            codigoHTTP = (int)response.StatusCode;
            return JsonConvert.DeserializeObject<T>(json);

        }
    }

    public class CustomerLoginInfo
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}