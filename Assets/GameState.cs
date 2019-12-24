using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {
    public static List<string> bulls = new List<string>() {"1", "2" };          // (i+1)*25
    public static List<string> oneTimes = new List<string>() { "3", "4"};       // (i+1)*1
    public static List<string> twoTimes = new List<string>() { "5","6" };       // (i+1)*2
    public static List<string> threeTimes = new List<string>() { "7", "8" };    // (i+1)*3
    public static List<List<string>> pointLUT = new List<List<string>>() { bulls, oneTimes, twoTimes, threeTimes };

    public static KeyCode nextPlayerBtnKeyCode = KeyCode.Space;

    public static int numOfPlayer;

    public static List<Player> playerList;
    public static Queue<Player> playingQueue;
    public static Queue<Player> waitingQueue;

    public static List<int> boardPointsOrder = new List<int>() { 20, 1, 18, 4, 13, 6, 10, 15, 2, 17, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5 };

    public static List<List<int>> alliesStageGreen;
    public static List<List<int>> alliesStageRed;
}
