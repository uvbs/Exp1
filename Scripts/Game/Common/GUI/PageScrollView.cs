/// <summary>
/// ページ付きスクロールビュー
/// 
/// 2014/05/30
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PageScrollViewAttach
{
	public GameObject prefab;
	public UIScrollView scrollView;
	public UITable table;
    public UIGrid grid;
	public UICenterOnChild center;
	public UILabel pageLabel;

	[SerializeField]
	UIWidget _backButtonGroup = null;
	public UIWidget BackButtonGroup { get { return _backButtonGroup; } }
	[SerializeField]
	UIWidget _nextButtonGroup = null;
	public UIWidget NextButtonGroup { get { return _nextButtonGroup; } }
}
public abstract class PageScrollView<T> where T : Component
{
	#region フィールド&プロパティ
	/// <summary>
	/// 1ページの最大アイテム数
	/// </summary>
	[SerializeField]
	int _itemMax = 100;
	public int ItemMax { get { return _itemMax; } }

	/// <summary>
	/// パネル内に表示できるアイテムの最大数
	/// </summary>
	[SerializeField]
	int _inPanelItemMax = 0;
	public int InPanelItemMax{get { return this._inPanelItemMax; }}

	/// <summary>
	/// 次のページに移った時の初期インデックス
	/// </summary>
	[SerializeField]
	int _nextItemIndex = 0;
	public int NextItemIndex { get { return _nextItemIndex; } }

	/// <summary>
	/// ページ数表示フォーマット
	/// </summary>
	[SerializeField]
	string _pageFormat = "{0:00}/{1:00}";
	public string PageFormat { get { return _pageFormat; } }

	public PageScrollViewAttach Attach { get; private set; }
	public List<UIButton> BackButtonList { get; private set; }
	public List<UIButton> NextButtonList { get; private set; }

	public List<T> ItemList { get; private set; }
	public int Total { get; private set; }
	public int PageIndex { get; private set; }
	public int PageMax { get; private set; }

	private System.Action _dragFinishedFunc = () => { };
	public System.Action DragFinishedFunc { get { return _dragFinishedFunc; } }

	private System.Action _stoppedMovingFunc = () => { };
	public System.Action StoppedMovingFunc { get { return _stoppedMovingFunc; } }

	public int NowPage { get { return PageIndex + 1; } }
	public int NowPageStartIndex { get { return PageIndex * ItemMax; } }
	public int NowPageItemMax
	{
		get
		{
			// 最後のページではない
			if (NowPage < PageMax)
				return ItemMax;
			// 最後のページなのでアイテムのあまりを計算する
			int itemMax = 0;
			{
				// 最後のページ以外のアイテム数を計算して
				// トータルから引けばあまりが出る
				int t = Mathf.Max(0, PageMax - 1) * ItemMax;
				itemMax = Total - t;
			}
			return itemMax;
		}
	}
	#endregion

	#region 抽象メソッド
	protected abstract T Create(GameObject prefab, Transform parent, int itemIndex);
	protected abstract void ClearValue(T item);
	#endregion

	#region 初期化
	/// <summary>
	/// 作成
	/// 初期化時に一回のみ、その後に Setup で個数を決める
	/// </summary>
	public void Create(PageScrollViewAttach attach, System.Action dragFinishedFunc)
	{
		Create(attach,dragFinishedFunc,()=>{});
	}
	public void Create(PageScrollViewAttach attach, System.Action dragFinishedFunc,System.Action stoppedMovingFunc)
	{
		this.Attach = attach;

		// テーブル以下の余計なオブジェクトを削除する
		this.DestroyItem();

		// ボタン取得
		this.BackButtonList = new List<UIButton>();
		if (attach.BackButtonGroup != null)
		{
			var list = attach.BackButtonGroup.GetComponentsInChildren<UIButton>();
			this.BackButtonList.AddRange(list);
		}
		this.NextButtonList = new List<UIButton>();
		if (attach.NextButtonGroup != null)
		{
			var list = attach.NextButtonGroup.GetComponentsInChildren<UIButton>();
			this.NextButtonList.AddRange(list);
		}

		// 必要数のアイテムをインスタンス化して追加する
		this.ItemList = new List<T>();
		for (int i = 0, max = this.ItemMax; i < max; i++)
		{
		    Transform root = (this.Attach.table == null ? this.Attach.grid.transform : this.Attach.table.transform);
			T t = this.Create(this.Attach.prefab, root, i);
			this.ItemList.Add(t);

			// 並びを整えるため一旦全部オンに
			t.gameObject.SetActive(true);
		}

		// 表示アイテム数が_centerItemPadding以下の時に
		// CenterOnすると位置が変になるのであらかじめ並べる
		this.ScrollReset();
		this.Reposition();

		// 全部オフに
		foreach( var item in this.ItemList )
		{
			item.gameObject.SetActive(false);
		}

		// ドラッグ終了時の更新処理設定
		this._dragFinishedFunc = (dragFinishedFunc != null ? dragFinishedFunc : () => { });
		{
			if (attach.center != null)
			{
				// HACK:NGUI3.5.8 UICenterOnChild 内で OnDragFinished を上書きしているので対処
				// 先に UICenterOnChild 内で設定させてから OnDragFinished の処理を追加する
				// UITable に子供がいない状態で Recenter しないと余計な処理が走ってしまうため上記で先に子供を削除しておく
				// HACK:NGUI3.6.5 Transform.ChildCount が 0 の時には処理を走らせないように最適化していた為、アイテム生成後に設定するように処理を変更した
				attach.center.Recenter();
			}
			var scrollView = attach.scrollView;
			if (scrollView != null)
			{
				scrollView.onDragFinished += this.OnDragFinished;
				if (scrollView.horizontalScrollBar != null)
					scrollView.horizontalScrollBar.onDragFinished += this.OnDragFinished;
				if (scrollView.verticalScrollBar != null)
					scrollView.verticalScrollBar.onDragFinished += this.OnDragFinished;
				// UICenterOnChild コンポーネントを入れてオフにしていても
				// scrollView.centerOnChild には UICenterOnChild 側から入れてしまうので
				// attach.center で null の場合は上書きするようにした
				// ※ scrollView.centerOnChild が設定されているとアクティブをオフにする時に
				// ※ SpringPosition.Begin を呼んでしまい、アクティブをオンにした時にズレてしまう
				scrollView.centerOnChild = attach.center;
			}
			if (attach.center != null)
				attach.center.onFinished += this.OnCenterFinished;
		}
		// スクロール終了時のコールバック設定
		this._stoppedMovingFunc = (stoppedMovingFunc != null ? stoppedMovingFunc : ()=>{});
		{
			var scrollView = attach.scrollView;
			if (scrollView != null)
				scrollView.onStoppedMoving += OnStoppedMovingFinish;
		}
		// ドラッグ開始の挙動設定
		{
			var scrollView = this.Attach.scrollView;
			if(scrollView != null)
				scrollView.onDragStarted += this.OnDragStarted;
		}

		// 作成
		this.Setup(0, this.NextItemIndex);
	}
	/// <summary>
	/// テーブル内にあるアイテムを全削除
	/// </summary>
	void DestroyItem()
	{
        if (this.Attach.table == null && this.Attach.grid == null)
			return;

        Transform transform = (this.Attach.table == null ? this.Attach.grid.transform : this.Attach.table.transform);
		for (int i = 0, max = transform.childCount; i < max; i++)
		{
			var child = transform.GetChild(i);
			Object.Destroy(child.gameObject);
		}
	}
	/// <summary>
	/// ドラッグ終了時のイベント
	/// </summary>
	void OnDragFinished()
	{
		this.DragFinishedFunc();
	}
	/// <summary>
	/// CenterOn メソッド終了時のイベント
	/// </summary>
	void OnCenterFinished()
	{
		if (this.Attach.center != null)
			this.Attach.center.enabled = false;
		this.OnDragFinished();
	}
	/// <summary>
	/// スクロール終了時のイベント
	/// </summary>
	void OnStoppedMovingFinish()
	{
		this.StoppedMovingFunc();
	}
	/// <summary>
	/// ドラッグ開始時のイベント
	/// </summary>
	void OnDragStarted()
	{
		// CenterOn中にドラッグした時にCenterOnが切れないのでfalseにする
		if( this.Attach.center != null )
			this.Attach.center.enabled = false;
	}
	#endregion

	#region 設定
	/// <summary>
	/// セットアップ
	/// </summary>
	public void Setup(int total, int centerIndexInTotal)
	{
		this.Total = total;

		// ページの最大数を求める
		// ギリギリページが追加されない程度に下駄を履かせて計算
		if (this.ItemMax != 0)
		{
			int t = (total + (this.ItemMax - 1));
			this.PageMax = t / this.ItemMax;
		}
		// センターアイテムを設定する
		this.PageIndex = -1;
		this.CenterOnInTotal(centerIndexInTotal);
	}
	/// <summary>
	/// センターアイテムを設定する
	/// 全アイテム中のインデックスを指定
	/// </summary>
	/// <returns>ページを切り替えたかどうか戻す</returns>
	public bool CenterOnInTotal(int indexInTotal)
	{
		int pageIndex, itemIndex;
		this.ConverIndex(indexInTotal, out pageIndex, out itemIndex);

		return this.SetPage(pageIndex, itemIndex);
	}
	/// <summary>
	/// センターアイテムを設定する
	/// 現ページにあるアイテムのインデックスを指定
	/// </summary>
	public void CenterOn(int itemIndex)
	{
		// アイテム計算
		{
			int begin = 0;
			int end = (this.NowPageItemMax - 1);
			end = Mathf.Max(begin, end);
			itemIndex = Mathf.Clamp(itemIndex, begin, end);
		}

		// アイテム数がパネルに表示できる最大数以下なら一番下のアイテムをCenterOnして
		// バーが一番上に来るようにする
		if( this.NowPageItemMax <= this.InPanelItemMax )
			itemIndex = this.ItemMax-1;

		// アイテムを設定する
		T item = this.GetItem(itemIndex);
		if (item != null && this.Attach.center != null)
		{
			this.Attach.center.enabled = true;
			this.Attach.center.CenterOnRestictWithinPanel(item.transform);
			// CenterOn 終了時に OnCenterFinished を呼ぶ
		}
	}
	/// <summary>
	/// 次のページヘ
	/// </summary>
	/// <returns>ページを切り替えたかどうか戻す</returns>
	public bool SetNextPage(int add)
	{
		return this.SetPage(this.PageIndex + add, this.NextItemIndex);
	}
	/// <summary>
	/// 指定したページヘ
	/// </summary>
	/// <returns>ページを切り替えたかどうか戻す</returns>
	public bool SetPage(int pageIndex, int itemIndex)
	{
		var t = this.Attach;

		// 現在のページにいる場合は更新しない
		bool isPageChange = true;
		{
			int pageEnd = Mathf.Max(this.PageMax - 1, 0);
			pageIndex = Mathf.Clamp(pageIndex, 0, pageEnd);
			if (this.PageIndex == pageIndex)
				isPageChange = false;
		}
		this.PageIndex = pageIndex;

		// ページ表示を設定する
		if (t.pageLabel != null)
		{
			if(this.PageMax > 0) {
				t.pageLabel.text = string.Format(this.PageFormat, this.NowPage, this.PageMax);
			} else {
				t.pageLabel.text = string.Format(this.PageFormat, 1, 1);
			}
		}

		// ページの端ならボタンを押せなくする
		{
			bool isBackEnable = (this.PageIndex != 0);
			if (t.BackButtonGroup != null)
				t.BackButtonGroup.gameObject.SetActive(isBackEnable);
			bool isNextEnable = (this.NowPage < this.PageMax);
			if (t.NextButtonGroup != null)
				t.NextButtonGroup.gameObject.SetActive(isNextEnable);
		}

		// ページを切り替える場合
		if (isPageChange)
		{
			int count = this.NowPageItemMax;
			// アイテム初期化
			this.Clear(count);
			// アイテムアクティブ設定
			this.SetItemActive(count);
			// テーブル整形
			this.Reposition();
		}

		// 指を離した後のスクロールを切る　
		// スクロールして指を離した時にページを切り替えるとCenterOnが効かないバグの対策
		if (this.Attach.scrollView != null)
			this.Attach.scrollView.currentMomentum = Vector3.zero;

		// アイテムをセンターに設定する
		this.CenterOn(itemIndex);

		return isPageChange;
	}
	/// <summary>
	/// テーブル整形
	/// </summary>
	public void Reposition()
	{
	    if (this.Attach.table != null)
	    {
	        this.Attach.table.Reposition();
            return;
	    }

        if (this.Attach.grid != null)
        {
            this.Attach.grid.Reposition();
            return;
        }
			
	}
	/// <summary>
	/// スクロールリセット
	/// </summary>
	public void ScrollReset()
	{
		if( this.Attach.scrollView != null ) 
			this.Attach.scrollView.ResetPosition();
	}
	/// <summary>
	/// CenterOnChildの設定
	/// </summary>
	public void CenterOnEnabled(bool isEnabled)
	{
		if( this.Attach.center != null )
			this.Attach.center.enabled = isEnabled;
	}
	/// <summary>
	/// アイテム初期化
	/// </summary>
	public void Clear()
	{
		this.Clear(this.ItemMax);
	}
	/// <summary>
	/// アイテム初期化
	/// </summary>
	public void Clear(int count)
	{
		for (int i = 0; i < count; i++)
		{
			T t = this.GetItem(i);

			// アイテム初期化
			this.ClearValue(t);
		}
	}
	/// <summary>
	/// アイテムのアクティブ化
	/// </summary>
	void SetItemActive()
	{
		this.SetItemActive(this.NowPageItemMax);
	}
	/// <summary>
	/// アイテムのアクティブ化
	/// </summary>
	void SetItemActive(int count)
	{
		for (int i = 0; i < this.ItemMax; i++)
		{
			T t = this.GetItem(i);

			// アイテムのアクティブ設定
			bool isActive = (i < count);
			t.gameObject.SetActive(isActive);
		}
	}
	#endregion

	#region 取得
	/// <summary>
	/// アイテム取得
	/// </summary>
	public T GetItem(int itemIndex)
	{
		try
		{
			if (0 <= itemIndex && itemIndex < this.ItemList.Count)
			{
			T item = this.ItemList[itemIndex];
			return item;
		}
			else
			{
				return null;
			}
		}
#if XW_DEBUG
		catch (System.Exception e)
		{
			Debug.LogWarning(string.Format("List.Count={0} Index={1}\r\n{2}", this.ItemList.Count, itemIndex, e));
			return null;
		}
#else
		catch (System.Exception)
		{
			return null;
		}
#endif
	}
	/// <summary>
	/// インデックスを変換する
	/// </summary>
	public bool ConverIndex(int indexInTotal, out int pageIndex, out int itemIndex)
	{
		pageIndex = 0;
		itemIndex = 0;
		// 範囲内
		if (this.IsValid(indexInTotal))
		{
			// アイテムが存在する
			if (this.ItemMax > 0)
			{
				// 総アイテム数から
				// ページインデックスとアイテムインデックスを求める
				pageIndex = (int)(indexInTotal / this.ItemMax);
				itemIndex = (int)(indexInTotal % this.ItemMax);
				return true;
			}
		}
		return false;
	}
	/// <summary>
	/// 指定したインデックスが現在のページかどうか
	/// </summary>
	public bool IsNowPage(int indexInTotal)
	{
		if (this.ItemMax == 0)
			return false;
		// 現在のページ内のインデックスかどうか
		int pageIndex = (int)(indexInTotal / this.ItemMax);
		if (pageIndex != this.PageIndex)
			return false;
		return true;
	}
	/// <summary>
	/// 指定したインデックスが有効な値かどうか
	/// </summary>
	public bool IsValid(int indexInTotal)
	{
		return (0 <= indexInTotal && indexInTotal < this.Total);
	}
	/// <summary>
	/// 全アイテム中のインデックスからアイテムインデックスに変換する
	/// </summary>
	public int GetItemIndex(int indexInTotal)
	{
		// アイテム
		if (this.ItemMax == 0)
			return -1;
		int itemIndex = (int)(indexInTotal % this.ItemMax);
		return itemIndex;
	}
	/// <summary>
	/// 現在のページにいるアイテムインデックスから全アイテム中のインデックスに変換する
	/// </summary>
	public int GetTotalIndex(int itemIndex)
	{
		return itemIndex + (this.PageIndex * this.ItemMax);
	}
	/// <summary>
	/// アイテムが表示されているかどうか
	/// </summary>
	public bool IsVisible(T item)
	{
		bool isVisible = false;
		UIWidget[] widgets = item.transform.GetComponentsInChildren<UIWidget>();
		foreach (var w in widgets)
		{
			if (!this.Attach.scrollView.panel.IsVisible(w))
				continue;
			isVisible = true;
			break;
		}
		return isVisible;
	}
	#endregion
}
