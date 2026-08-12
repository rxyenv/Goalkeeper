using UnityEngine;

public class BackGroundTeamManager : MonoBehaviour
{
    [SerializeField] private BackGroundPlayer[] blueTeam;
    [SerializeField] private BackGroundPlayer[] purpleTeam;

    private Vector3[] blueTeamStartPos; 
    private Vector3[] purpleTeamStartPos;
    private Quaternion[] blueTeamStartRotation; 
    private Quaternion[] purpleTeamStartRotation;

    private void Start()
    {
        blueTeamStartPos = new Vector3[blueTeam.Length];
        purpleTeamStartPos = new Vector3[purpleTeam.Length];
        blueTeamStartRotation = new Quaternion[blueTeam.Length];
        purpleTeamStartRotation = new Quaternion[purpleTeam.Length];

        CollectBGPlayerPositionAndRotation(blueTeam, blueTeamStartPos, blueTeamStartRotation);
        CollectBGPlayerPositionAndRotation(purpleTeam, purpleTeamStartPos, purpleTeamStartRotation);
    }

    private void CollectBGPlayerPositionAndRotation(BackGroundPlayer[] teamPos, Vector3[] posArr, Quaternion[] rotArr)
    {
        for (int i = 0; i < teamPos.Length; i++)
        {
            posArr[i] = teamPos[i].transform.position;
            rotArr[i] = teamPos[i].transform.rotation;
        }
    }
    private void ResetBGPlayerPositionAndRotation(BackGroundPlayer[] teamPos, Vector3[] posArr, Quaternion[] rotArr)
    {
        for (int i = 0; i < teamPos.Length; i++)
        {
            teamPos[i].transform.position = posArr[i];
            teamPos[i].transform.rotation = rotArr[i];
        }
    }

    public void PlayWin()
    {
        foreach (BackGroundPlayer player in blueTeam)
            player.PlayWinAnimation();

        foreach (BackGroundPlayer player in purpleTeam)
            player.PlayLoseAnimation();

        ResetBGPlayerPositionAndRotation(blueTeam, blueTeamStartPos, blueTeamStartRotation);
        ResetBGPlayerPositionAndRotation(purpleTeam, purpleTeamStartPos, purpleTeamStartRotation);
    }

    public void PlayLose()
    {
        foreach (BackGroundPlayer player in blueTeam)
            player.PlayLoseAnimation();

        foreach (BackGroundPlayer player in purpleTeam)
            player.PlayWinAnimation();

        ResetBGPlayerPositionAndRotation(blueTeam, blueTeamStartPos, blueTeamStartRotation);
        ResetBGPlayerPositionAndRotation(purpleTeam, purpleTeamStartPos, purpleTeamStartRotation);
    }
}