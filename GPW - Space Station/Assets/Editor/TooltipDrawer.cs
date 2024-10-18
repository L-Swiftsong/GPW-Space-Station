using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;


// Link: 'https://discussions.unity.com/t/can-you-add-editor-tooltips-to-the-shader-properties/713679/2'.


/// <summary>
/// Draws a tooltip for material properties. <see cref="TooltipAttribute"/>
/// Usage: [Tooltip(Write your tooltip here without quotes] _Color("Albedo", Color) = (1,1,1,1))
/// </summary>
/// <remarks> You cannot use quotation marks, apostrophes, ampersands, commas, or similar punctuation.</remarks>
public class TooltipDrawer : MaterialPropertyDrawer
{
    private GUIContent _guiContent;

    private MethodInfo _internalMethod;
    private Type[] _methodArgumentTypes;
    private object[] _methodArguments;

    public TooltipDrawer(string tooltip)
    {
        // Create a GUIContent instance for the '_guiContent' variable.
        _guiContent = new GUIContent(string.Empty, tooltip);

        // Setup the 'methodArgumentTypes' and 'methodArguments' arrays.
        _methodArgumentTypes = new[] { typeof(Rect), typeof(MaterialProperty), typeof(GUIContent) };
        _methodArguments = new object[3];

        // Cache the instanced, non-public method 'DefaultShaderPropertyInternal' with parameters of the types found in the '_methodArgumentTypes' array.
        _internalMethod = typeof(MaterialEditor)
            .GetMethod("DefaultShaderPropertyInternal", BindingFlags.Instance | BindingFlags.NonPublic, null, _methodArgumentTypes, null);
    }


    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
        _guiContent.text = label;

        if (_internalMethod != null)
        {
            _methodArguments[0] = position;
            _methodArguments[1] = prop;
            _methodArguments[2] = _guiContent;

            // Using reflection, invoke the method cached in the constructor.
            _internalMethod.Invoke(editor, _methodArguments);
        }
    }
}
