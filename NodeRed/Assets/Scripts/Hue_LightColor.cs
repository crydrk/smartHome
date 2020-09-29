using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hue_LightColor : MonoBehaviour
{
    public Color LightColor;
    public float Size = 1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = LightColor;
        Gizmos.DrawSphere(transform.position, Size);
    }
}
