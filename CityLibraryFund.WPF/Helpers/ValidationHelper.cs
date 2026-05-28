using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CityLibraryFund.WPF.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static bool IsValidPhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return true;
            }

            return Regex.IsMatch(phone, @"^\+?[0-9\s\-\(\)]{10,20}$");
        }

        public static void AllowOnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]+$");
        }

        public static void PreventPasteNonNumeric(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            string text = (string)e.DataObject.GetData(typeof(string));

            if (!Regex.IsMatch(text, @"^[0-9]+$"))
            {
                e.CancelCommand();
            }
        }
    }
}