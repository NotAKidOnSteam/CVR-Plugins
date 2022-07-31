using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using HopLib;
using HopLib.Extras;
using UnityEngine;
using CVRInputManager = ABI_RC.Core.Savior.CVRInputManager;
using CVR_MovementSystem = ABI_RC.Core.Player.CVR_MovementSystem;
using InputModuleMouseKeyboard = ABI_RC.Core.Savior.InputModuleMouseKeyboard;
using PlayerSetup = ABI_RC.Core.Player.PlayerSetup;

namespace PlayerRotater
{
	internal enum EnabledState
	{
		Off,
		Toggled,
		Holding
	}

	[BepInDependency(HopLibInfo.Id, HopLibInfo.Version)]
	[BepInPlugin(BuildInfo.Id, BuildInfo.Name, BuildInfo.Version)]
	[BepInProcess("ChilloutVR.exe")]
	public class PlayerRotaterPlugin : BaseUnityPlugin
	{
		private static PlayerRotaterPlugin Instance;
		private ConfigEntry<KeyboardShortcut> mouseModeToggleKeybind, mouseModeHoldKeybind;
		private Vector3? originalRotation;
		private static Transform PlayerRotationTransform
		{
			get
			{
				return PlayerSetup.Instance._movementSystem.transform;
			}
		}

		private EnabledState _mouseLookEnabledField = EnabledState.Off;

		private EnabledState MouseLookEnabled
		{
			get
			{
				return _mouseLookEnabledField;
			}
			set
			{
				if (_mouseLookEnabledField != value)
					Logger.LogInfo($"Mouse look set to {value}");

				_mouseLookEnabledField = value;
			}
		}

		public void Awake()
		{
			const string inputPrefsCategory = "Inputs";

			mouseModeToggleKeybind = Config.Bind(
				inputPrefsCategory,
				"KeybindMouseMode",
				new KeyboardShortcut(KeyCode.None),
				"The keybind for toggling mouse mode");

			mouseModeHoldKeybind = Config.Bind(
				inputPrefsCategory,
				"KeycodeMouseModeHold",
				new KeyboardShortcut(KeyCode.Mouse0),
				"A key that enables mouse mode when holding it down.");

			Instance = this;

			try
			{
				Harmony.CreateAndPatchAll(typeof(PlayerRotaterPlugin));
				HopApi.InstanceJoined += delegate { ResetRotation(); };

#if DEBUG
				Logger.LogInfo($"{nameof(PlayerRotaterPlugin)} started successfully");
#endif
			}
			catch (System.Exception ex)
			{
				Logger.LogError($"Failed to apply patch: {ex}");
			}
		}

		private void RotatePlayer(float pitch = 0f, float roll = 0f, float yaw = 0f)
		{
			if (pitch == 0f && roll == 0f && yaw == 0f) return;
			float m = 5f;
			try
			{
				m = Traverse.Create(PlayerSetup.Instance._movementSystem).Field("rotationMultiplier").GetValue<float>();
			}
			catch (System.Exception ex)
			{
				Logger.LogWarning($"Failed to get rotationMultiplier: {ex}");
			}
			PlayerRotationTransform.Rotate(new Vector3(pitch * m, yaw * m, roll * m));
		}

		// A patch to handle mouse mode.
		[HarmonyPatch(typeof(InputModuleMouseKeyboard), "UpdateInput")]
		[HarmonyPostfix]
		private static void InputPatch()
		{
			Instance.OnUpdateInput();
		}

		private void ResetRotation()
		{
			MouseLookEnabled = EnabledState.Off;
			if (originalRotation is not null)
			{
#if DEBUG
				Logger.LogInfo($"Disabled rotation mode");
#endif
				PlayerRotationTransform.eulerAngles = new Vector3(
					originalRotation.Value.x,
					PlayerRotationTransform.eulerAngles.y,
					originalRotation.Value.z
				);
				originalRotation = null;
			}
		}

		private void OnUpdateInput()
		{
			// Don't touch rotations if CVR doesn't want us to currently.
			if (!PlayerSetup.Instance._movementSystem.canRot) return;

			// Only use rotation whilst flying.
			if (!PlayerSetup.Instance._movementSystem.flying)
			{
				ResetRotation();
				return;
			}

			ProcessMouseLookState();

			Vector2 lookVector = CVRInputManager.Instance.lookVector;

			if (originalRotation is not null &&
				!CVRInputManager.Instance.independentHeadTurn &&
				!CVRInputManager.Instance.independentHeadToggle)
			{
				CVRInputManager.Instance.lookVector = Vector2.zero;

				if (MouseLookEnabled == EnabledState.Off)
				{
					RotatePlayer(pitch: lookVector.y * -1, yaw: lookVector.x);
					/*Transform head = PlayerSetup.Instance.animatorManager.getHumanHeadTransform();
					PlayerRotationTransform.transform.RotateAround(
						transform.position,
						head.up,
						CVRInputManager.Instance.lookVector.x * this.RotationMultiplier
						);*/
				}
			}

			if (MouseLookEnabled == EnabledState.Off) return;

			if (originalRotation is null)
			{
#if DEBUG
				Logger.LogInfo($"Enabled rotation mode");
#endif
				originalRotation = PlayerRotationTransform.eulerAngles;
			}

			RotatePlayer(pitch: lookVector.y * -1, roll: lookVector.x * -1);
		}

		private void ProcessMouseLookState()
		{
			if (mouseModeToggleKeybind.Value.AllowingIsDown())
			{
				if (MouseLookEnabled == EnabledState.Toggled) MouseLookEnabled = EnabledState.Off;
				else MouseLookEnabled = EnabledState.Toggled;
			}
			else if (mouseModeHoldKeybind.Value.AllowingIsPressed()) MouseLookEnabled = EnabledState.Holding;
			else if (MouseLookEnabled == EnabledState.Holding) MouseLookEnabled = EnabledState.Off;
		}
	}
}
