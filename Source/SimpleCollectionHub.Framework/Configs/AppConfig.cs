namespace SimpleCollectionHub.Framework.Configs;

public static class AppConfig
{
	public const string CompanyName = "RainbowWolfer";
	public const string Copyright = "RainbowWolfer";

	public const string AppName = "SimpleCollectionHub";

	/// <summary> 是否是发布版本 </summary>
	public static bool IsRelease
	{
		get
		{
#if RELEASE
			return true;
#else
			return false;
#endif
		}
	}

	public static string BuildMode
	{
		get
		{
			return IsRelease ? "Release" : "Debug";
		}
	}

}
