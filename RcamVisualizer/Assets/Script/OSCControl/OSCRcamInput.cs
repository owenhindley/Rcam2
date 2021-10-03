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

    public bool[] BoolButtonStates = new bool[16];
    public bool[] BoolToggleStates = new bool[16];
    public float[] FloatKnobStates = new float[32];
    
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
                //Debug.Log("Message from OSC : " + msg.Address);
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

        bool stateDidChange = false;
        
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
                        BoolButtonStates[controlId] = val > 0;
                        Debug.Log($"Setting button {controlId} to {(val > 0)}");
                        stateDidChange = true;
                    }
                    break;
                case "toggle":
                    if (!int.TryParse(bits[3], out controlId))
                    {
                        Debug.LogError("ERROR could not find control number");
                    }
                    else
                    {
                        var currentToggle = BoolToggleStates[controlId];
                        var newToggle = val > 0.0f;

                        if (newToggle) BoolToggleStates[controlId] = !BoolToggleStates[controlId];
                        Debug.Log($"Setting toggle {controlId} to {!currentToggle}");
                        stateDidChange = true;
                    }
                    break;
                case "knob":
                    if (!int.TryParse(bits[3], out controlId))
                    {
                        Debug.LogError("ERROR could not find control number");
                    }
                    else
                    {
                        FloatKnobStates[controlId] = val;
                        stateDidChange = true;
                        Debug.Log($"Setting knob {controlId} to {val}");
                    }
                    break;
            }

            if (stateDidChange)
            {
                // this is weeeeird
                for (var i = 0; i < 2; i++)
                {
                    var bdata = 0;
                    var tdata = 0;

                    for (var bit = 0; bit < 8; bit++)
                    {
                        if (BoolButtonStates[bit]) bdata += 1 << bit;
                        if (BoolToggleStates[bit]) tdata += 1 << bit;
                    }

                    inputState.SetButtonData(i, bdata);
                    inputState.SetToggleData(i, tdata);
                }
                
                for (var i = 0; i < 32; i++)
                    inputState.SetKnobData(i, (int)(FloatKnobStates[i] * 255));
            }
            
        }
    }

    private void OnDestroy()
    {
        oscReceiver.Close();
    }
}
