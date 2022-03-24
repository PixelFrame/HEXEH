using HEXEH.Core;
using HEXEH.Core.DataType;
using HEXEH.Core.Utility;
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

namespace HEXEH.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Converter _converter = new();
        public MainWindow()
        {
            InitializeComponent();
            cbbDataType.ItemsSource = _converter.DataTypeNames;
            cbbDataType.DisplayMemberPath = "Value";
            cbbDataType.SelectedValuePath = "Key";
            cbbDataType.SelectedIndex = 0;
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            var rawInput = tbInput.Text;
            var bytesInput = Common.StringToBytes(rawInput);
            if(bytesInput.Length == 0)
            {
                MessageBox.Show("Invalid HEX input!", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            var hdInput = HexDump.ConvertFromBytes(bytesInput);
            tbkHexDumpLeft.Text = hdInput.LineNum;
            tbHexDumpMid.Text = hdInput.FormattedHex;
            tbHexDumpRight.Text = hdInput.FormattedChars;
            DataConversion(bytesInput);
        }

        private void cbbDataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var last = stkPanelSettings.Children.Count - 1;
            if(last > 4)
            {
                stkPanelSettings.Children.RemoveRange(4, last - 4);
            }
            
            /*switch(selected)
            {
                case "String":
                    RadioButton rbStrAscii = new();
                    RadioButton rbStrUtf8 = new();
                    RadioButton rbStrUtf16 = new();
                    rbStrAscii.Content = "ASCII";
                    rbStrUtf8.Content = "UTF-8";
                    rbStrUtf16.Content = "Unicode";
                    rbStrAscii.GroupName = "StringEncoding";
                    rbStrUtf8.GroupName = "StringEncoding";
                    rbStrUtf16.GroupName = "StringEncoding";
                    rbStrAscii.IsChecked = true;

                    stkPanelSettings.Children.Insert(4, rbStrUtf16);
                    stkPanelSettings.Children.Insert(4, rbStrUtf8);
                    stkPanelSettings.Children.Insert(4, rbStrAscii);
                    break;
                default:break;
            }*/
        }

        private void DataConversion(byte[] blob)
        {
            var selectedGuid = (Guid)cbbDataType.SelectedValue;
            if (selectedGuid == new Guid("37d68d01-f2ab-4674-9f8f-11e942d49abb")) return;
            _converter.DoConversion(selectedGuid, blob);
        }
    }
}
