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

    public void Initialize(Material back, Material face)
    {
        //use sharedMaterials[] if you have only one model
        backRenderer.sharedMaterial = back;
        faceRenderer.sharedMaterial = face;
    }

    public void OnTouchDown()
    {
        throw new System.NotImplementedException();
    }

    public void OnTouchMove()
    {
        throw new System.NotImplementedException();
    }

    public void OnTouchStatic()
    {
        throw new System.NotImplementedException();
    }

    public void OnTouchUp()
    {
        throw new System.NotImplementedException();
    }
}
