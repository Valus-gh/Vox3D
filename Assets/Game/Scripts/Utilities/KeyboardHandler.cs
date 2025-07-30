using UnityEngine;
using UnityEngine.UI;

namespace Game.Utilities
{
    public class KeyboardHandler : MonoBehaviour
    {
        public static string Input = "";

        [SerializeField]
        private InputField          _HostAddressInput; 
        private TouchScreenKeyboard _Keyboard;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(_HostAddressInput.isFocused && _Keyboard is null)
            {
                _Keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumbersAndPunctuation);
            }

            if(_Keyboard is not null && _Keyboard.status == TouchScreenKeyboard.Status.Done)
            {
                Input = _Keyboard.text;
                _Keyboard.active = false;
            }
        }
    }
}