
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

public enum FeatureType
{
    None,
    Motor,
    Color,
    Float,
    Hue_Bulb,
    Debug
}

[System.Serializable]
public class FixtureFeature
{
    public String Name;
    public int Channel;
    public FeatureType Type;
    public GameObject SourceObject;
    public int Index;
    public float DebugValue;
    public Vector4 MotorRange = new Vector4();
    public bool IsActive = true;
    public bool DebugOutput = false;
    public float FloatMult = 1f;

    // Variables for Motor type
    private Transform sourceTransform;

    // Variables for Color type
    private Material colorMat;

    // Variables for Float type
    private Transform sourceScaleReference;

    // Variables for Hue_Bulb type
    private Hue_LightColor hueLightColor;

    private bool hasInit = false;

    public void Init()
    {
        if (DebugOutput) Debug.Log("Initializing " + Name + " " + Type.ToString() + " " + SourceObject.name);

        if (Type == FeatureType.Motor)
        {
            sourceTransform = SourceObject.transform;
        }
        else if (Type == FeatureType.Color)
        {
            hueLightColor = SourceObject.GetComponent<Hue_LightColor>();
        }
        else if (Type == FeatureType.Float)
        {
            sourceScaleReference = SourceObject.transform;
        }
        else if (Type == FeatureType.Hue_Bulb)
        {
            hueLightColor = SourceObject.GetComponent<Hue_LightColor>();
        }

        hasInit = true;
    }

    public void Update()
    {
        /*if (!hasInit)
        {
            Init();
        }*/
    }

    public string GetData(int startChannel)
    {
        string result = (Channel + startChannel - 1).ToString();

        if (Type == FeatureType.Motor)
        {
            float compensatedRot = sourceTransform.localEulerAngles[Index];
            if (compensatedRot < 0f)
            {
                compensatedRot = 360f + compensatedRot;
            }
            if (DebugOutput)
            {
                Debug.Log(sourceTransform.localEulerAngles[Index].ToString() + " becomes " + compensatedRot.Remap(MotorRange[0], MotorRange[1], MotorRange[3], MotorRange[2]).ToString());
            }
            result += ",motor," + Mathf.Clamp( compensatedRot.Remap(MotorRange[0], MotorRange[1], MotorRange[3], MotorRange[2]), 0f, 255f ).ToString();
        }
        else if (Type == FeatureType.Color)
        {
            result += ",color," + (hueLightColor.LightColor.r * 255).ToString() + "," + (hueLightColor.LightColor.g * 255).ToString() + "," + (hueLightColor.LightColor.b * 255).ToString();
        }
        else if (Type == FeatureType.Float)
        {
            result += ",float," + Mathf.Clamp(sourceScaleReference.localScale[0] * 255 * FloatMult, 0f, 255f).ToString();
        }
        else if (Type == FeatureType.Hue_Bulb)
        {
            result += ",hueBulb," + (hueLightColor.LightColor.r * 255).ToString() + "," + (hueLightColor.LightColor.g * 255).ToString() + "," + (hueLightColor.LightColor.b * 255).ToString() + "," + (Mathf.Clamp( hueLightColor.LightColor.a * 254, 1f, 254f)).ToString() + "," + Name;
        }
        else if (Type == FeatureType.Debug)
        {
            result += ",motor," + DebugValue.ToString();
        }

        if (DebugOutput) Debug.Log("Sending: " + result);

        return result;
    }
}

[ExecuteAlways]
public class UDP_DMXFixture : MonoBehaviour
{
    public List<FixtureFeature> Features = new List<FixtureFeature>();

    public string IP = "127.0.0.1";
    public int Port = 8051;

    public int StartChannel = 1;

    public float SendRate = 1f;

    protected IPEndPoint remoteEndPoint;
    protected UdpClient client;

    public bool PlayInEditor = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (FixtureFeature f in Features)
        {
            f.Init();
        }

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
        client = new UdpClient();

        InvokeRepeating("SendAllData", UnityEngine.Random.Range(0f, 3f), SendRate);
    }

    private void SendAllData()
    {
        if (!Application.IsPlaying(gameObject) && !PlayInEditor)
        {
            return;
        }

        foreach (FixtureFeature f in Features)
        {
            if (f.IsActive)
            {
                SendData(f);                
            }
        }
    }

    public void SendData(FixtureFeature feature)
    {
        //byte[] data = Encoding.UTF8.GetBytes(SourceObject.rotation.eulerAngles.y.ToString());
        string compiledString = StartChannel.ToString() + "," + feature.GetData(StartChannel);
        byte[] data = Encoding.UTF8.GetBytes(compiledString);
        client.Send(data, data.Length, remoteEndPoint);
    }
}
