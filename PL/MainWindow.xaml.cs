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

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.Enums.Role Role { get;  set; }
    public BO.Volunteer volunteer;
    private VolunteerListWindow volunteerWindow;
    private CallListWindow callWindow;
    public MainWindow(BO.Enums.Role r,BO.Volunteer vol)
    {
        InitializeComponent();
        Role = r;
        volunteer = vol;
    }
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentTimeProperty =
    DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(DateTime.MinValue));
    private void BtnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.Enums.TimeUnit.MINUTE);
    }

    private void BtnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.Enums.TimeUnit.HOUR);
    }
    private void BtnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.Enums.TimeUnit.MONTH);
    }
    private void BtnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.Enums.TimeUnit.YEAR);
    }
    private void BtnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.Enums.TimeUnit.DAY);
    }
    private void BtnUpdateRiskRange_Click(object sender, RoutedEventArgs e)
    {
        
        s_bl.Admin.SetMaxRange(RiskRange);
    }

    private void ClockObserver()
    {
        Dispatcher.Invoke(() => CurrentTime = s_bl.Admin.GetClock());
    }
    private void ConfigObserver()
    {
        Dispatcher.Invoke(() => RiskRange = s_bl.Admin.GetMaxRange());
    }

    public TimeSpan RiskRange
    {
        get { return (TimeSpan)GetValue(RiskRangeProperty); }
        set { SetValue(RiskRangeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for RiskRange.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty RiskRangeProperty =
    DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow), new PropertyMetadata(TimeSpan.Zero));
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        
        CurrentTime = s_bl.Admin.GetClock();
        RiskRange = s_bl.Admin.GetMaxRange();
        s_bl.Admin.AddConfigObserver(ConfigObserver);
        s_bl.Admin.AddClockObserver(ClockObserver);
    }
   
    private void MainWindow_Closed(object sender, EventArgs e)
    {
        s_bl.Admin.RemoveClockObserver(ClockObserver);
        s_bl.Admin.RemoveConfigObserver(ConfigObserver);
    }
    private void HandleVolunteer_Click(object sender, RoutedEventArgs e)
    {
        if (volunteerWindow == null || !volunteerWindow.IsLoaded)
        {
            volunteerWindow = new VolunteerListWindow();
            volunteerWindow.Owner = this; 
            volunteerWindow.Closed += (s, args) => volunteerWindow = null;
            volunteerWindow.Show();
        }
        else
        {
            MessageBox.Show("Sorry! but the window is already open");
        }
    }




    private void HandleCall_click(object sender, RoutedEventArgs e)
    {
        if (callWindow == null || !callWindow.IsLoaded)
        {
            callWindow = new CallListWindow();
            callWindow.Owner = this;
            callWindow.Closed += (s, args) => callWindow = null;
            callWindow.Show();
        }
        else
        {
            MessageBox.Show("Sorry! but the window is already open");
        }
    }
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // כרגע היא ריקה - תוסיפי פה קוד אם צריך
    }
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
}

