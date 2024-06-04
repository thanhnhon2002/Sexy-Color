using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogOnHandDown : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log(transform.name);
    }
}
