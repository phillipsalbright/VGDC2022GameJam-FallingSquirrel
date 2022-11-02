using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] Button buttonToSelect;

    private void Start()
    {
        buttonToSelect.Select();
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadTheScene(sceneIndex));
    }

    private IEnumerator LoadTheScene(int sceneIndex)
    {
        Destroy(GameObject.Find("Eventsystem"));
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
