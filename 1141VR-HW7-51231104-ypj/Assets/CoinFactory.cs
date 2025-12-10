using UnityEngine;

public class CoinFactory : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject goldPrefab;
    public GameObject silverPrefab;
    public GameObject copperPrefab;

    [Header("Spawn Settings")]
    public float interval = 1.0f; // 生成间隔
    public Vector2 spawnXRange = new Vector2(-5f, 5f); // 随机 X
    public float spawnY = 8f; // 从上方生成的 Y
    public float initialFallSpeed = 3f; // 默认掉落速度
    public float initialHorizontalSpeed = 2f; // 默认水平漂移速度

    [Header("Rates (0-1)")]
    public float goldRate = 0.1f;   // 金币概率
    public float silverRate = 0.3f; // 银币概率
    public float copperRate = 0.6f; // 铜币概率

    private float timer;
    private bool isRunning = true;

    void Update()
    {
        if (!isRunning) return;
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            SpawnOne();
        }
    }

    public void StopFactory()
    {
        isRunning = false;
    }

    public void StartFactory()
    {
        isRunning = true;
        timer = 0f;
    }

    private void SpawnOne()
    {
        float r = Random.value;
        GameObject prefab = copperPrefab;
        if (r < goldRate) prefab = goldPrefab;
        else if (r < goldRate + silverRate) prefab = silverPrefab;
        else prefab = copperPrefab;

        if (prefab == null) return;

        float spawnX = Random.Range(spawnXRange.x, spawnXRange.y);
        Vector3 pos = new Vector3(spawnX, spawnY, 0f);
        var go = Instantiate(prefab, pos, Quaternion.identity);

        var d = go.GetComponent<drop>();
        var coin = go.GetComponent<Coin>();

        if (coin != null)
        {
            // 生成高度偏移
            if (Mathf.Abs(coin.spawnOffsetY) > Mathf.Epsilon)
            {
                var p = go.transform.position;
                p.y += coin.spawnOffsetY;
                go.transform.position = p;
            }
        }

        // 统一由 Factory 显式配置 drop，避免 OnEnable 时机差导致的误判
        if (d != null)
        {
            float destroyYValue = (coin != null) ? coin.groundY : d.destroyY;
            d.ConfigureFromFactory(spawnXRange, initialFallSpeed, initialHorizontalSpeed, destroyYValue, spawnX);
        }
    }
}
