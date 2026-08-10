using UnityEngine;
using System.Collections;

public class BackGroundPlayer : MonoBehaviour
{
    Animator animator;

    [SerializeField] private int totalIdleAnimations = 4;
    [SerializeField] private int totalWinAnimations=4;
    [SerializeField] private int totalLoseAnimations=4;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveSpeed = 2f;


    

    private void Start()
    {
        animator = GetComponent<Animator>();

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

           


        }
    }
    

    private void SetRandomIdle()
    {
        int randomNum = Random.Range(0, totalIdleAnimations);
        animator.SetFloat("IdleIndex", randomNum);
    }

    public void PlayWinAnimation()
    {
        StopCoroutine(IdleRoutine());
        int randomNum = Random.Range(0, totalWinAnimations);
        animator.SetTrigger("Win");
        animator.SetFloat("WinIndex", randomNum);
    }

    public void PlayLoseAnimation()
    {
        StopCoroutine(IdleRoutine());
        int randomNum = Random.Range(0, totalLoseAnimations);
        animator.SetTrigger("Lose");
        animator.SetFloat("LoseIndex", randomNum);
    }
}