using UnityEngine;
using System;



//------------------------------------------------------------------------------------
// サーバー通信用APIベース
//------------------------------------------------------------------------------------
public abstract class SCAPI_Base {
	public ServerClient sc;
	public long last_access_at = 0;
	public virtual void Open(){}
	public virtual bool Done(string res){return false;}
	public virtual ServerClient.SEQ Next(){return ServerClient.SEQ.IDLE;}
	public Action OnSuccess = null;
	public Action OnError   = null;

	/// <summary>OnSuccessを一度だけ実行します</summary>
	public void OnSuccessOneShot ()
	{
		OnSuccess();
		OnSuccess = null;
		OnError   = null;
	}

	/// <summary>OnErrorを一度だけ実行します</summary>
	public void OnErrorOneShot ()
	{
		OnError();
		OnError   = null;
		OnSuccess = null;
	}

	//------------------------------------------------------------------------------------
	// 時間更新
	//------------------------------------------------------------------------------------
	protected void UpdateTime(long st){
		sc.UpdateTime(st);
		last_access_at = st;
	}

	//------------------------------------------------------------------------------------
	// Token更新
	//------------------------------------------------------------------------------------
	protected void UpdateToken(string token){
		sc.UpdateToken(token);
	}

	//------------------------------------------------------------------------------------
	// リザルトコード
	//------------------------------------------------------------------------------------
	protected void ResultCode(int result){
		sc.ErrorCode = result;
	}

	//------------------------------------------------------------------------------------
	// マーケットID
	//------------------------------------------------------------------------------------
	public int MarketID(){
#if UNITY_IPHONE
		return 1;
#else
		return 2;
#endif
	}

	//------------------------------------------------------------------------------------
	// デバイスID
	//------------------------------------------------------------------------------------
	public int DeviceID(){
#if UNITY_IPHONE
		return 1;
#else
		return 2;
#endif
	}

	//------------------------------------------------------------------------------------
	// 言語ID
	//------------------------------------------------------------------------------------
	public string LanguageID(){
		string lang = "JA";
/*
		switch(Application.systemLanguage){
		case SystemLanguage.Japanese:
			lang = "JA";
			break;
		case SystemLanguage.Korean:
			lang = "KO";
			break;
		}
*/
		return lang;
	}

	public string DeviceUUID(){
		return SystemInfo.deviceUniqueIdentifier;
	}

	public string PushRegistrationID(){
		return "0000000000000000000000000000000000000000000000000000000000000000";
	}
}
