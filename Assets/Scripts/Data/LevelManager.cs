using UnityEngine;

[CreateAssetMenu(menuName = "Demo/LevelConfig")]
public class LevelManager : ScriptableObject
{
    [Header("Level Config")]
    [Tooltip("Maximum number of goals the player can concede before losing.")]
    public int maxHealth;

    [Tooltip("Maximum strength / difficulty cap for this level.")]
    public int maxStrength;
}
