/// <summary>
/// チュートリアル リザルト
/// 
/// 2014/06/17
/// </summary>
using UnityEngine;
using System.Collections;

public class TutorialResult : TutorialScript
{
    #region Fields & Properties


    #endregion

    #region MonoBehaviour
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    #endregion

    #region TutorialScript

    protected override IEnumerator loadResources()
    {
        yield return StartCoroutine(base.loadResources());
    }

    protected override void unloadResources()
    {
        base.unloadResources();
    }

    protected override IEnumerator setup()
    {
        yield return StartCoroutine(base.setup());
    }

    protected override void startTutorial()
    {
        base.startTutorial();
    }

    protected override void stopTutorial()
    {
        base.stopTutorial();
    }

    protected override IEnumerator script()
    {
		GUIResultOld resultUI;
		resultUI = GUIResultOld.Instance;
		if(resultUI == null)
		{
			Debug.LogError("GUIResult is not Exsits");
		}

        yield return new WaitSeconds(2.0f);

		setupMessageWindow();
        openNextWindow();
        setGuidePlate();

        setMessage("バトルが終わると\r\nリザルトが表示されるんだ\r\nここで戦績を確認できるよ");

        yield return new WaitResultState(resultUI, GUIResultOld.StateType.Team);

        setMessage("こっちでは、戦闘に参加した\r\n全員のスコアを見れるんだ");

        yield return new WaitResultState(resultUI, GUIResultOld.StateType.Rewards);

        setMessage("そして、次はお待ちかねの報酬だよ！");
        
        yield return new WaitNext();

        setMessage("ギルベルトのキューブを\r\nゲットしたね！\r\nおめでとう！");

        yield return new WaitNext();
        
        setMessage("それじゃ、ロビーに戻ろうか");
        
        yield return new WaitNext();
        
        closeWindow();
		resetMessageWindow();

        yield break;
    }

    #endregion

    #region Privete Methods
    #endregion
}

public class WaitResultState : IFiberWait
{
    private GUIResultOld resultUI;
	private GUIResultOld.StateType state;

    public WaitResultState(GUIResultOld resultUI, GUIResultOld.StateType state)
    {
        this.resultUI = resultUI;
        this.state = state;
    }

    #region IFiberWait

    public bool IsWait
    {
        get
        {
            return this.resultUI.State != this.state;
        }
    }

    #endregion
}
