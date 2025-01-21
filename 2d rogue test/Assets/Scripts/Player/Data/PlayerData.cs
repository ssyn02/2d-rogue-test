using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Move State")]
    public float movementVelocity = 10f;

    [Header("Jump State")]
    public float jumpVelocity = 15f;
    public int jumpAmount = 1;

    [Header("In Air State")]
    public float jumpHeightMultiplier;

    [Header("Check Variables")]
    public float groundCheckRadius = 0.3f;
    public LayerMask ground;
}
