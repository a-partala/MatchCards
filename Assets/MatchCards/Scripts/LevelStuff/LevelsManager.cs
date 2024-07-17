using System;
using Zenject;

public class LevelsManager
{
    private int levelID;
    private int levelCounter;
    private Board board;
    private LevelsConfig levelsConfig;
    public event Action OnVictory;
    public event Action OnFail;
    public event Action OnLevelStarted;

    public LevelsManager(LevelsConfig levelsConfig, Board board)
    {
        this.levelsConfig = levelsConfig;
        this.board = board;
        board.Initialize();
        board.OnAllCardsMatched += Win;
        board.OnFailed += () => OnFail?.Invoke();
        Load();
    }

    private void Win()
    {
        OnVictory?.Invoke();
    }

    public void IncreaseLevel()
    {
        levelID++;
        if(levelID >= levelsConfig.levels.Length)
        {
            levelID = 0;
        }
        levelCounter++;
        Save();
    }

    public void LauchLevel()
    {
        board.PrepareLevel(levelsConfig.levels[levelID]);
        OnLevelStarted?.Invoke();
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