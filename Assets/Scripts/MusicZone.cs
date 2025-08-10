using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public AudioSource audioSource; // ������ ����� AudioSource
    public float fadeTiome; // ���� ���̵� �ð�
    public float maxVolume; // �ִ� ����
    private float targetVolume; // ��ǥ ����
    void Start()
    {
        targetVolume = 0f; // �ʱ� ��ǥ ������ 0
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = targetVolume;
        audioSource.Play(); // ���� ����
    }

    // Update is called once per frame
    void Update()
    {
        // Approximately = �� ���� ���� ������ Ȯ���ϴ� �Լ�
        if (!Mathf.Approximately(audioSource.volume, targetVolume))
        {
            // ���� ������ ��ǥ ���� ���̸� ���� �����Ͽ� �ε巴�� ����
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, maxVolume * Time.deltaTime / fadeTiome);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // �÷��̾ ������ ������ ��ǥ ������ �ִ� �������� ����
            targetVolume = maxVolume;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // �÷��̾ ������ ������ ��ǥ ������ 0���� ����
            targetVolume = 0f;
        }
    }
}
