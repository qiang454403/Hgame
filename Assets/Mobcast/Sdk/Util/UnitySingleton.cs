using UnityEngine;
using System.Collections;

namespace Mobcast.Sdk.Util
{
	/// <summary>
	/// UnitySingleton用.
	/// </summary>
	public class UnitySingleton<T>: MonoBehaviour where T : MonoBehaviour {
		
		private static T _instance = null;
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					string objName = typeof(T).ToString();
					GameObject obj = GameObject.Find(objName);
					if (obj == null)
					{
						obj = GameObject.Find(objName + "(Clone)");
					}

					if (obj == null)
					{
						obj = new GameObject();
						obj.name = objName;
					}

					_instance = obj.GetComponent<T>();
					if (_instance == null)
					{
						_instance = obj.AddComponent<T>();
					}

					// 自身のGameObjectを解放させないようにする.
					DontDestroyOnLoad(obj);
				}

				return _instance;
			}
		}

		/// <summary>
		/// アプリケーション終了時.
		/// </summary>
		public void OnApplicationQuit ()
		{
			_instance = null;
		}
	}
}