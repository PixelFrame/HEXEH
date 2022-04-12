using HEXEH.CON;
using HEXEH.Core;
using HEXEH.Core.DataType;
using System.Reflection;

Conversion.LoadTypes();

Console.WriteLine("Available Types: ");

Conversion.PrintTypes();

Console.WriteLine("Input target type index");

var select = Console.ReadLine();
var selectNum = int.Parse(select) - 1;

Conversion.PrintAndReadSettings(selectNum);

Console.WriteLine("Input HEX string");

var strHEX = Console.ReadLine();

Conversion.DoConversion(selectNum, strHEX);

return 0;