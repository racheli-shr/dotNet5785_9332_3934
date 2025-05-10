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

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerListWindow.xaml
/// </summary>
public partial class VolunteerListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.VolunteerInList? SelectedVolunteer { get; set; }

    public VolunteerListWindow()
    {
        InitializeComponent();
    }
    public IEnumerable<BO.VolunteerInList> VolunteerList
    {
        get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
        set { SetValue(VolunteerListProperty, value); }
    }

    public static readonly DependencyProperty VolunteerListProperty = DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));
    public BO.Enums.VolunteerSortField Filter { get; set; } = BO.Enums.VolunteerSortField.NONE;

   

    private void Sort_By(object sender, SelectionChangedEventArgs e)
    {
        VolunteerList = (Filter == BO.Enums.VolunteerSortField.NONE) ?s_bl?.Volunteer.GetVolunteersList(null,null)! : s_bl?.Volunteer.GetVolunteersList(null,Filter)!;
    }
    private void queryVolunteerList()
    => VolunteerList = (Filter == BO.Enums.VolunteerSortField.NONE) ?
        s_bl?.Volunteer.GetVolunteersList(null,null)! : s_bl?.Volunteer.GetVolunteersList(null, Filter)!;

    private void volunteerListObserver()
        => queryVolunteerList();
 

    private void VolunteerListWindow_Closed(object sender, EventArgs e)
    {
        s_bl.Volunteer.RemoveObserver(volunteerListObserver);

    }

    private void VolunteerListWindow_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Volunteer.AddObserver(volunteerListObserver);
        

    }

 

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerWindow(0).Show();
    }

   

    private void lsvVolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedVolunteer != null)
            new VolunteerWindow(SelectedVolunteer.Id).Show();
    }

    private void DeleteVolunteer_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is BO.VolunteerInList volunteer)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete volunteer {volunteer.FullName} (ID: {volunteer.Id})?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Volunteer.DeleteVolunteer(volunteer.Id);
                    MessageBox.Show("Volunteer deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete volunteer:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

