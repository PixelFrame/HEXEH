﻿using HEXEH.Core;
using HEXEH.Core.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
                 return $"{name}: {description} ({datatype.GUID})";
            });
        }

        internal void DoConversion(Guid classId, byte[] blob)
        {
            var selectedType = DataTypes.FirstOrDefault(x => x.GUID == classId);
            var convertedObj = selectedType.GetMethod("ConvertFromBytes").Invoke(null, new object[] { blob });
        }
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}