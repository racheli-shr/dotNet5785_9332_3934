using BO;
using PL.Volunteer;
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
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Call call { get; set; }
        public int CallId { get; set; } = 0;
        public CallWindow(int id)
        {
            try
            {
                ButtonText = id == 0 ? "Add" : "Update";

                if (id != 0)
                {
                    CallId = id;
                    IsUpdatingEditable = true;
                    CurrentCall = s_bl.Call.Read(id)!;

                    var hasAssignments = s_bl.Call.isExistingAssignmentToCall(id);
                    

                    if (hasAssignments)
                    {
                        IsEditable = true;
                        IsMaxTimeEditable = true;
                    }
                    else
                    {
                        switch (CurrentCall.Status)
                        {
                            case BO.Enums.CallStatus.Open:
                            case BO.Enums.CallStatus.OpenAtRisk:
                                IsEditable = false;
                                IsMaxTimeEditable = false;
                                break;

                            case BO.Enums.CallStatus.InProgress:
                            case BO.Enums.CallStatus.InProgressAtRisk:
                                IsEditable = true;
                                IsMaxTimeEditable = false;
                                break;

                            case BO.Enums.CallStatus.Closed:
                            case BO.Enums.CallStatus.Expired:
                                IsEditable = true;
                                IsMaxTimeEditable = true;
                                break;
                        }
                    }
                }
                else
                {
                    CurrentCall = new BO.Call() { Id = 0 };
                    IsEditable = true;
                    IsMaxTimeEditable = true;
                }

                DataContext = this; // חשוב כדי שכל התכונות יעבדו

            InitializeComponent();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public BO.Enums.CallType callType { get; set; } = BO.Enums.CallType.NONE;



        public BO.Enums.CallType CallType
        {
            get { return (BO.Enums.CallType)GetValue(CallTypeProperty); }
            set { SetValue(CallTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CallType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallTypeProperty =
            DependencyProperty.Register("CallType", typeof(BO.Enums.CallType), typeof(VolunteerWindow), new PropertyMetadata(BO.Enums.CallType.NONE));



        public bool IsUpdatingEditable
        {
            get { return (bool)GetValue(IsUpdatingEditableProperty); }
            set { SetValue(IsUpdatingEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsUpdatingEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUpdatingEditableProperty =
            DependencyProperty.Register("IsUpdatingEditable", typeof(bool), typeof(CallWindow), new PropertyMetadata(true));




        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(CallWindow), new PropertyMetadata("add"));



        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(CallWindow), new PropertyMetadata(true));




        public bool IsMaxTimeEditable
        {
            get { return (bool)GetValue(IsMaxTimeEditableProperty); }
            set { SetValue(IsMaxTimeEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMaxTimeEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMaxTimeEditableProperty =
            DependencyProperty.Register("IsMaxTimeEditable", typeof(bool), typeof(CallWindow), new PropertyMetadata(true));



        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        private void AddUpdate_btn(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    Console.WriteLine(CurrentCall);
                    s_bl.Call.AddCall(CurrentCall!);
                    MessageBox.Show("The call was been added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var call = s_bl.Call.Read(CurrentCall.Id);
                    if(call.OpeningTime!=CurrentCall.OpeningTime)
                    {
                        MessageBox.Show("can't change the opening date.");
                        return;
                    }
                    s_bl.Call.UpdateCall( CurrentCall!);
                    MessageBox.Show("The call was been Updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                Close(); // סגירת חלון לאחר הצלחה
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Selected_callType(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentCall != null)
            {
                CurrentCall.CallType = callType;
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void queryCallWindow()
        {
            try
            {
                if (CallId != 0)
                {
                CurrentCall = s_bl.Call.Read(CallId)!;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void CallWindowObserver()
        {
            try
            {
                if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryCallWindow();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                s_bl.Call.AddObserver(CallWindowObserver);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
                try
                {
                    s_bl.Call.RemoveObserver(CallWindowObserver);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
