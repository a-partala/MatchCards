using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;

public class Card : MonoBehaviour, ITouchable
{
    public enum State
    {
        Back,
        Face,
        Hidden,
    }

    [SerializeField] private MeshRenderer faceRenderer;
    [SerializeField] private MeshRenderer backRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private SerializedDictionary<State, string> stateAnimKeysMap = new();
    private State state = State.Face;
    public event Action OnFlippedToFace;

    public void Initialize(Material back, Material face)
    {
        //use sharedMaterials[] if you have only one mesh renderer and it has 2 materials
        backRenderer.sharedMaterial = back;
        faceRenderer.sharedMaterial = face;
    }

    public void SetState(State state)
    {
        if (this.state == state)
        {
            return;//to avoid actions like unneccessary animations
        }
        this.state = state;
        SetAnim(state);
    }

    private void SetAnim(State state)
    {
        if(!stateAnimKeysMap.ContainsKey(state))
        {
            return;
        }
        animator.Play(stateAnimKeysMap[state], 0, 0);//play anim of key that setted to a state
        animator.Update(0);//set 0 frame
    }

    public Material GetFaceMat()
    {
        return faceRenderer.sharedMaterial;
    }

    public void OnTouchDown()
    {
        if(state == State.Back)
        {
            SetState(State.Face);
            OnFlippedToFace?.Invoke();
        }
    }

    public void OnTouchMove()
    {

    }

    public void OnTouchStatic()
    {

    }

    public void OnTouchUp()
    {

    }
}
