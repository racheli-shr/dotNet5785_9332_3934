using PL.Call;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for VolunteerMainWindow.xaml
    /// </summary>
    public partial class VolunteerMainWindow : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();




        public BO.Call VolunteerCall
        {
            get { return (BO.Call)GetValue(VolunteerCallProperty); }
            set { SetValue(VolunteerCallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerCall.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerCallProperty =
            DependencyProperty.Register("VolunteerCall", typeof(BO.Call), typeof(VolunteerMainWindow), null);





        public bool IsAssignedCallAble
        {
            get { return (bool)GetValue(IsAssignedCallAbleProperty); }
            set { SetValue(IsAssignedCallAbleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAssignedCallAble.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAssignedCallAbleProperty =
            DependencyProperty.Register("IsAssignedCallAble", typeof(bool), typeof(VolunteerMainWindow), new PropertyMetadata(false));



        public bool IsChooseCallAble
        {
            get { return (bool)GetValue(IsChooseCallAbleProperty); }
            set { SetValue(IsChooseCallAbleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChooseCallAble.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsChooseCallAbleProperty =
            DependencyProperty.Register("IsChooseCallAble", typeof(bool), typeof(VolunteerMainWindow), new PropertyMetadata(false));





        public BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentVolunteer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerMainWindow), new PropertyMetadata(null));


        BO.Enums.Role role;
        public VolunteerMainWindow(BO.Enums.Role r, BO.Volunteer v)
        {

            InitializeComponent();

            //MapBrowser.LoadCompleted += (s, e) =>
            //{
            //    dynamic activeX = MapBrowser.GetType().InvokeMember("ActiveXInstance",
            //        System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            //        null, MapBrowser, new object[] { });

            //    if (activeX != null)
            //    {
            //        activeX.Silent = true; // מבטל הודעות שגיאה
            //    }
            //};


            var call = s_bl.Volunteer.checkIfExistingAssignment(v);
            if (call != null)
            {
                VolunteerCall = call;
                IsAssignedCallAble = true;
                IsChooseCallAble = false;
            }
            else
            {
                IsAssignedCallAble = false;
                IsChooseCallAble = false;
            }

            CurrentVolunteer = v;
            role = r;
            Console.WriteLine(CurrentVolunteer);
            //LoadMap();
        }

        private void Update_btn(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow(CurrentVolunteer.Id, "VolunteerMainWindow").Show();
        }

        private void GoToCallHistoryPage_Btn(object sender, RoutedEventArgs e)
        {
            new Call.CallsHistoryWindow(CurrentVolunteer).Show();

        }



        private void chooseCall_Click(object sender, RoutedEventArgs e)
        {
            var chooseCallWindow = new ChooseCallToTreatWindow(CurrentVolunteer);
            chooseCallWindow.Closed += ChooseCallWindow_Closed;
            chooseCallWindow.Show();
        }

        private void ChooseCallWindow_Closed(object sender, EventArgs e)
        {
            var call = s_bl.Volunteer.checkIfExistingAssignment(CurrentVolunteer);
            if (call != null)
            {
                VolunteerCall = call;
                IsAssignedCallAble = true;
                IsChooseCallAble = false;
            }
            else
            {
                IsAssignedCallAble = false;
                IsChooseCallAble = false;
            }
        }
        private void EndTreatment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                s_bl.Call.CompleteAssignmentToCall(CurrentVolunteer.Id,VolunteerCall.Id);
                VolunteerCall = null;
                IsAssignedCallAble = false;
                IsChooseCallAble = true;
                MessageBox.Show("הטיפול הסתיים בהצלחה");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בסיום הטיפול: " + ex.Message);
            }
        }

        private void CancelTreatment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.DeleteAssignmentToCall(VolunteerCall.Id);
                VolunteerCall = null;
                IsAssignedCallAble = false;
                IsChooseCallAble = true;
                MessageBox.Show("הטיפול בוטל");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בביטול הטיפול: " + ex.Message);
            }
        }
//        private async void Window_Loaded(object sender, RoutedEventArgs e)
//        {
//            await MyWebView.EnsureCoreWebView2Async();

//            string html = $@"
//<!DOCTYPE html>
//<html>
//  <head>
//    <meta charset='utf-8'>
//    <style>html, body, #map {{ height: 100%; margin: 0; padding: 0; }}</style>
//    <script src='https://maps.googleapis.com/maps/api/js?key=YOUR_API_KEY'></script>
//    <script>
//      function initMap() {{
//        const origin = new google.maps.LatLng(32.0853, 34.7818); // תל אביב
//        const destination = new google.maps.LatLng(31.7683, 35.2137); // ירושלים

//        const map = new google.maps.Map(document.getElementById('map'), {{
//          zoom: 7,
//          center: origin
//        }});

//        const directionsService = new google.maps.DirectionsService();
//        const directionsRenderer = new google.maps.DirectionsRenderer();
//        directionsRenderer.setMap(map);

//        directionsService.route({{
//          origin: origin,
//          destination: destination,
//          travelMode: 'DRIVING'
//        }}, function(result, status) {{
//          if (status === 'OK') {{
//            directionsRenderer.setDirections(result);
//          }} else {{
//            alert('Directions request failed due to ' + status);
//          }}
//        }});
//      }}
//    </script>
//  </head>
//  <body onload='initMap()'>
//    <div id='map'></div>
//  </body>
//</html>";

//            MyWebView.NavigateToString(html);
//        }


    }
}
