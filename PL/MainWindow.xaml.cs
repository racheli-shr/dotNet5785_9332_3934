using PL.Volunteer;
using PL.Call;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.Enums.Role Role { get; set; }
    private VolunteerListWindow volunteerWindow;
    private CallListWindow callWindow;

    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentTimeProperty =
    DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(DateTime.MinValue));



    public int Interval
    {
        get { return (int)GetValue(IntervalProperty); }
        set { SetValue(IntervalProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IntervalProperty =
        DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow), new PropertyMetadata(2000));



    public BO.Volunteer volunteer
    {
        get { return (BO.Volunteer)GetValue(volunteerProperty); }
        set { SetValue(volunteerProperty, value); }
    }

    // Using a DependencyProperty as the backing store for volunteer.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty volunteerProperty =
        DependencyProperty.Register("volunteer", typeof(BO.Volunteer), typeof(MainWindow), new PropertyMetadata(null));



    public bool EnableToChange
    {
        get { return (bool)GetValue(EnableToChangeProperty); }
        set { SetValue(EnableToChangeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for EnableToChange.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty EnableToChangeProperty =
        DependencyProperty.Register("EnableToChange", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));



    public bool SimulatorFlag
    {
        get { return (bool)GetValue(SimulatorFlagProperty); }
        set { SetValue(SimulatorFlagProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SimulatorFlag.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SimulatorFlagProperty =
        DependencyProperty.Register("SimulatorFlag", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));



    public string SimulatorButtonText
    {
        get { return (string)GetValue(SimulatorButtonTextProperty); }
        set { SetValue(SimulatorButtonTextProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SimulatorButtonText.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SimulatorButtonTextProperty =
        DependencyProperty.Register("SimulatorButtonText", typeof(string), typeof(MainWindow), new PropertyMetadata("Start simulator"));

    public MainWindow(BO.Enums.Role r, BO.Volunteer vol)
    {
        InitializeComponent();
        try
        {
            CallStatusSummaries = s_bl.Call.GetCallStatusSummaries();

        }
        catch (Exception ex) { MessageBox.Show(ex.Message); }

        Role = r;
        volunteer = vol;
    }

    private void BtnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Admin.ForwardClock(BO.Enums.TimeUnit.MINUTE);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    // Advances system clock by one hour.

    private void BtnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Admin.ForwardClock(BO.Enums.TimeUnit.HOUR);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    // Advances system clock by one month.

    private void BtnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Admin.ForwardClock(BO.Enums.TimeUnit.MONTH);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    // Advances system clock by one year.

    private void BtnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Admin.ForwardClock(BO.Enums.TimeUnit.YEAR);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    // Advances system clock by one day.

    private void BtnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Admin.ForwardClock(BO.Enums.TimeUnit.DAY);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    // Updates the risk range configuration in the system.

    private void BtnUpdateRiskRange_Click(object sender, RoutedEventArgs e)
    {

        try
        {
            s_bl.Admin.SetMaxRange(RiskRange);

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    // Updates UI to reflect the current system clock time.

    private volatile DispatcherOperation? _observerOperation = null; //stage 7
    private void updateSummaryCallsObserver()
    {
        try
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    CallStatusSummaries = s_bl.Call.GetCallStatusSummaries();
                });
        }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    

    private void ClockObserver()
    {
        try
        {

            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    CurrentTime = s_bl.Admin.GetClock();
                });
        }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }
    // Updates UI to reflect the current risk range configuration.

 
    public object CallStatusSummaries
    {
        get { return (object)GetValue(CallStatusSummariesProperty); }
        set { SetValue(CallStatusSummariesProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CallStatusSummaries.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CallStatusSummariesProperty =
        DependencyProperty.Register("CallStatusSummaries", typeof(object), typeof(MainWindow), new PropertyMetadata(null));



    public TimeSpan RiskRange
    {
        get { return (TimeSpan)GetValue(RiskRangeProperty); }
        set { SetValue(RiskRangeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for RiskRange.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty RiskRangeProperty =
    DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow), new PropertyMetadata(TimeSpan.Zero));
    // Initializes UI with current clock and config values, and registers observers.

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            CurrentTime = s_bl.Admin.GetClock();
            RiskRange = s_bl.Admin.GetMaxRange();

            //s_bl.Admin.AddConfigObserver(ConfigObserver);
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Call.AddObserver(updateSummaryCallsObserver);
            s_bl.Admin.AddRiskObserver(updateSummaryCallsObserver);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

    }
    // Unregisters observers when the window closes.

    private void MainWindow_Closed(object sender, EventArgs e)
    {
        try
        {
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            //s_bl.Admin.RemoveConfigObserver(ConfigObserver);
            s_bl.Call.RemoveObserver(updateSummaryCallsObserver);
            s_bl.Admin.RemoveRiskObserver(updateSummaryCallsObserver);

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    // Opens the volunteer list window if not already open.

    private void HandleVolunteer_Click(object sender, RoutedEventArgs e)
    {
        if (volunteerWindow == null || !volunteerWindow.IsLoaded)
        {
            volunteerWindow = new VolunteerListWindow(volunteer);
            volunteerWindow.Owner = this;
            volunteerWindow.Closed += (s, args) => volunteerWindow = null;
            volunteerWindow.Show();
        }
        else
        {
            MessageBox.Show("Sorry! but the window is already open");
        }
    }



    // Opens the call list window if not already open.

    private void HandleCall_click(object sender, RoutedEventArgs e)
    {
        if (callWindow == null || !callWindow.IsLoaded)
        {
            try
            {
                callWindow = new CallListWindow();
                callWindow.Owner = this;
                callWindow.Closed += (s, args) => callWindow = null;
                callWindow.Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        else
        {
            MessageBox.Show("Sorry! but the window is already open");
        }
    }
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
    // Resets the database after user confirmation, closing other windows and showing progress.

    private void BtnResetDB(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show(
        "האם אתה בטוח שברצונך לאתחל/לאפס את בסיס הנתונים?",
        "אישור פעולה",
        MessageBoxButton.YesNo,
        MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
            return;


        try
        {
            // שנה את סמן העכבר לשעון חול
            Mouse.OverrideCursor = Cursors.Wait;

            // סגור את כל החלונות מלבד הראשי
            foreach (Window win in Application.Current.Windows)
            {
                if (win != this)
                    win.Close();
            }

            // אתחול/איפוס DB — הפוך את זה ל־await אם זו פעולה איטית (Task)
            Task.Run(() =>
            {
                // כאן תוכל לבחור איזו מתודה להפעיל — או שתעשה תנאי אם יש כפתור נפרד לכל אחת
                s_bl.Admin.ResetDB();  // או InitializeDB()
            });

            MessageBox.Show("בסיס הנתונים אופס בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"אירעה שגיאה:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            // החזר את סמן העכבר לקדמותו
            Mouse.OverrideCursor = null;
        }
    }
    // Initializes the database after user confirmation, closing other windows and showing progress.

    private void BtnInitializeDB(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show(
        "האם אתה בטוח שברצונך לאתחל/לאפס את בסיס הנתונים?",
        "אישור פעולה",
        MessageBoxButton.YesNo,
        MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
            return;


        try
        {
            // שנה את סמן העכבר לשעון חול
            Mouse.OverrideCursor = Cursors.Wait;

            // סגור את כל החלונות מלבד הראשי
            foreach (Window win in Application.Current.Windows)
            {
                if (win != this)
                    win.Close();
            }

            // אתחול/איפוס DB — הפוך את זה ל־await אם זו פעולה איטית (Task)
            Task.Run(() =>
            {
                // כאן תוכל לבחור איזו מתודה להפעיל — או שתעשה תנאי אם יש כפתור נפרד לכל אחת
                s_bl.Admin.InitializeDB();
                // או InitializeDB()
            });

            MessageBox.Show("בסיס הנתונים אותחל בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"אירעה שגיאה:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            // החזר את סמן העכבר לקדמותו
            Mouse.OverrideCursor = null;
        }
    }

    private void startOrStopSimulator(object sender, RoutedEventArgs e)
    {
        //אם הוא היה מכובה
        if (SimulatorFlag == false)
        {
            try
            {
                //הופך לדלוק
                SimulatorFlag = true;
                s_bl.Admin.StartSimulator(Interval); //stage 7
                SimulatorButtonText = "Stop Simulator";
                EnableToChange = false;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }
        else
        {

            try
            {
                //הופך למכובה
                SimulatorFlag = false;
                s_bl.Admin.StopSimulator(); //stage 7
                SimulatorButtonText = "Start Simulator";
                EnableToChange = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }



}

