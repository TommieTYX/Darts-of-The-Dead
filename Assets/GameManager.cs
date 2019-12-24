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

        //DEBUG
        debugButtons();

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

        if (!(stage == 3 || stage == 8)) {
            while (p.haveTurnLeft() && zombieHealth > 0) {
                registerDartInput(p, false);
                playerThrowCounter.SetText(p.numOfTurns + " / 3");
                yield return null;
            }

            overlayPanel.SetActive(true);
            yield return new WaitUntil(() => Input.GetKeyDown(GameState.nextPlayerBtnKeyCode) || debug4Press);

            debug4Press = false;

            if (zombieHealth <= 0) {
                stage++;
                playerTurnsPerStage = GameState.playerList.Count;
                initZombieLifeCounter(GameState.playerList.Count, stage);
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

            while (p.haveTurnLeft() && alliesStageHandler.hasGreenLeft()) {
                registerDartInput(p, true);
                playerThrowCounter.SetText(p.numOfTurns + " / 3");
                yield return null;
            }







            overlayPanel.SetActive(true);
            yield return new WaitUntil(() => Input.GetKeyDown(GameState.nextPlayerBtnKeyCode) || debug4Press);

            debug4Press = false;

            if (zombieHealth <= 0) {
                stage++;
                playerTurnsPerStage = GameState.playerList.Count;
                initZombieLifeCounter(GameState.playerList.Count, stage);
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
                    if (!isAlliesStage) {
                        zombieHealth -= (GameState.pointLUT[i].IndexOf(input) + 1) * (i == 0 ? 25 : i);
                        zombieLifeCounter.SetText(zombieHealth.ToString());

                        p.tookATurn();
                    } else {
                        //ALLIES STAGE
                        int basePointFromLUT = (GameState.pointLUT[i].IndexOf(input) + 1) * (i == 0 ? 25 : i);
                        bool hasThrown = false;

                        for (int g = 0; g < GameState.alliesStageGreen.Count; g++) {
                            if (GameState.alliesStageGreen[g].Contains(basePointFromLUT)) {
                                GameState.alliesStageGreen.RemoveAt(g);
                            }
                        }

                        for (int r = 0; r < GameState.alliesStageRed.Count; r++) {
                            if (GameState.alliesStageGreen[r].Contains(basePointFromLUT)) {
                                GameState.alliesStageGreen.RemoveAt(r);
                            }
                        }

                        hasThrown = true; //else count as missed
                        p.tookATurn();

                        //Debug.Log("HIT GREEN2 "+ Input.inputString + " ----  " + System.Int32.Parse(Input.inputString));
                        Debug.Log((GameState.pointLUT[i].IndexOf(input) + 1) * (i == 0 ? 25 : i));
                    }
                }
            }  
        }

        //DEBUG
        if (debug1Press) {
            zombieHealth -= 20;
            zombieLifeCounter.SetText(zombieHealth.ToString());

            p.tookATurn();
            debug1Press = false;
        }
        if (debug2Press) {
            zombieHealth -= 40;
            zombieLifeCounter.SetText(zombieHealth.ToString());

            p.tookATurn();
            debug2Press = false;
        }
        if (debug3Press) {
            zombieHealth -= 60;
            zombieLifeCounter.SetText(zombieHealth.ToString());

            p.tookATurn();
            debug3Press = false;
        }
    }


    

    private void alliesStage() {
        //INIT
        GameState.alliesStageGreen = new List<List<int>>();
        GameState.alliesStageRed = new List<List<int>>();

        //GREEN QUADRANT
        int rangeStart = Random.Range(0, 19);
        for (int i = 0; i < 4; i++) {
            List<int> tempList = new List<int>() {
                    GameState.boardPointsOrder[rangeStart = CommonUtils.getNextInt_wrappedAround(0, 19, rangeStart)],
                    GameState.boardPointsOrder[rangeStart = CommonUtils.getNextInt_wrappedAround(0, 19, rangeStart)],
                    GameState.boardPointsOrder[rangeStart = CommonUtils.getNextInt_wrappedAround(0, 19, rangeStart)]
                };

            rangeStart = CommonUtils.getNextInt_wrappedAround(0, 19, rangeStart);
            GameState.alliesStageGreen.Add(tempList);
        }

        //RED QUADRANT
        for (int i = 0; i < 1; i++) {
            List<int> tempList = new List<int>() {
                    GameState.boardPointsOrder[rangeStart = CommonUtils.getNextInt_wrappedAround(0, 19, rangeStart)],
                    GameState.boardPointsOrder[rangeStart = CommonUtils.getNextInt_wrappedAround(0, 19, rangeStart)],
                    GameState.boardPointsOrder[rangeStart = CommonUtils.getNextInt_wrappedAround(0, 19, rangeStart)]
                };

            rangeStart = CommonUtils.getNextInt_wrappedAround(0, 19, rangeStart);
            GameState.alliesStageRed.Add(tempList);
        }
    }


    
    #endregion

    #region UI_LOGICS
    void backToMenuOnClick() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    #endregion

    #region DEBUG
    public Button debug1;
    public Button debug2;
    public Button debug3;
    public Button debug4;

    private bool debug1Press = false;
    private bool debug2Press = false;
    private bool debug3Press = false;
    private bool debug4Press = false;

    void debugButtons() {
        debug1.onClick.AddListener(minus20hp);
        debug2.onClick.AddListener(minus40hp);
        debug3.onClick.AddListener(minus60hp);
        debug4.onClick.AddListener(debugNext);
    }
    void minus20hp() { debug1Press = true; }
    void minus40hp() { debug2Press = true; }
    void minus60hp() { debug3Press = true; }
    void debugNext() { debug4Press = true; }
    #endregion
}
