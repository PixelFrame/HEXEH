using HEXEH.Core;
using HEXEH.Core.DataType;
using HEXEH.Core.Utility;
using HEXEH.WPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private static readonly Converter _converter = new();
        private static Guid _selectedType = Guid.Empty;
        private static Dictionary<string, object>? _settingMap = null;
        private DataTreeViewModel? _vmDataTree = null;
        private uint _dwHexDumpLineLength = 16;
        private byte[] _bytesInput = Array.Empty<byte>();

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
            _bytesInput = Common.StringToBytes(rawInput);
            if(_bytesInput.Length == 0)
            {
                MessageBox.Show("Invalid HEX input!", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            UpdateHexDump();
        }

        private void UpdateHexDump()
        {
            var hdInput = HexDump.ConvertFromBytes(_bytesInput, new Dictionary<string, object>() { { "LineLength", _dwHexDumpLineLength } });
            tbkHexDumpLeft.Text = hdInput.LineNum;
            tbHexDumpMid.Text = hdInput.FormattedHex;
            tbHexDumpRight.Text = hdInput.FormattedChars;
            DataConversion(_bytesInput);
        }

        private void cbbDataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stkPanelSubSettings.Children.Clear();
            if (cbbDataType.SelectedValue == null || cbbDataType.SelectedValuePath != "Key") return;
            _selectedType = (Guid)cbbDataType.SelectedValue;
            if (_selectedType == new Guid("37d68d01-f2ab-4674-9f8f-11e942d49abb")) return;

            var settingMap = _converter.GetSettingMap(_selectedType);
            if (settingMap == null) return;
            if (_settingMap == null) _settingMap = new Dictionary<string, object>();
            else _settingMap.Clear();

            foreach (var setting in settingMap)
            {
                var settingName = setting.Key.Split('#');
                var tbkSubSettingName = new Label();
                tbkSubSettingName.Content = settingName[0];
                stkPanelSubSettings.Children.Add(tbkSubSettingName);
                switch (settingName[1])
                {
                    case ("single"):
                        {
                            _settingMap.Add(settingName[0], setting.Value[0]);
                            foreach (var option in setting.Value)
                            {
                                var rbSubsettingOption = new RadioButton();
                                rbSubsettingOption.Content = option;
                                rbSubsettingOption.GroupName = settingName[0];
                                rbSubsettingOption.Checked += rbSubsettingOption_Checked;
                                stkPanelSubSettings.Children.Add(rbSubsettingOption);
                            }
                            ((RadioButton)stkPanelSubSettings.Children[1]).IsChecked = true;
                            break;
                        }
                    case ("multi"):
                        {
                            _settingMap.Add(settingName[0], setting.Value);
                            foreach (var option in setting.Value)
                            {
                                var cbxSubsettingOption = new CheckBox();
                                cbxSubsettingOption.Resources = new ResourceDictionary { { "SettingName", settingName[0] }, { "Option", option } };
                                var tbSubSettingOption = new TextBlock();
                                tbSubSettingOption.Text = option;
                                cbxSubsettingOption.Content = tbSubSettingOption;
                                cbxSubsettingOption.IsChecked = true;
                                cbxSubsettingOption.Checked += cbxSubsettingOption_Checked;
                                cbxSubsettingOption.Unchecked += cbxSubsettingOption_Unchecked;
                                stkPanelSubSettings.Children.Add(cbxSubsettingOption);
                            }
                            break;
                        }
                    case ("num"): 
                        {
                            _settingMap.Add(settingName[0], 0);
                            var tbSubSettingNumInput = new TextBox();
                            tbSubSettingNumInput.Resources = new ResourceDictionary { { "SettingName", settingName[0] } };
                            tbSubSettingNumInput.AcceptsReturn = false;
                            tbSubSettingNumInput.PreviewTextInput += NumberValidationTextBox;
                            tbSubSettingNumInput.TextChanged += tbSubSettingNumInput_TextChanged;
                            stkPanelSubSettings.Children.Add(tbSubSettingNumInput);
                            break;
                        }
                    case ("string"):
                        {
                            _settingMap.Add(settingName[0], "");
                            var tbSubSettingStrInput = new TextBox();
                            tbSubSettingStrInput.Resources = new ResourceDictionary { { "SettingName", settingName[0] } };
                            tbSubSettingStrInput.AcceptsReturn = false;
                            tbSubSettingStrInput.TextChanged += tbSubSettingStrInput_TextChanged;
                            stkPanelSubSettings.Children.Add(tbSubSettingStrInput);
                            break;
                        }
                }
            }
        }

        private void tbSubSettingStrInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            var srcTb = (TextBox)sender;
            var settingName = (string)srcTb.Resources["SettingName"];
            _settingMap[settingName] = srcTb.Text;
        }

        private void tbSubSettingNumInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            var srcTb = (TextBox)sender;
            var settingName = (string)srcTb.Resources["SettingName"];
            _settingMap[settingName] = uint.Parse(srcTb.Text);
        }

        private void cbxSubsettingOption_Checked(object sender, RoutedEventArgs e)
        {
            var srcCbx = (CheckBox)sender;
            var settingName = (string)srcCbx.Resources["SettingName"];
            var option = (string)srcCbx.Resources["Option"];
            var settingList = (List<string>)_settingMap[settingName];
            settingList.Add(option);
        }

        private void cbxSubsettingOption_Unchecked(object sender, RoutedEventArgs e)
        {
            var srcCbx = (CheckBox)sender;
            var settingName = (string)srcCbx.Resources["SettingName"];
            var option = (string)srcCbx.Resources["Option"];
            var settingList = (List<string>)_settingMap[settingName]; 
            settingList.Remove(option);
        }

        private void rbSubsettingOption_Checked(object sender, RoutedEventArgs e)
        {
            var srcRb = (RadioButton)e.Source;
            _settingMap[srcRb.GroupName] = srcRb.Content;
        }

        private void DataConversion(byte[] blob)
        {
            if (_selectedType == new Guid("37d68d01-f2ab-4674-9f8f-11e942d49abb")) return;
            _vmDataTree = new DataTreeViewModel(_converter.DoConversion(_selectedType, blob, _settingMap));
            _vmDataTree.Head[0].IsExpanded = true;
            tvDataTree.DataContext = _vmDataTree;
        }

        private void tvDataItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)sender;
            tvi.IsSelected = true;
        }

        private void tvDataTreeMenu_CopyValue_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var vmDataTreeNode = (DataTreeNodeViewModel)menuItem.DataContext;
            Clipboard.SetText(vmDataTreeNode.Value);
        }

        private void tvDataTreeMenu_CopyBoth_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var vmDataTreeNode = (DataTreeNodeViewModel)menuItem.DataContext;
            Clipboard.SetText(vmDataTreeNode.Label + ": " + vmDataTreeNode.Value);
        }

        private void tvDataTreeMenu_CopySub_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var vmDataTreeNode = (DataTreeNodeViewModel)menuItem.DataContext;
            Clipboard.SetText(vmDataTreeNode.ToTreeText());
        }

        private void sldHexDumpLineNum_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _dwHexDumpLineLength = (uint)e.NewValue;
            if (_bytesInput.Length > 0) UpdateHexDump();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
