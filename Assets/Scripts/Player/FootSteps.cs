using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioClip[] footstepClips; // �߼Ҹ� ���� Ŭ�� �迭
    private AudioSource audioSource; // ����� �ҽ� ������Ʈ
    private Rigidbody _rigidbody;
    public float footstepThreshold; // �߼Ҹ��� ����� �ּ� �ӵ�
    public float footstepRate; // �߼Ҹ� ��� ����
    private float footstepTime; // �߼Ҹ� ��� �ð�
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // - Rigidbody�� �ӵ� Y���� 0.1 ������ ��
        if (Mathf.Abs(_rigidbody.velocity.y) < 0.1f)
        {
            // - Rigidbody�� �ӵ� X��� Z���� ũ�Ⱑ footstepThreshold���� Ŭ ��
            if (_rigidbody.velocity.magnitude > footstepThreshold)
            {
                if(Time.time - footstepRate > footstepTime)
                {
                    footstepTime = Time.time;
                    audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]); // ������ �߼Ҹ� ���
                }
            }
        }
    }
}
