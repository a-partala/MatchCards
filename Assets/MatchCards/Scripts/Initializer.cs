using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] private Deck deck;
    private LevelsManager levelsController;

    public void Awake()
    {
        
    }

    public void Start()
    {
        levelsController = new();
    }
}
