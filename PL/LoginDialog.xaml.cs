using PL.Call;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL;

/// <summary>
/// Interaction logic for LoginDialog.xaml
/// </summary>
public partial class LoginDialog : Window
{
    // Reference to the manager page window (to prevent multiple instances)

    private ManagerChoosePageWindow mcpWindow;
    // Holds the logged-in volunteer's details

    public BO.Volunteer v;
    // Role of the user (Volunteer, Manager, etc.)

    public BO.Enums.Role Role { get; private set; }
    // User ID entered in login form

    public int userId { get; private set; }
    // Password entered in login form

    public string password { get; private set; }
    // Static reference to the business logic layer

    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    // Constructor initializes the login window

    public LoginDialog()
    {
       
        InitializeComponent();
    }
    // Handles the login button click event

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Get user input

            userId = int.Parse(txtUserId.Text);
             password = txtPassword.Password;
            BO.Enums.Role Role = BO.Enums.Role.NONE;
            // Attempt login and get role

            Role= s_bl.Volunteer.Login(userId, password);
            // If login is successful

            if (Role!= BO.Enums.Role.NONE && Role !=null)
            {
                // Retrieve volunteer details

                v = s_bl.Volunteer.Read(userId);
                // Open volunteer main window

                if (BO.Enums.Role.volunteer == Role) {
                    new VolunteerMainWindow(Role, v).Show();
                    userId = 0;
                    password = "";
                    Role = BO.Enums.Role.NONE;
                }
                // Open manager choose page window

                else
                {
                    // Ensure only one manager window is open at a time

                    if (mcpWindow == null || !mcpWindow.IsLoaded)
                    {
                        mcpWindow = new ManagerChoosePageWindow(Role, v);
                        userId = 0;
                        password = "";
                        Role = BO.Enums.Role.NONE;
                        mcpWindow.Owner = this;
                        mcpWindow.Closed += (s, args) => mcpWindow = null;
                        mcpWindow.Show();
                    }
                    else
                    {
                        // Show message if manager window already open

                        MessageBox.Show("Sorry, but Only one manager can enter at once!!");
                    }
                }
                // Clear credentials

                userId = 0;
                password = "";
                Role = BO.Enums.Role.NONE;

            }
            else
            {
                // Show error for invalid credentials

                MessageBox.Show("פרטי התחברות שגויים", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            // Handle unexpected errors

            MessageBox.Show(ex.Message);
        }
        
        
    }
    

}
