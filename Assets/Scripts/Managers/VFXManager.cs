using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private GameObject trailVfx;
    [SerializeField] private GameObject ballSheildVfx;

    private GameObject trailInstance;
    private GameObject ballsheildInstance;

    public static VFXManager instance;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void PlayTrailEffect()
    {
        var ballController = FindAnyObjectByType<BallController>();

        trailInstance = Instantiate(trailVfx, ballController.transform);
        ballsheildInstance = Instantiate(ballSheildVfx, ballController.transform);
    }

    public void PlayGroundTouchEffect()
    {
        Destroy(trailInstance);
        Destroy(ballsheildInstance);
    }
}
