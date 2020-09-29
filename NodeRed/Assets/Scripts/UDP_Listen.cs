using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using UnityEngine.Video;



public class UDP_Listen : MonoBehaviour
{
    
    Thread receiveThread;
    UdpClient client;

    public string IP = "127.0.0.1";
    public int port = 8052;
    
    public string lastReceivedUDPPacket = "";

    public Animator ShowAnimator;

    public List<VideoDisplay> Videos = new List<VideoDisplay>();

    string animToPlay = "";
    bool shouldPlayAnim = false;
    public bool isExistingVideoPlaying = false;
    private VideoDisplay currentVideoDisplay;

    public Fade_Manager FadeObj;

    // start from shell
    private static void Main()
    {
        UDP_Listen receiveObj = new UDP_Listen();
        receiveObj.init();

        string text = "";
        do
        {
            text = Console.ReadLine();
        }
        while (!text.Equals("exit"));
    }
    
    public void Start()
    {
        init();
    }

    public void Update()
    {
        if (shouldPlayAnim)
        {
            if (animToPlay == "Stop")
            {
                ShowAnimator.SetTrigger("Stop");
                StopCurrentVideoDisplay();
                FadeObj.SetTargetColor(new Color (0f, 0f, 0f, 1f));
                //ShowAnimator.gameObject.SetActive(false);
            }
            else
            {
                //ShowAnimator.gameObject.SetActive(true);
                ShowAnimator.Play(animToPlay);
                CheckIfVideoShouldPlay(animToPlay);
                StartCoroutine("FadeOutAndIn");
            }
            shouldPlayAnim = false;
        }
    }

    private IEnumerator FadeOutAndIn()
    {
        FadeObj.SetTargetColor(Color.black);
        yield return new WaitForSeconds(2f);
        FadeObj.SetTargetColor(new Color(0f, 0f, 0f, 0f));
    }

    VideoDisplay CheckIfVideoShouldPlay(string name)
    {
        foreach (VideoDisplay i in Videos)
        {
            if (i.Name == name)
            {
                StopCurrentVideoDisplay();
                currentVideoDisplay = i;
                i.VidObject.SetActive(true);
                i.AudioTarget = 1f;
                isExistingVideoPlaying = true;
                return i;
            }
        }

        return null;
    }

    private void StopCurrentVideoDisplay()
    {
        
        // If one is playing, stop it
        if (isExistingVideoPlaying)
        {
            currentVideoDisplay.StopVideo();
            isExistingVideoPlaying = false;
        }
    }

    /*
    void OnGUI()
    {
        Rect rectObj = new Rect(40, 10, 200, 400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj, "# UDPReceive\n127.0.0.1 " + port + " #\n"
                    + "shell> nc -u 127.0.0.1 : " + port + " \n"
                    + "\nLast Packet: \n" + lastReceivedUDPPacket
                    + "\n\nAll Messages: \n" + allReceivedUDPPackets
                , style);
    }
    */

    // init
    private void init()
    {
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // receive thread
    private void ReceiveData()
    {

        client = new UdpClient(port);
        while (true)
        {

            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                
                string text = Encoding.UTF8.GetString(data);

                Debug.Log(text);

                animToPlay = text;
                shouldPlayAnim = true;
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
    
}