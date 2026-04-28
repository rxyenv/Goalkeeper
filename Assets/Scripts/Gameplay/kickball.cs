using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kickball : MonoBehaviour
{
    
  [SerializeField] private float forwardForce = 10f;
  [SerializeField] private float upwardForce = 7f;
  [SerializeField] private Animator kickerAnimator;
  [SerializeField] private float StartingZPos = -2f;
  float[] lanes = { -2f, -1f, 0f, 1f, 2f };
  Rigidbody Rb;
  bool isResetting = false;
  Vector3 startPosition;



  void Start()
  {
    Rb = GetComponent<Rigidbody>();
    startPosition = new Vector3(0, transform.position.y, StartingZPos);
    transform.position = startPosition;
    Shoot();
  }
  void Shoot()
  {
    kickerAnimator.SetTrigger("Kick");
    float laneX = lanes[Random.Range(0, lanes.Length)];
    Vector3 direction = new Vector3(laneX - transform.position.x, 0, -8f).normalized;
    Rb.AddForce(direction * forwardForce, ForceMode.Impulse);
    Rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
  }

  void OnCollisionEnter(Collision collision)
  {
    if (isResetting) return;
    if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Goal")
    {
      isResetting = true;
      StartCoroutine(ResetBall());
    }

  }
  IEnumerator ResetBall()
  {
    yield return new WaitForSeconds(1.5f);
    Rb.linearVelocity = Vector3.zero;
    Rb.angularVelocity = Vector3.zero;
    transform.position = startPosition;
    Shoot();
    isResetting = false;
  }
}


