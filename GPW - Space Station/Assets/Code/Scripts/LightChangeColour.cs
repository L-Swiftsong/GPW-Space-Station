using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightChangeColour : MonoBehaviour
{

    public Light targetLight; 
    public Color newColor = Color.red; 

    
    public void ChangeLightColor()
    {
        if (targetLight != null)
        {
            targetLight.color = newColor;
        }
    }

}
