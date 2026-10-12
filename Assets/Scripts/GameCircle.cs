using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameCircle : MonoBehaviour
{
    public AudioClip CollisionSound;
    public AudioSource audioSource;
    public string Name;
    public string Url;
    public string Author;
    public int Id;

    public TextMeshPro NameTmp;
    public SpriteRenderer Sr;

    public void Start()
    {
        if (Name != null)
        {
            NameTmp.text = Name;
        }

        Sr.color = Random.ColorHSV(hueMin:0.2f, hueMax: 1, saturationMin:0.2f, saturationMax: 1, valueMin:0.2f, valueMax: 1, alphaMax:1f,alphaMin:1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CollisionSound == null) return;

        // Создаём временный объект для звука
        GameObject soundObj = new GameObject("TempCollisionSound");
        soundObj.transform.position = transform.position;

        AudioSource tempSource = soundObj.AddComponent<AudioSource>();
        tempSource.clip = CollisionSound;
        tempSource.pitch = Random.Range(0.9f, 1.1f);   // разная тональность
        tempSource.spatialBlend = 1f;                 // 2D/3D звук, можно настроить
        tempSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        tempSource.Play();

        // Удаляем объект после окончания звука
        Destroy(soundObj, CollisionSound.length / tempSource.pitch);
    }
}
