using UnityEngine;

public class drop : MonoBehaviour
{
	[Header("Movement Settings")]
	public float fallSpeed = 3f; // 掉落速度（向下）
	public float horizontalSpeed = 2f; // 水平速度（左右漂移）
	public Vector2 horizontalRange = new Vector2(-5f, 5f); // 画面左右范围（世界坐标）

	[Header("Lifetime Settings")]
	public float destroyY = -10f; // 掉出画面后销毁的 Y 阈值
	public float minAliveTime = 0.2f; // 最小存活时间，避免刚生成立刻被判定销毁

	private float dirX; // 当前水平移动方向与速度系数
	private float startY; // 记录生成时的初始 Y，用于安全阈值
	private const float SafetyMargin = 0.1f; // 防止 destroyY 高于生成点时立刻销毁
	private bool configuredByFactory = false;
	private float aliveTime = 0f;

	// 工厂调用：显式配置所有关键参数（兼容旧接口，新增可选 spawnX）
	public void ConfigureFromFactory(Vector2 range, float fall, float horiz, float destroyYValue, float? spawnX = null)
	{
		horizontalRange = range;
		fallSpeed = fall;
		horizontalSpeed = horiz;
		destroyY = destroyYValue;
		configuredByFactory = true;

		// 设置水平移动初始方向
		dirX = Random.Range(-horizontalSpeed, horizontalSpeed);

		// 若提供了生成 X，则立即应用
		if (spawnX.HasValue)
		{
			var p = transform.position;
			p.x = spawnX.Value;
			transform.position = p;
		}
	}

	void OnEnable()
	{
		if (!configuredByFactory)
		{
			// 随机水平速度方向
			dirX = Random.Range(-horizontalSpeed, horizontalSpeed);
			// 初始位置随机（仅当工厂未配置时才随机）
			if (horizontalRange.x < horizontalRange.y)
			{
				var p = transform.position;
				p.x = Random.Range(horizontalRange.x, horizontalRange.y);
				transform.position = p;
			}
			// 使用每种硬币的地平线作为销毁 Y（如果设置了）
			var coin = GetComponent<Coin>();
			if (coin != null)
			{
				destroyY = coin.groundY;
			}
		}

		// 记录初始高度，用于避免一开始阈值配置错误造成的瞬间销毁
		startY = transform.position.y;
		aliveTime = 0f;
	}

	void Update()
	{
		aliveTime += Time.deltaTime;
		// 向下移动
		transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
		// 左右移动
		transform.Translate(Vector3.right * dirX * Time.deltaTime, Space.World);

		// 掉出画面销毁（根据 Y），并加入安全阈值：阈值不允许高于起始点
		float effectiveDestroyY = destroyY;
		if (effectiveDestroyY >= startY)
		{
			effectiveDestroyY = startY - SafetyMargin;
		}
		if (aliveTime >= minAliveTime && transform.position.y <= effectiveDestroyY)
		{
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		// 被角色接住（触发事件）
		var player = other.GetComponent<Player>();
		if (player != null)
		{
			// 将金币价值传回玩家/导演
			var coin = GetComponent<Coin>();
			if (coin != null)
			{
				player.OnCollectCoin(coin.value);
			}
			Destroy(gameObject);
		}
	}
}

