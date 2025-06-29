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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Volunteer Volunteer { get; set; }
        public string ButtonText { get; set; }
        public string password { get; set; } = "";
        public bool IsTextBoxEnabled { set; get; }
        public int sender_Id = 0;
        //public string CameFromWindow;
        public bool isAbleToChange = false;
        public VolunteerWindow(int id ,string CameFromWindow)
        {
            
            InitializeComponent();
            if (CameFromWindow == "VolunteerListWindow")
            {
                isAbleToChange = true;
            }
            //CameFromWindow = window;
            IsTextBoxEnabled = id!=0 ? true : false;
            sender_Id = id;
            ButtonText = id == 0 ? "Add" : "Update";
            CurrentVolunteer = (id != 0) ? s_bl.Volunteer.Read(id)! : new BO.Volunteer() { Id = 0 };

        }
        public BO.Enums.Role Role { get; set; } = BO.Enums.Role.NONE;
        public BO.Enums.DistanceType distanceType { get; set; } = BO.Enums.DistanceType.airDistance;


        public string status_message
        {
            get { return (string)GetValue(status_messageProperty); }
            set { SetValue(status_messageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for status_message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty status_messageProperty =
            DependencyProperty.Register("status_message", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(""));



        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        private void AddUpdate_btn(object sender, RoutedEventArgs e)
        {
            status_message = ButtonText == "Add" ? "Adding..." : "Updating";
            try
            {
                
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
                CurrentVolunteer.DistanceType = distanceType;
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
            if (CurrentVolunteer != null && sender is PasswordBox passwordBox)
            {
                CurrentVolunteer.Password = passwordBox.Password;
            }
        }

        
    }
}
