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
        BO.Volunteer volunteer;


        public BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentVolunteer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerMainWindow), new PropertyMetadata(0));


        BO.Enums.Role role;
        public VolunteerMainWindow(BO.Enums.Role r,BO.Volunteer v)
        {
            InitializeComponent();
            volunteer = v;
            role = r;

        }
    }
}
