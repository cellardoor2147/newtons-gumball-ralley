#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Destructible2D
{
	/// <summary>This is the base class for all inspectors.</summary>
	public abstract class D2dEditor : Editor
	{
		protected static SerializedObject data;

		private static GUIContent customContent = new GUIContent();

		private static GUIStyle expandStyle;

		private static List<Color> colors = new List<Color>();

		private static List<float> labelWidths = new List<float>();

		public void GetTargets<T>(out T tgt, out T[] tgts)
			where T : Object
		{
			tgt  = (T)target;
			tgts = System.Array.ConvertAll(targets, item => (T)item);
		}

		public static SerializedObject Data
		{
			set
			{
				data = value;
			}

			get
			{
				return data;
			}
		}

		public override void OnInspectorGUI()
		{
			data = serializedObject;

			ClearStacks();

			Separator();

			OnInspector();

			Separator();

			serializedObject.ApplyModifiedProperties();

			if (serializedObject.hasModifiedProperties == true)
			{
				GUI.changed = true; Repaint();

				foreach (var t in targets)
				{
					EditorUtility.SetDirty(t);
				}
			}
		}

		public virtual void OnSceneGUI()
		{
			OnScene();

			if (GUI.changed == true)
			{
				EditorUtility.SetDirty(target);
			}
		}

		protected void Each<T>(T[] tgts, System.Action<T> update, bool dirty = false)
			where T : Object
		{
			foreach (var t in tgts)
			{
				update(t);

				if (dirty == true)
				{
					EditorUtility.SetDirty(t);
				}
			}
		}

		protected bool Any<T>(T[] tgts, System.Func<T, bool> check)
			where T : Object
		{
			foreach (var t in tgts)
			{
				if (check(t) == true)
				{
					return true;
				}
			}

			return false;
		}

		protected bool All<T>(T[] tgts, System.Func<T, bool> check)
			where T : Object
		{
			foreach (var t in tgts)
			{
				if (check(t) == false)
				{
					return false;
				}
			}

			return true;
		}

		public static void Info(string message)
		{
			EditorGUILayout.HelpBox(message, MessageType.Info);
		}

		public static void Warning(string message)
		{
			EditorGUILayout.HelpBox(message, MessageType.Warning);
		}

		public static void Error(string message)
		{
			EditorGUILayout.HelpBox(message, MessageType.Error);
		}

		public static void Separator()
		{
			EditorGUILayout.Separator();
		}

		public static void BeginIndent()
		{
			EditorGUI.indentLevel += 1;
		}

		public static void EndIndent()
		{
			EditorGUI.indentLevel -= 1;
		}

		public static bool Button(string text)
		{
			return GUILayout.Button(text);
		}

		public static bool HelpButton(string helpText, UnityEditor.MessageType type, string buttonText, float buttonWidth)
		{
			var clicked = false;

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.HelpBox(helpText, type);

				var style = new GUIStyle(GUI.skin.button); style.wordWrap = true;

				clicked = GUILayout.Button(buttonText, style, GUILayout.ExpandHeight(true), GUILayout.Width(buttonWidth));
			}
			EditorGUILayout.EndHorizontal();

			return clicked;
		}

		public static void ClearStacks()
		{
			while (colors.Count > 0)
			{
				EndColor();
			}

			while (labelWidths.Count > 0)
			{
				EndLabelWidth();
			}
		}

		public static void BeginMixed(bool mixed = true)
		{
			EditorGUI.showMixedValue = mixed;
		}

		public static void EndMixed()
		{
			EditorGUI.showMixedValue = false;
		}

		public static void BeginDisabled(bool disabled = true)
		{
			EditorGUI.BeginDisabledGroup(disabled);
		}

		public static void EndDisabled()
		{
			EditorGUI.EndDisabledGroup();
		}

		public static void BeginError(bool error = true)
		{
			BeginColor(Color.red, error);
		}

		public static void EndError()
		{
			EndColor();
		}

		public static void BeginColor(Color color, bool show = true)
		{
			colors.Add(GUI.color);

			GUI.color = show == true ? color : colors[0];
		}

		public static void EndColor()
		{
			if (colors.Count > 0)
			{
				var index = colors.Count - 1;

				GUI.color = colors[index];

				colors.RemoveAt(index);
			}
		}

		public static void BeginLabelWidth(float width)
		{
			labelWidths.Add(EditorGUIUtility.labelWidth);

			EditorGUIUtility.labelWidth = width;
		}

		public static void EndLabelWidth()
		{
			if (labelWidths.Count > 0)
			{
				var index = labelWidths.Count - 1;

				EditorGUIUtility.labelWidth = labelWidths[index];

				labelWidths.RemoveAt(index);
			}
		}

		public static bool DrawFoldout(string title, string tooltip, string property = "m_Name")
		{
			var prop = data.FindProperty(property);

			prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, new GUIContent(title, tooltip));

			return prop.isExpanded;
		}

		public static bool DrawExpand(ref bool expand, string propertyPath, string overrideTooltip = null, string overrideText = null)
		{
			var rect     = D2dHelper.Reserve();
			var property = data.FindProperty(propertyPath);

			customContent.text    = string.IsNullOrEmpty(overrideText   ) == false ? overrideText    : property.displayName;
			customContent.tooltip = string.IsNullOrEmpty(overrideTooltip) == false ? overrideTooltip : property.tooltip;

			if (expandStyle == null)
			{
				expandStyle = new GUIStyle(EditorStyles.miniLabel); expandStyle.alignment = TextAnchor.MiddleRight;
			}

			if (EditorGUI.DropdownButton(new Rect(rect.position + Vector2.left * 15, new Vector2(15.0f, rect.height)), new GUIContent(expand ? "-" : "+"), FocusType.Keyboard, expandStyle) == true)
			{
				expand = !expand;
			}

			EditorGUI.BeginChangeCheck();

			EditorGUI.PropertyField(rect, property, customContent, true);

			var changed = EditorGUI.EndChangeCheck();

			return changed;
		}

		public static bool Draw(string propertyPath, string overrideTooltip = null, string overrideText = null)
		{
			var property = data.FindProperty(propertyPath);

			customContent.text    = string.IsNullOrEmpty(overrideText   ) == false ? overrideText    : property.displayName;
			customContent.tooltip = string.IsNullOrEmpty(overrideTooltip) == false ? overrideTooltip : property.tooltip;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(property, customContent, true);

			return EditorGUI.EndChangeCheck();
		}

		public static void Draw(string propertyPath, ref bool dirty, string overrideTooltip = null, string overrideText = null)
		{
			if (Draw(propertyPath, overrideTooltip, overrideText) == true)
			{
				dirty = true;
			}
		}

		public static bool DrawMinMax(string propertyPath, float min, float max, string overrideTooltip = null, string overrideText = null)
		{
			var property = data.FindProperty(propertyPath);
			var value    = property.vector2Value;

			customContent.text    = string.IsNullOrEmpty(overrideText   ) == false ? overrideText    : property.displayName;
			customContent.tooltip = string.IsNullOrEmpty(overrideTooltip) == false ? overrideTooltip : property.tooltip;

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.MinMaxSlider(customContent, ref value.x, ref value.y, min, max);

			if (EditorGUI.EndChangeCheck() == true)
			{
				property.vector2Value = value;

				return true;
			}

			return false;
		}

		protected virtual void OnInspector()
		{
		}

		protected virtual void OnScene()
		{
		}
	}
}
#endif