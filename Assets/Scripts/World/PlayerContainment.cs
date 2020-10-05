using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerContainment : MonoBehaviour
{
    private GameObject player;
    private GameObject[] playerGhosts;
    private Camera mainCamera;
    private CinemachineVirtualCamera cmCamera;
    private BoxCollider2D screenCollider;
    private GameObject leftRewind;
    private GameObject rightRewind;
    private GameObject leftStop;
    private GameObject rightStop;

    [SerializeField]
    private LayerMask levelLayer;

    Vector2 screenDimensions;
    // Rect corners = new Rect();

    private GameObject lastEnteredGhost;
    [SerializeField]
    GameObject activeGhostPrefab;


    [SerializeField]
    GameObject leftBorder;

    [SerializeField]
    GameObject rightBorder;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerGhosts = GameObject.FindGameObjectsWithTag("PlayerGhost");
        if (playerGhosts.Length < 8)
            Debug.LogError("Insufficient amount of ghosts! (8 required)");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Camera>();
        cmCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CinemachineVirtualCamera>();
        screenCollider = GetComponent<BoxCollider2D>();
        leftRewind = transform.Find("LeftRewind").gameObject;
        rightRewind = transform.Find("RightRewind").gameObject;
        leftStop = transform.Find("LeftStop").gameObject;
        rightStop = transform.Find("RightStop").gameObject;
        AdjustCollider();
    }

    // Update is called once per frame
    void Update()
    {
        if (screenDimensions != new Vector2(Screen.width, Screen.height))
            AdjustCollider();
        transform.position = cmCamera.transform.position;
    }

    private void AdjustCollider() {
        screenDimensions = new Vector2(Screen.width, Screen.height);
//        Debug.Log(mainCamera.orthographicSize + " - " + screenDimensions + " - " + mainCamera.ScreenToWorldPoint(screenDimensions));
        // corners = new Rect(mainCamera.ScreenToWorldPoint(mainCamera.pixelRect.position), mainCamera.ScreenToWorldPoint(mainCamera.pixelRect.size) * 2);
        screenCollider.size = new Vector2(cmCamera.m_Lens.OrthographicSize * cmCamera.m_Lens.Aspect, cmCamera.m_Lens.OrthographicSize) * 2;
        // Positioning ghosts
        playerGhosts[0].transform.position = new Vector2(player.transform.position.x, player.transform.position.y) - screenCollider.size;
        playerGhosts[1].transform.position = new Vector2(player.transform.position.x, player.transform.position.y) + screenCollider.size;
        playerGhosts[2].transform.position = new Vector2(player.transform.position.x, player.transform.position.y - screenCollider.size.y);
        playerGhosts[3].transform.position = new Vector2(player.transform.position.x, player.transform.position.y + screenCollider.size.y);
        playerGhosts[4].transform.position = new Vector2(player.transform.position.x - screenCollider.size.x, player.transform.position.y);
        playerGhosts[5].transform.position = new Vector2(player.transform.position.x + screenCollider.size.x, player.transform.position.y);
        playerGhosts[6].transform.position = new Vector2(player.transform.position.x - screenCollider.size.x, player.transform.position.y + screenCollider.size.y);
        playerGhosts[7].transform.position = new Vector2(player.transform.position.x + screenCollider.size.x, player.transform.position.y - screenCollider.size.y);
        // Fake walls
        leftBorder.transform.position = new Vector2(screenCollider.transform.position.x - screenCollider.size.x /2 , screenCollider.transform.position.y);
        rightBorder.transform.position = new Vector2(screenCollider.transform.position.x + screenCollider.size.x /2 , screenCollider.transform.position.y);
        leftBorder.GetComponent<BoxCollider2D>().size = new Vector2(1,screenCollider.size.y);
        rightBorder.GetComponent<BoxCollider2D>().size = new Vector2(1,screenCollider.size.y);
        ToggleFakeWall(false);
        // GUI Elements
        leftRewind.transform.position = new Vector3(leftBorder.transform.position.x + 1.2f, 0, 20);
        rightRewind.transform.position = new Vector3(rightBorder.transform.position.x - 1.2f, 0, 20);
        leftStop.transform.position = new Vector3(leftBorder.transform.position.x + 1.2f, 0, 20);
        rightStop.transform.position = new Vector3(rightBorder.transform.position.x - 1.2f, 0, 20);
        // Enforced ratio
        AspectUtility.SetCamera();
    }

    void OnTriggerEnter2D(Collider2D collider) {
        // Debug.Log("Enter " + collider.name);
        if (collider.CompareTag("PlayerGhost")) {
            if(canEnter(collider)){
                GameObject newActiveGhost = Instantiate(activeGhostPrefab, collider.transform.position, collider.transform.rotation, player.transform); //Instantiate(collider.gameObject, collider.transform.position, collider.transform.rotation, player.transform);
                newActiveGhost.tag = "ActiveGhost";
                lastEnteredGhost = collider.gameObject;
                // Debug.Log("Last entered ghost is now " + collider.name);
                PurgateActiveGhosts();
            } else {
                ToggleFakeWall(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        Debug.Log("Exiting collider: " + collider.name);
        if (collider.CompareTag("Player") && lastEnteredGhost) {
            player.transform.position = lastEnteredGhost.transform.position;
            // Debug.Log("Player taking " + lastEnteredGhost.name + " place");
        } else if (collider.CompareTag("ActiveGhost")) {
            Destroy(collider.gameObject);
        } else if (collider.CompareTag("PlayerGhost")) {
            ToggleFakeWall(false);
        }
    }

    private bool canEnter(Collider2D collider){
        RaycastHit2D hit = Physics2D.Raycast(collider.transform.position,Vector3.forward,20f,levelLayer);
        if(hit){
            return false;
        } 
        return true;
    }

    public void PurgateActiveGhosts() {
        GameObject[] activeGhosts = GameObject.FindGameObjectsWithTag("ActiveGhost");
        foreach(GameObject activeGhost in activeGhosts) {
            Vector2 ghostPosition = activeGhost.transform.position;
            Vector2 ghostSize = activeGhost.GetComponent<SpriteRenderer>().bounds.size;
            if ((ghostPosition.x + ghostSize.x / 2 < leftBorder.transform.position.x || ghostPosition.x - ghostSize.x / 2 > rightBorder.transform.position.x)
                    || (ghostPosition.y + ghostSize.y / 2 < leftBorder.transform.position.y || ghostPosition.y - ghostSize.y / 2 > rightBorder.transform.position.y)) {
                Destroy(activeGhost);
                // Debug.Log("Purgated an active ghost!");
            }
        }
    }

    private void ToggleFakeWall(bool enable){
        leftBorder.SetActive(enable);
        rightBorder.SetActive(enable);
    }
}
