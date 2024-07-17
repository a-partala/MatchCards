using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Boolean button to manage game parameters
/// </summary>
public class SwitchButton : MonoBehaviour
{
    [Serializable]
    private struct StateData
    {
        public GameObject Container;
        public UnityEvent Event;
    }
    [SerializeField] private SerializedDictionary<bool, StateData> stateObjects;
    private bool isActive;

    public void Action()
    {
        SetActive(!isActive);
    }

    public void SetActive(bool active)
    {
        if(stateObjects.Count < 2)//less than boolean values amount
        {
            return;
        }
        stateObjects[active].Event?.Invoke();
        stateObjects[active].Container.SetActive(true);
        stateObjects[!active].Container.SetActive(false);

        isActive = active;
    }
}
