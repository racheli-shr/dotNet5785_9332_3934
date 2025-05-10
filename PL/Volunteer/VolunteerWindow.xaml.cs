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
        
        public VolunteerWindow(int id)
        {
            ButtonText = id == 0 ? "Add" : "Update";

            InitializeComponent();
            CurrentVolunteer = (id != 0) ? s_bl.Volunteer.Read(id)! : new BO.Volunteer() { Id = 0 };

        }
        public BO.Enums.Role Role { get; set; } = BO.Enums.Role.IsNotDefined;
        public BO.Enums.DistanceType distanceType { get; set; } = BO.Enums.DistanceType.airDistance;
        

        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        private void AddUpdate_btn(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    Console.WriteLine(  CurrentVolunteer);
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                    MessageBox.Show("The Volunteer was been added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
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
