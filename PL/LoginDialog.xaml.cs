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

namespace PL;

/// <summary>
/// Interaction logic for LoginDialog.xaml
/// </summary>
public partial class LoginDialog : Window
{

    public BO.Enums.Role Role { get; private set; }
    public int userId { get; private set; }
    public string password { get; private set; }
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public LoginDialog()
    {
       
        InitializeComponent();
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        try
        {
             userId = int.Parse(txtUserId.Text);
             password = txtPassword.Password;
            BO.Enums.Role Role = BO.Enums.Role.NONE;
            Role= s_bl.Volunteer.Login(userId, password);
            if(Role!= BO.Enums.Role.NONE && Role !=null)
            {
                this.Role = Role;
                this.DialogResult = true;
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
