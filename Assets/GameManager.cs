using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region PUBLIC_VAR
    public TextMeshProUGUI playerCounter;
    public TextMeshProUGUI zombieLifeCounter;
    public GameObject overlayPanel;
    public GameObject playerQueuePanel;
    public GameObject gameOverPanel;
    public GameObject alliesStagePanel;
    public Button backToMenuBtn;
    public Button gameOverBackToMenuBtn;
    public AudioClip shotgunFiredClip;
    public AudioClip missedClip;
    public AudioSource audioSource;
    #endregion

    #region PRIVATE_VAR
    private PlayerQueueHandler playerQueueHandler;
    private AlliesStageHandler alliesStageHandler;

    private int zombieHealth = 0;
    private int stage;
    private int health;
    private int playerTurnsPerStage;

    private bool isInitDone = false;
    private bool hasPlayerTurnEnded = true;
    private bool hasZombieDied = true;
    #endregion

    //TEMP
    public TextMeshProUGUI playerThrowCounter;
    public TextMeshProUGUI stageCounter;
    public TextMeshProUGUI healthCounter;
    public TextMeshProUGUI alliesHitArea;

    void Start()
    {
        initGame();
    }

    void Update()
    {
        if (hasPlayerTurnEnded) {
            StartCoroutine(playerRoutine(GameState.playingQueue.Peek()));
            hasPlayerTurnEnded = false;
        }
    }        

    #region INITIALIZATION
    private void initGame() {
        stage = 1;
        GameState.playerList = initPlayerList(GameState.numOfPlayer);
        GameState.playingQueue = initPlayingQueue(GameState.playerList);
        initZombieLifeCounter(GameState.playerList.Count, stage);
        initPlayerHealth(GameState.playerList.Count);
        playerQueuePanel.SetActive(true);
        playerQueueHandler = playerQueuePanel.GetComponent<PlayerQueueHandler>();
        alliesStageHandler = alliesStagePanel.GetComponent<AlliesStageHandler>();

        backToMenuBtn.onClick.AddListener(backToMenuOnClick);
        gameOverBackToMenuBtn.onClick.AddListener(backToMenuOnClick);
        
        isInitDone = true;
    }

    private void initPlayerHealth(int playerCount) {
        health = playerCount;
        playerTurnsPerStage = playerCount;
        healthCounter.SetText(health.ToString());
    }

    private void initZombieLifeCounter(int playerCount, int stage) {
        zombieHealth = Random.Range(20, 60)/2*stage*playerCount;
        zombieLifeCounter.SetText(zombieHealth.ToString());
    }

    private List<Player> initPlayerList(int numOfPlayer) {
        List<Player> temp = new List<Player>();
        for (int i = 0; i < numOfPlayer; i++) {
            temp.Add(new Player(i));
        }
        return temp;
    }

    private Queue<Player> initPlayingQueue(List<Player> playerList) {
        Queue<Player> temp = new Queue<Player>();
        foreach(Player p in playerList) {
            temp.Enqueue(p);
        }
        return temp;
    }
    #endregion

    #region GAME_LOGICS
    IEnumerator playerRoutine(Player p) {
        playerTurnsPerStage--;
        stageCounter.SetText(stage + " / 10");       

        if (!isAllyStage()) {
            while (p.haveTurnLeft() && zombieHealth > 0) {
                registerDartInput(p, false);
                playerThrowCounter.SetText(p.numOfTurns + " / 3");
                yield return null;
            }
            overlayPanel.SetActive(true);

            yield return new WaitUntil(() => (!p.haveTurnLeft() || zombieHealth <= 0) && Input.GetKeyDown(GameState.nextPlayerBtnKeyCode));

            if (zombieHealth <= 0) {
                stage++;
                playerTurnsPerStage = GameState.playerList.Count;
                initZombieLifeCounter(GameState.playerList.Count, stage);
                playerQueueHandler.playerEndTurn(); // move current player to the last
                playerQueueHandler.requeueWaitingPlayers();
            } else if (playerTurnsPerStage <= 0) {
                zombieAttack();

                if (health <= 0) {
                    gameOverPanel.SetActive(true);
                } else {
                    playerQueueHandler.requeueWaitingPlayers();
                }
            } else {
                playerQueueHandler.playerEndTurn();
            }

            overlayPanel.SetActive(false);
            p.endTurn();
            hasPlayerTurnEnded = true;
        } else {
            ////  ALLIES STAGE
            if (!alliesStageHandler.isInitDone()) {
                alliesStagePanel.SetActive(true);
                alliesStageHandler.initAliiesStage();
            }


            //TODO: HIT RED DID NOT END THE ALLY STAGE
            //TODO: FINISH GREEN ALLY STAGE OVERLAY REMOVED BUT STUCK WITH NEXT PLAYER OVERLAY
            while (p.haveTurnLeft() && (alliesStageHandler.hasGreenLeft() && alliesStageHandler.hasRedLeft())) {
                registerDartInput(p, true);
                playerThrowCounter.SetText(p.numOfTurns + " / 3");

                alliesHitArea.SetText("");
                Debug.Log(GameState.alliesStageGreen.Count > 0);
                foreach (List<int> innerList in GameState.alliesStageGreen) {
                    foreach (int g in innerList) {
                        alliesHitArea.text += g + " ";
                    }
                    alliesHitArea.text += "\r\n";
                }

                foreach (List<int> innerList in GameState.alliesStageRed) {
                    alliesHitArea.text += "**";
                    foreach (int r in innerList) {
                        alliesHitArea.text += r + " ";
                    }
                    alliesHitArea.text += "**\r\n";
                }

                yield return null;
            }
            overlayPanel.SetActive(true);

            yield return new WaitUntil(() => (!p.haveTurnLeft() || zombieHealth <= 0 || GameState.alliesStageGreen.Count == 0 || GameState.alliesStageRed.Count == 0) && Input.GetKeyDown(GameState.nextPlayerBtnKeyCode));

            // TURN OFF ALLIES STAGE PANEL AFTER ALL PLAY FINISH TURNS

            if (playerTurnsPerStage <= 0) {
                stage++;
                alliesStagePanel.SetActive(false);
                playerQueueHandler.requeueWaitingPlayers();
            } else {
                playerQueueHandler.playerEndTurn();
            }

            overlayPanel.SetActive(false);
            p.endTurn();
            hasPlayerTurnEnded = true;
        }        
    }

    private void zombieAttack() {
        health--;
        healthCounter.SetText(health.ToString());
    }

    private void registerDartInput(Player p, bool isAlliesStage) {
        if (Input.anyKeyDown) {
            string input = Input.inputString;            

            for (int i = 0; i < GameState.pointLUT.Count; i++) {
                if (GameState.pointLUT[i].Contains(input)) {

                    playSoundClip(shotgunFiredClip);

                    if (!isAlliesStage) {
                        zombieHealth -= i == 0 ? 50 : (GameState.pointLUT[i].IndexOf(input) + 1) * (i == 0 ? 25 : (i == 4) ? 1 : i);
                        zombieLifeCounter.SetText(zombieHealth.ToString());

                        p.tookATurn();
                    } else {
                        //ALLIES STAGE
                        int basePointFromLUT = i == 0 ? 50 : GameState.pointLUT[i].IndexOf(input) + 1;

                        Debug.Log(basePointFromLUT);

                        bool hitGreen = false;
                        bool hitRed = false;

                        for (int g = 0; g < GameState.alliesStageGreen.Count; g++) {
                            if (GameState.alliesStageGreen[g].Contains(basePointFromLUT)) {
                                GameState.alliesStageGreen.RemoveAt(g);
                                hitGreen = true;
                            }
                        }

                        for (int r = 0; r < GameState.alliesStageRed.Count; r++) {
                            if (GameState.alliesStageRed[r].Contains(basePointFromLUT)) {
                                GameState.alliesStageRed.RemoveAt(r);
                                hitRed = true;
                            }
                        }
                        if (hitRed) {
                            p.tookAllTurns();
                        } else {
                            if (!hitGreen)
                            {
                                playSoundClip(missedClip);
                            }
                            p.tookATurn();
                        }
                    }
                }
            }  
        }
    }

    private void playSoundClip(AudioClip ac)
    {
        audioSource.clip = ac;
        audioSource.Play();
    }

    private bool isAllyStage()
    {
        return stage == 3 || stage == 8;
    }    
    #endregion

    #region UI_LOGICS
    void backToMenuOnClick() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    #endregion
}
