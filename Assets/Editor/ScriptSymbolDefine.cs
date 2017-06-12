using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Reflection;

namespace UnityEditor
{
	
	/// <summary>定義シンボルアセット.</summary>
	public class ScriptSymbolDefine : ScriptableObject
	{

		/// <summary>シンボルスタイル.</summary>
		public enum Style
		{
			/// <summary>セパレータ.</summary>
			Separator = 0,
			/// <summary>通常.</summary>
			Default,
			/// <summary>グループ（シアン）.</summary>
			GroupCyan,
			/// <summary>グループ（グリーン）.</summary>
			GroupGreen,
			/// <summary>グループ（イエロー）.</summary>
			GroupYellow,
			/// <summary>グループ（オレンジ）.</summary>
			GroupOrange,
			/// <summary>グループ（レッド）.</summary>
			GroupRed,
		}

		/// <summary>定義シンボルクラス.</summary>
		[System.Serializable]
		public class ScriptSymbol
		{
			/// <summary>シンボルスタイル.</summary>
			public Style style = Style.Default;
			/// <summary>定義シンボルは有効か.</summary>
			public bool enabled = false;
			/// <summary>定義シンボル名.</summary>
			public string name = "NEW_SYMBOL";
			/// <summary>定義シンボル説明.</summary>
			public string description = "Symbol Description(<i>option, ritch text enabled</i>)";
		}

		/// <summary>定義シンボルリスト.</summary>
		public List<ScriptSymbol> list = new List<ScriptSymbol> ();


		/// <summary>有効な定義シンボルリストを適用.</summary>
		public void Apply ()
		{
			BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;

			IEnumerable<string> defines = list
				.Where (_ => _.style != Style.Separator && !string.IsNullOrEmpty (_.name) && _.enabled)
				.Select (_ => _.name)
				.Distinct ();

			PlayerSettings.SetScriptingDefineSymbolsForGroup (group, defines.Any () ? defines.Aggregate ((a, b) => a + ";" + b) : string.Empty);
		}

		/// <summary>定義シンボルリストを戻す.</summary>
		public void Revert ()
		{
			string define = PlayerSettings.GetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup);

			IEnumerable<string> currentDefines = define.Replace (" ", "").Split (new char[] { ';' });
			list.ForEach (_ => {
				_.enabled = _.style != Style.Separator && !string.IsNullOrEmpty (_.name) && currentDefines.Contains (_.name);
			});
			foreach (string name in currentDefines.Where(_ => !string.IsNullOrEmpty(_) && !list.Any(__ => __.name == _))) {
				list.Add (new ScriptSymbolDefine.ScriptSymbol () { enabled = true, name = name });
			}
		}


		/// <summary>ProjectBuilderを起動します.いずれかのbuilderをアクティブにします.</summary>
		[MenuItem ("Window/Script Symbol Define", false, 150)]
		static void Open ()
		{
			Selection.activeObject = AssetDatabase.FindAssets ("t:" + typeof(ScriptSymbolDefine).Name)
			.Select (_ => AssetDatabase.LoadAssetAtPath (AssetDatabase.GUIDToAssetPath (_), typeof(ScriptSymbolDefine)) as ScriptSymbolDefine)
				.FirstOrDefault () ?? Create ();

		}

		/// <summary>新しいDefineSymbolsアセットを生成します.</summary>
		static ScriptSymbolDefine Create ()
		{
			if (!Directory.Exists ("Assets/Editor"))
				AssetDatabase.CreateFolder ("Assets", "Editor");
		
			//DefineSymbolsアセット生成して保存.
			ScriptSymbolDefine symbols = ScriptableObject.CreateInstance (typeof(ScriptSymbolDefine)) as ScriptSymbolDefine;
			AssetDatabase.CreateAsset (symbols, "Assets/Editor/ScriptSymbolDefine.asset");
			AssetDatabase.SaveAssets ();
			return symbols;
		}
	}

	[CustomEditor (typeof(ScriptSymbolDefine))]
	internal class ScriptSymbolDefineEditor : Editor
	{
		ScriptSymbolDefine current;
		string currentDefine;
		ReorderableList ro;
		static GUIContent contentApply;
		static GUIStyle styleToggle;
		static GUIStyle styleName;
		static GUIStyle styleDescription;
		static GUIStyle styleOption;
	
		//static int[] colors = new int[]{ 1, 2, 3, 4, 5, 6 };
		//static string[] colorNames = new string[]{ "Blue", "Cyan", "Green", "Yellow", "Orange", "Red" };
		int addIndex = -1;

		void OnEnable ()
		{
			current = target as ScriptSymbolDefine;
			currentDefine = string.Empty;

			contentApply = new GUIContent ("Apply", EditorGUIUtility.FindTexture ("vcs_check"), "Apply script symbols");

			ro = new ReorderableList (new List<ScriptSymbolDefine> (), typeof(ScriptSymbolDefine));
			ro.drawElementCallback = DrawSymbol;
			ro.drawHeaderCallback = _ => GUI.Label (_, "Available Script Symbols");
			ro.onAddCallback = _ => {
				current.list.Add (new ScriptSymbolDefine.ScriptSymbol ());
				addIndex = current.list.Count - 1;
				EditorUtility.SetDirty (current);

			};
			ro.onRemoveCallback = _ => {
				current.list.RemoveAt (_.index);
				if (0 < _.index)
					_.index--;
			};
			ro.onCanRemoveCallback = _ => (0 <= _.index && _.index < current.list.Count);
			ro.elementHeight = 50;
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();
		
			bool isCompiling = EditorApplication.isCompiling;

			//変更チェック.PlayerSettingsなどで直接変えた場合も検知.
			string define = PlayerSettings.GetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup);
			if (currentDefine != define) {
				currentDefine = define;
				current.Revert ();
			}

			EditorGUI.BeginDisabledGroup (isCompiling);
			{
				//リスト表示.
				ro.list = current.list;
				ro.DoLayoutList ();

			
				EditorGUILayout.BeginHorizontal ();
				{
					GUILayout.FlexibleSpace ();
			
					//Applyボタン. PlayerSettingsのDefineSymbolに適用.
					if (GUILayout.Button (contentApply, GUILayout.Width (57)))
						current.Apply ();
				}
				EditorGUILayout.EndHorizontal ();
			}
			EditorGUI.EndDisabledGroup ();

			//コンパイル中表示.
			if (EditorApplication.isCompiling)
				GUILayout.Label ("Compiling...", "NotificationBackground", GUILayout.ExpandWidth (true));
		
			serializedObject.ApplyModifiedProperties ();
		}

		/// <summary>シンボルを表示.</summary>
		void DrawSymbol (Rect rect, int index, bool isActive, bool isFocused)
		{
			ScriptSymbolDefine.ScriptSymbol symbol = ro.list [index] as ScriptSymbolDefine.ScriptSymbol;

			//GUIキャッシュ.
			if (styleToggle == null) {
				styleToggle = new GUIStyle ("OL Toggle");
				styleToggle.focused.background = null;

				styleDescription = new GUIStyle ("HelpBox");
				styleDescription.richText = true;

				styleOption = new GUIStyle ("PaneOptions");
				styleOption.imagePosition = ImagePosition.ImageOnly;

				styleName = new GUIStyle (EditorStyles.boldLabel);
			}

			//セパレータの場合、線引くだけ.
			if (symbol.style == ScriptSymbolDefine.Style.Separator) {
				GUI.Label (new Rect (rect.x + 20, rect.y + 24, rect.width - 40, 16), GUIContent.none, "sv_iconselector_sep");

				//シンボルスタイル.
				DrawSymbolOption (new Rect (rect.width + 10, rect.y + 4, 24, 24), symbol);
				return;
			}

			//シンボル表示.
			EditorGUI.BeginChangeCheck ();
			{
				//背景&トグル.
				bool isGrouped = symbol.style != ScriptSymbolDefine.Style.Default;
				EditorGUI.BeginDisabledGroup (!symbol.enabled);
				{
					GUI.Label (new Rect (rect.x, rect.y, rect.width, 16), GUIContent.none, "flow node " + (int)symbol.style);
				}
				EditorGUI.EndDisabledGroup ();
				bool e = EditorGUI.Toggle (new Rect (rect.x + 5, rect.y - 1, 15, 16), symbol.enabled, isGrouped ? "Radio" : "Toggle");
				if (symbol.enabled != e) {
					if (isGrouped) {
						foreach (var s in current.list.Where(_=>_.style== symbol.style))
							s.enabled = false;
						symbol.enabled = true;
					} else {
						symbol.enabled = e;
					}
				}


				//シンボル名.
				string symbolNameId = string.Format ("symbol neme {0}", index);
				GUI.SetNextControlName ("DefineSymbolsEditor" + index);
				styleName.normal.textColor = !EditorGUIUtility.isProSkin ? Color.black : Color.white;
				EditorGUI.BeginDisabledGroup (!symbol.enabled);
				{
					symbol.name = GUI.TextField (new Rect (rect.x + 21, rect.y, rect.width - 50, 16), symbol.name, styleName);
				}
				EditorGUI.EndDisabledGroup ();

				//シンボルスタイル.
				DrawSymbolOption (new Rect (rect.width + 10, rect.y + 4, 24, 24), symbol);

				//シンボル説明.
				string symbolDescriptionId = string.Format ("symbol desctription {0}", index);
				GUI.SetNextControlName (symbolDescriptionId);
				styleDescription.richText = GUI.GetNameOfFocusedControl () != symbolDescriptionId;
				symbol.description = GUI.TextArea (new Rect (rect.x, rect.y + 16, rect.width, 32), symbol.description, styleDescription);
			
				//Addされたら自動で入力開始.
				if (addIndex == index) {
					EditorGUI.FocusTextInControl (symbolNameId);
					addIndex = -1;
				}
			}
			if (EditorGUI.EndChangeCheck ())
				EditorUtility.SetDirty (current);
		}

		/// <summary>
		/// シンボルスタイルオプションを表示します.
		/// ボタンを押した時にドロップダウンメニューを表示します.
		/// </summary>
		/// <param name="r">表示する座標.</param>
		/// <param name="symbol">表示するシンボル.</param>
		void DrawSymbolOption (Rect r, ScriptSymbolDefine.ScriptSymbol symbol)
		{
			//シンボルスタイル.
			if (GUI.Button (r, GUIContent.none, styleOption)) {
				GenericMenu menu = new GenericMenu ();
				menu.AddItem (new GUIContent ("Delete"), false, () => {
					if (current && current.list.Contains (symbol))
						current.list.Remove (symbol);
					EditorUtility.SetDirty (current);
				});
				foreach (ScriptSymbolDefine.Style style in System.Enum.GetValues(typeof(ScriptSymbolDefine.Style))) {
					menu.AddItem (new GUIContent ("Style/" + style.ToString ()), symbol.style == style
						, _ => {
						symbol.style = (ScriptSymbolDefine.Style)_;
						EditorUtility.SetDirty (current);
					}
						, style);
				}
				menu.DropDown (r);
			}
		}
	}
}