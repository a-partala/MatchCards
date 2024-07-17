using System;

/// <summary>
/// Class to manage levels
/// </summary>
public class LevelsManager
{
    private int levelID;//Actual ID
    private int levelCounter;//Counter (doesn't get less)
    private Board board;
    private LevelsConfig levelsConfig;//ScriptableObject with levels info
    public event Action OnVictory;
    public event Action OnFail;
    public event Action OnLevelStarted;

    public LevelsManager(LevelsConfig levelsConfig, Board board)
    {
        this.levelsConfig = levelsConfig;
        this.board = board;
        board.Initialize();
        board.OnAllCardsMatched += Win;
        board.OnFailed += () =>
        {
            Audio.Play("Fail");
            OnFail?.Invoke();
        };
        Load();
    }

    private void Win()
    {
        Audio.Play("Win");
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