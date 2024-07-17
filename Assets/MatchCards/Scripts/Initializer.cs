using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] private Audio audioSystem;
    [SerializeField] private LevelUI levelUI;
    [SerializeField] private Board board;
    [SerializeField] private LevelsConfig levelsConfig;
    private LevelsManager levelsController;
    private TouchController touchController;

    private void Awake()
    {
        audioSystem.Initialize();
    }

    private void Start()
    {
        touchController = new();
        levelsController = new(levelsConfig, board);
        levelUI.Initialize(levelsController);

        levelsController.LauchLevel();
    }

    private void Update()
    {
        touchController.Update();
    }
}
