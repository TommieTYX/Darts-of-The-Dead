using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlliesStageHandler : MonoBehaviour
{
    private bool isDoneInit = false;

    void Start() {
        
    }

    void Update() {
        
    }

    public void initAliiesStage() {
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
        if (GameState.alliesStageGreen.Count == 4) {
            Debug.Log("Init GREEN... DONE");
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
        if (GameState.alliesStageRed.Count == 1) {
            Debug.Log("Init RED... DONE");
        }

        isDoneInit = true;
    }

    public bool isInitDone() {
        return isDoneInit;
    }

    public bool hasGreenLeft() {
        return GameState.alliesStageGreen.Count > 0;
    }

    public bool hasRedLeft() {
        return GameState.alliesStageRed.Count > 0;
    }
}
