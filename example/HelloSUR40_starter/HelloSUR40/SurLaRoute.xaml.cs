using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using SurLaRoute.Rooms;
using SurLaRoute.VCards;
using ShowRoomApplication;
using SurLaRoute.Tags;

namespace SurLaRoute
{
    /// <summary>
    /// Interaction logic for ShowRoomApplicationWindowobject.xaml
    /// </summary>
    public partial class MapWindow : SurfaceWindow
    {

        Dictionary<String, Room> rooms = new Dictionary<String, Room>();
        Dictionary<int, VCard> cards = new Dictionary<int,VCard>();

        public MapWindow()
        {
            initWindow();
            intiTangibleDefinitions();
            //initFloorPlan();
        }

        private void initWindow()
        {
            InitializeComponent();
            AddWindowAvailabilityHandlers();
            setBackgroundImage("Resources/Images/background.png");

            initFloorPlan();
            initVCards();
        }

        private void intiTangibleDefinitions()
        {
            
            for (int i = 0; i < 12; i++)
            {
                TagVisualizationDefinition tagDefinition = new TagVisualizationDefinition();
                tagDefinition.Value = i;
                tagDefinition.Source = new Uri("Tags/TagVisualization.xaml", UriKind.Relative);
                tagDefinition.MaxCount = 1;
                tagDefinition.LostTagTimeout = 2000.0; 
                tagDefinition.OrientationOffsetFromTag = 0;
                tagDefinition.PhysicalCenterOffsetFromTag = new Vector(0, 0);
                tagDefinition.TagRemovedBehavior = TagRemovedBehavior.Fade;
                tagDefinition.UsesTagOrientation = true;
                TagVisualizer.Definitions.Add(tagDefinition);
            }
            
        }

        private void initFloorPlan()
        {
            rooms.Add("3.0.60", new Room(290, 50, 100, 100, ApplicationCanvas));
            rooms.Add("3.0.60E", new Room(50, 160, 100, 100, ApplicationCanvas));
            rooms.Add("3.0.44", new Room(50, 50, 100, 100, ApplicationCanvas));
            rooms.Add("3.0.41", new Room(400, 50, 100, 100, ApplicationCanvas));
            rooms.Add("3.0.32", new Room(800, 160, 100, 100, ApplicationCanvas));
            rooms.Add("3.0.31", new Room(800, 270, 100, 100, ApplicationCanvas));
            rooms.Add("3.0.30", new Room(800, 380, 100, 100, ApplicationCanvas));
            rooms.Add("3.0.14", new Room(1010, 710, 100, 100, ApplicationCanvas));
            //rooms.Add("8", new Room(800, 600, 100, 100, ApplicationCanvas));
            //rooms.Add("9", new Room(800, 710, 100, 100, ApplicationCanvas));

            foreach (var room in rooms)
            {
                //Console.WriteLine(room.Key);
                ApplicationCanvas.Children.Add(room.Value);
            }
        }

        private void initVCards()
        {
            cards.Add(0, new VCard(0, "Christian Wolff", "3.0.60"));
            cards.Add(1, new VCard(1, "Ingrid Boehm", "3.0.60E"));
            cards.Add(2, new VCard(2, "Florian Echtler", "3.0.44"));
            cards.Add(3, new VCard(3, "Ingrid Stitz", "3.0.41"));
            cards.Add(4, new VCard(4, "Raphael Wimmer", "3.0.32"));
            cards.Add(5, new VCard(5, "Patricia Boehm", "3.0.31"));
            cards.Add(6, new VCard(6, "Victoria Boehm", "3.0.31"));
            cards.Add(7, new VCard(7, "Manuel Burghard", "3.0.30"));
            cards.Add(8, new VCard(8, "Tim Schneidermeier", "3.0.30"));
            cards.Add(9, new VCard(9, "Martin Brockelmann", "3.0.14"));
        }

        

        private void setBackgroundImage(String ressourceString)
        {
            ImageBrush image_brush = new ImageBrush(
                new BitmapImage(
                    new Uri(BaseUriHelper.GetBaseUri(this), ressourceString)));
            ApplicationCanvas.Background = image_brush  ;
        }


        private void CloseApplicationItemClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            RemoveWindowAvailabilityHandlers();
        }

   
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        private void OnWindowInteractive(object sender, EventArgs e)
        {
        }

        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
        }

        private void OnWindowUnavailable(object sender, EventArgs e)
        {
        }


        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            SimpleVisualization visualization = (SimpleVisualization)e.TagVisualization;

            visualization.VCard_Name.Content = cards[(int)e.TagVisualization.VisualizedTag.Value].Name;
            rooms[cards[(int)e.TagVisualization.VisualizedTag.Value].Room].setActive();            
        }

        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            rooms[cards[(int)e.TagVisualization.VisualizedTag.Value].Room].setPassive();
        }

       
       

    }
}