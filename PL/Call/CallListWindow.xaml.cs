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
        Console.WriteLine(FilterByCallType);
        CallList = (FilterByCallType == BO.Enums.CallType.NONE) ? s_bl?.Call.GetFilteredAndCallList(null, null)! : s_bl?.Call.GetFilteredAndCallList(Enums.CallInListFields.CallType, FilterByCallType)!;

    }
    private void queryCallList()
    => CallList = (FilterByCallType == BO.Enums.CallType.NONE) ?
        s_bl?.Call.GetFilteredAndCallList(null, null)! : s_bl?.Call.GetFilteredAndCallList(null, FilterByCallType)!;

    private volatile DispatcherOperation? _observerOperation = null; //stage 7

    private void callListObserver()
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                queryCallList();
            });


    }


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

    private void DeleteAssignmentsCall_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is BO.CallInList call)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete **all assignments** from call {call.CallId}?",
                "Confirm Assignment Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (s_bl.Call.closeLastAssignmentByCallId(call.CallId,DO.Enums.AssignmentStatus.MANAGER_CANCELLED))
                    {
                        MessageBox.Show("success");
                    }
                    else
                    {
                        MessageBox.Show("denied");

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete assignments:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
