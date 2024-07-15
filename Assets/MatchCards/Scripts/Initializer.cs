using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private LevelsConfig levelsConfig;
    private LevelsManager levelsController;
    private TouchController touchController;

    private void Awake()
    {
        
    }

    private void Start()
    {
        touchController = new();
        levelsController = new(levelsConfig, board);

        levelsController.LauchLevel();
    }

    private void Update()
    {
        touchController.Update();
    }
}
