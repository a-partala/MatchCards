using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private MeshRenderer faceRenderer;
    [SerializeField] private MeshRenderer backRenderer;
    [SerializeField] private Material BaseMaterial;

    public Vector2 ActualSizes
    {
        get;
        private set;
    } = new Vector2(1, 1.5f);

    public void Initialize()
    {

    }
    
    private void SetFace(Texture2D texture)
    {
        SetTexture(faceRenderer, texture);
    }

    private void SetBack(Texture2D texture)
    {
        SetTexture(backRenderer, texture);
    }

    private void SetTexture(MeshRenderer renderer, Texture2D texture)
    {
        renderer.material = new Material(renderer.material);
        renderer.material.mainTexture = texture;
    }
}
