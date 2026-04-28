using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scoring : MonoBehaviour
{
    private int Goals=0;
    [SerializeField]private TextMeshProUGUI scoreText;  
                

    private void Start()
    {
        scoreText.text = "Goals: 0"; 
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Goalline")
        {
            Goals++;
            scoreText.text="Goals: "+Goals;
        }
    }

}