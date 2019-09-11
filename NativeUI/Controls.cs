using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System.Linq;

namespace NativeUI
{
	/// <summary>
	/// Class that provides tools to handle the game controls.
	/// </summary>
	public static class Controls
	{
		/// <summary>
		/// All of the controls required for using a keyboard.
		/// </summary>
		private static readonly Control[] NecessaryControlsKeyboard = new Control[]
		{
			Control.FrontendAccept,
			Control.FrontendAxisX,
			Control.FrontendAxisY,
			Control.FrontendDown,
			Control.FrontendUp,
			Control.FrontendLeft,
			Control.FrontendRight,
			Control.FrontendCancel,
			Control.FrontendSelect,
			Control.CursorScrollDown,
			Control.CursorScrollUp,
			Control.CursorX,
			Control.CursorY,
			Control.MoveUpDown,
			Control.MoveLeftRight,
			Control.Sprint,
			Control.Jump,
			Control.Enter,
			Control.VehicleExit,
			Control.VehicleAccelerate,
			Control.VehicleBrake,
			Control.VehicleMoveLeftRight,
			Control.VehicleFlyYawLeft,
			Control.FlyLeftRight,
			Control.FlyUpDown,
			Control.VehicleFlyYawRight,
			Control.VehicleHandbrake,
		};
		/// <summary>
		/// All of the controls required for using a keyboard.
		/// </summary>
		private static readonly Control[] NecessaryControlsGamePad = NecessaryControlsKeyboard.Concat(new Control[]
		{
			Control.LookUpDown,
			Control.LookLeftRight,
			Control.Aim,
			Control.Attack,
		})
		.ToArray();

		/// <summary>
		/// Toggles the availability of the controls.
		/// It does not disable the basic movement and frontend controls.
		/// </summary>
		/// <param name="toggle">If we want to enable or disable the controls.</param>
		public static void Toggle(bool toggle)
		{
			// If we want to enable the controls
			if (toggle)
			{
				// Enable all of them
				Game.EnableAllControlsThisFrame(2);
			}
			// If we don't need them
			else
			{
				// Disable all of the controls
				Game.DisableAllControlsThisFrame(2);

				// Now, re-enable the controls that are required for the game
				// First, pick the right controls for gamepad or keyboard and mouse
				Control[] list = Game.CurrentInputMode == InputMode.GamePad ? NecessaryControlsGamePad : NecessaryControlsKeyboard;
				// Then, enable all of the controls for that input mode
				foreach (Control control in list)
					EnableControlAction(0, (int)control, true);
			}
		}
	}
}