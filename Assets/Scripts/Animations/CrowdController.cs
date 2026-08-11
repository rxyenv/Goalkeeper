using UnityEngine;
public class CrowdController : MonoBehaviour
{
  public Animator[] crowdAnimators;
  public void PlayCheer()
  {
    for (int i = 0; i < crowdAnimators.Length; i++)
    {
      crowdAnimators[i].SetTrigger("cheer");
    }
  }
  public void PlaySad()
  {
    for (int i = 0; i < crowdAnimators.Length; i++)
    {
      crowdAnimators[i].SetTrigger("sad");
    }
  }
}
