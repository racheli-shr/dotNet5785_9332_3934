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

namespace PL.Call;

/// <summary>
/// Interaction logic for CallListWindow.xaml
/// </summary>
public partial class CallListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.CallInList? SelectedCall { get; set; }

    public BO.Enums.CallType FilterByCallType { get; set; } = BO.Enums.CallType.NONE;
    public CallListWindow()
    {
        InitializeComponent();
    }
    public IEnumerable<BO.CallInList> CallList
    {
        get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

    private void CallType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CallList = (FilterByCallType == BO.Enums.CallType.NONE) ? s_bl?.Call.GetFilteredAndCallList(null, null)! : s_bl?.Call.GetFilteredAndCallList(null, FilterByCallType)!;

    }
    private void queryCallList()
    => CallList = (FilterByCallType == BO.Enums.CallType.NONE) ?
        s_bl?.Call.GetFilteredAndCallList(null, null)! : s_bl?.Call.GetFilteredAndCallList(null, FilterByCallType)!;

    private void callListObserver()
        => queryCallList();


    private void CallListWindow_Closed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(callListObserver);

    }

    private void CallListWindow_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(callListObserver);


    }


    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        new CallWindow(0).Show();
    }



    private void lsvCallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedCall != null)
            new CallWindow(SelectedCall.CallId).Show();
    }

    private void DeleteCall_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is BO.CallInList call)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete call {call.CallId}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Console.WriteLine(call.CallId);
                    s_bl.Call.DeleteCall(call.CallId);
                    MessageBox.Show("call deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete call:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
