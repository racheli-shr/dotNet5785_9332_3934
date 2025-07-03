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
    /// Interaction logic for ManagerChoosePageWindow.xaml
    /// </summary>
    public partial class ManagerChoosePageWindow : Window
    {
        private MainWindow _mainWindow;
        // Stores the role of the user (e.g., Manager, Volunteer, etc.)

        BO.Enums.Role Role { get; set; }
        // Stores the current volunteer's information

        BO.Volunteer volunteer { get; set; }
        // Constructor receives role and volunteer, initializes the window

        public ManagerChoosePageWindow(BO.Enums.Role r, BO.Volunteer v)
        {
            InitializeComponent();
            Role = r;
            volunteer = v;
        }
        // Opens the volunteer main window

        private void volunteerPageBtn_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerMainWindow(Role, volunteer).Show();
        }
        // Opens the manager main window

        private void managerPageBtn_Click(object sender, RoutedEventArgs e)
        {

            new MainWindow(Role,volunteer).Show();

        }
    }
}
