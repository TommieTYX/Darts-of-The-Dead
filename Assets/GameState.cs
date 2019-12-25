using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static List<string> bulls = new List<string>() { "J", "O" };                                                                                                     // (i+1)*25
    public static List<string> oneTimesInner = new List<string>() { "$", "8", "Q", "£", "3", "F", "[", "(", "*", "1", "n", "P", ";", "N", "Z", "_", "T", "C", "6", "j" };   // (i+1)*1
    public static List<string> twoTimes = new List<string>() { "<", "r", "/", "S", "g", "2", "@", "e", "!", "b", "]", "K", "0", "W", "V", "-", "t", "c", "9", "H" };        // (i+1)*2
    public static List<string> threeTimes = new List<string>() { "l", "d", "7", "^", "h", "k", "p", "f", "i", "&", "U", ":", "4", "D", "L", "#", "+", "%", "'", "X" };      // (i+1)*3
    public static List<string> oneTimesOuter = new List<string>() { "Y", "o", "\"", "a", "?", "G", "5", "I", "s", "m", "=", "E", "B", "A", ">", ",", "q", ".", "R", "M" };  // (i+1)*1) 
    public static List<List<string>> pointLUT = new List<List<string>>() { bulls, oneTimesInner, twoTimes, threeTimes, oneTimesOuter };

    public static KeyCode nextPlayerBtnKeyCode = KeyCode.Return;

    public static int numOfPlayer;

    public static List<Player> playerList;
    public static Queue<Player> playingQueue;
    public static Queue<Player> waitingQueue;

    public static List<int> boardPointsOrder = new List<int>() { 20, 1, 18, 4, 13, 6, 10, 15, 2, 17, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5, 25, 50};

    public static List<List<int>> alliesStageGreen;
    public static List<List<int>> alliesStageRed;
}