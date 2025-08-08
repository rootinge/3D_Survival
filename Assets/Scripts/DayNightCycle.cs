using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0f, 1f)]
    public float time; // 현재 시간(0~1, 0: 자정, 0.5: 정오)
    public float fullDayLength; // 하루가 흐르는 데 걸리는 실제 시간(초)
    public float startTime = 0.4f; // 시작 시간(0~1, 기본값: 오전)
    private float timeRate; // 시간 진행 속도(초당 증가량)
    public Vector3 noon; // 정오 기준 태양/달의 방향 벡터

    [Header("Sun")]
    public Light sun; // 태양 광원
    public Gradient sunColor; // 시간에 따른 태양 색상 변화
    public AnimationCurve sunInternsity; // 시간에 따른 태양 밝기 변화

    [Header("Moon")]
    public Light moon; // 달 광원
    public Gradient moonColor; // 시간에 따른 달 색상 변화
    public AnimationCurve moonIntensity; // 시간에 따른 달 밝기 변화

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier; // 전체 조명 밝기 곡선
    public AnimationCurve reflectionIntensityMultiplier; // 반사광 밝기 곡선


    void Start()
    {
        timeRate = 1f / fullDayLength; // 하루 길이에 따라 시간 진행 속도 계산
        time = startTime; // 시작 시간 설정
    }


    void Update()
    {
        // 시간 진행 (0~1 사이로 순환)
        time = (time + timeRate * Time.deltaTime) % 1f;

        // 태양과 달의 광원 상태 갱신
        UpdateLighting(sun, sunColor, sunInternsity);
        UpdateLighting(moon, moonColor, moonIntensity);

        // 환경광 및 반사광 밝기 갱신
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }


    /// 광원(태양/달)의 방향, 색상, 밝기 및 활성화 상태를 갱신합니다.
    /// <param name="lightSource">조명 오브젝트(태양 또는 달)</param>
    /// <param name="gradient">시간에 따른 색상 변화 그래디언트</param>
    /// <param name="intensityCurve">시간에 따른 밝기 변화 곡선</param>
    void UpdateLighting(Light lightSource, Gradient gradient, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time); // 현재 시간에 따른 밝기 계산

        // 광원의 방향을 시간에 따라 회전
        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4f;
        // 광원의 색상 및 밝기 적용
        lightSource.color = gradient.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        // 밝기가 0이면 비활성화, 0보다 크면 활성화
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
