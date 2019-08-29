using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace RegEditor
{
    public partial class MainWindow : Window
    {
        private Color NormalColor = Color.FromRgb(0, 0, 0);
        private Color NormalBackGround = Color.FromRgb(0, 0, 0);
        private double NormalOpacity = 0;

        private Color ErrorColor = Color.FromRgb(0xFF, 0xFF, 0xFF);
        private Color ErrorBackGround = Color.FromRgb(0xA0, 0x10, 0x10);
        private double ErrorOpacity = 0.8;

        private double NormalSize = 13;
        private double ErrorSize = 13;

        public MainWindow()
        {
            InitializeComponent();
            SetAllLabelStyle(NormalColor, NormalBackGround, NormalOpacity, FontWeights.Normal, NormalSize);
        }

        private bool DigitCheck(string str)
        {
            if (str[0] != '-' && (str[0] < '0' || str[0] > '9'))
                return false;
            foreach (char c in str.Substring(1))
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        private void SetIsEnabled(bool val)
        {
            this.PathValue1.IsEnabled = val;
            this.PathValue2.IsEnabled = val;
            this.NameValue.IsEnabled = val;
            this.TypeValue.IsEnabled = val;
            this.DataValue1.IsEnabled = val;
            this.DataValue2.IsEnabled = val;
        }

        private void SetLabelStyle(System.Windows.Controls.Label label, Color foreground, Color background, double opacity, FontWeight weight, double size)
        {
            label.Foreground = new SolidColorBrush(foreground);
            label.Background = new SolidColorBrush(background);
            label.Background.Opacity = opacity;
            label.FontWeight = weight;
            label.FontSize = size;
        }

        private void SetAllLabelStyle(Color foreground, Color background, double opacity, FontWeight weight, double size)
        {
            SetLabelStyle(this.Path1, foreground, background, opacity, weight, size);
            SetLabelStyle(this.Path2, foreground, background, opacity, weight, size);
            SetLabelStyle(this.Name, foreground, background, opacity, weight, size);
            SetLabelStyle(this.Type, foreground, background, opacity, weight, size);
        }

        private void ConfirmDelete(object sender, RoutedEventArgs e)
        {
            RegistryKey RegKey, SubRegKey;

            switch (this.PathValue1.SelectedIndex)
            {
                case 0:
                    RegKey = Registry.ClassesRoot;
                    break;
                case 1:
                    RegKey = Registry.CurrentUser;
                    break;
                case 2:
                    RegKey = Registry.LocalMachine;
                    break;
                case 3:
                    RegKey = Registry.Users;
                    break;
                case 4:
                    RegKey = Registry.CurrentConfig;
                    break;
                default:
                    RegKey = null;
                    break;
            }
            SubRegKey = RegKey.OpenSubKey(this.PathValue2.Text, true);
            if (SubRegKey != null)
            {
                try
                {
                    SubRegKey.DeleteValue(this.NameValue.Text);
                }
                catch
                {
                }
            }
            SetIsEnabled(true);
            this.ConfirmBorder.Visibility = Visibility.Hidden;
            this.ConfirmBox.Visibility = Visibility.Hidden;
        }

        private void CancelDelete(object sender, RoutedEventArgs e)
        {
            SetIsEnabled(true);
            this.ConfirmBorder.Visibility = Visibility.Hidden;
            this.ConfirmBox.Visibility = Visibility.Hidden;
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            bool valid = true;
            SetAllLabelStyle(NormalColor, NormalBackGround, NormalOpacity, FontWeights.Normal, NormalSize);

            if (this.PathValue1.Text == "")
            {
                SetLabelStyle(this.Path1, ErrorColor, ErrorBackGround, ErrorOpacity, FontWeights.Normal, ErrorSize);
                valid = false;
            }
            if (this.PathValue2.Text == "")
            {
                SetLabelStyle(this.Path2, ErrorColor, ErrorBackGround, ErrorOpacity, FontWeights.Normal, ErrorSize);
                valid = false;
            }
            if (this.NameValue.Text == "")
            {
                SetLabelStyle(this.Name, ErrorColor, ErrorBackGround, ErrorOpacity, FontWeights.Normal, ErrorSize);
                valid = false;
            }
            if (this.TypeValue.Text == "")
            {
                SetLabelStyle(this.Type, ErrorColor, ErrorBackGround, ErrorOpacity, FontWeights.Normal, ErrorSize);
                valid = false;
            }

            if (valid)
            {
                RegistryKey RegKey, SubRegKey;

                switch (this.PathValue1.SelectedIndex)
                {
                    case 0:
                        RegKey = Registry.ClassesRoot;
                        break;
                    case 1:
                        RegKey = Registry.CurrentUser;
                        break;
                    case 2:
                        RegKey = Registry.LocalMachine;
                        break;
                    case 3:
                        RegKey = Registry.Users;
                        break;
                    case 4:
                        RegKey = Registry.CurrentConfig;
                        break;
                    default:
                        RegKey = null;
                        break;
                }
                SubRegKey = RegKey.OpenSubKey(this.PathValue2.Text, true);
                if (SubRegKey == null)
                {
                    RegKey.CreateSubKey(this.PathValue2.Text, true);
                    SubRegKey = RegKey.OpenSubKey(this.PathValue2.Text, true);
                }

                switch (this.TypeValue.SelectedIndex)
                {
                    case 0:
                    case 5:
                        {
                            string DataValue = this.DataValue1.Text;
                            SubRegKey.SetValue(this.NameValue.Text, DataValue);
                        }
                        break;
                    case 1:
                        {
                            List<byte> DataValue = new List<byte>();
                            for (int i = 0; i < this.DataValue2.LineCount; i++)
                            {
                                string[] parts = this.DataValue2.GetLineText(i).Replace("\r", "").Replace("\n", "").Split(' ');
                                foreach (string b in parts)
                                {
                                    DataValue.Add(Convert.ToByte(b));
                                }
                            }
                            SubRegKey.SetValue(this.NameValue.Text, DataValue.ToArray());
                        }
                        break;
                    case 2:
                        {
                            string DataValue = this.DataValue1.Text.Replace("\r", "").Replace("\n", "");
                            if (DigitCheck(DataValue))
                            {
                                try
                                {
                                    SubRegKey.SetValue(this.NameValue.Text, Convert.ToInt32(DataValue));
                                }
                                catch
                                {
                                }
                            }
                        }
                        break;
                    //case 3:
                    //    {
                    //        string DataValue = this.DataValue1.Text.Replace("\r", "").Replace("\n", "");
                    //        if (DigitCheck(DataValue))
                    //        {
                    //            try
                    //            {
                    //                SubRegKey.SetValue(this.NameValue.Text, Convert.ToInt32(DataValue));
                    //            }
                    //            catch
                    //            {
                    //            }
                    //        }
                    //    }
                    //    break;
                    case 4:
                        {
                            List<string> DataValue = new List<string>();
                            for (int i = 0; i < this.DataValue2.LineCount; i++)
                            {
                                DataValue.Add(this.DataValue2.GetLineText(i).Replace("\r", ""));
                            }
                            SubRegKey.SetValue(this.NameValue.Text, DataValue.ToArray());
                        }
                        break;
                }
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            bool valid = true;
            SetAllLabelStyle(NormalColor, NormalBackGround, NormalOpacity, FontWeights.Normal, NormalSize);

            if (this.PathValue1.Text == "")
            {
                SetLabelStyle(this.Path1, ErrorColor, ErrorBackGround, ErrorOpacity, FontWeights.Normal, ErrorSize);
                valid = false;
            }
            if (this.PathValue2.Text == "")
            {
                SetLabelStyle(this.Path2, ErrorColor, ErrorBackGround, ErrorOpacity, FontWeights.Normal, ErrorSize);
                valid = false;
            }
            if (this.NameValue.Text == "")
            {
                SetLabelStyle(this.Name, ErrorColor, ErrorBackGround, ErrorOpacity, FontWeights.Normal, ErrorSize);
                valid = false;
            }

            if (valid)
            {
                this.ConfirmBorder.Visibility = Visibility.Visible;
                this.ConfirmBox.Visibility = Visibility.Visible;
                string msg = "確定刪除以下登錄檔 ?\n" + this.PathValue1.Text + "\\" + this.PathValue2.Text + ":" + this.NameValue.Text;
                this.ConfirmMsg.Content = msg;
                SetIsEnabled(false);
            }
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            SetAllLabelStyle(NormalColor, NormalBackGround, NormalOpacity, FontWeights.Normal, NormalSize);
            this.PathValue1.SelectedIndex = -1;
            this.PathValue2.Text = "";
            this.NameValue.Text = "";
            this.TypeValue.SelectedIndex = -1;
            this.DataValue1.Text = "";
            this.DataValue2.Text = "";
        }
    }
}
