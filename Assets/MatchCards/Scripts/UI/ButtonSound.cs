using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    [SerializeField] private string soundKey = "Button";
    [SerializeField] private Button button;

    public void Awake()
    {
        button.onClick.AddListener(new UnityAction(PlaySound));
    }

    private void PlaySound()
    {
        Audio.Play(soundKey);
    }
}
