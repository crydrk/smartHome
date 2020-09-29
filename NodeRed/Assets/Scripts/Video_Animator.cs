using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[ExecuteInEditMode]
public class Video_Animator : MonoBehaviour
{
    public VideoPlayer VPlayer;

    public float Frame;

    private void Awake()
    {
        VPlayer.Prepare();
    }

    void Update()
    {
        VPlayer.frame = Convert.ToInt64(Frame);
    }
}
