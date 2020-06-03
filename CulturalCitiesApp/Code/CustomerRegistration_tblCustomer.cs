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

namespace CulturalCitiesApp
{
    [Activity(Label = "Activity1")]
    public class CustomerRegistration_tblCustomer : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.customer_registration_tblCustomer);

            // Create your application here
        }
    }
}