using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [Header("씬 설정")]
    [SerializeField] private string gameSceneName = "SampleScene";

    [Header("로딩 화면")]
    [SerializeField] private GameObject loadingPanel;

    [Header("최소 로딩 표시 시간")]
    [SerializeField] private float minimumLoadingTime = 1f;

    private bool isLoading;

    public void StartGame()
    {
        if (!isLoading)
        {
            StartCoroutine(LoadGameScene());
        }
    }

    private IEnumerator LoadGameScene()
    {
        isLoading = true;

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(gameSceneName);

        // 로딩 완료 후 자동으로 씬이 바뀌지 않게 막음
        operation.allowSceneActivation = false;

        float timer = 0f;

        while (timer < minimumLoadingTime ||
               operation.progress < 0.9f)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // 준비가 끝나면 게임 씬으로 전환
        operation.allowSceneActivation = true;
    }
}