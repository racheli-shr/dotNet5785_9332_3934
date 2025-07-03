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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallsHistoryWindow.xaml
    /// </summary>
    public partial class CallsHistoryWindow : Window
    {


        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        //public BO.Enums.CallType FilterByCallType { get; set; } = BO.Enums.CallType.NONE;


        public IEnumerable<BO.ClosedCallInList> ClosedCallInList
        {
            get { return (IEnumerable<BO.ClosedCallInList>)GetValue(ClosedCallInListProperty); }
            set { SetValue(ClosedCallInListProperty, value); }
        }

        public static readonly DependencyProperty ClosedCallInListProperty =
            DependencyProperty.Register("ClosedCallInList", typeof(IEnumerable<BO.ClosedCallInList>), typeof(CallsHistoryWindow), new PropertyMetadata(null));




        public BO.Enums.TypeOfTreatmentTerm EndTreatmentType
        {
            get { return (BO.Enums.TypeOfTreatmentTerm)GetValue(EndTreatmentTypeProperty); }
            set { SetValue(EndTreatmentTypeProperty, value); }
        }

        public static readonly DependencyProperty EndTreatmentTypeProperty =
            DependencyProperty.Register("EndTreatmentType", typeof(BO.Enums.TypeOfTreatmentTerm), typeof(CallsHistoryWindow), new PropertyMetadata(BO.Enums.TypeOfTreatmentTerm.NONE));

        private BO.Volunteer volunteer { get; set; }

        public CallsHistoryWindow(BO.Volunteer v)
        {
            InitializeComponent();
            volunteer = v;
            ClosedCallInList = s_bl.Call.GetClosedCallsByVolunteer(v.Id, null).ToList();
        }

        private void FilterClosedCalls(object sender, SelectionChangedEventArgs e)
        {
            if (volunteer != null)
            {
                var allClosedCalls = s_bl.Call.GetClosedCallsByVolunteer(volunteer.Id, null);

                ClosedCallInList = (EndTreatmentType == BO.Enums.TypeOfTreatmentTerm.NONE) ?
                    allClosedCalls.ToList() :
                    allClosedCalls.Where(c => c.FinishType == EndTreatmentType).ToList();
            }
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(CallsHistoryWindowObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(CallsHistoryWindowObserver);
        }

        private void queryCallsHistoryWindow()
        {
            ClosedCallInList = (EndTreatmentType == BO.Enums.TypeOfTreatmentTerm.NONE) ?
                s_bl.Call.GetClosedCallsByVolunteer(volunteer.Id, null).ToList() : s_bl.Call.GetClosedCallsByVolunteer(volunteer.Id, (closedCall) => closedCall.FinishType == EndTreatmentType).ToList()!;
        }
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void CallsHistoryWindowObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryCallsHistoryWindow();
                });

        }





    }
}