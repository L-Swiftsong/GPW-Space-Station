using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class AreaTrigger : MonoBehaviour

{ 
    public string areaName;
    public TMP_Text uiText; 

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Player"))
        {
            
            uiText.text = areaName;
        }
    }

}
