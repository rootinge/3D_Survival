using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float statValue;
    public float maxValue;
    public float passiveValue;
    public Image uiBar;


    void Start()
    {
        curValue = statValue;
    }

    void Update()
    {
        // ui������Ʈ
        uiBar.fillAmount = GetPercentage();
    }

    float GetPercentage()
    {
        return curValue / maxValue;
    }

    public void Add(float value)
    {
        curValue = Mathf.Min(curValue + value, maxValue);

    }

    public void Subtract(float value)
    {
        curValue = Mathf.Max(curValue - value, 0);
    }
}
