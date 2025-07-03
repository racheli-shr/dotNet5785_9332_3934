using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

namespace PL
{
    // Contains custom value converters for data binding
    public class Converter
    {
        // Converts a boolean value to Visibility:
        // true -> Visible, false -> Collapsed
        public class BoolToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
                (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
                throw new NotImplementedException();// One-way binding only
        }
        // Inverts a boolean value: true -> false, false -> true
        public class InverseBoolConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
                !(value is bool b && b);

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
                throw new NotImplementedException(); // One-way binding only
        }
        //public class CallTypeToBrushConverter : IValueConverter
        //{
        //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        if (value is BO.Enums.CallType callType)
        //        {
        //            switch (callType)
        //            {
        //                case BO.Enums.CallType.desert:
        //                    return Brushes.LightPink;
        //                case BO.Enums.CallType.mainMeal:
        //                    return Brushes.LightYellow;
        //                case BO.Enums.CallType.pastry:
        //                    return Brushes.LightBlue;
        //                case BO.Enums.CallType.salad:
        //                    return Brushes.LightGray;
        //                default:
        //                    return Brushes.White;
        //            }
        //        }

        //        return Brushes.Transparent;
        //    }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException(); // חד כיווני
            }
        }
    }



