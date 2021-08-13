using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle(WhyFiltered.BuildInfo.GUID)]
[assembly: AssemblyProduct(WhyFiltered.BuildInfo.Name)]
[assembly: AssemblyVersion(WhyFiltered.BuildInfo.Version)]
[assembly: AssemblyCompany("cvr.ljoonal.xyz")]

namespace WhyFiltered
{
	public static class BuildInfo
	{
		public const string GUID = "xyz.ljoonal.cvr.skipintro";

		public const string Name = "Skip intro";

		public const string Version = "0.0.1";
	}
}
