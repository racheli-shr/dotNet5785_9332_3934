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
         BO.Enums.Role Role { get; set; }
         BO.Volunteer volunteer { get; set; }
        public ManagerChoosePageWindow(BO.Enums.Role r, BO.Volunteer v)
        {
            InitializeComponent();
            Role = r;
            volunteer = v;
        }

        private void volunteerPageBtn_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerMainWindow(Role, volunteer).Show();
        }

        private void managerPageBtn_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(Role,volunteer).Show();
        }
    }
}
