using UnityEngine;

public class BackGroundTeamManager : MonoBehaviour
{
    [SerializeField] private BackGroundPlayer[] blueTeam;
    [SerializeField] private BackGroundPlayer[] purpleTeam;

    private Vector3[] blueTeamStartPos; 
    private Vector3[] purpleTeamStartPos;

    private void Start()
    {
        blueTeamStartPos = new Vector3[blueTeam.Length];
        purpleTeamStartPos = new Vector3[purpleTeam.Length];

        for(int i = 0; i < blueTeam.Length; i++)
        {
            blueTeamStartPos[i] = blueTeam[i].transform.position;
        }

        for(int i = 0; i < purpleTeam.Length; i++)
        {
            purpleTeamStartPos[i] = purpleTeam[i].transform.position;
        }
    }

    private void ResetBackgroundPlayersPos()
    {
        for(int i = 0; i < blueTeam.Length; i++)
        {
            blueTeam[i].transform.position = blueTeamStartPos[i];
        }

        for(int i = 0; i < purpleTeam.Length; i++)
        {
            purpleTeam[i].transform.position = purpleTeamStartPos[i];
        }
    }

    public void PlayWin()
    {
        foreach (BackGroundPlayer player in blueTeam)
            player.PlayWinAnimation();

        foreach (BackGroundPlayer player in purpleTeam)
            player.PlayLoseAnimation();

        ResetBackgroundPlayersPos();
    }

    public void PlayLose()
    {
        foreach (BackGroundPlayer player in blueTeam)
            player.PlayLoseAnimation();

        foreach (BackGroundPlayer player in purpleTeam)
            player.PlayWinAnimation();

        ResetBackgroundPlayersPos();
    }
}