using UnityEngine;
using System.Collections;

public class BackGroundPlayer : MonoBehaviour
{
    Animator animator;
    Vector3 startPosition;

    [SerializeField] private int totalIdleAnimations = 4;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveSpeed = 2f;


    

    private void Start()
    {
        animator = GetComponent<Animator>();
        startPosition = transform.position;

        StartCoroutine(IdleRoutine());
    }

    IEnumerator IdleRoutine()
    {
        while (true)
        {
            SetRandomIdle();

            
            yield return null;

            
            float animationLength = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

            
            yield return new WaitForSeconds(animationLength);

            yield return Walk();


        }
    }
    IEnumerator Walk()
    {
        Vector3 RandomOffSet = new Vector3(0f,0f,Random.Range(0, moveDistance));

        Vector3 targetPos = startPosition+RandomOffSet;
        animator.SetBool("IsWalking",true);

        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position,targetPos,moveSpeed*Time.deltaTime);


            yield return null;
        }
        animator.SetBool("IsWalking",false);

    }

    private void SetRandomIdle()
    {
        int randomNum = Random.Range(0, totalIdleAnimations);
        animator.SetFloat("IdleIndex", randomNum);
    }

    public void PlayWinAnimation()
    {
        StopCoroutine(IdleRoutine());
        animator.SetTrigger("Win");
    }

    public void PlayLoseAnimation()
    {
        StopCoroutine(IdleRoutine());
        animator.SetTrigger("Lose");
    }
}