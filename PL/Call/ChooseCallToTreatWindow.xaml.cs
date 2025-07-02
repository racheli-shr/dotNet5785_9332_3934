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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for ChooseCallToTreatWindow.xaml
    /// </summary>
    public partial class ChooseCallToTreatWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Enums.OpenCallInListFields? SelectedSearchOption { get; set; } = null;
        public object SearchValue { get; set; } = string.Empty;
        public BO.OpenCallInList? SelectedOpenCall { get; set; }
        public BO.Enums.OpenCallInListFields? SelectedSortOption { get; set; }
        private BO.Volunteer Volunteer { get; set; }

        public BO.Enums.OpenCallInListFields FilterField
        {
            get { return (BO.Enums.OpenCallInListFields)GetValue(FilterFieldProperty); }
            set { SetValue(FilterFieldProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterField.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterFieldProperty =
            DependencyProperty.Register("FilterField", typeof(BO.Enums.OpenCallInListFields), typeof(ChooseCallToTreatWindow), new PropertyMetadata(BO.Enums.OpenCallInListFields.None));



        public List<BO.OpenCallInList> OpenCalls
        {
            get { return (List<BO.OpenCallInList>)GetValue(OpenCallsProperty); }
            set { SetValue(OpenCallsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenCalls.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenCallsProperty =
            DependencyProperty.Register("OpenCalls", typeof(List<BO.OpenCallInList>), typeof(ChooseCallToTreatWindow), new PropertyMetadata(null));



        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ChooseCallToTreatWindow), new PropertyMetadata(""));

        public string MapUrl
        {
            get { return (string)GetValue(MapUrlProperty); }
            set { SetValue(MapUrlProperty, value); }
        }

        public static readonly DependencyProperty MapUrlProperty =
            DependencyProperty.Register("MapUrl", typeof(string), typeof(ChooseCallToTreatWindow), new PropertyMetadata(null));

        public BO.OpenCallInList SelectedCall
        {
            get { return (BO.OpenCallInList)GetValue(SelectedCallProperty); }
            set { SetValue(SelectedCallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedCall.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedCallProperty =
    DependencyProperty.Register(
        "SelectedCall",
        typeof(BO.OpenCallInList),
        typeof(ChooseCallToTreatWindow),
        new PropertyMetadata(null, OnSelectedCallChanged)
    );
        private static void OnSelectedCallChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as ChooseCallToTreatWindow;
            var selectedCall = e.NewValue as BO.OpenCallInList;

            if (window != null && selectedCall != null)
            {
                window.Description = selectedCall.Description ?? "";
            }
        }
        // Constructor: initializes the window and loads open calls for the volunteer

        public ChooseCallToTreatWindow(BO.Volunteer volunteer)
        {
            Volunteer = volunteer;
            OpenCalls = s_bl.Call.GetOpenCallsForVolunteer(Volunteer.Id).ToList();
            InitializeComponent();
        }
        // Adds observer to the open calls list when window is loaded

        private void Window_Loaded(object sender, RoutedEventArgs e)
        => s_bl.Call.AddObserver(CallsListObserver);

        // Removes observer when window is closed

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(CallsListObserver);

        // Observer callback that triggers call list update

        public void CallsListObserver() => QueryOpenCall();
        // Filters the open calls based on the selected search criteria

        public void QueryOpenCall()
        {
            if (SelectedSearchOption == null || string.IsNullOrEmpty(SearchValue?.ToString()))
            {
                OpenCalls = s_bl.Call.GetOpenCallsForVolunteer(Volunteer.Id).ToList();
                return;
            }
            try
            {
                OpenCalls = s_bl.Call.FilterOpenCalls(Volunteer.Id, SelectedSearchOption.Value, SearchValue.ToString()).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Updates the open calls list when the filter option changes

        private void FilterOpenCalls(object sender, SelectionChangedEventArgs e)
        {
            OpenCalls = (FilterField == BO.Enums.OpenCallInListFields.None) ?
                s_bl.Call.GetOpenCallsForVolunteer(Volunteer.Id).ToList() :
                s_bl.Call.GetOpenCallsForVolunteer(Volunteer.Id).ToList();
        }
        // Assigns the selected call to the volunteer and closes the window

        private void AssignCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.AssignCallToVolunteer(Volunteer.Id, SelectedCall.Id);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Updates the description when a new call is selected

        private void OpenCallsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedCall != null)
            {
                Description = SelectedCall.Description ?? "";
                //MapBrowser.Navigate(url);
                //InitializeAsync();
            }
        }
        // Clears the filters and reloads the open calls list

        private void ClearButton_Click(object sender, RoutedEventArgs e)
=> OpenCalls = s_bl?.Call.GetOpenCallsForVolunteer(Volunteer.Id).ToList()!;
        // Performs a search based on the selected criteria

        private void SearchButton_Click(object sender, RoutedEventArgs e) => QueryOpenCall();
        // Sorts the open calls based on the selected sort option

        private void SortButton_Click(object sender, RoutedEventArgs e)
        => OpenCalls = s_bl.Call.SortOpenCalls(Volunteer.Id, SelectedSortOption).ToList();

        //private async void InitializeAsync()
        //{
        //    var studentCall = s_bl.StudentCall.Read(SelectedCall.Id);
        //    var tutor = s_bl.Tutor.Read(TutorId);
        //    string url = $"https://www.google.com/maps/dir/?api=1&origin={tutor.Latitude},{tutor.Longitude}&destination={studentCall.Latitude},{studentCall.Longitude}&travelmode=driving";
        //    await MyWebView.EnsureCoreWebView2Async(null);
        //    MyWebView.Source = new Uri(url);
        //}
    }
}
