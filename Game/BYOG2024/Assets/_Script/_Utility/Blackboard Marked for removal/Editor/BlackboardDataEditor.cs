using UnityEditor;
using UnityEngine;
namespace _Scripts._Utility.Blackboard.Editor
{
	[CustomPropertyDrawer(typeof(DictElement))]
	public class IngredientDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			SerializedProperty enumValue = property.FindPropertyRelative("Type");

			// Calculate rects for each property field
			Rect nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);
			Rect amountRect = new Rect(position.x, position.y, 30, position.height);
			Rect unitRect = new Rect(position.x + 35, position.y, 50, position.height);

			// Draw the fields
			switch ((DataType)enumValue.enumValueIndex)
			{
				case DataType.Int:
					SerializedProperty intValue = property.FindPropertyRelative("IntField");
					EditorGUI.PropertyField(amountRect, intValue, GUIContent.none);
					break;
				case DataType.Float:
					SerializedProperty floatValue = property.FindPropertyRelative("FloatField");
					EditorGUI.PropertyField(amountRect, floatValue, GUIContent.none);
					break;
				case DataType.Bool:
					SerializedProperty boolValue = property.FindPropertyRelative("BoolField");
					EditorGUI.PropertyField(amountRect, boolValue, GUIContent.none);
					break;
			}
			EditorGUI.PropertyField(unitRect, enumValue, GUIContent.none);
			EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);

			EditorGUI.EndProperty();
		}
	}
}