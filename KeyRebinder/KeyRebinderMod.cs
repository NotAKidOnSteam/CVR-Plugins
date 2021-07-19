using BepInEx;
using BepInEx.Configuration;

namespace KeyRebinder
{

	[BepInPlugin(BuildInfo.GUID, BuildInfo.Name, BuildInfo.Version)]
	[BepInProcess("ChilloutVR.exe")]
	public class KeyRebinderMod : BaseUnityPlugin
	{
		private const string EnabledModules = "EnabledModules";
		private static ConfigEntry<bool> EnableModuleMic;
		private static ConfigEntry<bool> EnableModuleMisc;
		private static ConfigEntry<bool> EnableModuleMovement;
		private static ConfigEntry<bool> EnableModuleGestures;
		private static ConfigEntry<bool> EnableModuleStates;
		private static ConfigEntry<bool> EnableModuleEmotes;

		KeyRebinderMod()
		{
			RegisterEnabledModuleConfigs();
			MiscPatches.RegisterConfigs(Config);
			MicPatches.RegisterConfigs(Config);
			MovementPatches.RegisterConfigs(Config);
			GesturePatches.RegisterConfigs(Config);
			StatePatches.RegisterConfigs(Config);
			EmotePatches.RegisterConfigs(Config);
		}

		void Awake()
		{
			if (EnableModuleMisc.Value)
			{
				MiscPatches.Patch();
				Logger.LogInfo($"{nameof(MiscPatches)} applied");
			}
			if (EnableModuleMic.Value)
			{
				MicPatches.Patch();
				Logger.LogInfo($"{nameof(MicPatches)} applied");
			}
			if (EnableModuleMovement.Value)
			{
				MovementPatches.Patch();
				Logger.LogInfo($"{nameof(MovementPatches)} applied");
			}
			if (EnableModuleGestures.Value)
			{
				GesturePatches.Patch();
				Logger.LogInfo($"{nameof(GesturePatches)} applied");
			}
			if (EnableModuleStates.Value)
			{
				StatePatches.Patch();
				Logger.LogInfo($"{nameof(StatePatches)} applied");
			}
			if (EnableModuleEmotes.Value)
			{
				EmotePatches.Patch();
				Logger.LogInfo($"{nameof(EmotePatches)} applied");
			}
		}

		private void RegisterEnabledModuleConfigs()
		{
			EnableModuleMic = Config.Bind(
				EnabledModules,
				"EnableModuleMic",
				true,
				"If to apply mic patches. Requires a restart to take effect.");
			EnableModuleMisc = Config.Bind(
				EnabledModules,
				"EnableModuleMisc",
				true,
				"If to apply misc patches. Requires a restart to take effect.");
			EnableModuleMovement = Config.Bind(
				EnabledModules,
				"EnableModuleMovement",
				true,
				"If to apply movement patches. Requires a restart to take effect.");
			EnableModuleGestures = Config.Bind(
				EnabledModules,
				"EnableModuleGestures",
				true,
				"If to apply gesture patches. Requires a restart to take effect.");
			EnableModuleStates = Config.Bind(
				EnabledModules,
				"EnableModuleStates",
				true,
				"If to apply state patches. Requires a restart to take effect.");
			EnableModuleEmotes = Config.Bind(
				EnabledModules,
				"EnableModuleEmotes",
				true,
				"If to apply emote patches. Requires a restart to take effect.");
		}
	}
}
