using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Environment.Buttons;

namespace Items.Keycards
{
    public class KeycardDecoderScreen : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private KeycardDecoder _keycardDecoder;
        
        [Space(5)]
        [SerializeField] private Renderer _keycardScreenRenderer;
        [SerializeField] private MaterialPropertyBlock _keycardDecoderPropertyBlock;
        private const string SECURITY_LEVEL_VALID_SHADER_IDENTIFIER = "_IsSecurityLevelValid";
        private const string SECURITY_LEVEL_SHADER_IDENTIFIER = "_SecurityLevel";



        private void Awake()
        {
            _keycardDecoderPropertyBlock = new MaterialPropertyBlock();
            KeycardReader.OnAnyKeycardReaderHighlighted += KeycardReader_OnAnyKeycardReaderHighlighted;
        }
        private void OnDestroy() => KeycardReader.OnAnyKeycardReaderHighlighted -= KeycardReader_OnAnyKeycardReaderHighlighted;
        


        private void KeycardReader_OnAnyKeycardReaderHighlighted(object sender, System.EventArgs e)
        {
            KeycardReader keycardReader = sender as KeycardReader;

            // Get the current values for the renderer's property block.
            _keycardScreenRenderer.GetPropertyBlock(_keycardDecoderPropertyBlock);

            // Set the values for the material to display the correct properties (Whether we can use the reader + the our current security level).
            _keycardDecoderPropertyBlock.SetInteger(SECURITY_LEVEL_SHADER_IDENTIFIER, _keycardDecoder.GetSecurityLevel());
            if (keycardReader.GetIsUnlocked() || keycardReader.GetSecurityLevel() <= _keycardDecoder.GetSecurityLevel())
            {
                // We can use this reader.
                Debug.Log("Valid");
                _keycardDecoderPropertyBlock.SetInteger(SECURITY_LEVEL_VALID_SHADER_IDENTIFIER, 1);
            }
            else
            {
                // We cannot use this reader.
                Debug.Log("Invalid");
                _keycardDecoderPropertyBlock.SetInteger(SECURITY_LEVEL_VALID_SHADER_IDENTIFIER, 0);
            }

            // Apply the changes to the Renderer.
            _keycardScreenRenderer.SetPropertyBlock(_keycardDecoderPropertyBlock);
        }
    }
}