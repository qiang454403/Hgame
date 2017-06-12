using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : SingletonMonoBehaviour<UiManager> {


    [SerializeField]
    private GameObject _LowRoot;
    [SerializeField]
    private GameObject _MidRoot;
    [SerializeField]
    private GameObject _HigRoot;

    public Dictionary<string,BaseUI> _All_UI = new Dictionary<string,BaseUI>();
	void Start ()
    {
      
	}
	
	
	void Update () {
		
	}

    private void InitRoot()
    {
       
    }
    public void Add_Low(string face)
    {
        if (_All_UI.ContainsKey(face))
        {
            Debug.LogError("已经出现了");
            return;
        }
        else
        {
           
            GameObject d = ResourcesManage.Instance.Load("Prefab/" + face);
            GameObject dt = GameObject.Instantiate(d);
            dt.transform.SetParent(_LowRoot.transform,false);
            _All_UI.Add("Login", dt.GetComponent<BaseUI>());
        }
    }
}
