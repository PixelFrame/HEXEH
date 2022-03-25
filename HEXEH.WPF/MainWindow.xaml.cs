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
        }

        private void DataConversion(byte[] blob)
        {
            tvDataTree.Items.Clear();
            var selectedGuid = (Guid)cbbDataType.SelectedValue;
            if (selectedGuid == new Guid("37d68d01-f2ab-4674-9f8f-11e942d49abb")) return;
            tvDataTree.Items.Add(_converter.DoConversion(selectedGuid, blob).Head);
        }
    }
}
