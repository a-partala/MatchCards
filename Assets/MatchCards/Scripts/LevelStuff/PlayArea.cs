using UnityEngine;

/// <summary>
/// Area to fit level in
/// </summary>
public class PlayArea : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public float GetRescaleCoef(Vector2 levelSizes)
    {
        float widthRatio = transform.localScale.x / levelSizes.x;
        float heightRatio = transform.localScale.y / levelSizes.y;

        if(widthRatio > heightRatio)
        {
            return heightRatio;
        }
        else
        {
            return widthRatio;
        }
    }
}
