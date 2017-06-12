using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManage : SingletonMonoBehaviour<ResourcesManage>
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public  GameObject Load(string name)
    {
        GameObject rd = Resources.Load(name) as GameObject;
        return rd;
    }
}
