using UnityEngine;

/// <summary>
/// Main UI controller
/// </summary>
public class LevelUI : MonoBehaviour
{
    [SerializeField] private GameObject settiingsWindow;
    [SerializeField] private GameObject victoryWindow;
    [SerializeField] private GameObject failWindow;
    private LevelsManager levelsManager;

    public void Initialize(LevelsManager levelsManager)
    {
        this.levelsManager = levelsManager;
        levelsManager.OnVictory += Win;
        levelsManager.OnFail += Fail;
        levelsManager.OnLevelStarted += HideWindows;
    }

    public void NextLevel()
    {
        levelsManager.IncreaseLevel();
        levelsManager.LauchLevel();
    }

    public void Fail()
    {
        failWindow.SetActive(true);
    }

    public void Win()
    {
        victoryWindow.SetActive(true);
    }

    public void ShowSettings()
    {
        if(TouchController.IsWorking())
        {
            settiingsWindow.SetActive(true);
        }
    }

    private void HideWindows()
    {
        failWindow.SetActive(false);
        victoryWindow.SetActive(false);
        settiingsWindow.SetActive(false);
    }

    public void Restart()
    {
        levelsManager.LauchLevel();
    }
}
