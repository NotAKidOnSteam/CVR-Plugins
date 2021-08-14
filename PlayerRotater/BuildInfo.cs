using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle(PlayerRotater.BuildInfo.GUID)]
[assembly: AssemblyProduct(PlayerRotater.BuildInfo.Name)]
[assembly: AssemblyVersion(PlayerRotater.BuildInfo.Version)]
[assembly: AssemblyCompany("cvr.ljoonal.xyz")]

namespace PlayerRotater
{
	public static class BuildInfo
	{
		public const string GUID = "xyz.ljoonal.cvr.playerrotater";

		public const string Name = "Player Rotater";

		public const string Version = "0.1.0";
	}
}
