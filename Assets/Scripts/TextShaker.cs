using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextShake : MonoBehaviour
{
    [Header("Настройки тряски")]
    [SerializeField] private float shakeIntensity = 2f;
    [SerializeField] private float shakeIntensityZ = 0f;
    [SerializeField] private float shakeSpeed = 10f;
    [SerializeField] private float shakeSpeedZ = 10f;
    [SerializeField] private bool randomPhase = true;
    [SerializeField] private bool isShaking = true;

    private TMP_Text textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }

    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

    private void OnTextChanged(UnityEngine.Object obj) { }

    private void Update()
    {
        if (!isShaking) return;
        if (textComponent == null) return;

        textComponent.ForceMeshUpdate(false, false);
        TMP_TextInfo textInfo = textComponent.textInfo;

        if (textInfo == null || textInfo.characterCount == 0) return;

        float t = Time.time;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo ch = textInfo.characterInfo[i];
            if (!ch.isVisible) continue;

            int matIndex = ch.materialReferenceIndex;
            int vertIndex = ch.vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[matIndex].vertices;

            float phase = randomPhase ? i * 1.37f : 0f;

            // Своё смещение для КАЖДОЙ из 4 вершин
            for (int v = 0; v < 4; v++)
            {
                // Свой сдвиг фазы для каждой вершины (v * 7.13f)
                float vPhase = phase + v * 7.13f;

                float x = Mathf.PerlinNoise(t * shakeSpeed + vPhase, 0f) * 2f - 1f;
                float y = Mathf.PerlinNoise(0f, t * shakeSpeed + vPhase) * 2f - 1f;
                float z = Mathf.PerlinNoise(0f, t * shakeSpeedZ + vPhase + 10f) * 2f - 1f;

                Vector3 offset = new Vector3(
                    x * shakeIntensity,
                    y * shakeIntensity,
                    z * shakeIntensityZ
                );

                vertices[vertIndex + v] += offset;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
            if (meshInfo.mesh == null) continue;
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    public void StartShake() => isShaking = true;
    public void StopShake() => isShaking = false;
    public void SetIntensity(float value) => shakeIntensity = value;
    public void SetSpeed(float value) => shakeSpeed = value;
}