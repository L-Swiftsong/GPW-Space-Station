using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Icons
{
    public static class InputIconManager
    {
        private static InputIconDataSO s_inputIconData;
        private const string INPUT_ICON_DATA_PATH = "UI/Icons/InputIconData";


        static InputIconManager()
        {
            try
            {
                s_inputIconData = Resources.Load(INPUT_ICON_DATA_PATH, typeof(InputIconDataSO)) as InputIconDataSO;
                if (s_inputIconData == null)
                    throw new System.Exception();
            }
            catch
            {
                Debug.LogError("Error when trying to load the InputIconDataSO from path: " + INPUT_ICON_DATA_PATH);
            }
        }


        public static Sprite GetIconForScheme(InputActionAsset inputActionAsset, string schemeName)
        {
            var bindings = inputActionAsset[schemeName].bindings;
            var bindingCount = bindings.Count;
            for (int i = 0; i < bindingCount; ++i)
            {
                if (bindings[i].groups.Contains(PlayerInput.LastUsedDevice.ToString()))
                {
                    // This binding is compatable with our active device. Use it to determine our Icon.
                    inputActionAsset[schemeName].GetBindingDisplayString(i, out string deviceLayoutName, out string controlPath);
                    Debug.Log(controlPath);
                    return InputIconManager.GetIconForPath(controlPath);
                }
            }

            Debug.LogError($"Error: No Binding for the action scheme '{schemeName}' for the last used device ({PlayerInput.LastUsedDevice.ToString()})");
            return null;
        }
        public static Sprite GetIconForPath(string controlPath)
        {
            PlayerInput.DeviceType activeDevice = PlayerInput.LastUsedDevice;
            return s_inputIconData.GetSprite(activeDevice, controlPath);
        }
    }
}
