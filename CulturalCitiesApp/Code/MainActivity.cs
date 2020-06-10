using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Support.V4.Widget;
using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.Design.Widget;
using Android.Content;
using Android.Preferences;

namespace CulturalCitiesApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.DesignDemo"/*Theme = "@style/AppTheme"/*, MainLauncher = true*/)]
    public class MainActivity : AppCompatActivity
    {

        RecyclerView mRecyclerView; // RecyclerView instance that displays the photo album:
        RecyclerView.LayoutManager mLayoutManager; // Layout manager that lays out each card in the RecyclerView:
        PhotoAlbumAdapter mAdapter; // Adapter that accesses the data set (a photo album):
        PhotoAlbum mPhotoAlbum; // Photo album that is managed by the adapter:

        NavigationView navigationView; // Part of sidebar menu
        DrawerLayout drawerLayout; // Part of sidebar menu

        protected override void OnCreate(Bundle savedInstanceState)
        {

            // Instantiate the photo album:
            mPhotoAlbum = new PhotoAlbum();
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            //DrawerLayout fragment
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar); // Create ActionBarDrawerToggle button and add it to the toolbar  
            SetSupportActionBar(toolbar);
            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.drawer_open, Resource.String.drawer_close);
            drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            setupDrawerContent(navigationView); //Calling Function  
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView); // Get our RecyclerView layout:

            //............................................................
            // Layout Manager Setup:

            mLayoutManager = new LinearLayoutManager(this); // Use the built-in linear layout manager:

            // Or use the built-in grid layout manager (two horizontal rows):
            // mLayoutManager = new GridLayoutManager
            //        (this, 2, GridLayoutManager.Horizontal, false);

            mRecyclerView.SetLayoutManager(mLayoutManager); // Plug the layout manager into the RecyclerView:

            //............................................................
            // Adapter Setup:

            // Create an adapter for the RecyclerView, and pass it the
            // data set (the photo album) to manage:
            mAdapter = new PhotoAlbumAdapter(mPhotoAlbum);

            mAdapter.ItemClick += OnItemClick; // Register the item click handler (below) with the adapter:

            mRecyclerView.SetAdapter(mAdapter); // Plug the adapter into the RecyclerView:

            //............................................................
            // Random Pick Button:
            /*
            // Get the button for randomly swapping a photo:
            Button randomPickBtn = FindViewById<Button>(Resource.Id.randPickButton);

            // Handler for the Random Pick Button:
            randomPickBtn.Click += delegate
            {
                if (mPhotoAlbum != null)
                {
                    // Randomly swap a photo with the top:
                    int idx = mPhotoAlbum.RandomSwap();

                    // Update the RecyclerView by notifying the adapter:
                    // Notify that the top and a randomly-chosen photo has changed (swapped):
                    mAdapter.NotifyItemChanged(0);
                    mAdapter.NotifyItemChanged(idx);
                }
            };*/

        }

        void setupDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.session_close:
                        ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(this);
                        ISharedPreferencesEditor editer = pref.Edit();
                        editer.Remove("Username").Commit(); ////Remove Spec key values  
                        editer.Remove("UserID").Commit(); ////Remove Spec key values  
                        StartActivity(new Intent(Application.Context, typeof(LoginIntent)));
                        this.Finish();
                        break;
                    case Resource.Id.preferences:
                        StartActivity(new Intent(Application.Context, typeof(UserPreferences)));
                        break;
                    default:
                        break;
                }
                drawerLayout.CloseDrawers();
            };
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            navigationView.InflateMenu(Resource.Menu.nav_menu); //Navigation Drawer Layout Menu Creation  
            return true;
        }

        void OnItemClick(object sender, int position) // Handler for the item click event:
        {
            int photoNum = position + 1; // Display a toast that briefly shows the enumeration of the selected photo:
            Toast.MakeText(this, "This is photo number " + photoNum, ToastLength.Short).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }

    //----------------------------------------------------------------------
    // VIEW HOLDER

    // Implement the ViewHolder pattern: each ViewHolder holds references
    // to the UI components (ImageView and TextView) within the CardView 
    // that is displayed in a row of the RecyclerView:
    public class PhotoViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Caption { get; private set; }

        public PhotoViewHolder(View itemView, Action<int> listener) // Get references to the views defined in the CardView layout.
            : base(itemView)
        {
            // Locate and cache view references:
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            Caption = itemView.FindViewById<TextView>(Resource.Id.textView);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }

    //----------------------------------------------------------------------
    // ADAPTER

    // Adapter to connect the data set (photo album) to the RecyclerView: 
    public class PhotoAlbumAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick; // Event handler for item clicks:
        PhotoAlbum mPhotoAlbum; // Underlying data set (a photo album):

        public PhotoAlbumAdapter(PhotoAlbum photoAlbum) // Load the adapter with the data set (photo album) at construction time:
        {
            mPhotoAlbum = photoAlbum;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) // Create a new photo CardView (invoked by the layout manager): 
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
            Inflate(Resource.Layout.PhotoCardView, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            PhotoViewHolder vh = new PhotoViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) // Fill in the contents of the photo card (invoked by the layout manager):
        {
            PhotoViewHolder vh = holder as PhotoViewHolder;

            // Set the ImageView and TextView in this ViewHolder's CardView 
            // from this position in the photo album:
            vh.Image.SetImageResource(mPhotoAlbum[position].PhotoID);
            vh.Caption.Text = mPhotoAlbum[position].Caption;
        }

        public override int ItemCount // Return the number of photos available in the photo album:
        {
            get { return mPhotoAlbum.NumPhotos; }
        }

        void OnClick(int position) // Raise an event when the item-click takes place:
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}