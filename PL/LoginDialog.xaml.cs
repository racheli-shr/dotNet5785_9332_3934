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
    public BO.Volunteer v;
    public BO.Enums.Role Role { get; private set; }
    public int userId { get; private set; }
    public string password { get; private set; }
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private static bool _isManagerLoggedIn = false;

    public LoginDialog()
    {
       
        InitializeComponent();
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            //bool manager=false;
             userId = int.Parse(txtUserId.Text);
             password = txtPassword.Password;
            BO.Enums.Role Role = BO.Enums.Role.NONE;
            Role= s_bl.Volunteer.Login(userId, password);
            if(Role!= BO.Enums.Role.NONE && Role !=null)
            {
                v = s_bl.Volunteer.Read(userId);
                if (BO.Enums.Role.volunteer == Role) {
                    new VolunteerMainWindow(Role, v).Show();
                    userId = 0;
                    password = "";
                    Role = BO.Enums.Role.NONE;
                }
                else
                {
                    if (!_isManagerLoggedIn) {
                        new ManagerChoosePageWindow(Role, v).Show();
                        userId = 0;
                        password = "";
                        Role = BO.Enums.Role.NONE;
                        _isManagerLoggedIn =true;
                    }
                    else
                    {
                        MessageBox.Show("Sorry, but Only one manager can enter at once!!");
                    }
                }
                userId = 0;
                password = "";
                Role = BO.Enums.Role.NONE;

            }
            else
            {
                MessageBox.Show("פרטי התחברות שגויים", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        
        
    }
    

}
