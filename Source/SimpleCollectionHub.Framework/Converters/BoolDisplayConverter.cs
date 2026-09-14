using RW.Common.Helpers;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SimpleCollectionHub.Framework.Converters;

public class BoolDisplayConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (NumberHelper.ConvertBool(value, out bool b))
		{
			return b ? "是" : "否";
		}
		return string.Empty;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
