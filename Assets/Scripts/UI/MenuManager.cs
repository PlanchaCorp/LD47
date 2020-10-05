using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private string[] levelNames;

    void Start()
    {
        Transform menuItemModel = transform.Find("MenuItemModel");
        bool eventTargetInitialized = false;
        foreach(string levelName in levelNames) {
            GameObject newMenuItem = Instantiate(menuItemModel.gameObject, transform);
            newMenuItem.name = levelName;
            newMenuItem.GetComponent<MenuController>().SetLevelName(levelName);
            newMenuItem.transform.Find("LevelNameText").GetComponent<TextMeshProUGUI>().text = levelName;
            newMenuItem.SetActive(true);
            float bestTime = PlayerPrefs.GetFloat("BestTime: " + levelName, 0);
            if (!eventTargetInitialized) {
                eventTargetInitialized = true;
                EventSystem.current.SetSelectedGameObject(newMenuItem);
            }
            if (bestTime > 0) {
                int minutes = (int) bestTime / 60;
                int seconds = (int) bestTime % 60;
                int milliseconds = (int) ((bestTime - minutes * 60 - seconds) * 1000);
                string displayedTime = minutes.ToString("d2") + ":" + seconds.ToString("d2") + "." + milliseconds.ToString("d2");
                newMenuItem.transform.Find("LevelBestTime").GetComponent<TextMeshProUGUI>().text = displayedTime;
            }
        }
    }
}
