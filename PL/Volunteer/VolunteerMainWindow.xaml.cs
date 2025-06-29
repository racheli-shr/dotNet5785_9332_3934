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
        
        BO.Call VolunteerCall;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public bool isChooseCallAble;
        public bool isAssignedCallAble;
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
            
            var call = s_bl.Volunteer.checkIfExistingAssignment(v);
            if(call!=null) {
                VolunteerCall = call; 
                isAssignedCallAble = true;
                isChooseCallAble = false;
            }
            
            CurrentVolunteer = v;
            role = r;
            Console.WriteLine(  CurrentVolunteer);
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
            new ChooseCallToTreatWindow(CurrentVolunteer).Show();
        }
    }
}
