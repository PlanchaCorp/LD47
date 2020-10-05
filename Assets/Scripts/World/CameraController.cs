using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    // TODO: Use level data for camera limit
    [SerializeField]
    private float LEFT_LIMIT = 0;
    [SerializeField]
    private float RIGHT_LIMIT = 30;
    private float calculatedLeftLimit, calculatedRightLimit;
    [SerializeField]
    private float CAMERA_SPEED = 5;

    public bool canMove = true;
    private AudioSource squeakSource, tapeBeginSource, tapeEndSource;

    private SpriteRenderer leftRewind, rightRewind, leftStop, rightStop;


    // Start is called before the first frame update
    void Start()
    {
        Vector2 cameraOrigin = transform.position;
        calculatedLeftLimit = LEFT_LIMIT + cameraOrigin.x;
        calculatedRightLimit = RIGHT_LIMIT + cameraOrigin.x;
        squeakSource = transform.Find("SqueakSound").GetComponent<AudioSource>();
        tapeBeginSource = transform.Find("TapeBeginSound").GetComponent<AudioSource>();
        tapeEndSource = transform.Find("TapeEndSound").GetComponent<AudioSource>();

        GameObject[] guiElements = GameObject.FindGameObjectsWithTag("GUIElement");
        foreach(GameObject guiElement in guiElements) {
            if (guiElement.name == "LeftRewind")
                leftRewind = guiElement.GetComponent<SpriteRenderer>();
            else if (guiElement.name == "RightRewind")
                rightRewind = guiElement.GetComponent<SpriteRenderer>();
            else if (guiElement.name == "LeftStop")
                leftStop = guiElement.GetComponent<SpriteRenderer>();
            else if (guiElement.name == "RightStop")
                rightStop = guiElement.GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove){
            float horizontalMovement = Input.GetAxis("CameraHorizontal");
            if (horizontalMovement != 0) {
                bool isAlreadyAtLimit = transform.position.x <= calculatedLeftLimit || transform.position.x >= calculatedRightLimit;
                transform.Translate(new Vector2(horizontalMovement * CAMERA_SPEED * Time.deltaTime, 0));
                if (transform.position.x < calculatedLeftLimit) {
                    transform.position = new Vector2(calculatedLeftLimit, transform.position.y);
                    StopSoundEffect(squeakSource);
                    if (!isAlreadyAtLimit)
                        tapeBeginSource.Play();
                    EnableElement(leftStop);
                } else if (transform.position.x > calculatedRightLimit) {
                    transform.position = new Vector2(calculatedRightLimit, transform.position.y);
                    StopSoundEffect(squeakSource);
                    if (!isAlreadyAtLimit)
                        tapeEndSource.Play();
                    EnableElement(rightStop);
                } else {
                    StartSoundEffect(squeakSource);
                    if (horizontalMovement < 0)
                        EnableElement(leftRewind);
                    else
                        EnableElement(rightRewind);
                }
            } else {
                StopSoundEffect(squeakSource);
                EnableElement(null);
            }
        }
    }
    void StartSoundEffect(AudioSource soundEffect) {
        soundEffect.mute = false;
    }

    void StopSoundEffect(AudioSource soundEffect) {
        soundEffect.mute = true;
    }

    void EnableElement(SpriteRenderer element) {
        leftRewind.enabled = false;
        rightRewind.enabled = false;
        leftStop.enabled = false;
        rightStop.enabled = false;
        if (element)
            element.enabled = true;
    }
}
