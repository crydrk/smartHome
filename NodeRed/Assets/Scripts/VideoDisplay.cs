using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoDisplay : MonoBehaviour
{
    public string Name = "";
    public GameObject VidObject;
    public VideoPlayer VidPlayer;

    public float AudioTarget = 1;

    private void Update()
    {
        VidPlayer.SetDirectAudioVolume(0, Mathf.Lerp( VidPlayer.GetDirectAudioVolume(0), AudioTarget, Time.deltaTime * 2f) );
    }

    private void Awake()
    {
        AudioTarget = 1f;
    }

    public void StopVideo()
    {
        AudioTarget = 0f;
        StartCoroutine("StopAfterTime");
    }

    private IEnumerator StopAfterTime()
    {
        yield return new WaitForSeconds(2f);
        VidObject.SetActive(false);
    }
}