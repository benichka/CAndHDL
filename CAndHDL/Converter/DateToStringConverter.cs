using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CAndHDL.Converter
{
    /// <summary>
    /// Converter converting a date (DateTime) to a String
    /// </summary>
    class DateToStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert a string to a visibility object
        /// </summary>
        /// <param name="value">DateTime value</param>
        /// <param name="targetType">TODO</param>
        /// <param name="parameter">The string format to display</param>
        /// <param name="language">TODO</param>
        /// <returns>The DateTime converted to String with to desired format</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return string.Empty;
            }
            else
            {
                return (value as DateTimeOffset?).Value.ToString(parameter.ToString());
            }
        }

        /// <summary>
        /// Convert a String with a specified format to a date
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="targetType">TODO</param>
        /// <param name="parameter">TODO</param>
        /// <param name="language">TODO</param>
        /// <returns>NotImplementedException</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
