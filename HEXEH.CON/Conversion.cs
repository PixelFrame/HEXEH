using HEXEH.Core.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HEXEH.CON
{
    internal static class Conversion
    {
        static List<Type> DataTypes = new List<Type>();
        static Dictionary<Guid, string> DataTypeNames = new Dictionary<Guid, string>();
        static Dictionary<string,object> SettingMap = new Dictionary<string,object>();

        internal static void LoadTypes()
        {
            Assembly.Load("HEXEH.Core");
            DataTypes = AppDomain.CurrentDomain.GetAssemblies()
                                               .SelectMany(x => x.GetTypes())
                                               .Where(x => typeof(IDataType).IsAssignableFrom(x)
                                                    && !x.IsInterface
                                                    && !x.IsAbstract
                                                    && x.Namespace == @"HEXEH.Core.DataType")
                                               .ToList();
            DataTypeNames = DataTypes.ToDictionary(datatype => datatype.GUID, datatype =>
            {
                var name = datatype.GetProperty("Name").GetValue(null) as string;
                var description = datatype.GetProperty("Description").GetValue(null) as string;
                return $"{name}: {description}";
            });
        }

        internal static void PrintTypes()
        {
            var cnt = 1;
            foreach (var type in DataTypeNames)
            {
                Console.WriteLine($"{cnt++}. {type.Value}");
            }
        }

        internal static void PrintAndReadSettings(int idx)
        {
            Type targetType = DataTypes[idx];
            var settings = (Dictionary<string, List<string>?>?)targetType.GetProperty("SettingMap").GetValue(null);
            if (settings == null)
            {
                Console.WriteLine("No option for this type");
                return;
            }
            foreach (var setting in settings)
            {
                Console.WriteLine(setting.Key);
                if (setting.Value != null)
                {
                    var cnt = 1;
                    foreach (var value in setting.Value)
                    {
                        Console.WriteLine($"    {cnt++}. {value}");
                    }
                }
                var settingSplit = setting.Key.Split("#");
                switch (settingSplit[1])
                {
                    case "single":
                        Console.WriteLine("Input selection index");
                        var singleSelection = Console.ReadLine();
                        var numSingleSelection = int.Parse(singleSelection) - 1;
                        SettingMap.Add(settingSplit[0], setting.Value[numSingleSelection]);
                        break;
                    case "multi":
                        Console.WriteLine("Input selection index, split multiple selections with comma");
                        var multiSelections = Console.ReadLine().Split(',');
                        var multiSelectionValues = new List<string>();
                        foreach (var multiSelection in multiSelections)
                        {
                            var numMultiSelection = int.Parse(multiSelection) - 1;
                            multiSelectionValues.Add(setting.Value[numMultiSelection]);
                        }
                        SettingMap.Add(settingSplit[0], multiSelectionValues);
                        break;
                    case "num":
                        Console.WriteLine("Input number");
                        var numInput = uint.Parse(Console.ReadLine());
                        SettingMap.Add(settingSplit[0], numInput);
                        break;
                    case "string":
                        Console.WriteLine("Input string");
                        var strInput = Console.ReadLine();
                        SettingMap.Add(settingSplit[0], strInput);
                        break;
                }
            }
        }

        internal static void DoConversion(int idx, string data)
        {
            byte[] blob = Core.Utility.Common.StringToBytes(data);
            Type targetType = DataTypes[idx];
            try
            {
                var convertedObj = (IDataType?)targetType.GetMethod("ConvertFromBytes").Invoke(null, new object[] { blob, SettingMap });
                Console.WriteLine(convertedObj.ToDataTree().Head.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Conversion Failed!");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
