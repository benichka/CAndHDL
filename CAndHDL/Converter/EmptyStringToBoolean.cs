﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CAndHDL.Converter
{
    /// <summary>
    /// Converter converting a string or a date to a visibility
    /// </summary>
    class EmptyStringToBoolean : IValueConverter
    {
        /// <summary>
        /// Convert a string or a date to a visibility object
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="targetType">TODO</param>
        /// <param name="parameter">TODO</param>
        /// <param name="language">TODO</param>
        /// <returns>Visibility.Collapsed if the string is empty, Visibility.Visible otherwise</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((value as DateTimeOffset?).HasValue)
            {
                // The object can be a date
                return Visibility.Visible;
            }
            else
            {
                // Otherwise, consider it as a string
                return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        /// <summary>
        /// Convert a visibility object to a string
        /// </summary>
        /// <param name="value">Visibility value</param>
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
