using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenuBehaviour : MonoBehaviour
{
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    private GameObject play;
    [SerializeField]
    private GameObject rewind;
    [SerializeField]
    private GameObject stop;

    private TimeController timeController;
    
    private float startTime;

    private bool isPaused; 
    
    [SerializeField]
    private TMPro.TMP_Text timerText;

    private float currentTime = 0f;
    [SerializeField]
    private TMPro.TMP_Text helpText;


    
    public void Start(){
        startTime = Time.time;
        var camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Camera>();
        canvas.worldCamera = camera;
    }

    public void ToggleState(string state){
        play.SetActive(false);
        rewind.SetActive(false);
        stop.SetActive(false);
        switch(state){
            case "play":
                startTime = Time.time;
                play.SetActive(true);
                isPaused = false;
                helpText.enabled = false;
                break;
            // This is player death behaviour /!\
            case "pause":
                rewind.SetActive(true);
                isPaused = true;
                helpText.enabled =true;
                helpText.text = "Press \"R\" to replay, \"Esc\" to go to Main menu";
                break;
            // This is player win behaviour /!\
            case "stop":
                stop.SetActive(true);
                isPaused = true;
                helpText.enabled = true;
                helpText.text = "Press \"Enter\" to move to Next Level, \"Esc\" to go to Main menu";
                if (PlayerPrefs.GetFloat("BestTime: " + SceneManager.GetActiveScene().name, 3600) > currentTime)
                    PlayerPrefs.SetFloat("BestTime: " + SceneManager.GetActiveScene().name, currentTime);
                break;
        }
    }


    public void OnGUI () { 
        if(!isPaused){
            currentTime = Time.time - startTime;
        }
        var minutes  = currentTime / 60;
        var seconds  = currentTime % 60;
        var fraction  = (currentTime * 1000) % 1000;
        string text = string.Format ("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction); 
        timerText.text = text;
        
    }
}
