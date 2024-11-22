using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Keycards
{
    public class KeycardDecoderScreen : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private KeycardDecoder _keycardDecoder;
        
        [Space(5)]
        [SerializeField] private Shader _keycardDecoderScreenShader;
        private Material _keycardDecoderMaterial;
        private const string SECURITY_LEVEL_VALID_SHADER_IDENTIFIER = "_IsSecurityLevelValid";
        private const string SECURITY_LEVEL_SHADER_IDENTIFIER = "_SecurityLevel";



        private void Awake()
        {
            foreach(Material material in GetComponentInChildren<Renderer>().materials)
            {
                if (material.shader == _keycardDecoderScreenShader)
                {
                    _keycardDecoderMaterial = material;
                    break;
                }
            }
            
            KeycardReader.OnAnyKeycardReaderHighlighted += KeycardReader_OnAnyKeycardReaderHighlighted;
        }
        private void OnDestroy() => KeycardReader.OnAnyKeycardReaderHighlighted -= KeycardReader_OnAnyKeycardReaderHighlighted;
        


        private void KeycardReader_OnAnyKeycardReaderHighlighted(object sender, System.EventArgs e)
        {
            // Set the values for the material to display the correct properties (Whether we can use the reader + the reader's required level).
            KeycardReader keycardReader = sender as KeycardReader;

            _keycardDecoderMaterial.SetInt(SECURITY_LEVEL_SHADER_IDENTIFIER, keycardReader.GetSecurityLevel());

            if (keycardReader.GetSecurityLevel() <= _keycardDecoder.GetSecurityLevel())
            {
                // We can use this reader.
                Debug.Log("Valid");
                _keycardDecoderMaterial.SetInt(SECURITY_LEVEL_VALID_SHADER_IDENTIFIER, 1);
            }
            else
            {
                // We cannot use this reader.
                Debug.Log("Invalid");
                _keycardDecoderMaterial.SetInt(SECURITY_LEVEL_VALID_SHADER_IDENTIFIER, 0);
            }
        }
    }
}