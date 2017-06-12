using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerClient : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    public int ErrorCode = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public enum SEQ
    {
        IDLE,
        WAIT,
        ERROR,
        RETRY,

        MAINTENANCE,
        VERSION_CHECK,

        CONFIG,
        START,
        SESSION,
        TOKEN,
        MQ,

        ITEM_LIST,
        SHOP_ITEM_LIST,
        LOGIN_BONUS_LIST,

        LOGIN,
        USER_PROFILE,
        USER_SUMMARY_INFOMATION,
        USER_NAME,
        USER_RANK,
        USER_TRANSFER_PASSWORD,
        USER_TRANSFER,

        AP_UPDATE,
        PLAYER_INFO,

        GAME_DATA_AUTO_SKILL,
        GAME_DATA_DIVER,
        GAME_DATA_DIVER_EXP_REINFORCE_TABLE,
        GAME_DATA_DIVER_EXP_TABLE,
        GAME_DATA_DIVER_SKILL_RATE_REINFORCE_TABLE,
        GAME_DATA_GODDESS,
        GAME_DATA_GODDESS_SKILL,
        GAME_DATA_SKILL,
        GAME_DATA_STAGE,
        GAME_DATA_STAGE_REWARD,
        GAME_DATA_GRADE_SEASON,
        GAME_DATA_OVER_DIVE_SKILL,

        DIVER_LIST,

        PARTY_LIST,
        PARTY_LIST_SEARCH,
        PARTY_MEMBER_UPDATE,
        PARTY_ALL_UPDATE,

        DIVER_SELL,
        DIVER_ADD,
        DIVER_HISTORY,

        DIVER_REINFORCE,
        DVVER_REINFORCE_PARAMETER,
        DIVER_EVOLVE,
        DIVER_EVOLVE_PARAMETER,
        DIVER_PARAMETER_RESET,

        FACEBOOK_LOGIN,
        FACEBOOK_LOGOUT,

        COOPERATOR_LIST,

        GODDESS_LIST,
        GODDESS_SET,
        GODDESS_PIECE,


        FRIEND_LIST,
        FRIEND_DELETE,
        FRIEND_REQUEST,
        FRIEND_ACCEPT,
        FRIEND_DECLINE,
        /// <summary>フレンド一括承認・拒否</summary>
        FRIEND_BULK_ACCEPT_DECLINE,
        FRIEND_SEARCH,
        FRIEND_INVITECODE_SEARCH,

        // 招待コードを使用しない友達招待用ZeroAPI
        FRIEND_INVITE_URL,
        FRIEND_INVITE_LOCATION_URL,
        FRIEND_INVITE_RESULT_NOTIFICATION,

        STAGE_INFO,
        STAGE_INFO_FRIEND,
        STAGE_INFO_RANKING,

        RANKING_LIST,
        RANKING,

        PRESENTBOX_LIST,
        PRESENTBOX_ACQUIRE,
        PRESENTBOX_ACQUIRE_ITEMALL,
        PRESENTBOX_ACQUIRE_ALL,

        TUTORIAL_UPDATE,

        GAME_START,
        GAME_START_WEEK,
        GAME_START_TERM,
        GAME_START_RANKING,
        /// <summary>サブストーリークエスト開始</summary>
        GAME_START_SUBSTORY,
        GAME_RESULT,
        GAME_RESULT_WEEK,
        GAME_RESULT_TERM,
        GAME_RESULT_RANKING,
        /// <summary>サブストーリークエスト終了</summary>
        GAME_RESULT_SUBSTORY,

        MOBCAST_BANK_INIT,  //課金初期化
        MOBCAST_BANK_CHECK, //課金チェック
        MOBCAST_BANK_INFO,  //課金情報

        SHOP_AP,
        SHOP_DIVER_WAKU,
        SHOP_CONTINUE,
        SHOP_GEM,
        SHOP_ITEM,
        SHOP_SKILL_CHARGE,
        SHOP_SKILL_CHARGE_ALL,

        GACHA,
        GACHA_LIST,

        USER_ITEM_LIST,
        ITEM_USE,
        EVENT_LIST,
        EVENT_REGIST,

        INVITEMASTAER_LIST,
        INVITECOUNT_LIST,
        INVITEREGIST,

        BANNER_LIST,

        EVENT_STAGELEVEL_LIST_TERM,


        ANALYSTICS,

        ELEMENT_LEADER_GET,
        ELEMENT_LEADER_UPDATE,
        ELEMENT_LEADER_SEARCH,

        SPECIAL_RANKING_LIST,
        SPECIAL_RANKING_DETAIL,
        SPECIAL_RANKING_REWARD,
        SPECIAL_PAST_RANKING_LIST,
        /// <summary>グループランキング.</summary>
        SPECIAL_RANKING_GROUP_DETAIL,

        #region shin (08/18) - New API : HighScore (D3-007)
        SPECIAL_RANKING_HIGHSCORE,
        #endregion

        #region shin (01/20) - New API : Exchange shop (F1-001, F1-002)
        EXCHANGE_ITEM_LIST,
        EXCHANGE_ITEM_PURCHASE,
        #endregion

        SPECIAL_RANKING_SCORE_LOG,

        CURRENCY_HISTORY_CHECK,
        CURRENCY_HISTORY_UPDATE,

        /// <summary>プレイスメントのプリロード.</summary>
        PRELOAD_PLACEMENT,

        /// <summary>mobcastID連携.</summary>
        MOBCAST_CONNECT_URL,
        MOBCAST_CONNECT_REGISTER,
        MOBCAST_CONNECT_TRANSFER,

        /// <summary>サブストーリーのマスタ取得.</summary>
        SUBSTORY_MASTER_GET,
        /// <summary>サブストーリーの難易度取得.</summary>
        SUBSTORY_GRADE_LIST,

        /// <summary>スペシャルトップ画面の情報取得.</summary>
        SPECIAL_TOP_ENTRANCE,

        /// <summary>ノーマルミッションリストを取得.</summary>
        MISSION_LIST_NORMAL,
        /// <summary>期間限定ミッションリストを取得.</summary>
        MISSION_LIST_TERM,
        /// <summary>ノーマルミッションの報酬を取得.</summary>
        MISSION_ACIRE_NORMAL,
        /// <summary>期間限定ミッションの報酬を取得.</summary>
        MISSION_ACIRE_TERM,
        /// <summary>ブラウズ系ミッション達成報告.</summary>
        MISSION_POST,
        /// <summary>期間イベントマスタ取得.</summary>
        SPECIAL_TERM_MASTER_GET,
        /// <summary>曜日イベントマスタ取得.</summary>
        SPECIAL_WEEK_MASTER_GET,
        SPECIAL_WEEK_LEVEL_GET,

        /// <summary>曜日イベントマスタ取得.</summary>
        SPECIALSTAGE_RANKING_REWARD,
        SPECIALSTAGE_POINT_REWARD,
        SPECIALSTAGE_RANKING_DETAIL,
        SPECIALSTAGE_RANKING_SEARCH,
        CHALLENGE_RANKING_DETAIL,
        CHALLENGE_RANKING_SEARCH,
        CHALLENGE_RANKING_REWARD,

        PROFIE_SEARCH,  // Profile Search by ID

        SET_CUSTOM_SKILL,	// Set Custom OverDive Skill

#if CN_DEV
		ACTIVITY_LIST,
		ACTIVITY_AWARD,
        LOADDATA_LOCAL,
        DIVER_LOCK,
        SOCIAL_SHARE,
        GET_BANNER,
#if PP_CHECK
		GACHACOUNT
#endif
#endif
    };

    SEQ mSeq = SEQ.IDLE;

    public void UpdateToken(string token)
    {
    }
    //時間更新
    //------------------------------------------------------------------------------------
    public void UpdateTime(long st)
    { 
    }
}
