using System.Collections;
using UnityEngine;
public class CrowdController : MonoBehaviour
{
    public Animator[] crowdAnimators;
    private Vector3[] orginalPositions;
    [SerializeField] private GameObject[] crowdGOs;

    private void Start()
    {
        //RemoveShadows();
        orginalPositions = new Vector3[crowdAnimators.Length];

        for(int i = 0; i < crowdAnimators.Length; i++)
        {
            orginalPositions[i] = crowdAnimators[i].transform.position;
        }
    }

    public void PlayCheer()
    {
        for (int i = 0; i < crowdAnimators.Length; i++)
        {
            crowdAnimators[i].SetTrigger("cheer");
            ResetPositions();
        }
    }

    public void PlaySad()
    {
        for (int i = 0; i < crowdAnimators.Length; i++)
        {
            crowdAnimators[i].SetTrigger("sad");
            ResetPositions();
        }
    }

    private void ResetPositions()
    {
        for (int i = 0; i < crowdAnimators.Length; i++)
        {
            crowdAnimators[i].transform.position = orginalPositions[i];
        }
    }
    private void RemoveShadows()
    {
        foreach(GameObject go  in crowdGOs)
        {
            TurnOffShadows(go.GetComponentsInChildren<MeshRenderer>());
        }

        void TurnOffShadows(MeshRenderer[] rendererArray)
        {
            foreach(var mr in rendererArray)
            {
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
            }
        }
    }
}
