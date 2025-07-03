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
using System.Windows.Threading;

namespace PL
{
    /// <summary>
    /// Interaction logic for VolunteerMainWindow.xaml
    /// </summary>
    ///  // Initialize window and set up volunteer's current call assignment status.
    // Sets UI flags based on whether the volunteer has an existing assignment.
    public partial class VolunteerMainWindow : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public int VolunteerId { get; set; } = 0;


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
            try
            {

                VolunteerCall = s_bl.Volunteer.checkIfExistingAssignment(v);
            if (VolunteerCall != null)
            {
                IsAssignedCallAble = true;
                IsChooseCallAble = false;
            }
            else
            {
                IsAssignedCallAble = false;
                IsChooseCallAble = true;
            }

            CurrentVolunteer = v;
            role = r;
            Console.WriteLine(CurrentVolunteer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // Opens a window to update the current volunteer's details.

        private void Update_btn(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow(CurrentVolunteer.Id, "VolunteerMainWindow").Show();
        }
        // Opens the call history window for the current volunteer.

        private void GoToCallHistoryPage_Btn(object sender, RoutedEventArgs e)
        {
            new Call.CallsHistoryWindow(CurrentVolunteer).Show();

        }


        // Opens a window allowing the volunteer to choose a call to treat.
        // Registers a callback for when that window closes.
        private void chooseCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CurrentVolunteer.IsActive)
                {
                    MessageBox.Show("can't assign volunteer to a not active volunteer");
                    return;
                }
                var chooseCallWindow = new ChooseCallToTreatWindow(CurrentVolunteer);
                chooseCallWindow.Closed += ChooseCallWindow_Closed;
                chooseCallWindow.Show();
            }
            catch(Exception ex)
            {
                   MessageBox.Show("error choosing call"+ex.Message);
            }
        }
        // Refreshes the current assignment status when the call selection window closes.
        // Updates UI flags accordingly.
        private void ChooseCallWindow_Closed(object sender, EventArgs e)
        {
            try
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
                IsChooseCallAble = true;
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // Completes the current call assignment and updates UI state.
        // Handles any exceptions by showing an error message.
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
        // Cancels the current call assignment and updates UI state.
        // Handles any exceptions by showing an error message.
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


        private void queryVolunteerMainWindow()
        {
            try
            {
                CurrentVolunteer = s_bl.Volunteer.Read(VolunteerId)!;
            VolunteerCall= s_bl.Volunteer.checkIfExistingAssignment(CurrentVolunteer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void VolunteerMainWindowObserver()
        {
            try
            {
                if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryVolunteerMainWindow();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.AddObserver(VolunteerMainWindowObserver);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
                try
                {
                    s_bl.Call.RemoveObserver(VolunteerMainWindowObserver);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
