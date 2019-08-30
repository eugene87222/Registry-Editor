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

        private double NormalSize = 15;
        private double ErrorSize = 15;

        private int LanguageIdx = 1;
        private string[,] Msg = new string[2, 4]
        {
            {
                "以下名稱不存在\n", "以下名稱已刪除\n", "以下機碼不存在\n", "確定刪除以下名稱 ?\n"
            },
            {
                "The name below doesn't exist\n", "The name below is deleted successfully\n", "The key below doesn't exist\n", "Are you sure you want to delete the name below ?\n"
            }
        };
        private string[,] BtnContent = new string[2, 6]
        {
            {
                "確定", "取消", "確定", "新增/修改登錄檔", "刪除登錄檔", "重設"
            },
            {
                "Confirm", "Cancel", "OK", "Add/Modify", "Delete", "Reset"
            }
        };
        private string[,] TitleContent = new string[2, 5]
        {
            {
                "登錄檔路徑 1", "登錄檔路徑 2", "名稱", "類型", "資料"
            },
            {
                "Key Path 1", "Key Path 2", "Name", "Type", "Data"
            }
        };

        public MainWindow()
        {
            InitializeComponent();
            SetAllLabelStyle(NormalColor, NormalBackGround, NormalOpacity, FontWeights.Normal, NormalSize);
            SetLanguage();
        }

        private void SetLanguage()
        {
            this.Path1.Content = TitleContent[LanguageIdx, 0];
            this.Path2.Content = TitleContent[LanguageIdx, 1];
            this.Name.Content = TitleContent[LanguageIdx, 2];
            this.Type.Content = TitleContent[LanguageIdx, 3];
            this.Data.Content = TitleContent[LanguageIdx, 4];

            this.ModifyBtn.Content = BtnContent[LanguageIdx, 3];
            this.DeleteBtn.Content = BtnContent[LanguageIdx, 4];
            this.ResetBtn.Content = BtnContent[LanguageIdx, 5];
            this.ConfirmDeleteBtn.Content = BtnContent[LanguageIdx, 0];
            this.CancelDeleteBtn.Content = BtnContent[LanguageIdx, 1];
            this.OKBtn.Content = BtnContent[LanguageIdx, 2];
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
            this.ModifyBtn.IsEnabled = val;
            this.DeleteBtn.IsEnabled = val;
            this.ResetBtn.IsEnabled = val;
            this.DataValue2.IsEnabled = val;
            this.ChineseBtn.IsEnabled = val;
            this.EnglishBtn.IsEnabled = val;
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

        private void Switch2Cht(object sender, RoutedEventArgs e)
        {
            LanguageIdx = 0;
            SetLanguage();
        }

        private void Switch2Eng(object sender, RoutedEventArgs e)
        {
            LanguageIdx = 1;
            SetLanguage();
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
                    if (SubRegKey.GetValue(this.NameValue.Text) == null)
                    {
                        this.ConfirmMsg.Content = Msg[LanguageIdx, 0] + this.PathValue1.Text + "\\" + this.PathValue2.Text + ":" + this.NameValue.Text; ;
                    }
                    else
                    {
                        SubRegKey.DeleteValue(this.NameValue.Text);
                        this.ConfirmMsg.Content = Msg[LanguageIdx, 1] + this.PathValue1.Text + "\\" + this.PathValue2.Text + ":" + this.NameValue.Text; ;
                    }
                }
                catch
                {
                }
            }
            else
            {
                this.ConfirmMsg.Content = Msg[LanguageIdx, 2] + this.PathValue1.Text + "\\" + this.PathValue2.Text;
            }
            this.ConfirmDeleteBtn.Visibility = Visibility.Hidden;
            this.CancelDeleteBtn.Visibility = Visibility.Hidden;
            this.OKBtn.Visibility = Visibility.Visible;
        }

        private void CancelDelete(object sender, RoutedEventArgs e)
        {
            SetIsEnabled(true);
            this.ConfirmBorder.Visibility = Visibility.Hidden;
            this.ConfirmBox.Visibility = Visibility.Hidden;
        }

        private void OK(object sender, RoutedEventArgs e)
        {
            SetIsEnabled(true);
            this.ConfirmBorder.Visibility = Visibility.Hidden;
            this.ConfirmBox.Visibility = Visibility.Hidden;
            this.ConfirmDeleteBtn.Visibility = Visibility.Visible;
            this.CancelDeleteBtn.Visibility = Visibility.Visible;
            this.OKBtn.Visibility = Visibility.Hidden;
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
                string msg = Msg[LanguageIdx, 3] + this.PathValue1.Text + "\\" + this.PathValue2.Text + ":" + this.NameValue.Text;
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