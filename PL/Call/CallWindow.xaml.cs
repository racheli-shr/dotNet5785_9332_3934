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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Call call { get; set; }
        public string ButtonText { get; set; }
        public bool IsEditable { get; set; } = true;
        public bool IsMaxTimeEditable { get; set; } = true;
        public bool IsUpdatingEditable { get; set; } = false;

        public CallWindow(int id)
        {
            
                ButtonText = id == 0 ? "Add" : "Update";
                InitializeComponent();

                if (id != 0)
                {
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
                    IsEditable = false;
                    IsMaxTimeEditable = false;
                }

                DataContext = this; // חשוב כדי שכל התכונות יעבדו
            


            //ButtonText = id == 0 ? "Add" : "Update";

            //InitializeComponent();
            //CurrentCall = (id != 0) ? s_bl.Call.Read(id)! : new BO.Call() { Id = 0 };

        }
        public BO.Enums.CallType callType { get; set; } = BO.Enums.CallType.NONE;


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
    }
}
