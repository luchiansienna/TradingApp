using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;

namespace Ferovinum.DatabaseInitializer
{
    public class FeeTypeConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return float.Parse(text.Replace("%", "")) / 100F;
        }
    }
}
