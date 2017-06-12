using UnityEngine;
using System.Collections;

public class SingletonMonoBehaviour<T>: MonoBehaviour where T : MonoBehaviour {
	private static T instance = null;
	public static T Instance {
		get {
			if (instance == null) {
				instance = (T)FindObjectOfType (typeof(T));
 
				if (instance == null) {
					//Debug.LogError (typeof(T) + " is nothing");
				}
			}
			return instance;
		}
	}

	/// <summary>
	/// コンポーネントの破棄コールバック.
	/// インスタンスが破棄された時にコールされます.
	/// </summary>
	protected virtual void OnDestroy ()
	{
		//登録を解除します.
		if (instance == this)
			instance = null;
	}
}
