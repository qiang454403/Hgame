using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;

public class BaseUI : MonoBehaviour {

    protected GameObject _Root;
   
    public virtual void Init()
    {
        _Root = gameObject;
    }
    public virtual void Open()
    {
        _Root.SetActive(true);
    }
    public virtual void Close()
    {
        _Root.SetActive(false);
    }
    public virtual void Destroy()
    {
        GameObject.Destroy(_Root);
        _Root = null;
    }

    public override bool Equals(object other)
    {
        Type ot = other.GetType();
        if (ot.Name == this.name)
        {
            return true;
        }
        else
            return false;
    }

}
