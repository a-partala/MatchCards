using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Deck deck;

    public void Show()//For some initialization in the future
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
