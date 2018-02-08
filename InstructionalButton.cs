using CitizenFX.Core.Native;


namespace NativeUI
{
    public class InstructionalButton
    {
        public string Text { get; set; }

        public UIMenuItem ItemBind { get; private set; }

        private readonly string _buttonString;
        private readonly CitizenFX.Core.Control _buttonControl;
        private readonly bool _usingControls;

        /// <summary>
        /// Add a dynamic button to the instructional buttons array.
        /// Changes whether the controller is being used and changes depending on keybinds.
        /// </summary>
        /// <param name="control">CitizenFX.Core.Control that gets converted into a button.</param>
        /// <param name="text">Help text that goes with the button.</param>
        public InstructionalButton(CitizenFX.Core.Control control, string text)
        {
            Text = text;
            _buttonControl = control;
            _usingControls = true;
        }


        /// <summary>
        /// Adds a keyboard button to the instructional buttons array.
        /// </summary>
        /// <param name="keystring">Custom keyboard button, like "I", or "O", or "F5".</param>
        /// <param name="text">Help text that goes with the button.</param>
        public InstructionalButton(string keystring, string text)
        {
            Text = text;
            _buttonString = keystring;
            _usingControls = false;
        }


        /// <summary>
        /// Bind this button to an item, so it's only shown when that item is selected.
        /// </summary>
        /// <param name="item">Item to bind to.</param>
        public void BindToItem(UIMenuItem item)
        {
            ItemBind = item;
        }

        public string GetButtonId()
        {
            return _usingControls ? Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, (int) _buttonControl, 0) : "t_" + _buttonString;
        }
    }
}