using System;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
namespace AstekUtility.DesignPattern.Blackboard
{
	[CustomEditor(typeof(BlackBoardData))]
	public class BlackboardDataEditor : OdinEditor
	{
		private ReorderableList entryList;

		protected override void OnEnable()
		{
			entryList = new ReorderableList(serializedObject, serializedObject.FindProperty("Entries"), true, true, true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight), "Key");
					EditorGUI.LabelField(new Rect(rect.x + rect.width * 0.3f + 10, rect.y, rect.width * 0.3f,
						EditorGUIUtility.singleLineHeight), "Type");
					EditorGUI.LabelField(new Rect(rect.x + rect.width * 0.6f + 5, rect.y, rect.width * 0.4f,
						EditorGUIUtility.singleLineHeight), "Value");
				}
			};

			entryList.drawElementCallback = (rect, index, isActive, isFocused) =>
			{
				SerializedProperty element = entryList.serializedProperty.GetArrayElementAtIndex(index);

				rect.y += 2;
				SerializedProperty keyName = element.FindPropertyRelative("KeyName");
				SerializedProperty valueType = element.FindPropertyRelative("ValueType");
				SerializedProperty value = element.FindPropertyRelative("Value");

				Rect keyNameRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
				Rect valueTypeRect = new Rect(rect.x + rect.width * 0.3f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
				Rect valueRect = new Rect(rect.x + rect.width * 0.6f, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight);

				EditorGUI.PropertyField(keyNameRect, keyName, GUIContent.none);
				EditorGUI.PropertyField(valueTypeRect, valueType, GUIContent.none);

				switch ((AnyValue.ValueType)valueType.enumValueIndex)
				{
					case AnyValue.ValueType.Int:
						SerializedProperty intValue = value.FindPropertyRelative("IntValue");
						EditorGUI.PropertyField(valueRect, intValue, GUIContent.none);
						break;
					case AnyValue.ValueType.Float:
						SerializedProperty floatValue = value.FindPropertyRelative("FloatValue");
						EditorGUI.PropertyField(valueRect, floatValue, GUIContent.none);
						break;
					case AnyValue.ValueType.Bool:
						SerializedProperty boolValue = value.FindPropertyRelative("BoolValue");
						EditorGUI.PropertyField(valueRect, boolValue, GUIContent.none);
						break;
					case AnyValue.ValueType.String:
						SerializedProperty strValue = value.FindPropertyRelative("StringValue");
						EditorGUI.PropertyField(valueRect, strValue, GUIContent.none);
						break;
					case AnyValue.ValueType.Vector3:
						SerializedProperty vec3Value = value.FindPropertyRelative("Vector3Value");
						EditorGUI.PropertyField(valueRect, vec3Value, GUIContent.none);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

			};
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			entryList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}
	}
}