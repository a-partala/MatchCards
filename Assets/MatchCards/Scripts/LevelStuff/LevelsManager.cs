public class LevelsManager
{
    private int levelID;
    private int levelCounter;
    private Board board;
    private LevelsConfig levelsConfig;

    public LevelsManager(LevelsConfig levelsConfig, Board board)
    {
        this.levelsConfig = levelsConfig;
        this.board = board;
        board.OnAllCardsMatched += Win;
        Load();
    }

    private void Win()
    {

    }

    public void LauchLevel()
    {
        board.PrepareLevel(levelsConfig.levels[levelID]);
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