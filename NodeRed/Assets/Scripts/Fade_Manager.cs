using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade_Manager : MonoBehaviour
{
    public Color TargetColor = Color.black;
    public float fadeSpeed = 5f;
    private Material mat;

    public void SetTargetColor(Color newColor)
    {
        TargetColor = newColor;
    }

    private void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        mat.color = Color.Lerp(mat.color, TargetColor, Time.deltaTime * fadeSpeed);
    }
}
