using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rcam2;
using Script.OSCControl;
using UnityEngine;
using UnityOSC;

public class OSCRcamInput : MonoBehaviour
{
    public static InputState inputState;
    
    public OSCReciever oscReceiver = new OSCReciever();

    public int receivePort = 8888;
    
    // Start is called before the first frame update
    void Start()
    {
        oscReceiver.Open(receivePort);
    }

    // Update is called once per frame
    void Update()
    {
        
        while (oscReceiver.hasWaitingMessages())
        {
            var msg = oscReceiver.getNextMessage();
            if (msg != null)
            {
                Debug.Log("Message from OSC : " + msg.Address);
                HandleMessage(msg);
            }
        }
      
    }

    private void HandleMessage(OSCMessage m)
    {
        float val = 0.0f;
        try
        {
            val = (float)m.Data[0];
        }
        catch (Exception e)
        {
            Debug.LogError("error parsing argument");
            Debug.LogError(m.Data);
        }
        
        var bits = m.Address.Split('/');
        if (bits[1] == "rcam")
        {
            int controlId;
            switch (bits[2])
            {
                case "button":
                    if (!int.TryParse(bits[3], out controlId))
                    {
                        Debug.LogError("ERROR could not find control number");
                    }
                    else
                    {
                        inputState.SetButtonData(controlId, (int)val);
                        Debug.Log($"Setting button {controlId} to {val}");
                    }
                    break;
                case "toggle":
                    if (!int.TryParse(bits[3], out controlId))
                    {
                        Debug.LogError("ERROR could not find control number");
                    }
                    else
                    {
                        var currentToggle = inputState.GetToggleData(controlId) > 0;
                        var newToggle = val > 0.0f;

                        if (newToggle)
                        {
                            inputState.SetToggleData(controlId, currentToggle ? 0 : 1);
                            Debug.Log($"Setting toggle {controlId} to {!currentToggle}");
                        }
                        
                    }
                    break;
                case "knob":
                    if (!int.TryParse(bits[3], out controlId))
                    {
                        Debug.LogError("ERROR could not find control number");
                    }
                    else
                    {
                        inputState.SetKnobData(controlId, val);
                        Debug.Log($"Setting knob {controlId} to {val}");
                    }
                    break;
            }
            
        }
    }

    private void OnDestroy()
    {
        oscReceiver.Close();
    }
}
