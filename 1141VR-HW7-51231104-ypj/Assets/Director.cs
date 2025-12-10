using UnityEngine;

public class Director : MonoBehaviour
{
    [Header("Game Settings")]
    public float gameDuration = 60f; // 倒数时间（秒）

    [Header("Refs")]
    public UIManager uiManager;
    public CoinFactory factory;
    public Camera playerCamera; // 可选：将 UI Canvas 作为相机的子物体

    private float remaining;
    private int money;
    private bool isRunning;

    void Start()
    {
        remaining = gameDuration;
        money = 0;
        isRunning = true;
        factory?.StartFactory();
        UpdateUI();
        // 如果场景中未赋值 UIManager，尝试自动查找
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
    }

    void Update()
    {
        if (!isRunning) return;

        remaining -= Time.deltaTime;
        if (remaining <= 0f)
        {
            remaining = 0f;
            EndGame();
        }
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        if (!isRunning) return;
        money += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        uiManager?.SetMoney(money);
        uiManager?.SetTimer(remaining);
    }

    private void EndGame()
    {
        isRunning = false;
        factory?.StopFactory();
        // 可在此扩展：显示结算 UI、重开按钮等
    }
}
