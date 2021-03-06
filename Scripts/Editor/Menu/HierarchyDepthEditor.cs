/// <summary>
/// 【Unity】HierarchyにNGUIのUIWidgetのdepthを編集するGUIを表示するエディタ拡張
/// http://baba-s.hatenablog.com/entry/2015/05/25/125552
/// 
/// 2015/06/08
/// </summary>
using UnityEditor;
using UnityEngine;

public static class ScmHierarchyDepth
{
	private const int BUTTON_WIDTH = 40;
/*
	[InitializeOnLoadMethod]
	private static void Example()
	{
		EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
	}
*/

	static public bool IsActive { get; private set; }
	static private int width = 40;
	static public int Width { get{ return width; } private set{ width = value; } }

	/// <summary>
	/// ReferenceData 内のメッシュに合わせて SphereCollider を追加する
	/// </summary>
	static public void SetNGUIHierarchyDepth(bool isActive, int width)
	{
		Width = width;
		if(IsActive != isActive)
		{
			if (!isActive)
			{
				EditorApplication.hierarchyWindowItemOnGUI -= OnGUI;
			}
			else
			{
				EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
			}
			IsActive = isActive;
		}
		EditorApplication.RepaintHierarchyWindow();
	}

	private static void OnGUI( int instanceID, Rect selectionRect )
	{
		var go = EditorUtility.InstanceIDToObject( instanceID ) as GameObject;
		
		if ( go == null )
		{
			return;
		}
		
		var uiWidget = go.GetComponent<UIWidget>();
		
		if ( uiWidget == null )
		{
			return;
		}
		
		//var depth = uiWidget.depth;
		
		var pos     = selectionRect;
		pos.x       = pos.xMax - BUTTON_WIDTH;
		pos.width   = BUTTON_WIDTH;
		
		if ( GUI.Button( pos, "F" ) )
		{
			uiWidget.depth++;
		}
		
		pos.x -= Width;
		pos.width = Width;
		
		var so = new SerializedObject( uiWidget );
		var sp = so.FindProperty( "mDepth" );
		EditorGUI.PropertyField( pos, sp, new GUIContent( string.Empty ) );
		
		if ( uiWidget.depth != sp.intValue )
		{
			uiWidget.depth = sp.intValue;
		}
		
		pos.x -= BUTTON_WIDTH;
		pos.width = BUTTON_WIDTH;
		
		if ( GUI.Button( pos, "B" ) )
		{
			uiWidget.depth--;
		}
	}
}

/// <summary>
/// Depth設定ウィンドウクラス
/// </summary>
public class ScmHierarchyDepthEditor : EditorWindow
{
	private const int SIZE_MIN = 40;
	private const int SIZE_MAX = 500;

	[MenuItem("Scm/UI/Depth設定ウィンドウ...")]
	static public void OpenWindow()
	{
		EditorWindow.GetWindow<ScmHierarchyDepthEditor>(false, "Editor HierarchyDepth", true);
	}

	void OnGUI()
	{
		EditorGUI.BeginChangeCheck();
		var size = EditorGUILayout.IntSlider("サイズ", ScmHierarchyDepth.Width, SIZE_MIN, SIZE_MAX );
		var isActive = EditorGUILayout.Toggle("Depthの表示", ScmHierarchyDepth.IsActive);
		if (EditorGUI.EndChangeCheck())
		{
			ScmHierarchyDepth.SetNGUIHierarchyDepth(isActive, size);
		}
	}
}