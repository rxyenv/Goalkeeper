using UnityEngine;

public class BackGroundTeamManager : MonoBehaviour
{
    [SerializeField] private BackGroundPlayer[] blueTeam;
    [SerializeField] private BackGroundPlayer[] purpleTeam;

    public void PlayWin()
    {
        foreach (BackGroundPlayer player in blueTeam)
            player.PlayWinAnimation();

        foreach (BackGroundPlayer player in purpleTeam)
            player.PlayLoseAnimation();
    }

    public void PlayLose()
    {
        foreach (BackGroundPlayer player in blueTeam)
            player.PlayLoseAnimation();

        foreach (BackGroundPlayer player in purpleTeam)
            player.PlayWinAnimation();
    }
}