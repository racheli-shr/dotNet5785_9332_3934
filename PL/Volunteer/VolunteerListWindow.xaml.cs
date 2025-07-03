using BO;
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
using System.Windows.Threading;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerListWindow.xaml
/// </summary>
public partial class VolunteerListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.VolunteerInList? SelectedVolunteer { get; set; }

    public VolunteerListWindow(BO.Volunteer vol)
    {
        InitializeComponent();
        try { 
        Manager = vol;
        VolunteerList = (Filter == BO.Enums.VolunteerSortField.NONE) ?
        s_bl?.Volunteer.GetVolunteersList(null, null)! : s_bl?.Volunteer.GetVolunteersList(null, Filter)!;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    public IEnumerable<BO.VolunteerInList> VolunteerList
    {
        get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
        set { SetValue(VolunteerListProperty, value); }
    }

    public static readonly DependencyProperty VolunteerListProperty = DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));


    public BO.Enums.VolunteerSortField Filter { get; set; } = BO.Enums.VolunteerSortField.NONE;


    // Updates the volunteer list based on the selected sorting filter.


    public BO.Volunteer Manager
    {
        get { return (BO.Volunteer)GetValue(ManagerProperty); }
        set { SetValue(ManagerProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Manager.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ManagerProperty =
        DependencyProperty.Register("Manager", typeof(BO.Volunteer), typeof(VolunteerListWindow), new PropertyMetadata(null));



    private void Sort_By(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            VolunteerList = (Filter == BO.Enums.VolunteerSortField.NONE) ?s_bl?.Volunteer.GetVolunteersList(null,null)! : s_bl?.Volunteer.GetVolunteersList(null,Filter)!;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    // Queries and updates the volunteer list according to the current filter.



    // Queries and updates the volunteer list according to the current filter.

    private void queryVolunteerList()
    {
        try
        {
            VolunteerList = (Filter == BO.Enums.VolunteerSortField.NONE) ? s_bl?.Volunteer.GetVolunteersList(null, null)! : s_bl?.Volunteer.GetVolunteersList(null, Filter)!;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
   
    
    private volatile DispatcherOperation? _observerOperation = null; //stage 7
    // Observer callback that refreshes the volunteer list when notified.

    private void volunteerListObserver()
    {
        try
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                queryVolunteerList();
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }


    private void VolunteerListWindow_Closed(object sender, EventArgs e)
    {
        try
        {
            s_bl.Volunteer.RemoveObserver(volunteerListObserver);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    // Adds the volunteer list observer when the window is loaded.

    private void VolunteerListWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Volunteer.AddObserver(volunteerListObserver);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

    }


    // Opens a new window to add a new volunteer.


    private void AddButton_Click(object sender, RoutedEventArgs e)

    {
        try
        {
            new VolunteerWindow(0, "VolunteerListWindow").ShowDialog();

        }
        catch (Exception ex)
        {
            MessageBox.Show("error opening add/update window" + ex.Message);

        }
    }

    // Opens the volunteer details window for the selected volunteer on double-click.



    private void lsvVolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {


        try
        {
            if (SelectedVolunteer != null)
                new VolunteerWindow(SelectedVolunteer.Id, "VolunteerListWindow").ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show("error opening add/update window" + ex.Message);

        }
    }

    private void DeleteVolunteer_Click(object sender, RoutedEventArgs e)
    {    // Confirms and deletes the selected volunteer, with error handling.

        if (sender is Button btn && btn.Tag is BO.VolunteerInList volunteer)
        {
            if(volunteer.IsActive==true) { MessageBox.Show("can't delete an active volunteer");return; }

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

