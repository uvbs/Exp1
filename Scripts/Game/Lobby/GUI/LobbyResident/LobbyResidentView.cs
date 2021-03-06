/// <summary>
/// 常駐メニュー表示
/// 
/// 2016/05/27
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Packet;
using XDATA;

namespace XUI.LobbyResident
{
    #region 定義
    public class TeamMemberEventArgs : EventArgs
    {
        public GUITeamMemberItem Member { get; set; }
    }
    #endregion 定義

    /// <summary>
    /// 常駐メニュー表示インターフェイス
    /// </summary>
    public interface IView
    {
        #region アクティブ
        void RankMatchStart();

        /// <summary>
        /// アクティブ状態にする
        /// </summary>
        void SetActive(bool isActive, bool isTweenSkip);
        /// <summary>
        /// 現在のアクティブ状態を取得する
        /// </summary>
        GUIViewBase.ActiveState GetActiveState();
        #endregion アクティブ

        #region ロビーメニューボタン
        event EventHandler<EventArgs> OnLobbyMenu;
        #endregion ロビーメニューボタン

        #region マッチングボタン
        event EventHandler<EventArgs> OnQuickMatching;

        /// <summary>
        /// Rank matching
        /// </summary>
        event EventHandler<EventArgs> OnRankMatching;

        /// <summary>
        /// マッチングボタンの有効化
        /// </summary>
        void SetMatchingButtonEnable(bool isEnable);
        /// <summary>
        /// マッチングアニメーション再生
        /// </summary>
        void MatchingPlay(bool forward);
        #endregion マッチングボタン

        #region ロビー選択ボタン
        event EventHandler<EventArgs> OnLobbySelect;

        /// <summary>
        /// ロビー番号設定
        /// </summary>
        void SetLobbyNo(int lobbyNo, string format);
        /// <summary>
        /// ロビー選択ボタンの有効化
        /// </summary>
        void SetLobbySelectButtonEnable(bool isEnable);
        #endregion ロビー選択ボタン

        #region ロビーメンバーボタン
        event EventHandler<EventArgs> OnLobbyMember;

        /// <summary>
        /// ロビーメンバー設定
        /// </summary>
        void SetLobbyMember(int count, int capacity, string format);
        #endregion ロビーメンバー

        #region ショップメニューボタン
        event EventHandler<EventArgs> OnShopMenu;

        /// <summary>
        /// ショップメニューボタンの有効化
        /// </summary>
        void SetShopMenuButtonEnable(bool isEnable);
        #endregion ショップメニューボタン

        #region 練習ボタン
        event EventHandler<EventArgs> OnTraining;

        /// <summary>
        /// 練習ボタンの有効化
        /// </summary>
        void SetTrainingButtonEnable(bool isEnable);
        #endregion 練習ボタン

        #region 通知系
        #region アチーブメント
        event EventHandler<EventArgs> OnAchievement;

        /// <summary>
        /// 未取得アチーブメント数設定
        /// </summary>
        void SetAchieveUnreceived(int count, string format);
        /// <summary>
        /// 未取得アチーブメント数の有効化
        /// </summary>
        void SetAchieveUnreceivedEnable(bool isEnable);
        #endregion アチーブメント

        #region メール
        event EventHandler<EventArgs> OnMail;

        /// <summary>
        /// 未読メール数設定
        /// </summary>
        void SetMailUnread(int count, string format);
        /// <summary>
        /// 未読メール数の有効化
        /// </summary>
        void SetMailUnreadEnable(bool isEnable);
        #endregion メール

        #region 申請
        event EventHandler<EventArgs> OnApply;

        /// <summary>
        /// 未処理申請数設定
        /// </summary>
        void SetApplyUnprocessed(int count, string format);
        /// <summary>
        /// 未処理申請の有効化
        /// </summary>
        void SetApplyUnprocessedEnable(bool isEnable);
        #endregion 申請
        #endregion 通知系

        #region プレイヤー情報
        /// <summary>
        /// プレイヤー名設定
        /// </summary>
        void SetPlayerName(string text);
        /// <summary>
        /// プレイヤーのリーダーアイコンの有効化
        /// </summary>
        void SetPlayerLeaderIconEnable(bool isEnable);
        /// <summary>
        /// プレイヤー勝利数設定
        /// </summary>
        void SetPlayerWin(int num, string format);
        /// <summary>
        /// プレイヤー敗北数設定
        /// </summary>
        void SetPlayerLose(int num, string format);
        #endregion プレイヤー情報

        #region チーム情報
        event EventHandler<EventArgs> OnTeamMenu;
        event EventHandler<EventArgs> OnTeamMemberPlayer;
        event EventHandler<TeamMemberEventArgs> OnTeamMember;

        /// <summary>
        /// チームメンバーを開く
        /// </summary>
        void TeamMemberOpen(string teamName, List<GroupMemberParameter> memberParams, int playerID);
        /// <summary>
        /// チームメンバーを閉じる
        /// </summary>
        void TeamMemberClose();
        /// <summary>
        /// チームメンバーリセット
        /// </summary>
        void TeamMemberReset();
        /// <summary>
        /// チーム再生
        /// </summary>
        void TeamPlay(bool forward);
        /// <summary>
        /// シングルボタン更新
        /// </summary>
        void UpdateSingleButtonEnable();
        #endregion チーム情報

        #region ヘルプボタン
        event EventHandler<EventArgs> OnHelp;
        #endregion ロビーメニューボタン

        #region 審査会用
        event EventHandler<EventArgs> OnDummyCharaGacha;
        #endregion 審査会用
    }

    /// <summary>
    /// 常駐メニュー表示
    /// </summary>
    public class LobbyResidentView : GUIViewBase, IView
    {
        #region 破棄
        /// <summary>
        /// 破棄
        /// </summary>
        void OnDestroy()
        {
            this.OnLobbyMenu = null;
            this.OnQuickMatching = null;
            this.OnLobbySelect = null;
            this.OnLobbyMember = null;
            this.OnShopMenu = null;
            this.OnTraining = null;
            this.OnAchievement = null;
            this.OnMail = null;
            this.OnApply = null;
            this.OnTeamMenu = null;
            this.OnTeamMemberPlayer = null;
            this.OnTeamMember = null;
            this.OnHelp = null;
            this.OnDummyCharaGacha = null;

            PlayerData.Instance.OnGoldChange -= this.SetGold;
            PlayerData.Instance.OnCoinChange -= this.SetCoin;
        }
        #endregion 破棄

        #region アクティブ
        /// <summary>
        /// アクティブ状態にする
        /// </summary>
        public void SetActive(bool isActive, bool isTweenSkip)
        {
            this.SetRootActive(isActive, isTweenSkip);
        }
        /// <summary>
        /// 現在のアクティブ状態を取得する
        /// </summary>
        public GUIViewBase.ActiveState GetActiveState()
        {
            return this.GetRootActiveState();
        }
        #endregion アクティブ

        #region ロビーメニューボタン
        public event EventHandler<EventArgs> OnLobbyMenu = (sender, e) => { };

        /// <summary>
        /// ロビーメニューボタンイベント
        /// </summary>
        public void OnLobbyMenuEvent()
        {
            // 通知
            this.OnLobbyMenu(this, EventArgs.Empty);
        }
        #endregion ロビーメニューボタン

        #region マッチングボタン
        public event EventHandler<EventArgs> OnQuickMatching = (sender, e) => { };

        public event EventHandler<EventArgs> OnRankMatching = (sender, e) => { };

        /// <summary>
        /// マッチングボタンイベント
        /// </summary>
        public void OnMatchingEvent()
        {
            SoundController.PlaySe(SoundController.SeID.LockOn);
//            if (InTeamMatch())
//            {
//                return;
//            }
            //Set Matching flag
            GUIMatchingState.Instance.IsQuick = true;
            // 通知
            this.OnQuickMatching(this, EventArgs.Empty);
        }

        [SerializeField]
        UIButton _matchingButton = null;
        UIButton MatchingButton { get { return _matchingButton; } }
        [SerializeField]
        UIPlayTween _matchingTween = null;
        UIPlayTween MatchingTween { get { return _matchingTween; } }

        /// <summary>
        /// マッチングボタンの有効化
        /// </summary>
        public void SetMatchingButtonEnable(bool isEnable)
        {
            if (this.MatchingButton != null)
            {
                this.MatchingButton.gameObject.SetActive(isEnable);
            }
        }
        /// <summary>
        /// マッチングアニメーション再生
        /// </summary>
        public void MatchingPlay(bool forward)
        {
            if (this.MatchingTween != null)
            {
                this.MatchingTween.Play(forward);
            }
        }
        #endregion マッチングボタン

        #region ロビー選択ボタン
        public event EventHandler<EventArgs> OnLobbySelect = (sender, e) => { };

        /// <summary>
        /// ロビー選択ボタンイベント
        /// </summary>
        public void OnLobbySelectEvent()
        {
            // 通知
            this.OnLobbySelect(this, EventArgs.Empty);
        }

        [SerializeField]
        UILabel _lobbyNoLabel = null;
        UILabel LobbyNoLabel { get { return _lobbyNoLabel; } }
        [SerializeField]
        UIButton _lobbySelectButton = null;
        UIButton LobbySelectButton { get { return _lobbySelectButton; } }

        /// <summary>
        /// ロビー番号設定
        /// </summary>
        public void SetLobbyNo(int lobbyNo, string format)
        {
            if (this.LobbyNoLabel != null)
            {
                this.LobbyNoLabel.text = string.Format(format, lobbyNo);
            }
        }
        /// <summary>
        /// ロビー選択ボタンの有効化
        /// </summary>
        public void SetLobbySelectButtonEnable(bool isEnable)
        {
            if (this.LobbySelectButton != null)
            {
                this.LobbySelectButton.isEnabled = isEnable;
            }
        }
        #endregion ロビー選択ボタン

        #region ロビーメンバーボタン
        public event EventHandler<EventArgs> OnLobbyMember = (sender, e) => { };

        /// <summary>
        /// ロビーメンバーボタンイベント
        /// </summary>
        public void OnLobbyMemberEvent()
        {
            // 通知
            this.OnLobbyMember(this, EventArgs.Empty);
        }

        [SerializeField]
        UILabel _lobbyMemberLabel = null;
        UILabel LobbyMemberLabel { get { return _lobbyMemberLabel; } }

        /// <summary>
        /// ロビーメンバー設定
        /// </summary>
        public void SetLobbyMember(int count, int capacity, string format)
        {
            if (this.LobbyMemberLabel != null)
            {
                this.LobbyMemberLabel.text = string.Format(format, count, capacity);
            }
        }
        #endregion ロビーメンバー

        #region ショップボタン
        public event EventHandler<EventArgs> OnShopMenu = (sender, e) => { };

        [SerializeField]
        UIButton _shopMenuButton = null;
        UIButton ShopMenuButton { get { return _shopMenuButton; } }

        /// <summary>
        /// ショップメニューボタンイベント
        /// </summary>
        public void OnShopMenuEvent()
        {
            // 通知
            //			this.OnShopMenu( this, EventArgs.Empty );
            this.OnShop();
        }
        /// <summary>
        /// ショップメニューボタンの有効化
        /// </summary>
        public void SetShopMenuButtonEnable(bool isEnable)
        {
            if (this.ShopMenuButton != null)
            {
                this.ShopMenuButton.isEnabled = isEnable;
            }
        }
        #endregion ショップボタン

        #region 練習ボタン
        public event EventHandler<EventArgs> OnTraining = (sender, e) => { };

        /// <summary>
        /// 練習ボタンイベント
        /// </summary>
        public void OnTrainingEvent()
        {
            // 通知
            this.OnTraining(this, EventArgs.Empty);
        }

        [SerializeField]
        UIButton _trainingButton = null;
        UIButton TrainingButton { get { return _trainingButton; } }

        /// <summary>
        /// 練習ボタンの有効化
        /// </summary>
        public void SetTrainingButtonEnable(bool isEnable)
        {
            if (this.TrainingButton != null)
            {
                this.TrainingButton.isEnabled = isEnable;
            }
        }
        #endregion 練習ボタン

        #region 通知系
        #region 定義
        [System.Serializable]
        public class AlertAttach
        {
            [SerializeField]
            UILabel _countLabel = null;
            UILabel CountLabel { get { return _countLabel; } }
            [SerializeField]
            GameObject _countGroup = null;
            GameObject CountGroup { get { return _countGroup; } }

            public void SetCount(int count, string format)
            {
                if (this.CountLabel != null) { this.CountLabel.text = string.Format(format, count); }
            }
            public void SetCountEnable(bool isEnable)
            {
                if (this.CountGroup != null) { this.CountGroup.SetActive(isEnable); }
            }
        }
        #endregion 定義

        #region アチーブメントボタン
        public event EventHandler<EventArgs> OnAchievement = (sender, e) => { };

        /// <summary>
        /// アチーブメントボタンイベント
        /// </summary>
        public void OnAchievementEvent()
        {
            // 通知
            this.OnAchievement(this, EventArgs.Empty);
        }

        [SerializeField]
        AlertAttach _achieveUnreceived = new AlertAttach();
        AlertAttach AchieveUnreceived { get { return _achieveUnreceived; } }

        /// <summary>
        /// 未取得アチーブメント数設定
        /// </summary>
        public void SetAchieveUnreceived(int count, string format) { this.AchieveUnreceived.SetCount(count, format); }
        /// <summary>
        /// 未取得アチーブメント数の有効化
        /// </summary>
        public void SetAchieveUnreceivedEnable(bool isEnable) { this.AchieveUnreceived.SetCountEnable(isEnable); }
        #endregion アチーブメントボタン

        #region メールボタン
        public event EventHandler<EventArgs> OnMail = (sender, e) => { };

        /// <summary>
        /// メールボタンイベント
        /// </summary>
        public void OnMailEvent()
        {
            // 通知
            this.OnMail(this, EventArgs.Empty);
        }

        [SerializeField]
        AlertAttach _mailUnread = new AlertAttach();
        AlertAttach MailUnread { get { return _mailUnread; } }

        /// <summary>
        /// 未読メール数設定
        /// </summary>
        public void SetMailUnread(int count, string format) { this.MailUnread.SetCount(count, format); }
        /// <summary>
        /// 未読メール数の有効化
        /// </summary>
        public void SetMailUnreadEnable(bool isEnable) { this.MailUnread.SetCountEnable(isEnable); }
        #endregion メールボタン

        #region 申請ボタン
        public event EventHandler<EventArgs> OnApply = (sender, e) => { };

        /// <summary>
        /// 申請ボタンイベント
        /// </summary>
        public void OnApplyEvent()
        {
            // 通知
            this.OnApply(this, EventArgs.Empty);
        }

        [SerializeField]
        AlertAttach _applyUnprocessed = new AlertAttach();
        AlertAttach ApplyUnprocessed { get { return _applyUnprocessed; } }

        /// <summary>
        /// 未処理申請数設定
        /// </summary>
        public void SetApplyUnprocessed(int count, string format) { this.ApplyUnprocessed.SetCount(count, format); }
        /// <summary>
        /// 未処理申請の有効化
        /// </summary>
        public void SetApplyUnprocessedEnable(bool isEnable) { this.ApplyUnprocessed.SetCountEnable(isEnable); }
        #endregion 申請ボタン
        #endregion 通知系

        #region プレイヤー情報
        [System.Serializable]
        public class PlayerParam
        {
            [SerializeField]
            UILabel _nameLabel = null;
            UILabel NameLabel { get { return _nameLabel; } }
            [SerializeField]
            UISprite _leaderIcon = null;
            UISprite LeaderIcon { get { return _leaderIcon; } }
            [SerializeField]
            UILabel _winLabel = null;
            UILabel WinLabel { get { return _winLabel; } }
            [SerializeField]
            UILabel _loseLabel = null;
            UILabel LoseLabel { get { return _loseLabel; } }

            public void SetName(string text)
            {
                if (this.NameLabel != null) { this.NameLabel.text = text; }
            }
            public void SetLeaderIconEnable(bool isEnable)
            {
                if (this.LeaderIcon != null) { this.LeaderIcon.enabled = isEnable; }
            }
            public void SetWin(int num, string format)
            {
                if (this.WinLabel != null) { this.WinLabel.text = string.Format(format, num); }
            }
            public void SetLose(int num, string format)
            {
                if (this.LoseLabel != null) { this.LoseLabel.text = string.Format(format, num); }
            }

            public void SetLeaderIcon(UIAtlas pAtlas, string pSpriteName)
            {
                LeaderIcon.atlas = pAtlas;
                LeaderIcon.spriteName = pSpriteName;
            }
        }

        [SerializeField]
        PlayerParam _player = new PlayerParam();
        PlayerParam Player { get { return _player; } }

        /// <summary>
        /// プレイヤー名設定
        /// </summary>
        public void SetPlayerName(string text) { this.Player.SetName(text); }
        /// <summary>
        /// プレイヤーのリーダーアイコンの有効化
        /// </summary>
        public void SetPlayerLeaderIconEnable(bool isEnable) { this.Player.SetLeaderIconEnable(isEnable); }
        /// <summary>
        /// プレイヤー勝利数設定
        /// </summary>
        public void SetPlayerWin(int num, string format) { this.Player.SetWin(num, format); }
        /// <summary>
        /// プレイヤー敗北数設定
        /// </summary>
        public void SetPlayerLose(int num, string format) { this.Player.SetLose(num, format); }
        #endregion プレイヤー情報

        #region チーム情報
        public event EventHandler<EventArgs> OnTeamMenu = (sender, e) => { };
        public event EventHandler<EventArgs> OnTeamMemberPlayer = (sender, e) => { };
        public event EventHandler<TeamMemberEventArgs> OnTeamMember = (sender, e) => { };

        public LobbyResidentView.PlayerParam GetPalyer()
        {
            return this.Player;
        }
        /// <summary>
        /// チームメニューボタンイベント
        /// </summary>
        public void OnTeamMenuEvent()
        {
            // 通知
            this.OnTeamMenu(this, EventArgs.Empty);
        }
        /// <summary>
        /// プレイヤーのチームメンバーボタンイベント
        /// </summary>
        public void OnTeamMemberPlayerEvent()
        {
            // 通知
            this.OnTeamMemberPlayer(this, EventArgs.Empty);
        }
        /// <summary>
        /// チームメンバーボタンイベント
        /// </summary>
        public void OnTeamMemberEvent(GUITeamMemberItem member)
        {
            // 通知
            var eventArgs = new TeamMemberEventArgs();
            eventArgs.Member = member;
            this.OnTeamMember(this, eventArgs);
        }

        [System.Serializable]
        public class TeamParam
        {
            [SerializeField]
            UIButton _teamButton = null;
            UIButton TeamButton { get { return _teamButton; } }
            [SerializeField]
            UIButton _singleButton = null;
            UIButton SingleButton { get { return _singleButton; } }
            [SerializeField]
            UILabel _nameLabel = null;
            UILabel NameLabel { get { return _nameLabel; } }

            [SerializeField]
            UIPlayTween _playTween = null;
            UIPlayTween PlayTween { get { return _playTween; } }
            [SerializeField]
            GUITeamMemberItem[] _memberItems = null;
            GUITeamMemberItem[] MemberItems { get { return _memberItems; } }

            public void MemberOpen(string teamName, List<GroupMemberParameter> memberParams, int playerID)
            {
                if (this.TeamButton != null) this.TeamButton.gameObject.SetActive(true);
                if (this.NameLabel != null) this.NameLabel.text = teamName;
                if (this.SingleButton != null) this.SingleButton.gameObject.SetActive(false);

                {
                    int i = 0;
                    foreach (var member in this.MemberItems)
                    {
                        while (i < memberParams.Count &&
                            memberParams[i].PlayerId == playerID)
                        {
                            ++i;
                        }

                        member.gameObject.SetActive(true);
                        if (i < memberParams.Count)
                        {
                            member.SetMemberParam(memberParams[i], i == 0);
                        }
                        else
                        {
                            member.SetMemberParam(null, false);
                        }
                        ++i;
                    }
                    this.Play(true);
                }
            }
            public void MemberClose()
            {
                this.Play(false);

                if (this.SingleButton != null) this.SingleButton.gameObject.SetActive(true);
                this.UpdateSingleButtonEnable();
                if (this.TeamButton != null) this.TeamButton.gameObject.SetActive(false);

//                foreach (var member in this.MemberItems)
//                {
//                    member.gameObject.SetActive(false);
//                }
            }
            public void MemberReset()
            {
                this.MemberClose();
            }
            // GameObject.SetActive() したフレームで UIPlayTween.Play() を行うと位置がずれる問題の対策.
            public void Play(bool forward)
            {
                FiberController.AddFiber(this.PlayCoroutine(forward));
            }
            IEnumerator PlayCoroutine(bool forward)
            {
                yield return null;
                if (this.PlayTween != null) this.PlayTween.Play(forward);
            }
            public void UpdateSingleButtonEnable()
            {
                // マッチング中はグレーアウトする.
                if (this.SingleButton != null) this.SingleButton.isEnabled = !GUIMatchingState.IsMatching;
#if OLD_TEAM_LOGIC
                GUITeamMenu.UpdateSingleButtonEnable();
#endif
            }
        }

        [SerializeField]
        TeamParam _team = new TeamParam();
        TeamParam Team { get { return _team; } }

        public List<GameObject> hideOnReview;
        public List<GameObject> moveOnReview;
        public GameObject ZuDuiContainer;

        void Start()
        {
            /*#if PLATE_NUMBER_REVIEW
                        foreach (var item in hideOnReview)
                        {
                            item.SetActive(false);
                        }

                        if (moveOnReview.Count > 1) moveOnReview[0].transform.localPosition = moveOnReview[1].transform.localPosition;
            #endif*/
        }

        /// <summary>
        /// チームメンバーを開く
        /// </summary>
        public void TeamMemberOpen(string teamName, List<GroupMemberParameter> memberParams, int playerID)
        {
            this.Team.MemberOpen(teamName, memberParams, playerID);
        }
        /// <summary>
        /// チームメンバーを閉じる
        /// </summary>
        public void TeamMemberClose() { this.Team.MemberClose(); }
        /// <summary>
        /// チームメンバーリセット
        /// </summary>
        public void TeamMemberReset() { this.Team.MemberReset(); }
        /// <summary>
        /// チーム再生
        /// </summary>
        public void TeamPlay(bool forward) { this.Team.Play(forward); }
        /// <summary>
        /// シングルボタン更新
        /// </summary>
        public void UpdateSingleButtonEnable() { this.Team.UpdateSingleButtonEnable(); }
        #endregion チーム情報

        #region ヘルプボタン
        public event EventHandler<EventArgs> OnHelp = (sender, e) => { };

        /// <summary>
        /// ヘルプボタンイベント
        /// </summary>
        public void OnHelpEvent()
        {
            // 通知
            this.OnHelp(this, EventArgs.Empty);
        }
        #endregion ロビーメニューボタン

        #region 審査会用
        public event EventHandler<EventArgs> OnDummyCharaGacha = (sender, e) => { };

        /// <summary>
        /// 審査会用イベント
        /// </summary>
        public void OnDummyCharaGachaEvent()
        {
            // 通知
            this.OnDummyCharaGacha(this, EventArgs.Empty);
        }
        #endregion 審査会用

        public Lobby View;
        [System.Serializable]
        public class Lobby
        {
            public UILabel gold;
            public UILabel coupon;
        }

        void Awake()
        {
            Debug.Log("-----------RegisterGoldEvent!");
            PlayerData.Instance.OnGoldChange += this.SetGold;
            PlayerData.Instance.OnCoinChange += this.SetCoin;
            this.SetGold(this, EventArgs.Empty);
            this.SetCoin(this, EventArgs.Empty);
        }

        private void SetGold(object sender, EventArgs args)
        {
            this.View.gold.text = PlayerData.Instance.Gold + "";
        }

        private void SetCoin(object sender, EventArgs args)
        {
            this.View.coupon.text = PlayerData.Instance.Coin + "";
        }

        public void OnUser()
        {
            Debug.Log("===> OnUser");
            SoundController.PlaySe(SoundController.SeID.Select);
            XUI.UserInfo.UserInfoController.Instance.Show();
        }

        public void OnChara()
        {
            Debug.Log("===> OnChara");
            SoundController.PlaySe(SoundController.SeID.Select);
            GUICharacters.Instance.Open();
        }

        public void OnDeckEdit()
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            DeckEdit.Instance.Open();
        }

        public void OnMoshi()
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            Debug.Log("===> OnMoshi");
        }

        public void OnZudui()
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            GUITeamMatch.Instance.ShowTeamMatchInfo(!GUITeamMatch.Instance.gameObject.activeSelf);
        }

        public void OnRankMatch()
        {
            SoundController.PlaySe(SoundController.SeID.LockOn);
            //Set Matching flag
            GUIMatchingState.Instance.IsQuick = false;
            //Open Rank panel
            GUIRankMatch.Instance.Open();
        }

        public void RankMatchStart()
        {
            SoundController.PlaySe(SoundController.SeID.LockOn);
            // 通知
            this.OnRankMatching(this, EventArgs.Empty);
            Debug.Log("rank matching...");
        }

        public void OnDungeon()
        {
            SoundController.PlaySe(SoundController.SeID.LockOn);
            GUIMatchingState.Instance.IsQuick = false;
            DungeonController.Instance.Open();
        }

        public void OnVip()
        {
            Debug.Log("===> OnVip");
        }

        public void OnSign()
        {
            Debug.Log("===> OnSign");
            SoundController.PlaySe(SoundController.SeID.LockOn);
            XUI.GUILoginAward.Instance.Open();
        }

        public void OnTaskDaily()
        {
            SoundController.PlaySe(SoundController.SeID.LockOn);
            XUI.GUITaskDaily.Instance.Open();
        }

        public void OnActivity()
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            Debug.Log("===> OnActivity");
        }

        public void OnFriends()
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            //XUI.Friends.FriendsController.Instance.Show();
            XUI.GUIFriends.Instance.Open();
            Debug.Log("===> OnFriends");
        }

        public void OnEmail()
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            Debug.Log("===> OnEmail");
        }

        public void OnSet()
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            Debug.Log("===> OnSet");
        }

        public void OnChat()
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            XUI.GUIChatFrameController.Instance.Show();
        }

        public void OnShop()
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            XUI.GUIShop.Instance.OpenToXD();
        }

        public void OnBuyJB()
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            //LWZ: use new shop
            GUIShop.Instance.OpenToDQ();
        }

        public void OnBuyDq()
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            GUIShop.Instance.OpenToCZ();
        }

        public void OnParty() 
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            GUIRecruitment.Instance.OnRecruitmentClick();
        }

        public void OnGift()
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            GUIGift.Instance.Open();
        }
        
    }
}
