using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0f, 1f)]
    public float time; // ���� �ð�(0~1, 0: ����, 0.5: ����)
    public float fullDayLength; // �Ϸ簡 �帣�� �� �ɸ��� ���� �ð�(��)
    public float startTime = 0.4f; // ���� �ð�(0~1, �⺻��: ����)
    private float timeRate; // �ð� ���� �ӵ�(�ʴ� ������)
    public Vector3 noon; // ���� ���� �¾�/���� ���� ����

    [Header("Sun")]
    public Light sun; // �¾� ����
    public Gradient sunColor; // �ð��� ���� �¾� ���� ��ȭ
    public AnimationCurve sunInternsity; // �ð��� ���� �¾� ��� ��ȭ

    [Header("Moon")]
    public Light moon; // �� ����
    public Gradient moonColor; // �ð��� ���� �� ���� ��ȭ
    public AnimationCurve moonIntensity; // �ð��� ���� �� ��� ��ȭ

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier; // ��ü ���� ��� �
    public AnimationCurve reflectionIntensityMultiplier; // �ݻ籤 ��� �


    void Start()
    {
        timeRate = 1f / fullDayLength; // �Ϸ� ���̿� ���� �ð� ���� �ӵ� ���
        time = startTime; // ���� �ð� ����
    }


    void Update()
    {
        // �ð� ���� (0~1 ���̷� ��ȯ)
        time = (time + timeRate * Time.deltaTime) % 1f;

        // �¾�� ���� ���� ���� ����
        UpdateLighting(sun, sunColor, sunInternsity);
        UpdateLighting(moon, moonColor, moonIntensity);

        // ȯ�汤 �� �ݻ籤 ��� ����
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }


    /// ����(�¾�/��)�� ����, ����, ��� �� Ȱ��ȭ ���¸� �����մϴ�.
    /// <param name="lightSource">���� ������Ʈ(�¾� �Ǵ� ��)</param>
    /// <param name="gradient">�ð��� ���� ���� ��ȭ �׷����Ʈ</param>
    /// <param name="intensityCurve">�ð��� ���� ��� ��ȭ �</param>
    void UpdateLighting(Light lightSource, Gradient gradient, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time); // ���� �ð��� ���� ��� ���

        // ������ ������ �ð��� ���� ȸ��
        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4f;
        // ������ ���� �� ��� ����
        lightSource.color = gradient.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        // ��Ⱑ 0�̸� ��Ȱ��ȭ, 0���� ũ�� Ȱ��ȭ
        if (lightSource.intensity == 0 && go.activeInHierarchy)
        {
            go.SetActive(false);
        }
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
        {
            go.SetActive(true);
        }
    }
}
