using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Refs (auto)")]
    public Text moneyText;
    public Text timerText;
    public TMP_Text moneyTMP;
    public TMP_Text timerTMP;

    [Header("Auto Find Settings")]
    public string moneyObjectName = "MoneyText";
    public string timerObjectName = "TimerText";

    [Header("Auto Layout (Top-Left)")]
    public bool autoLayoutTopLeft = true;
    public Vector2 startOffset = new Vector2(20f, -20f); // 距离左上角偏移（像素）
    public float lineHeight = 30f; // 行间距（像素）

    void Awake()
    {
        // 尝试自动查找 UI 文本（优先 TMP_Text）
        if (moneyTMP == null)
        {
            var go = GameObject.Find(moneyObjectName);
            if (go) moneyTMP = go.GetComponent<TMP_Text>();
        }
        if (timerTMP == null)
        {
            var go = GameObject.Find(timerObjectName);
            if (go) timerTMP = go.GetComponent<TMP_Text>();
        }
        if (moneyText == null)
        {
            var go = GameObject.Find(moneyObjectName);
            if (go) moneyText = go.GetComponent<Text>();
        }
        if (timerText == null)
        {
            var go = GameObject.Find(timerObjectName);
            if (go) timerText = go.GetComponent<Text>();
        }
    }

    void Start()
    {
        if (autoLayoutTopLeft)
        {
            var moneyRT = GetRectTransform(moneyTMP, moneyText);
            var timerRT = GetRectTransform(timerTMP, timerText);
            if (moneyRT != null)
            {
                PlaceTopLeft(moneyRT, startOffset);
            }
            if (timerRT != null)
            {
                PlaceTopLeft(timerRT, startOffset + new Vector2(0f, -lineHeight));
            }
        }
    }

    public void SetMoney(int money)
    {
        var s = $"Money: {money}";
        if (moneyTMP != null) moneyTMP.text = s;
        else if (moneyText != null) moneyText.text = s;
    }

    public void SetTimer(float remaining)
    {
        var s = $"Time: {Mathf.CeilToInt(remaining)}";
        if (timerTMP != null) timerTMP.text = s;
        else if (timerText != null) timerText.text = s;
    }

    private static RectTransform GetRectTransform(TMP_Text tmp, UnityEngine.UI.Text ugui)
    {
        if (tmp != null) return tmp.rectTransform;
        if (ugui != null) return ugui.rectTransform;
        return null;
    }

    private static void PlaceTopLeft(RectTransform rt, Vector2 offset)
    {
        if (rt == null) return;
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = offset;
    }
}
