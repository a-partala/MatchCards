using UnityEngine;

public interface ITouchable
{
    public void OnTouchDown();

    public void OnTouchStatic();

    public void OnTouchMove();

    public void OnTouchUp();
}
