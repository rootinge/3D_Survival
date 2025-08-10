using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public AudioSource audioSource; // 음악을 재생할 AudioSource
    public float fadeTiome; // 음악 페이드 시간
    public float maxVolume; // 최대 볼륨
    private float targetVolume; // 목표 볼륨
    void Start()
    {
        targetVolume = 0f; // 초기 목표 볼륨은 0
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = targetVolume;
        audioSource.Play(); // 음악 시작
    }

    // Update is called once per frame
    void Update()
    {
        // Approximately = 두 값이 거의 같은지 확인하는 함수
        if (!Mathf.Approximately(audioSource.volume, targetVolume))
        {
            // 현재 볼륨과 목표 볼륨 사이를 선형 보간하여 부드럽게 변경
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, maxVolume * Time.deltaTime / fadeTiome);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // 플레이어가 영역에 들어오면 목표 볼륨을 최대 볼륨으로 설정
            targetVolume = maxVolume;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // 플레이어가 영역을 나가면 목표 볼륨을 0으로 설정
            targetVolume = 0f;
        }
    }
}
