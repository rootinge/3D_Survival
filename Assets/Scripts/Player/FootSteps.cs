using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioClip[] footstepClips; // 발소리 사운드 클립 배열
    private AudioSource audioSource; // 오디오 소스 컴포넌트
    private Rigidbody _rigidbody;
    public float footstepThreshold; // 발소리가 재생될 최소 속도
    public float footstepRate; // 발소리 재생 간격
    private float footstepTime; // 발소리 재생 시간
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // - Rigidbody의 속도 Y축이 0.1 이하일 때
        if (Mathf.Abs(_rigidbody.velocity.y) < 0.1f)
        {
            // - Rigidbody의 속도 X축과 Z축의 크기가 footstepThreshold보다 클 때
            if (_rigidbody.velocity.magnitude > footstepThreshold)
            {
                if(Time.time - footstepRate > footstepTime)
                {
                    footstepTime = Time.time;
                    audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]); // 랜덤한 발소리 재생
                }
            }
        }
    }
}
