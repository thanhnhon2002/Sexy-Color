using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDebugConsole : MonoBehaviour
{
    public GameObject debugConsole;
    public int clickCount;
    public float timeSpace;
    public float timeDown;
    public bool canCheck;
    private void Update()
    {
        if (debugConsole.activeInHierarchy) return;
        timeSpace += Time.deltaTime;
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;  
            if(timeSpace>1f)
            {
                clickCount = 0;
                timeSpace = 0;
                timeDown = 0;
            }
            timeSpace = 0;
            if(clickCount>10)
            {
                canCheck = true;
            }
        }     
        if (Input.GetMouseButton(0)&&canCheck)
        {         
            timeDown += Time.deltaTime;
            if(timeDown>7)
            {
                timeDown = 0;
                debugConsole.SetActive(true);
            }
        }
        if (Input.GetMouseButtonUp(0)&&timeDown>2)
        {
            canCheck = false;
            timeDown = 0;
        }
    }
}
