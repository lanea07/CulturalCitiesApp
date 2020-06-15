using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Simple.OData.Client;

namespace CulturalCitiesApp
{
    [Activity(Label = "EventDetail_Intent", NoHistory = true)]
    public class EventDetail_Intent : Activity
    {
        static string url = "http://192.168.0.111/culturalcities/webservice/";
        ODataClient clienteHTTP = new ODataClient(url);
        ImageView eventIMG;
        TextView eventTitle;
        TextView eventCity;
        TextView eventGeoLocation;
        TextView eventURL;
        TextView eventDate;
        TextView eventGenres;
        string eventId;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EventDetail_Intent);
            

            eventIMG = FindViewById<ImageView>(Resource.Id.eventIMG);
            eventTitle = FindViewById<TextView>(Resource.Id.eventTitle);
            eventCity = FindViewById<TextView>(Resource.Id.eventCity);
            eventGeoLocation = FindViewById<TextView>(Resource.Id.eventGeoLocation);
            eventURL = FindViewById<TextView>(Resource.Id.eventURL);
            eventDate = FindViewById<TextView>(Resource.Id.eventDate);
            eventGenres = FindViewById<TextView>(Resource.Id.eventGenres);
            eventId = Intent.Extras.GetInt("EVENT_ID").ToString();

            try
            {
                var eventInfo = await clienteHTTP.FindEntriesAsync("tblEvents(" + int.Parse(eventId) + ")?$expand=tblCity");
                foreach (var eventInfoItem in eventInfo)
                {
                    Android.Graphics.Bitmap bmp;
                    bmp = GetImageBitmapFromUrl(eventInfoItem["EventImagePath"].ToString());
                    eventIMG.SetImageBitmap(bmp);
                    eventTitle.Text = eventInfoItem["name"].ToString();
                    eventGeoLocation.Text = eventInfoItem["geographical_location"].ToString();
                    eventURL.Text = eventInfoItem["event_source_site_page"].ToString();
                    eventDate.Text = eventInfoItem["event_date"].ToString();
                    var cityDict = new Dictionary<string, object>();
                    cityDict = (Dictionary<string, object>)eventInfoItem["tblCity"];
                    eventCity.Text= cityDict["name"].ToString();
                    var genres= await clienteHTTP.FindEntriesAsync("tblEvents(" + int.Parse(eventId) + ")/tblGenre");
                    List<string> genresArray = new List<string>();
                    foreach (var genre in genres)
                    {
                        genresArray.Add(genre["name"].ToString());
                    }
                    eventGenres.Text = string.Join(",", genresArray);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        public static Android.Graphics.Bitmap GetImageBitmapFromUrl(string url)
        {
            Android.Graphics.Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                try
                {
                    if (!Regex.IsMatch(url, "^png|bmp|jpg|jpeg$", RegexOptions.IgnoreCase))
                    {
                        imageBitmap = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.event_image_not_available);
                    }
                    else
                    {
                        var imageBytes = webClient.DownloadData(url);
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                        }
                    }
                }
                catch (WebException ex)
                {
                    imageBitmap = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.event_image_not_available);
                }
                catch (Exception ex)
                {
                    switch (ex.InnerException.HResult)
                    {
                        case -2146233079:
                            
                            break;
                        case -2146233087:
                            string fixedURL = url.Replace("https://", "http://");
                            var imageBytes = webClient.DownloadData(fixedURL);
                            if (imageBytes != null && imageBytes.Length > 0)
                            {
                                imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                            }
                            break;
                    }
                }
            }

            if (imageBitmap == null)
            {
                imageBitmap = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.event_image_not_available);
            }

            return imageBitmap;
        }
    }
}