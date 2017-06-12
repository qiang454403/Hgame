using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

using Mobcast.Sdk.Util;

namespace Mobcast.Sdk.Manager
{
	/// <summary>
	/// リソース管理(AssetBundle,Resourcese).
	/// そのまま使用しても良いですが、ラッパークラスを作成しての使用を推奨します.
	/// </summary>
	public class ResourceManager : UnitySingleton<ResourceManager> {
		/// <summary>
		/// AssetBundleの拡張子.
		/// </summary>
		public string ASSETBUNDLE_EXTENSION = ".unity3d";
		/// <summary>
		/// AssetBundleの保存先.
		/// </summary>
		public string ASSETBUNDLE_SAVE_PATH = Application.persistentDataPath;

		/// <summary>
		/// 読み込みの状態.
		/// </summary>
		public enum LoadState
		{
			NONE = 0,	// 無し.
			START,	// 読み込み開始.
			WAIT,	// 読み込み中.
			END,	// 読み込み終了.
			ERROR,	// 読み込みエラー.
		};

		/// <summary>
		/// Object情報.
		/// </summary>
		class ObjectData
		{
			// Objectデータ.
			public Object data;
			// 読み込みパス.
			public string path;
			// 読み込みファイル名.
			public string fileName;
			// 読み込み状態.
			public LoadState state;
			// 読み込み用コルーチン.
			public IEnumerator coroutine;
			// データ優先度.
			public int priority = 0;
			// 読み込み終了イベント登録用.
			public CallbackLoadEndEvent loadEndEvent;
		}

		// ObjectDataリスト.
		List<ObjectData> objDataList = new List<ObjectData>();

		/// <summary>
		/// AssetBundle情報.
		/// </summary>
		class AssetBundleData
		{
			// AssetBundleデータ.
			public AssetBundle data;
			public string path;
			public string fileName;
			public LoadState state;
			// 読み込み用コルーチン.
			public IEnumerator coroutine;
			// データ優先度.
			public int priority = 0;
			// 読み込み終了イベント登録用.
			public CallbackLoadEndEvent loadEndEvent;
		}

		// AssetBundleDataリスト.
		List<AssetBundleData> abDataList = new List<AssetBundleData>();

		public delegate void CallbackLoadEndEvent();

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		#region ResourceData
		/// <summary>
		/// 指定データの読み込み.
		/// Resources,AssetBundle両方読み込めます
		/// </summary>
		/// <param name="path">読み込むデータのPath(Resources用)</param>
		/// <param name="fileName">読み込むファイル名</param>
		/// <param name="assetBundleName">AssetBundleから読み込む場合は、AssetBundle名</param>
		/// <param name="type">オブジェクトタイプ</param>
		/// <param name="priority">常駐優先度0~99</param>
		/// <param name="loadEndEvent">読み込み終了イベント登録</param>
		public void LoadData(string path, string fileName, string assetBundleName = null, System.Type type = null, int priority = 0, CallbackLoadEndEvent loadEndEvent = null)
		{
			// 以前に読み込んでいるか？.
			var cacheObj = objDataList.Find(a => fileName.Equals(a.fileName));
			if (cacheObj != null)
			{
				if (loadEndEvent != null)
				{
					// ロード状態によって処理分け.
					switch (cacheObj.state)
					{
						case LoadState.END:
						case LoadState.ERROR:
							loadEndEvent();
							break;
						default:
							cacheObj.loadEndEvent += loadEndEvent;
							break;	
					}
				}
			}
			else
			{
				ObjectData tmp = new ObjectData();
				tmp.path = path;
				tmp.fileName = fileName;
				tmp.state = LoadState.START;
				tmp.data = null;
				tmp.priority = priority;
				tmp.loadEndEvent = loadEndEvent;
				tmp.coroutine = LoadAsync(tmp, assetBundleName);
				objDataList.Add(tmp);
				StartCoroutine(tmp.coroutine);
			}
		}

		/// 非同期読み込み.
		/// </summary>
		/// <param name="objData">ObjectData</param>
		/// <param name="assetBundleName">AssetBundleから読み込む場合は、AssetBundle名</param>
		IEnumerator LoadAsync(ObjectData objData, string assetBundleName = null)
		{
#if UNITY_EDITOR && UNUSE_ASSETBUNDLE

			objData.data = GetAsset<Object>(objData.fileName);

			if (objData.data == null)
			{
				Debug.LogError ("LoadAsync : LoadError : " + objData.fileName);
				objData.state = LoadState.ERROR;
			}
			else
			{
				Debug.Log ("LoadAsync : Load : " + objData.fileName);
				objData.state = LoadState.END;
			}

			if(objData.loadEndEvent != null)
			{
				objData.loadEndEvent();
			}
			yield break;
#else
			bool isAssetBundleLoad = false;
			AssetBundleData assetBundleData = null;
			if(assetBundleName != null)
			{
				if(assetBundleName.Length > 0)
				{
					// 以前に読み込みしているか？.
					var abData = abDataList.Find(a => assetBundleName.Equals(a.fileName));
					if (abData != null)
					{
						isAssetBundleLoad = true;
						assetBundleData = abData;
					}
				}
			}
			else
			{
				// 読み込んでいるAssetBundleにあるか？.
				var abData = abDataList.Find(a => a.data.Contains(objData.fileName));
				if (abData != null) {
					isAssetBundleLoad = true;
					assetBundleData = abData;
				}
			}

			if (isAssetBundleLoad) {
				yield return new WaitWhile(() => assetBundleData.state == LoadState.WAIT || assetBundleData.state == LoadState.START);

				if(assetBundleData.state == LoadState.ERROR){
					objData.state = LoadState.ERROR;
					Debug.LogError("LoadAsync : AssetBundle Load Error " + objData.fileName);
				}
				else {
					var result = assetBundleData.data.LoadAssetAsync(objData.fileName);
					yield return new WaitUntil(() => result.isDone);

					objData.data = result.asset;
					if (objData.data == null) {
						Debug.LogError ("LoadAsync : LoadError : " + objData.fileName);
						objData.state = LoadState.ERROR;
					} else {
						objData.state = LoadState.END;
					}
				}
			}
			else {
				objData.state = LoadState.WAIT;

				string tmp = objData.fileName;
				if(tmp.LastIndexOf(".") != -1){
					tmp = tmp.Substring(0, tmp.LastIndexOf("."));
				}

				var result = Resources.LoadAsync(objData.path + tmp);
				yield return new WaitUntil(() => result.isDone);

				objData.data = result.asset;
				if (objData.data == null) {
					Debug.LogError ("LoadAsync : LoadError : " + objData.fileName);
					objData.state = LoadState.ERROR;
				} else {
					Debug.Log ("LoadAsync : Load : " + objData.fileName);
					objData.state = LoadState.END;
				}
			}
			if(objData.loadEndEvent != null)
			{
				objData.loadEndEvent();
			}
#endif
		}

		/// <summary>
		/// 指定の読み込み状態の取得.
		/// </summary>
		/// <param name="fileName">string</param>
		/// <returns>LoadState</returns>
		public LoadState GetLoadState(string fileName)
		{
			foreach (ObjectData objData in objDataList)
			{
				if (fileName.Equals(objData.fileName))
				{
					return objData.state;
				}
			}

			return LoadState.NONE;
		}

		/// <summary>
		/// 指定のデータを取得.
		/// </summary>
		/// <returns>Object.</returns>
		/// <param name="fileName">ファイル名</param>
		/// <typeparam name="T">指定したType.</typeparam>
		public T GetObject<T>(string fileName) where T: Object
		{
			foreach (ObjectData objData in objDataList)
			{
				if (fileName.Equals(objData.fileName))
				{
					if (objData.state == LoadState.END)
					{
						return objData.data as T;
					}
					break;
				}
			}

			return null;
		}

		/// <summary>
		/// 指定の破棄.
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		public void Release(string fileName){
			foreach (ObjectData objData in objDataList)
			{
				if (fileName.Equals(objData.fileName))
				{
					StopCoroutine(objData.coroutine);

					if(objData.data != null){
						Resources.UnloadAsset(objData.data);
#if UNITY_EDITOR && UNUSE_ASSETBUNDLE
#else
						Object.Destroy(objData.data);
#endif
						objData.data = null;
					}

					objDataList.Remove(objData);

					break;
				}
			}
		}

		/// <summary>
		/// 指定の優先度以下の破棄(節約解放:完全な解放では無い).
		/// </summary>
		/// <param name="priority">int</param>
		public void ReleasePriority(int priority){
			// TODO: 処理の時間がかかる場合は、objDataListをDictionaryとすることも考える.
			List<string> keyList = new List<string>();
			foreach (ObjectData objData in objDataList)
			{
				if(objData.priority <= priority) {
					keyList.Add(objData.fileName);
				}
			}
			foreach(string key in keyList) {
				ObjectData objData = objDataList.Find(c => c.fileName.Equals(key));
				StopCoroutine(objData.coroutine);

				if(objData.data != null){
					Resources.UnloadAsset(objData.data);
#if UNITY_EDITOR && UNUSE_ASSETBUNDLE
#else
					Object.Destroy(objData.data);
#endif
					objData.data = null;
				}

				objDataList.Remove(objData);
			}

			Resources.UnloadUnusedAssets();
		}

		/// <summary>
		/// 全破棄.
		/// </summary>
		public void ReleaseAll()
		{
			foreach (ObjectData objData in objDataList)
			{
				StopCoroutine(objData.coroutine);

				if(objData.data != null){
					Resources.UnloadAsset(objData.data);
#if UNITY_EDITOR && UNUSE_ASSETBUNDLE
#else
					Object.Destroy(objData.data);
#endif
					objData.data = null;
				}

				objData.data = null;
			}

			objDataList.Clear();

			Resources.UnloadUnusedAssets();
		}

		/// <summary>
		/// AssetBundleファイルの存在確認.
		/// </summary>
		/// <returns><c>true</c>, AssetBundleが有れば, <c>false</c> 無ければ.</returns>
		/// <param name="fileName">File name.</param>
		public bool ExistsAssetBundleData(string fileName)
		{
#if UNITY_EDITOR && UNUSE_ASSETBUNDLE
			return true;
#else
			return File.Exists (ASSETBUNDLE_SAVE_PATH + "/" + fileName + ASSETBUNDLE_EXTENSION);
#endif
		}

		#endregion

		#region AssetBundleData

		/// <summary>
		/// 指定AssetBundleの読み込み.
		/// </summary>
		/// <param name="path">読み込むデータのPath</param>
		/// <param name="fileName">読み込むファイル名</param>
		/// <param name="priority">常駐優先度0~99</param>
		/// <param name="loadEndEvent">読み込み終了イベント登録.</param>
		public void LoadAssetBundleData(string path, string fileName, int priority = 0, CallbackLoadEndEvent loadEndEvent = null)
		{
#if UNITY_EDITOR && UNUSE_ASSETBUNDLE
			// アセットバンドル使わない.
#else

			// 以前に読み込みしているか？
			var abData = abDataList.Find(a => fileName.Equals(a.fileName));
			if (abData != null)
			{
				if (loadEndEvent != null)
				{
					// ロード状態によって処理分け.
					switch (abData.state)
					{
						case LoadState.END:
						case LoadState.ERROR:
							loadEndEvent();
							break;
						default:
							abData.loadEndEvent += loadEndEvent;
							break;	
					}
				}
			}
			else
			{
				AssetBundleData tmp = new AssetBundleData();
				tmp.path = path;
				tmp.fileName = fileName;
				tmp.state = LoadState.START;
				tmp.data = null;
				tmp.priority = priority;
				tmp.loadEndEvent = loadEndEvent;
				tmp.coroutine = LoadAssetBundle(tmp, path, fileName);
				abDataList.Add(tmp);
				StartCoroutine(tmp.coroutine);
			}
#endif
		}

		/// <summary>
		/// 指定パスからの非同期読み込み.
		/// </summary>
		/// <param name="abData">AssetBundleData</param>
		/// <param name="path">string</param>
		/// <param name="filename">string</param>
		IEnumerator LoadAssetBundle(AssetBundleData abData, string path, string fileName)
		{
			var result = AssetBundle.LoadFromFileAsync(ASSETBUNDLE_SAVE_PATH + "/" + path + fileName.ToLower() + ASSETBUNDLE_EXTENSION);

			abData.state = LoadState.WAIT;
			yield return new WaitUntil(() => result.isDone);

			if (result.assetBundle == null)
			{
				Debug.LogError ("LoadAssetBundle Error");
				Debug.LogError (ASSETBUNDLE_SAVE_PATH + "/" + path + fileName.ToLower() + ASSETBUNDLE_EXTENSION);
				abData.state = LoadState.ERROR;
				abData.data = null;
				if (abData.loadEndEvent != null)
				{
					abData.loadEndEvent();
				}
			}
			else
			{
				Debug.Log ("LoadAssetBundle : Load : " + fileName);
				abData.data = result.assetBundle;
				abData.state = LoadState.END;
				if (abData.loadEndEvent != null)
				{
					abData.loadEndEvent();
				}
			}
		}

		/// <summary>
		/// 指定のAssetBundleの読み込み状態の取得.
		/// </summary>
		/// <param name="fileName">string</param>
		/// <returns>LoadState</returns>
		public LoadState GetLoadAssetBundleState(string fileName)
		{
			foreach (AssetBundleData abData in abDataList)
			{
				if (fileName.Equals(abData.fileName))
				{
					return abData.state;
				}
			}

			return LoadState.NONE;
		}

		/// <summary>
		/// 指定のAssetBundle破棄(節約解放:完全な解放では無い).
		/// </summary>
		/// <param name="fileName">string</param>
		public void ReleaseAssetBundle(string fileName){
#if UNITY_EDITOR && UNUSE_ASSETBUNDLE
			// アセットバンドル使わない.
#else
			foreach (AssetBundleData abData in abDataList)
			{
				if (fileName.Equals(abData.fileName))
				{
					StopCoroutine(abData.coroutine);

					if (abData.data != null)
					{
						abData.data.Unload (false);
					}

					abDataList.Remove(abData);

					break;
				}
			}
			#endif
		}

		/// <summary>
		/// 指定の優先度以下のAssetBundle破棄(節約解放:完全な解放では無い).
		/// </summary>
		/// <param name="priority">常駐優先度0~99</param>
		public void ReleasePriorityAssetBundle(int priority){
#if UNITY_EDITOR && UNUSE_ASSETBUNDLE
			// アセットバンドル使わない.
#else
			List<string> keyList = new List<string>();
			foreach (AssetBundleData abData in abDataList)
			{
				if(abData.priority <= priority) {
					keyList.Add(abData.fileName);
				}
			}
			foreach(string key in keyList) {
				AssetBundleData abData = abDataList.Find(c => c.fileName.Equals(key));
				StopCoroutine(abData.coroutine);

				if (abData.data != null)
				{
					abData.data.Unload (false);
				}

				abDataList.Remove(abData);
			}
			#endif
		}

		/// <summary>
		/// 全AssetBundle破棄.
		/// </summary>
		public void ReleaseAllAssetBundle()
		{
#if UNITY_EDITOR && UNUSE_ASSETBUNDLE
			// アセットバンドル使わない.
#else
			foreach (AssetBundleData abData in abDataList)
			{
				StopCoroutine(abData.coroutine);

				if (abData.data != null)
				{
					abData.data.Unload (true);
				}
			}

			abDataList.Clear();
			#endif
		}

		#endregion


#if UNITY_EDITOR && UNUSE_ASSETBUNDLE
		/// <summary>
		/// ファイル名にマッチするアセットを、プロジェクト内から取得します.
		/// </summary>
		/// <param name="fileName">ファイル名.</param>
		/// <param name="ext">ファイル拡張子.デフォルトでは無視します. png, xml などを指定してください.</param>
		public static T GetAsset<T>(string fileName, string ext = "") where T:Object
		{
			bool noExt = string.IsNullOrEmpty(ext);					//拡張子を指定しているか.
			fileName = fileName.ToLower();							//ファイル名を小文字に.
			ext = noExt || ext.StartsWith(".") ? ext : "." + ext;	//拡張子を補正.先頭にドットをつける.
			
			//フィルタ、guid-パス変換、ファイル名一致チェックを経て、目的のパスを取得.
			string path = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(fileName) + " t:" + typeof(T).Name)
				.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
				.FirstOrDefault(x =>Path.GetFileNameWithoutExtension(x).ToLower() == fileName && (noExt || Path.GetExtension(x) == ext));

			//プロジェクト内から見つからなかったらエラー.
			if (string.IsNullOrEmpty(path))
			{
				Debug.LogError("ResourceManager: GetAsset is failed. Asset is not found in project!.",fileName,ext);
				return null;
			}
			return AssetDatabase.LoadAssetAtPath<T>(path);
		}
#endif
	}
}