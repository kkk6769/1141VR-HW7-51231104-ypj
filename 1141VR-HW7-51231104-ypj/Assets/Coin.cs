using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int value = 10; // 铜 10、银 100、金 1000

    [Header("Position Settings")]
    public float groundY = -10f; // 该硬币的消失/落地地平线（世界坐标 Y）
    public float spawnOffsetY = 0f; // 相对 Factory 的额外生成高度偏移
}
