using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace regedit_TOOL
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddRegKey(string Path, string RegKey, string KeyVal)
        {

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

        private void Add(object sender, RoutedEventArgs e)
        {
            bool valid = true;

            this.Path1Err.Visibility = Visibility.Hidden;
            this.Path2Err.Visibility = Visibility.Hidden;
            this.NameErr.Visibility = Visibility.Hidden;
            this.TypeErr.Visibility = Visibility.Hidden;
            if (this.PathValue1.Text == "")
            {
                this.Path1Err.Visibility = Visibility.Visible;
                valid = false;
            }
            if (this.PathValue2.Text == "")
            {
                this.Path2Err.Visibility = Visibility.Visible;
                valid = false;
            }
            if (this.NameValue.Text == "")
            {
                this.NameErr.Visibility = Visibility.Visible;
                valid = false;
            }
            if (this.TypeValue.Text == "")
            {
                this.TypeErr.Visibility = Visibility.Visible;
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

            this.Path1Err.Visibility = Visibility.Hidden;
            this.Path2Err.Visibility = Visibility.Hidden;
            this.NameErr.Visibility = Visibility.Hidden;
            this.TypeErr.Visibility = Visibility.Hidden;
            if (this.PathValue1.Text == "")
            {
                this.Path1Err.Visibility = Visibility.Visible;
                valid = false;
            }
            if (this.PathValue2.Text == "")
            {
                this.Path2Err.Visibility = Visibility.Visible;
                valid = false;
            }
            if (this.NameValue.Text == "")
            {
                this.NameErr.Visibility = Visibility.Visible;
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
            }
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            this.PathValue1.SelectedIndex = -1;
            this.PathValue2.Text = "";
            this.NameValue.Text = "";
            this.TypeValue.SelectedIndex = -1;
            this.DataValue1.Text = "";
            this.DataValue2.Text = "";
        }
    }
}
