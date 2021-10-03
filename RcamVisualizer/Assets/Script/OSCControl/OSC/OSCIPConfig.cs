using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCIPConfig : MonoBehaviour
{
  public string OutboundIP = "127.0.0.1";
  public int OutboundPort = 5555;
    public void Awake()
    {
        if (PlayerPrefs.GetString("LastScene") == "Welcome")
        {
            OutboundIP = PlayerPrefs.GetString("IPAddress");            
            PlayerPrefs.SetString("LastScene", "null");
        }
    }
}
