using TMPro;
using UnityEngine;

public class LoadingTextWave : MonoBehaviour
{
    [SerializeField] private TMP_Text loadingText;

    [Header("웨이브 설정")]
    [SerializeField] private float waveHeight = 8f;
    [SerializeField] private float waveSpeed = 4f;
    [SerializeField] private float characterSpacing = 0.5f;

    private TMP_TextInfo textInfo;

    private void Awake()
    {
        if (loadingText == null)
        {
            loadingText = GetComponent<TMP_Text>();
        }
    }

    private void Update()
    {
        if (loadingText == null)
        {
            return;
        }

        loadingText.ForceMeshUpdate();
        textInfo = loadingText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo characterInfo =
                textInfo.characterInfo[i];

            if (!characterInfo.isVisible)
            {
                continue;
            }

            int materialIndex =
                characterInfo.materialReferenceIndex;

            int vertexIndex =
                characterInfo.vertexIndex;

            Vector3[] vertices =
                textInfo.meshInfo[materialIndex].vertices;

            float offsetY = Mathf.Sin(
                Time.unscaledTime * waveSpeed +
                i * characterSpacing
            ) * waveHeight;

            Vector3 offset =
                new Vector3(0f, offsetY, 0f);

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo =
                textInfo.meshInfo[i];

            meshInfo.mesh.vertices =
                meshInfo.vertices;

            loadingText.UpdateGeometry(
                meshInfo.mesh,
                i
            );
        }
    }
}