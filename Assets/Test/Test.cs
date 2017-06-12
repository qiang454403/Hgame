using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class Test : MonoBehaviour {

    public Player Data = new Player();
    JsonWriter da = new JsonWriter();
    // Use this for initialization
    void Start() {
        da.WriteObjectStart();
        da.WritePropertyName("NAME");
        da.Write("gzq");
        da.WritePropertyName("AGE");
        da.Write(12);
        da.WriteObjectEnd();

    }

    // Update is called once per frame
    void Update() {

    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 100), "ss"))
        {
            string res = da.ToString();
         
            Data = LitJson.JsonMapper.ToObject<Player>(res);
            Debug.Log(Data.NAME);
            Debug.Log(Data.AGE);
            Debug.Log(res);
            
            
        }
    }
}
public class Player
{
    public string NAME;
    public int AGE = 0; 
}
