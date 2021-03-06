using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MasterTextSetterList : MonoBehaviour
{
	[SerializeField]
	List<MasterTextSetter> _list = new List<MasterTextSetter>();
#if UNITY_EDITOR
	public
#endif
	List<MasterTextSetter> List { get { return _list; } set { _list = value; } }

	void OnEnable()
	{
		this.UpdateText();
	}

	public void UpdateText()
	{
		if (this.List != null)
		{
			foreach (var t in this.List)
			{
				t.SetMasterData();
			}
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(MasterTextSetterList))]
	public class MasterTextSetterListEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var com = target as MasterTextSetterList;

			if (GUILayout.Button("リスト整理"))
			{
				if (EditorUtility.DisplayDialog(
					"リスト整理",
					"以下のものをリストから削除します。\r\n" +
					"\r\n" +
					"１．MasterTextID が【0】のもの\r\n" +
					"２．UILabel が設定されていないもの",
					"OK", "キャンセル"))
				{
					Undo.RecordObject(com, "Organize List");
					var deleteList = new List<MasterTextSetter>();
					com.List.ForEach(t => { if (t.TextMasterID == 0 || t.Label == null) { deleteList.Add(t); } });
					deleteList.ForEach(t => { com.List.Remove(t); });

					string text = "";
					deleteList.Sort((a, b) => { return a.TextMasterID.CompareTo(b.TextMasterID); });
					deleteList.ForEach(t =>
						{
							var labelName = t.Label != null ? t.Label.name : "";
							text += string.Format("{0:0000}\t{1}\r\n", t.TextMasterID, labelName);
						});
					Debug.Log("リスト整理\t" + com.name + "\r\n" + "MasterTextID\tラベル名\r\n" + text);
					EditorUtility.DisplayDialog("結果", string.Format("{0} 個 削除しました。\r\n詳細はコンソールウィンドウへ。", deleteList.Count), "OK");
				}
			}

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("ID昇順ソート"))
			{
				if (EditorUtility.DisplayDialog("ID昇順ソート", "リストを MasterTextID で昇順ソートします。", "OK", "キャンセル"))
				{
					Undo.RecordObject(com, "Ascend Order by MasterTextID");
					com.List.Sort((a, b) => { return a.TextMasterID.CompareTo(b.TextMasterID); });
				}
			}
			if (GUILayout.Button("ID降順ソート"))
			{
				if (EditorUtility.DisplayDialog("ID昇順ソート", "リストを MasterTextID で降順ソートします。", "OK", "キャンセル"))
				{
					Undo.RecordObject(com, "Descend Order by MasterTextID");
					com.List.Sort((a, b) => { return b.TextMasterID.CompareTo(a.TextMasterID); });
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("ラベル名昇順ソート"))
			{
				if (EditorUtility.DisplayDialog("ラベル名昇順ソート", "リストをラベル名で昇順ソートします。", "OK", "キャンセル"))
				{
					Undo.RecordObject(com, "Ascend Order by LabeName");
					com.List.Sort((a, b) => { return (a.Label == null || b.Label == null) ? 0 : a.Label.name.CompareTo(b.Label.name); });
				}
			}
			if (GUILayout.Button("ラベル名降順ソート"))
			{
				if (EditorUtility.DisplayDialog("ラベル名降順ソート", "リストをラベル名で降順ソートします。", "OK", "キャンセル"))
				{
					Undo.RecordObject(com, "Descend Order by LabeName");
					com.List.Sort((a, b) => { return (a.Label == null || b.Label == null) ? 0 : b.Label.name.CompareTo(a.Label.name); });
				}
			}
			EditorGUILayout.EndHorizontal();

			base.OnInspectorGUI();
		}
	}
#endif
}

[System.Serializable]
public class MasterTextSetter
{
	[SerializeField]
	UILabel _label;
	public UILabel Label { get { return _label; } private set { _label = value; } }

	[SerializeField]
	int _textMasterID;
	public int TextMasterID { get { return _textMasterID; } private set { _textMasterID = value; } }

	public void SetMasterData()
	{
		if (this.Label != null)
		{
			this.Label.text = MasterData.GetText((TextType)this.TextMasterID).Replace("\\n", "\n");
		}
	}
}
