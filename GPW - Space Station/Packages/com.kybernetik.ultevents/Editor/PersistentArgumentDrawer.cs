﻿// UltEvents // https://kybernetik.com.au/ultevents // Copyright 2021-2024 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UltEvents.Editor
{
    [CustomPropertyDrawer(typeof(PersistentArgument), true)]
    internal sealed class PersistentArgumentDrawer : PropertyDrawer
    {
        /************************************************************************************************************************/

        private const float
            SpecialModeToggleWidth = PersistentCallDrawer.RemoveButtonWidth;

        /************************************************************************************************************************/

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propertytype = property.FindPropertyRelative(Names.PersistentArgument.Type);

            return (PersistentArgumentType)propertytype.enumValueIndex == PersistentArgumentType.String
                ? 3 * EditorGUIUtility.singleLineHeight
                : base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            if (DrawerState.Current.TryGetLinkable(out var linkIndex, out var linkType))
                area.width -= SpecialModeToggleWidth;

            var wideMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;

            var typeProperty = argumentProperty.FindPropertyRelative(Names.PersistentArgument.Type);
            switch ((PersistentArgumentType)typeProperty.enumValueIndex)
            {
                case PersistentArgumentType.None:
                    DoErrorMessageGUI(area, argumentProperty, label, "Error: argument type not set"); break;

                case PersistentArgumentType.Bool: DoBoolGUI(area, argumentProperty, label); break;
                case PersistentArgumentType.String: DoStringGUI(area, argumentProperty, label); break;
                case PersistentArgumentType.Int: DoIntGUI(area, argumentProperty, label); break;
                case PersistentArgumentType.Enum: DoEnumGUI(area, argumentProperty, label); break;
                case PersistentArgumentType.Float: DoFloatGUI(area, argumentProperty, label); break;
                case PersistentArgumentType.Vector2: DoVector2GUI(area, argumentProperty, label); break;
                case PersistentArgumentType.Vector3: DoVector3GUI(area, argumentProperty, label); break;
                case PersistentArgumentType.Vector4: DoVector4GUI(area, argumentProperty, label); break;
                case PersistentArgumentType.Quaternion: DoQuaternionGUI(area, argumentProperty, label); break;
                case PersistentArgumentType.Color: DoColorGUI(area, argumentProperty, label); break;
                case PersistentArgumentType.Color32: DoColor32GUI(area, argumentProperty, label); break;
                case PersistentArgumentType.Rect: DoRectGUI(area, argumentProperty, label); break;
                case PersistentArgumentType.Object: DoObjectGUI(area, argumentProperty, label); break;

                case PersistentArgumentType.Parameter:
                case PersistentArgumentType.ReturnValue:
                    DoLinkedValueGUI(area, argumentProperty, label); break;

                default:
                    DoErrorMessageGUI(area, argumentProperty, label,
                        "Error: unexpected argument type " + (PersistentArgumentType)typeProperty.enumValueIndex);
                    break;
            }

            EditorGUIUtility.wideMode = wideMode;

            DoLinkModeToggleGUI(area, argumentProperty, linkIndex, linkType);
        }

        /************************************************************************************************************************/

        private static void DoErrorMessageGUI(Rect area, SerializedProperty argumentProperty, GUIContent label, string message)
        {
            label = EditorGUI.BeginProperty(area, label, argumentProperty);

            EditorGUI.PrefixLabel(area, label);

            area.xMin += EditorGUIUtility.labelWidth;

            var color = GUI.color;
            GUI.color = Color.red;
            GUI.Label(area, message, EditorStyles.whiteLabel);
            GUI.color = color;
            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static void DoBoolGUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            var i = argumentProperty.FindPropertyRelative(Names.PersistentArgument.Int);

            label = EditorGUI.BeginProperty(area, label, i);
            EditorGUI.BeginChangeCheck();

            var value = EditorGUI.Toggle(area, label, i.intValue != 0);

            if (EditorGUI.EndChangeCheck())
            {
                i.intValue = value ? 1 : 0;
            }
            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static void DoStringGUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            var s = argumentProperty.FindPropertyRelative(Names.PersistentArgument.String);

            label = EditorGUI.BeginProperty(area, label, s);
            EditorGUI.BeginChangeCheck();

            area = EditorGUI.PrefixLabel(area, label);
            var value = EditorGUI.TextArea(area, s.stringValue);

            if (EditorGUI.EndChangeCheck())
            {
                s.stringValue = value;
            }
            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static void DoIntGUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            var i = argumentProperty.FindPropertyRelative(Names.PersistentArgument.Int);

            label = EditorGUI.BeginProperty(area, label, i);
            EditorGUI.BeginChangeCheck();

            var value = EditorGUI.IntField(area, label, i.intValue);

            if (EditorGUI.EndChangeCheck())
            {
                i.intValue = value;
            }
            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/
        #region Enum
        /************************************************************************************************************************/

        private static void DoEnumGUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            var enumType = argumentProperty.GetValue<PersistentArgument>().SystemType;
            if (enumType == null)
            {
                DoErrorMessageGUI(area, argumentProperty, label, "Error: enum type not set");
                return;
            }

            var i = argumentProperty.FindPropertyRelative(Names.PersistentArgument.Int);

            label = EditorGUI.BeginProperty(area, label, argumentProperty);
            EditorGUI.BeginChangeCheck();

            int value;
            if (enumType.IsDefined(typeof(FlagsAttribute), true))
            {
                value = DrawEnumMaskField(area, label, i.intValue, enumType);
            }
            else
            {
                value = Convert.ToInt32(EditorGUI.EnumPopup(area, label, (Enum)Enum.ToObject(enumType, i.intValue)));
            }

            if (EditorGUI.EndChangeCheck())
            {
                i.intValue = value;
            }
            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static Dictionary<Type, string[]> _Names;
        private static Dictionary<Type, int[]> _Values;

        /// <summary>Draw a field with a dropdown box for selecting values in a flags enum.</summary>
        /// <remarks>
        /// This method works properly for some enum configurations not supported by EditorGUI.EnumMaskField or EditorGUI.EnumMaskPopup.
        /// <para></para>
        /// This method only supports enums with int as their underlying type.
        /// </remarks>
        public static int DrawEnumMaskField(Rect area, GUIContent label, int enumValue, Type enumType)
        {
            if (_Names == null)
            {
                _Names = new();
                _Values = new();
            }

            if (!_Names.TryGetValue(enumType, out var names))
            {
                names = Enum.GetNames(enumType);
                _Names.Add(enumType, names);
            }

            if (!_Values.TryGetValue(enumType, out var values))
            {
                values = Enum.GetValues(enumType) as int[];
                _Values.Add(enumType, values);
            }

            var maskValue = 0;
            for (int i = 0; i < values.Length; i++)
            {
                var v = values[i];
                if (v != 0)
                {
                    if ((enumValue & v) == v)
                        maskValue |= 1 << i;
                }
                else if (enumValue == 0)
                {
                    maskValue |= 1 << i;
                }
            }

            EditorGUI.BeginChangeCheck();
            var newMaskVal = EditorGUI.MaskField(area, label, maskValue, names);
            if (EditorGUI.EndChangeCheck())
            {
                var changes = maskValue ^ newMaskVal;

                for (int i = 0; i < values.Length; i++)
                {
                    if ((changes & (1 << i)) != 0)
                    {
                        if ((newMaskVal & (1 << i)) != 0)
                        {
                            var v = values[i];
                            if (v == 0)// special case: if "0" is set, just set the value to 0.
                            {
                                enumValue = 0;
                                break;
                            }
                            else
                            {
                                enumValue |= v;
                            }
                        }
                        else
                        {
                            enumValue &= ~values[i];
                        }
                    }
                }
            }

            return enumValue;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

        private static void DoFloatGUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            var x = argumentProperty.FindPropertyRelative(Names.PersistentArgument.X);

            label = EditorGUI.BeginProperty(area, label, x);
            EditorGUI.BeginChangeCheck();

            var value = EditorGUI.FloatField(area, label, x.floatValue);

            if (EditorGUI.EndChangeCheck())
            {
                x.floatValue = value;
            }
            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static GUIContent[] _Vector2Labels;

        private static void DoVector2GUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            _Vector2Labels ??= new GUIContent[]
            {
                new("X"),
                new("Y"),
            };

            var x = argumentProperty.FindPropertyRelative(Names.PersistentArgument.X);

            label = EditorGUI.BeginProperty(area, label, argumentProperty);
            EditorGUI.MultiPropertyField(area, _Vector2Labels, x, label);
            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static GUIContent[] _Vector3Labels;

        private static void DoVector3GUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            _Vector3Labels ??= new GUIContent[]
            {
                new("X"),
                new("Y"),
                new("Z"),
            };

            var x = argumentProperty.FindPropertyRelative(Names.PersistentArgument.X);

            label = EditorGUI.BeginProperty(area, label, argumentProperty);
            EditorGUI.MultiPropertyField(area, _Vector3Labels, x, label);
            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static GUIContent[] _Vector4Labels;

        private static void DoVector4GUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            _Vector4Labels ??= new GUIContent[]
            {
                new("X"),
                new("Y"),
                new("Z"),
                new("W"),
            };

            var x = argumentProperty.FindPropertyRelative(Names.PersistentArgument.X);

            label = EditorGUI.BeginProperty(area, label, argumentProperty);
            EditorGUI.MultiPropertyField(area, _Vector4Labels, x, label);
            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static void DoQuaternionGUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();

            DoVector3GUI(area, argumentProperty, label);

            if (EditorGUI.EndChangeCheck())
            {
                var x = argumentProperty.FindPropertyRelative(Names.PersistentArgument.X);
                var y = argumentProperty.FindPropertyRelative(Names.PersistentArgument.Y);
                var z = argumentProperty.FindPropertyRelative(Names.PersistentArgument.Z);

                x.floatValue %= 360;
                y.floatValue %= 360;
                z.floatValue %= 360;
            }
        }

        /************************************************************************************************************************/

        private static void DoColorGUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            var x = argumentProperty.FindPropertyRelative(Names.PersistentArgument.X);
            var y = argumentProperty.FindPropertyRelative(Names.PersistentArgument.Y);
            var z = argumentProperty.FindPropertyRelative(Names.PersistentArgument.Z);
            var w = argumentProperty.FindPropertyRelative(Names.PersistentArgument.W);

            label = EditorGUI.BeginProperty(area, label, argumentProperty);
            EditorGUI.BeginChangeCheck();

            var value = EditorGUI.ColorField(area, label, new Color(x.floatValue, y.floatValue, z.floatValue, w.floatValue));

            if (EditorGUI.EndChangeCheck())
            {
                x.floatValue = value.r;
                y.floatValue = value.g;
                z.floatValue = value.b;
                w.floatValue = value.a;
            }
            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static void DoColor32GUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            var i = argumentProperty.FindPropertyRelative(Names.PersistentArgument.Int);

            label = EditorGUI.BeginProperty(area, label, argumentProperty);
            EditorGUI.BeginChangeCheck();

            var intValue = i.intValue;
            var value = EditorGUI.ColorField(area, label,
                new Color32((byte)(intValue), (byte)(intValue >> 8), (byte)(intValue >> 16), (byte)(intValue >> 24)));

            if (EditorGUI.EndChangeCheck())
            {
                var value32 = (Color32)value;
                i.intValue = value32.r | (value32.g << 8) | (value32.b << 16) | (value32.a << 24);
            }
            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static GUIContent[] _RectLabels;

        private static void DoRectGUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            _RectLabels ??= new GUIContent[]
            {
                new("X"),
                new("Y"),
                new("W"),
                new("H"),
            };

            var x = argumentProperty.FindPropertyRelative(Names.PersistentArgument.X);

            label = EditorGUI.BeginProperty(area, label, argumentProperty);
            EditorGUI.MultiPropertyField(area, _RectLabels, x, label);
            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static void DoObjectGUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            var o = argumentProperty.FindPropertyRelative(Names.PersistentArgument.Object);

            label = EditorGUI.BeginProperty(area, label, o);
            EditorGUI.BeginChangeCheck();

            var type = argumentProperty.GetValue<PersistentArgument>().SystemType ?? typeof(UnityEngine.Object);

            var value = EditorGUI.ObjectField(area, label, o.objectReferenceValue, type, true);

            if (EditorGUI.EndChangeCheck())
            {
                o.objectReferenceValue = value;
            }
            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static void DoLinkedValueGUI(Rect area, SerializedProperty argumentProperty, GUIContent label)
        {
            var color = GUI.color;

            label = EditorGUI.BeginProperty(area, label, argumentProperty);

            EditorGUI.PrefixLabel(area, label);

            area.xMin += EditorGUIUtility.labelWidth;

            var argument = argumentProperty.GetValue<PersistentArgument>();
            var callIndex = argument._Int;
            var argumentType = argument.SystemType;

            if (argumentType == null)
            {
                GUI.color = PersistentCallDrawer.ErrorFieldColor;
                GUI.Label(area, "Unable to determine argument type");
            }
            else if (DrawerState.Current.Event != null)
            {
                switch (argument.Type)
                {
                    case PersistentArgumentType.Parameter:
                        label.text = $"Parameter {callIndex}";
                        var parameterCount = DrawerState.Current.Event.ParameterCount;
                        var parameters = DrawerState.Current.Event.Parameters;

                        if (callIndex < 0 || callIndex >= parameterCount)
                        {
                            GUI.color = PersistentCallDrawer.ErrorFieldColor;
                            label.tooltip = "Parameter link index out of range";
                        }
                        else
                        {
                            var parameterType = DrawerState.Current.Event.GetParameterType(callIndex);

                            label.text += $" ({parameterType.GetNameCS(false)}";

                            if (parameters != null)
                                label.text += $" {parameters[callIndex].Name}";

                            label.text += ")";

                            if (!argumentType.IsAssignableFrom(parameterType))
                            {
                                GUI.color = PersistentCallDrawer.ErrorFieldColor;
                                label.tooltip = "Incompatible parameter type";
                            }
                        }
                        break;

                    case PersistentArgumentType.ReturnValue:
                        label.text = $"Return {callIndex}: ";
                        var linkedMember = DrawerState.Current.GetLinkedMember(callIndex);

                        if (linkedMember == null)
                        {
                            label.text += "(no method set)";
                            GUI.color = PersistentCallDrawer.ErrorFieldColor;
                        }
                        else
                        {
                            label.text += MemberSelectionMenu.GetSignature(linkedMember, true);

                            if ((uint)DrawerState.Current.callIndex <= (uint)callIndex)
                            {
                                GUI.color = PersistentCallDrawer.ErrorFieldColor;
                                label.tooltip =
                                    "The linked method must be called before this argument" +
                                    " can retrieve its return value";
                            }
                            else if (!argumentType.IsAssignableFrom(linkedMember.GetReturnType()))
                            {
                                GUI.color = PersistentCallDrawer.ErrorFieldColor;
                                label.tooltip = "Return type is incompatible with argument type";
                            }
                        }
                        break;
                }

                if (GUI.Button(area, label, PersistentCallDrawer.PopupButtonStyle))
                {
                    if (Event.current.button == 0)
                        ShowLinkMenu(area, argumentProperty, argumentType, callIndex, argument.Type);
                }
            }

            EditorGUI.EndProperty();

            GUI.color = color;
        }

        /************************************************************************************************************************/

        private static void ShowLinkMenu(Rect area, SerializedProperty argumentProperty, Type systemType, int linkIndex, PersistentArgumentType linkType)
        {
            var typeProperty = argumentProperty.FindPropertyRelative(Names.PersistentArgument.Type);
            var intProperty = argumentProperty.FindPropertyRelative(Names.PersistentArgument.Int);

            var menu = new GenericMenu();
            menu.AddDisabledItem(new($"Link to {systemType.GetNameCS()}"));

            // Parameters.
            var parameters = DrawerState.Current.Event.Parameters;

            var parameterCount = DrawerState.Current.Event.ParameterCount;
            for (int i = 0; i < parameterCount; i++)
            {
                var parameterType = DrawerState.Current.Event.GetParameterType(i);
                if (!systemType.IsAssignableFrom(parameterType))
                    continue;

                var content = parameters == null ?
                    new GUIContent($"Parameter {i} ({parameterType.GetNameCS(false)})") :
                    new GUIContent($"Parameter {i} ({parameterType.GetNameCS(false)} {parameters[i].Name})");

                var on = i == linkIndex && linkType == PersistentArgumentType.Parameter;

                var index = i;
                menu.AddItem(content, on, () =>
                {
                    typeProperty.enumValueIndex = (int)PersistentArgumentType.Parameter;
                    intProperty.intValue = index;
                    argumentProperty.serializedObject.ApplyModifiedProperties();
                });
            }

            // Returned Values.
            for (int i = 0; i < DrawerState.Current.PreviousCallCount; i++)
            {
                var member = DrawerState.Current.GetPreviousCall(i).GetMemberSafe();
                if (member == null || !systemType.IsAssignableFrom(member.GetReturnType()))
                    continue;

                var content = new GUIContent(
                    $"Returned Value {i} ({MemberSelectionMenu.GetSignature(member, true)})");

                var on = i == linkIndex && linkType == PersistentArgumentType.ReturnValue;

                var index = i;
                menu.AddItem(content, on, () =>
                {
                    typeProperty.enumValueIndex = (int)PersistentArgumentType.ReturnValue;
                    intProperty.intValue = index;
                    argumentProperty.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.DropDown(area);
        }

        /************************************************************************************************************************/

        private static readonly GUIContent
            LinkToggleContent = new("∞", "Link to Parameter or Returned Value");

        private static GUIStyle _LinkToggleStyle;

        private static void DoLinkModeToggleGUI(Rect area, SerializedProperty argumentProperty, int linkIndex, PersistentArgumentType linkType)
        {
            if (linkIndex < 0)
                return;

            _LinkToggleStyle ??= new GUIStyle(EditorStyles.miniButton)
            {
                padding = new RectOffset(0, -1, 0, 1),
                fontSize = 12,
            };

            area.x += area.width + 2;
            area.width = SpecialModeToggleWidth - 2;

            var currentArgument = DrawerState.Current.call._PersistentArguments[DrawerState.Current.parameterIndex];

            var wasLink =
                currentArgument.Type == PersistentArgumentType.Parameter ||
                currentArgument.Type == PersistentArgumentType.ReturnValue;

            if (wasLink != GUI.Toggle(area, wasLink, LinkToggleContent, _LinkToggleStyle))
            {
                argumentProperty.ModifyValues<PersistentArgument>((argument) =>
                {
                    if (wasLink)// Unlink.
                    {
                        argument.SystemType = argument.SystemType;

                        var parameter = DrawerState.Current.CurrentParameter;
                        if (parameter != null &&
                            (parameter.Attributes & ParameterAttributes.HasDefault) == ParameterAttributes.HasDefault)
                            argument.Value = parameter.DefaultValue;
                    }
                    else// Link.
                    {
                        argument.LinkTo(DrawerState.Current.Event, linkType, linkIndex);
                    }
                }, wasLink ? "Unlink Argument" : "Link Argument");
            }
        }

        /************************************************************************************************************************/
    }
}

#endif