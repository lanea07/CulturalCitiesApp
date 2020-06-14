using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Simple.OData.Client;

namespace CulturalCitiesApp
{
	public class Evento
	{
		public int event_id { get; set; }
		public string name { get; set; }
		public string EventImagePath { get; set; }

		public Evento()
        {

        }

		public Evento(int event_id, string name, string EventImagePath)
		{
			this.event_id = event_id;
			this.name = name;
			this.EventImagePath = EventImagePath;
		}

	}

	public class EventCollection
	{
		static string url = "http://192.168.0.111/culturalcities/webservice/";
		Evento[] eventList;
		ODataClient clienteHTTP = new ODataClient(url);

		public EventCollection()
		{
            LoadEvents();
		}

		public async Task LoadEvents(int beginIn = 0, int pageLenght = 10)
		{
			List<Evento> evt = new List<Evento>();
			object obj;
            try
            {
				var result = await clienteHTTP.FindEntriesAsync("tblEvents?$top=" + pageLenght + "&$skip=" + beginIn + "");
				foreach (var evento in result)
				{
					Evento eventos = new Evento
					{
						event_id = int.Parse(evento["event_id"].ToString()),
						name = evento["name"].ToString(),
						EventImagePath = evento["EventImagePath"].ToString()
						/*event_id = 11,
						name = "An Event",
						EventImagePath = "https://static.iris.net.co/semana/upload/images/2020/1/3/646930_1.jpg"*/
					};
					evt.Add(eventos);
				}
			}
            catch (Exception ex)
            {
				Log.Debug("Err", ex.Message);
				
            }
			eventList = evt.ToArray();
		}

		public int NumPhotos
		{
			get { return eventList == null ? 0 : eventList.Length; }
		}

		public Evento this[int i]
		{
			get { return eventList[i]; }
		}

	}
}