using System;
using System.Collections.Generic;
using System.Data.Common;
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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public int VolunteerId { get; set; }

        public string password { get; set; } = "";
        

        public int sender_Id = 0;

        public BO.Enums.Role Role { get; set; } = BO.Enums.Role.NONE;





        public BO.Enums.DistanceType DistanceType
        {
            get { return (BO.Enums.DistanceType)GetValue(DistanceTypeProperty); }
            set { SetValue(DistanceTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DistanceType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DistanceTypeProperty =
            DependencyProperty.Register("DistanceType", typeof(BO.Enums.DistanceType), typeof(MainWindow), new PropertyMetadata(BO.Enums.DistanceType.driveDistance));




        public bool IsTextBoxEnabled
        {
            get { return (bool)GetValue(IsTextBoxEnabledProperty); }
            set { SetValue(IsTextBoxEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTextBoxEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTextBoxEnabledProperty =
            DependencyProperty.Register("IsTextBoxEnabled", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(false));


        public bool IsActiveEnabled
        {
            get { return (bool)GetValue(IsActiveEnabledProperty); }
            set { SetValue(IsActiveEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsActiveEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveEnabledProperty =
            DependencyProperty.Register("IsActiveEnabled", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(false));





        public bool isAbleToChange
        {
            get { return (bool)GetValue(isAbleToChangeProperty); }
            set { SetValue(isAbleToChangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for isAbleToChange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty isAbleToChangeProperty =
            DependencyProperty.Register("isAbleToChange", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(false));




        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(""));



        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        public VolunteerWindow(int id, string CameFromWindow)
        {

            InitializeComponent();
            try
            {
                if (CameFromWindow == "VolunteerListWindow")
                {
                    isAbleToChange = true;
                }
                //CameFromWindow = window;
                IsTextBoxEnabled = id != 0 ? false : true;
                sender_Id = id;
                ButtonText = id == 0 ? "Add" : "Update";
                CurrentVolunteer = (id != 0) ? s_bl.Volunteer.Read(id)! : new BO.Volunteer() { Id = 0 };
                var call = s_bl.Volunteer.checkIfExistingAssignment(CurrentVolunteer);
                IsActiveEnabled = call != null ? true : false;
                VolunteerId = id == 0 ? 0 : id;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddUpdate_btn(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentVolunteer.DistanceType = DistanceType;
                Console.WriteLine(CurrentVolunteer.Password);
                if (ButtonText == "Add")
                {
                    //status_message = "Adding...";
                    Console.WriteLine(CurrentVolunteer);
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                    MessageBox.Show("The Volunteer was been added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    //status_message = "Updating...";
                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer!.Id, CurrentVolunteer!);
                    MessageBox.Show("The Volunteer was been Updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                Close(); // סגירת חלון לאחר הצלחה
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Selected_distanceType(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentVolunteer != null)
            {
                CurrentVolunteer.DistanceType = DistanceType;
            }

        }

        private void Selected_Role(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentVolunteer != null)
            {
                CurrentVolunteer.Role = Role;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null && DataContext is VolunteerWindow viewModel)
            {
                viewModel.CurrentVolunteer.Password = passwordBox.Password;
            }
        }

        private void queryVolunteerWindow()
        {
            try
            {
                CurrentVolunteer = s_bl.Volunteer.Read(VolunteerId)!;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void VolunteerWindowObserver()
        {
            try
            {
                if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                    _observerOperation = Dispatcher.BeginInvoke(() =>
                    {
                        queryVolunteerWindow();
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
                s_bl.Call.AddObserver(VolunteerWindowObserver);
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
                s_bl.Call.RemoveObserver(VolunteerWindowObserver);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


    }
}
