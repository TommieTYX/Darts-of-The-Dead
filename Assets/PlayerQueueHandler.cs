using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerQueueHandler : MonoBehaviour
{
    public GameObject playerQueueIcon;
    public float iconUpdateSpeedMultiplier = 4.0f;

    private float[] queueIconPositionX = new float[] { 500, 1020, 1540, 2060 };
    //private List<GameObject> iconList;

    private bool requiredUpdate = false;

    List<Player> currentPlayerList;    

    //TEMP
    public TextMeshProUGUI playerLabel;

    void Start()
    {
        GameState.playingQueue = new Queue<Player>();
        GameState.waitingQueue = new Queue<Player>();
        GameState.playingQueue = initPlayingQueue(GameState.playerList);

        currentPlayerList = GameState.playerList;
        requiredUpdate = true;
        /*icon1 = Instantiate(playerQueueIcon, new Vector2(3000, 0), Quaternion.identity) as GameObject;
        icon1.transform.SetParent(transform);

        icon2 = Instantiate(playerQueueIcon, new Vector2(3000, 0), Quaternion.identity) as GameObject;
        icon2.transform.SetParent(transform);

        icon3 = Instantiate(playerQueueIcon, new Vector2(3000, 0), Quaternion.identity) as GameObject;
        icon3.transform.SetParent(transform);

        icon4 = Instantiate(playerQueueIcon, new Vector2(3000, 0), Quaternion.identity) as GameObject;
        icon4.transform.SetParent(transform);*/
    }

    // Update is called once per frame
    void Update()
    {
        if (requiredUpdate) {
            //Debug.Log("Updating icons...");
            //updatePlayerIcons();
            
            requiredUpdate = false;
        }

        //TEMP
        if (GameState.playingQueue.Count > 0) {
            playerLabel.SetText("P" + (GameState.playingQueue.Peek().id + 1));
        }




        //Gap between icon 120,0
        //Gap from start to first icon is 500,0
        /*icon1.transform.position = Vector2.Lerp(icon1.transform.position,
            new Vector2(500, 0), Time.deltaTime * 4f);

        icon2.transform.position = Vector2.Lerp(icon2.transform.position,
            new Vector2(1020, 0), Time.deltaTime * 4f);

        icon3.transform.position = Vector2.Lerp(icon3.transform.position,
            new Vector2(1540, 0), Time.deltaTime * 4f);

        icon4.transform.position = Vector2.Lerp(icon4.transform.position,
            new Vector2(2060, 0), Time.deltaTime * 4f);*/
    }

   

    public void updateQueue() {
        requiredUpdate = true;
    }

    public void playerEndTurn() {
        if (GameState.playingQueue.Count > 0) {
            GameState.waitingQueue.Enqueue(GameState.playingQueue.Dequeue());
        } else {
            requeueWaitingPlayers();
        }
        
        updateQueue();
    }

    public void requeueWaitingPlayers() {
        while (GameState.waitingQueue.Count > 0) {
            GameState.playingQueue.Enqueue(GameState.waitingQueue.Dequeue());
        }

        updateQueue();
    }






    private Queue<Player> initPlayingQueue(List<Player> playerList) {
        Queue<Player> temp = new Queue<Player>();
        foreach (Player p in playerList) {
            temp.Enqueue(p);
            initPlayerIcon(p);
        }
        return temp;
    }

    private void initPlayerIcon(Player p) {
        p.icon = Instantiate(playerQueueIcon, new Vector2(3000, 0), Quaternion.identity) as GameObject;
        p.icon.name = "player-" + p.id + "-icon";
        p.icon.transform.SetParent(transform);
    }

    private void updatePlayerIcons() {
        Queue<Player> tempQueue = new Queue<Player>();
        int currentPlayingQueueCount = GameState.playingQueue.Count;

        for (int i = 0; i < currentPlayingQueueCount; i++) {
            Player p = GameState.playingQueue.Dequeue();

            Vector3 currentPosition = p.icon.transform.position;
            StartCoroutine(updateIconPos(p, new Vector2(queueIconPositionX[i], 0)));

            tempQueue.Enqueue(p);
        }
        GameState.playingQueue = tempQueue;
    }

    IEnumerator updateIconPos(Player p, Vector2 newPos) {
        Debug.Log("Updating P" + p.id);

        while (!(p.icon.transform.position.x <= newPos.x+1 && p.icon.transform.position.x >= newPos.x - 1)) {
        p.icon.transform.position = Vector2.Lerp(p.icon.transform.position,
            newPos, Time.deltaTime * 4f);
            yield return null;
        }
        yield return new WaitForSeconds(0);

        Debug.Log("DONE Updating P" + p.id);
    }
}
