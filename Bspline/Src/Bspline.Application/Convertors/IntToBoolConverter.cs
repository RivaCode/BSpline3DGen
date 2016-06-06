using System;
using System.Globalization;
using System.Windows.Data;

namespace BsplineKinect.Convertors
{
    /// <summary>
    /// Helper class to help WPF to convert from int to bool <see cref="IValueConverter"/>
    /// </summary>
    [ValueConversion(typeof(bool), typeof(int))]
    public sealed class IntToBoolConverter : IValueConverter
    {
        public int DisabledValue { get; set; }

        /// <summary>
        /// <see cref="IValueConverter"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (!(value is int))
                return null;

            int intValue = (int) value;
            return intValue > this.DisabledValue;
        }

        /// <summary>
        /// <see cref="IValueConverter"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}