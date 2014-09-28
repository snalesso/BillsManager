using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BillsManager.Views.Behaviors
{
    public class TextBoxBehavior
    {
        #region input filtering

        #region fields

        private const string DEFAULT_INPUT_FORMAT = "(?s).*";

        private static Regex inputFormatRegex = new Regex(DEFAULT_INPUT_FORMAT);

        private static readonly Dictionary<TextBox, Regex> regexes = new Dictionary<TextBox, Regex>();

        #endregion

        #region filter input

        public static readonly DependencyProperty FilterInputProperty =
            DependencyProperty.RegisterAttached(
            "FilterInput",
            typeof(bool),
            typeof(TextBoxBehavior),
            new UIPropertyMetadata(false, OnFilterInputChanged));

        public static bool GetFilterInput(TextBox sender)
        {
            return (bool)sender.GetValue(TextBoxBehavior.FilterInputProperty);
        }

        public static void SetFilterInput(TextBox sender, bool value)
        {
            sender.SetValue(TextBoxBehavior.FilterInputProperty, value);
        }

        private static void OnFilterInputChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var txt = depObj as TextBox;

            if (txt == null) return;

            if (e.NewValue is bool == false) return;

            if ((bool)e.NewValue)
            {
                regexes.Add(txt, new Regex(GetInputFormat(txt)));

                txt.PreviewTextInput += OnPreviewTextInput;
                DataObject.AddPastingHandler(txt, OnClipboardPaste);
            }
            else
            {
                txt.PreviewTextInput -= OnPreviewTextInput;
                DataObject.RemovePastingHandler(txt, OnClipboardPaste);

                regexes.Remove(txt);
            }
        }

        private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt == null) return;

            if (!IsValid(txt, e.Text))
                e.Handled = true;
        }

        private static void OnClipboardPaste(object sender, DataObjectPastingEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt == null) return;

            string text = e.SourceDataObject.GetData(e.FormatToApply) as string;

            if (!IsValid(txt, text))
                e.CancelCommand();
        }

        private static bool IsValid(TextBox txt, string newContent)
        {
            string testString = string.Empty;

            if (!string.IsNullOrEmpty(txt.SelectedText))
            {
                string pre = txt.Text.Substring(0, txt.SelectionStart);
                string after = txt.Text.Substring(txt.SelectionStart + txt.SelectionLength, txt.Text.Length - (txt.SelectionStart + txt.SelectionLength));
                testString = pre + newContent + after;
            }
            else
            {
                string pre = txt.Text.Substring(0, txt.CaretIndex);
                string after = txt.Text.Substring(txt.CaretIndex, txt.Text.Length - txt.CaretIndex);
                testString = pre + newContent + after;
            }

            if (regexes[txt].IsMatch(testString))
                return true;

            return false;
        }

        #endregion

        #region input format

        public static readonly DependencyProperty InputFormatProperty =
            DependencyProperty.RegisterAttached(
            "InputFormat",
            typeof(string),
            typeof(TextBoxBehavior),
            new PropertyMetadata(DEFAULT_INPUT_FORMAT, OnInputFormatChanged)); // ^\d{5}$

        public static string GetInputFormat(TextBox sender)
        {
            return (string)sender.GetValue(TextBoxBehavior.InputFormatProperty);
        }

        public static void SetInputFormat(TextBox sender, string value)
        {
            sender.SetValue(TextBoxBehavior.InputFormatProperty, value);
        }

        private static void OnInputFormatChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var txt = depObj as TextBox;
            if (txt == null) return;

            if (e.NewValue is string == false) return;

            if (regexes.ContainsKey(txt))
                regexes[txt] = new Regex(GetInputFormat(txt));
        }

        #endregion

        #region ValueWhenLeftEmpty

        public static readonly DependencyProperty ValueWhenLeftEmptyProperty =
            DependencyProperty.RegisterAttached(
            "ValueWhenLeftEmpty",
            typeof(string),
            typeof(TextBoxBehavior),
            new PropertyMetadata(null, OnValueWhenLeftEmptyChanged));

        public static string GetValueWhenLeftEmpty(TextBox sender)
        {
            return (string)sender.GetValue(TextBoxBehavior.ValueWhenLeftEmptyProperty);
        }

        public static void SetValueWhenLeftEmpty(TextBox sender, string value)
        {
            sender.SetValue(TextBoxBehavior.ValueWhenLeftEmptyProperty, value);
        }

        private static void OnValueWhenLeftEmptyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var txt = depObj as TextBox;
            if (txt == null) return;

            if (e.NewValue is string == false) return;

            if (e.NewValue != null)
                txt.LostFocus += SetValueWhenLeftEmpty;
            else
                txt.LostFocus -= SetValueWhenLeftEmpty;
        }

        private static void SetValueWhenLeftEmpty(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt == null) return;

            if (txt.Text == string.Empty)
                txt.Text = GetValueWhenLeftEmpty(txt);
        }

        #endregion

        #endregion
    }
}