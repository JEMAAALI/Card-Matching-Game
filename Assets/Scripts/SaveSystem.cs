using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class CardData
{
    public int cardValue;
    public string faceSpriteName;
    public string backSpriteName;
    public bool isFlipped;
}

[System.Serializable]
public class GameData
{
    public int rows;
    public int cols;
    public float timeRemaining;
    public int turns;
    public int matches;
    public int score; // <--- add this
    public List<CardData> cards = new List<CardData>();
}

public class SaveSystem : MonoBehaviour
{

     
    

     
}
