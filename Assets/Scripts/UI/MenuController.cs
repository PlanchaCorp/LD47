using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private string levelName;

    private GameObject canvas;

    public void SetLevelName(string newLevelName) {
        levelName = newLevelName;
    }

    private void Start() {
        canvas = GameObject.Find("Canvas");
        EventSystem.current.IsPointerOverGameObject();
    }

    public void GoToLevel() {
        Debug.Log(SceneManager.GetActiveScene().name + " going to level " + levelName);
        DontDestroyOnLoad(canvas);
        SceneManager.LoadScene(levelName);
        AudioSource audio = GetComponent<AudioSource>();
        if (audio)
            audio.Play();
        StartCoroutine(DestroyOnLoad());
    }

    private IEnumerator DestroyOnLoad() {
        yield return new WaitForSeconds(0.5f);
        Destroy(canvas);
    }

    GameObject lastSelected;
    void OnGUI() {
        Event e = Event.current;
        if (e.isMouse) {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(lastSelected);
            }
            else
            {
                lastSelected = EventSystem.current.currentSelectedGameObject;
            }
        }
    }
}
