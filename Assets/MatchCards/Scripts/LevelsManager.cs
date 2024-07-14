using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager
{
    [SerializeField]
    public class LevelData
    {
        public int PairsAmount = 2;
        public float Timer = 10;
    }
    private int levelID;
    private int levelCounter;

    public LevelsManager(Board board) 
    {
        board.OnAllCardsMatched += Win;
        Load();
    }

    private void Win()
    {

    }

    public void LauchLevel()
    {

    }

    private void Save()
    {
        SaveService.SetInt(nameof(levelID), levelID);
        SaveService.SetInt(nameof(levelCounter), levelCounter);
    }

    private void Load()
    {
        levelID = SaveService.GetInt(nameof(levelID), 0);
        levelCounter = SaveService.GetInt(nameof(levelCounter), 1);
    }
}