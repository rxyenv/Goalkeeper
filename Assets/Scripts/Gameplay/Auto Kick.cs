using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoKick : MonoBehaviour
{
   public Rigidbody ball;
    public float kickForce = 20f;

    void Start()
    {
        Invoke("KickBall",1f);
    }

    void KickBall()
    {
        ball.AddForce(transform.forward * kickForce, ForceMode.Impulse);
    }
}
