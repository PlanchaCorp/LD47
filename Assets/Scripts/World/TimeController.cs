using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeController: MonoBehaviour
{
    private GameObject player;
    private GameObject cameraGroup;
    private CameraController cameraController;
    private InGameMenuBehaviour menuBehaviour;
    private PlayerContainment playerContainment;
    private List<Vector2> playerPositions;
    private List<Vector3> playerRotations;
    private List<float> cameraPositions;

    private AudioSource rewindSound;
    private AudioSource rewindStopSound;
    private AudioSource backgroundMusic = null;

    private Vector2 playerFirstPosition;

    private int SPEED_MULTIPLIER = 3;
    private float SOUND_LOOP_TIME = 2.8f;
    private bool isReversing = false;
    
    private void Start()
    {
        playerPositions = new List<Vector2>();
        playerRotations = new List<Vector3>();
        cameraPositions = new List<float>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerFirstPosition = player.transform.position;
        cameraGroup = GameObject.Find("CameraGroup");
        cameraController = cameraGroup.GetComponent<CameraController>();
        menuBehaviour = GameObject.Find("GameMenu").GetComponent<InGameMenuBehaviour>();
        playerContainment = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<PlayerContainment>();
        rewindSound = GetComponent<AudioSource>();
        rewindStopSound = transform.Find("RewindStop").GetComponent<AudioSource>();
        GameObject soundtrack = GameObject.FindGameObjectWithTag("Soundtrack");
        if (soundtrack)
            backgroundMusic = soundtrack.GetComponent<AudioSource>();
    }

    private void Update() {
        if (isReversing && !rewindSound.isPlaying) {
            rewindSound.time = SOUND_LOOP_TIME;
            rewindSound.Play();
        }
    }
    
    private void FixedUpdate()
    {
        if (!player)
            return;
        if(!isReversing) {
            playerPositions.Add(player.transform.position);
            playerRotations.Add(player.transform.localEulerAngles);
            cameraPositions.Add(cameraController.transform.position.x);
        } else {
            for (int i = 0; i < SPEED_MULTIPLIER; i++) {
                if (playerPositions.Count == 0)
                    break;
                player.transform.position = (Vector3) playerPositions[playerPositions.Count - 1];
                playerPositions.RemoveAt(playerPositions.Count - 1);
        
                player.transform.localEulerAngles = (Vector3) playerRotations[playerRotations.Count - 1];
                playerRotations.RemoveAt(playerRotations.Count - 1);

                cameraGroup.transform.position = new Vector2(cameraPositions[cameraPositions.Count - 1], cameraGroup.transform.position.y);
                cameraPositions.RemoveAt(cameraPositions.Count - 1);
            }
            if (playerPositions.Count == 0)
                EndRewind();
        }
    }

    public void StartRewind() {
        rewindSound.Play();
        isReversing = true;
        cameraController.canMove = false;
        menuBehaviour.ToggleState("pause");
        if (backgroundMusic) {
            backgroundMusic.pitch = -SPEED_MULTIPLIER;
            backgroundMusic.volume = 0.5f;
        }
    }

    public void EndRewind() {
        rewindSound.Stop();
        isReversing = false;
        cameraController.canMove = true;
        player.transform.position = playerFirstPosition;
        menuBehaviour.ToggleState("play");
        rewindStopSound.Play();
        playerContainment.PurgateActiveGhosts();
        if (backgroundMusic) {
            backgroundMusic.pitch = 1;
            backgroundMusic.volume = 0.7f;
        }
    }
}