/// <summary>
/// AnimationReferenceの利便用Editorクラス.
/// 
/// 2014/06/11
/// </summary>

using UnityEngine;
using UnityEditor;

#if UNITY_3_5
[CustomEditor(typeof(AnimationReference))]
#else
[CustomEditor(typeof(AnimationReference), true)]
#endif
public class AnimationReferenceEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		GUILayout.Space(8f);

		AnimationReference animationReference = (AnimationReference)target;
		if(GUILayout.Button("Sort AnimationClip"))
		{
			if(animationReference != null)
			{
				animationReference.SortAnimationClip();
				EditorUtility.SetDirty(target);
			}
		}

		GUILayout.Space(8f);
	}
}
