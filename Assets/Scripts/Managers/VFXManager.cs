using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private GameObject trailVfx;
    [SerializeField] private GameObject ballSheildVfx;


    public static VFXManager instance;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        trailVfx.SetActive(false);
        ballSheildVfx.SetActive(false);
    }

    public void PlayTrailEffect()
    {
        trailVfx.SetActive(true);
        ballSheildVfx.SetActive(true);
    }

    public void PlayGroundTouchEffect()
    {
       trailVfx.SetActive(false);
       ballSheildVfx.SetActive(false);
    }
}
