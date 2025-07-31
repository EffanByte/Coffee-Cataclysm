using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private Image Progessbar;

    private int nextSceneIndex;

    void Start()
    {
        if (GameStateManager.Instance != null)
        {
            if (GameStateManager.Instance.currentState == GameSceneState.GameBar)
            {
                nextSceneIndex = 0;
            }
            else if (GameStateManager.Instance.currentState == GameSceneState.OutSide)
            {
                nextSceneIndex = 2;
            }
            else
            {
                Debug.Log("Invalid Scene Index");
            }
            StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
    
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextSceneIndex);

            while (asyncOperation.isDone == false)
            {
                Progessbar.fillAmount = asyncOperation.progress;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}



