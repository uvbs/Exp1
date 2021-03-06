//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

#if UNITY_3_5
[CustomEditor(typeof(XUIButton))]
#else
[CustomEditor(typeof(XUIButton), true)]
#endif
public class XUIButtonEditor : UIButtonEditor
{
	protected override void DrawProperties ()
	{
		base.DrawProperties();
		
		NGUIEditorTools.BeginContents();
		NGUIEditorTools.SetLabelWidth(200f);
		SerializedProperty sp = this.serializedObject.FindProperty( "eventTrigger" );
		EditorGUILayout.PropertyField( sp, true );
		NGUIEditorTools.EndContents();
	}
}
