using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Game.Interaction
{
    public class Vox3DKeyboard : MonoBehaviour
    {

        public static string Text;
        public static void Clear()
        {
            Text = "";
        }

        [SerializeField] private GameObject[] _Buttons;

        void Start()
        {
            foreach(var button in _Buttons)
            {
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => AppendButtonText(button));
            }
        }

        void Update()
        {

        }

        private void AppendButtonText(GameObject button)
        {
            if (button.name == "Button_Del" && Text.Length > 0)
            {
                Debug.Log($"Keyboard Text changed from {Text} to {Text.Substring(0, Text.Length - 1)}");
                Text = Text.Substring(0, Text.Length - 1);
            }
            else
            {
                Debug.Log($"Keyboard Text changed from {Text} to {Text + button.GetComponentInChildren<TextMeshProUGUI>().text}");
                Text += button.GetComponentInChildren<TextMeshProUGUI>().text;
            }
        }
    }
}