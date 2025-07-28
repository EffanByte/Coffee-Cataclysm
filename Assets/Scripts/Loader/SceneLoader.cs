using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private Image Progessbar;
    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(2);

        while (asyncOperation.isDone == false)
        {
            Progessbar.fillAmount = asyncOperation.progress;
            yield return new WaitForEndOfFrame();
        }
    }

}
