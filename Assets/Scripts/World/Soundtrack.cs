using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundtrack : MonoBehaviour
{
    private float MUSIC_START_DELAY = 1.6f;

    void Start() {
        if (GameObject.FindGameObjectsWithTag("Soundtrack").Length == 1) {
            DontDestroyOnLoad(gameObject);
            StartCoroutine(BeginMusic());
            transform.Find("TapeInsertion").GetComponent<AudioSource>().Play();
        }
    }

    private IEnumerator BeginMusic() {
        yield return new WaitForSeconds(MUSIC_START_DELAY);
        GetComponent<AudioSource>().Play();
    }
}
