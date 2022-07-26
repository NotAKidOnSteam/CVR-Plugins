using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;
using HopLib.Extras;
using UnityEngine;

namespace KeyRebinder
{
	class MiscPatches
	{
		private static ConfigEntry<KeyboardShortcut> ConfigKeybindReload,
			ConfigKeybindSwitchMode,
			ConfigKeybindNameplatesToggle,
			ConfigKeybindHudToggle,
			ConfigKeybindDisconnect,
			ConfigKeybindDisconnectAndGoHome,
			ConfigKeybindRejoinCurrentInstance,
			ConfigKeyZoom;

		public static void RegisterConfigs(ConfigFile Config)
		{
			ConfigKeybindReload = Config.Bind(
				nameof(MiscPatches),
				"BindReload",
				new KeyboardShortcut(KeyCode.F9, new KeyCode[] { KeyCode.LeftControl }),
				"The shortcut for reloading (Set to None to let the game manage it)");
			ConfigKeybindSwitchMode = Config.Bind(
				nameof(MiscPatches),
				"BindSwitchMode",
				new KeyboardShortcut(KeyCode.F9, new KeyCode[] { KeyCode.LeftShift }),
				"The shortcut for switching mode (Set to None to let the game manage it)");
			ConfigKeybindNameplatesToggle = Config.Bind(
				nameof(MiscPatches),
				"BindNameplateToggle",
				new KeyboardShortcut(KeyCode.None),
				"The shortcut for toggling nameplate visibility. (Set to None to let the game manage it)");
			ConfigKeybindHudToggle = Config.Bind(
				nameof(MiscPatches),
				"BindHudToggle",
				new KeyboardShortcut(KeyCode.None),
				"The shortcut for toggling hud visibility. (Set to None to let the game manage it)");

			ConfigKeyZoom = Config.Bind(
				nameof(MiscPatches),
				"BindZoom",
				new KeyboardShortcut(KeyCode.Mouse2),
				"The key for zooming.");

			// Connection related
			ConfigKeybindDisconnect = Config.Bind(
				nameof(MiscPatches),
				"BindDisconnect",
				new KeyboardShortcut(KeyCode.Y, new KeyCode[] { KeyCode.LeftControl }),
				"The shortcut for disconnecting from current instance.");
			ConfigKeybindDisconnectAndGoHome = Config.Bind(
				nameof(MiscPatches),
				"BindDisconnectAndGoHome",
				new KeyboardShortcut(KeyCode.U, new KeyCode[] { KeyCode.LeftControl }),
				"The shortcut for disconnecting from current instance and going home.");
			ConfigKeybindRejoinCurrentInstance = Config.Bind(
				nameof(MiscPatches),
				"BindRejoinCurrentInstance",
				new KeyboardShortcut(KeyCode.R, new KeyCode[] { KeyCode.LeftControl }),
				"The shortcut for rejoining the current instance.");
		}

		public static void Patch()
		{
			Harmony.CreateAndPatchAll(typeof(MiscPatches));
		}

		[HarmonyPatch(typeof(ABI_RC.Core.Savior.InputModuleMouseKeyboard), "UpdateInput")]
		[HarmonyPostfix]
		static void OverwriteInputs()
		{
			// Setting these only in the Harmony patch prefix,
			// because otherwise the game input handling may overwrite our changes on Update
			if (ConfigKeybindReload.Value.MainKey != KeyCode.None)
				ABI_RC.Core.Savior.CVRInputManager.Instance.reload = ConfigKeybindReload.Value.AllowingIsDown();
			if (ConfigKeybindSwitchMode.Value.MainKey != KeyCode.None)
				ABI_RC.Core.Savior.CVRInputManager.Instance.switchMode = ConfigKeybindSwitchMode.Value.AllowingIsDown();
			if (ConfigKeybindNameplatesToggle.Value.MainKey != KeyCode.None)
				ABI_RC.Core.Savior.CVRInputManager.Instance.toggleNameplates = ConfigKeybindNameplatesToggle.Value.AllowingIsDown();
			if (ConfigKeybindHudToggle.Value.MainKey != KeyCode.None)
				ABI_RC.Core.Savior.CVRInputManager.Instance.toggleHud = ConfigKeybindHudToggle.Value.AllowingIsDown();
			if (ConfigKeyZoom.Value.MainKey != KeyCode.None)
				ABI_RC.Core.Savior.CVRInputManager.Instance.zoom = ConfigKeyZoom.Value.AllowingIsPressed();

			// Connection related
			if (ConfigKeybindDisconnect.Value.AllowingIsDown())
			{
				ABI_RC.Core.Networking.NetworkManager.Instance.OnDisconnectionRequested(0, false);
			}
			else if (ConfigKeybindDisconnectAndGoHome.Value.AllowingIsDown())
			{
				ABI_RC.Core.Networking.NetworkManager.Instance.OnDisconnectionRequested(0, true);
			}
			else if (ConfigKeybindRejoinCurrentInstance.Value.AllowingIsDown())
			{
				var instanceID = ABI_RC.Core.Savior.MetaPort.Instance.CurrentInstanceId;
				var worldID = GameObject.FindObjectOfType<ABI.CCK.Components.CVRWorld>().GetComponent<ABI.CCK.Components.CVRAssetInfo>().guid;
				if (instanceID is string && worldID is string)
				{
					ABI_RC.Core.Networking.IO.Instancing.Instances.SetJoinTarget(instanceID, worldID);
				}
			}

		}
	}
}
