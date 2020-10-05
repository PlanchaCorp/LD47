using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelLoading : MonoBehaviour {

    private AudioSource soundtrack = null;
    public bool canGoNext = false;

    public void Start() {
        GameObject.Find("GameMenu").GetComponent<InGameMenuBehaviour>().ToggleState("play");
        GameObject soundtrackObject = GameObject.FindGameObjectWithTag("Soundtrack");
        if (soundtrackObject) {
        soundtrack = soundtrackObject.GetComponent<AudioSource>();
        }
    }

    public void Update() {
        if(Input.GetButtonDown("Restart")){
            Reload();
        }
        if(Input.GetButtonDown("Cancel")){
            Menu();
        }
        if(Input.GetButtonDown("Submit") && canGoNext){
            Next();
        }
    }
    public void Reload() {
        FixMusic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Next(){
        FixMusic();
        var len = SceneManager.sceneCountInBuildSettings -1;
        if(SceneManager.GetActiveScene().buildIndex >= len) {
            Menu();
        } else {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    public void Menu(){
        FixMusic();
        SceneManager.LoadScene("MainMenu");
    }

    private void FixMusic() {
        if (soundtrack) {
            soundtrack.pitch = 1;
            soundtrack.volume = 0.7f;
        }
    }
    
}