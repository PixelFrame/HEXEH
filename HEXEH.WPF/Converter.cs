using HEXEH.Core;
using HEXEH.Core.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HEXEH.WPF
{
    internal class Converter
    {
        internal List<Type> DataTypes;
        internal Dictionary<Guid,string> DataTypeNames;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        internal Converter()
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

        internal DataTree DoConversion(Guid classId, byte[] blob, Dictionary<string, object>? settingMap)
        {
            var selectedType = DataTypes.FirstOrDefault(x => x.GUID == classId);
            if (selectedType == null) return new DataTree("Null", "Null");
            try
            {
                var convertedObj = (IDataType?)selectedType.GetMethod("ConvertFromBytes").Invoke(null, new object[] { blob, settingMap });
                if (convertedObj == null) return new DataTree("Null", "Null");
                return convertedObj.ToDataTree();
            } 
            catch (Exception ex)
            {
                var dtEx = new DataTree("Conversion Failed", "");
                dtEx.Head.Childs.Add(new DataTreeNode("Exception", ex.Message));
                while(ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    dtEx.Head.Childs.Add(new DataTreeNode("Exception", ex.Message));
                }

                return dtEx;
            }
        }

        internal Dictionary<string, List<string>?>? GetSettingMap(Guid classId)
        {
            var selectedType = DataTypes.FirstOrDefault(x => x.GUID == classId);
            if (selectedType == null) return null;
            return (Dictionary<string, List<string>?>?)selectedType.GetProperty("SettingMap").GetValue(null);
        }
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}
