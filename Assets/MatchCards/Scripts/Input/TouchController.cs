using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchController
{
    private static TouchController Instance;
    private RaycastHit hit;
    private List<GraphicRaycaster> graphicRaycasters;//Canvases that can be included in system. It allows you to block rays by UI
    private bool clicked = false;
    private GameObject SelectedObject;//Operated object
    private ITouchable touchable;//Operated ITouchable (SelectedObject's)
    private List<GameObject> PauseReasons = new();//Need to stop touch system from different independed sources and resume when all them will be finished
    public static bool IsPaused = false;//The fastest way to stop the system. But it reqiers more control than 'PauseReasons'

    public TouchController()
    {
        Instance = this;
#if UNITY_EDITOR
        ResetStatic();//Unity doesn't make it by itself
#endif
        //Get all canvases for blocking touches in menus. You can just pause the system, but it needs more attention
        graphicRaycasters = Object.FindObjectsByType<GraphicRaycaster>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
    }

    public void Update()
    {
        if (IsPaused || PauseReasons.Count > 0)
        {
            return;
        }
        ProcessTouch();
    }

    /// <summary>
    /// Add reason to stop touch system.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool TryAddPauseReason(GameObject obj)
    {
        if (Instance.PauseReasons.Contains(obj))
        {
            return false;
        }
        Instance.PauseReasons.Add(obj);
        return true;
    }

    /// <summary>
    /// Remove reason to stop touch system.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static void RemovePauseReason(GameObject obj)
    {
        Instance.PauseReasons.Remove(obj);
    }

    /// <summary>
    /// Unity doesn't clears static fields
    /// </summary>
    public static void ResetStatic()
    {
        IsPaused = false;
        Clear();
    }

    /// <summary>
    /// Useful when you need to stop touch system straight in the input process. Some uncleared variables can affect input after resume
    /// </summary>
    public static void Clear()
    {
        Instance.SelectedObject = null;
        Instance.clicked = false;
    }

    /// <summary>
    /// Just main logic
    /// </summary>
    private void ProcessTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(IsBlockedByUI(touch))
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            if (SelectedObject == null)
            {
                var hits = Physics.RaycastAll(ray.origin, ray.direction);
                if (hits.Length == 0 || hits[0].collider == null)
                {
                    return;
                }
                SelectedObject = hits[0].collider.gameObject;
                touchable = SelectedObject.GetComponent<ITouchable>();
            }
            if (touchable == null)
            {
                SelectedObject = null;
                return;
            }

            ProcessActions(touch, hit);
        }
    }

    /// <summary>
    /// Invoke touch actions
    /// </summary>
    /// <param name="touch"></param>
    /// <param name="hit"></param>
    private void ProcessActions(Touch touch, RaycastHit hit)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                clicked = true;
                touchable.OnTouchDown();
                break;
            case TouchPhase.Stationary:
                if (!clicked)
                {
                    SelectedObject = null;
                    return;
                }
                touchable.OnTouchStatic();
                break;
            case TouchPhase.Moved:
                if (!clicked)
                {
                    SelectedObject = null;
                    return;
                }
                touchable.OnTouchMove();
                break;
            case TouchPhase.Ended:
                if (!clicked)
                {
                    SelectedObject = null;
                    return;
                }
                touchable.OnTouchUp();
                SelectedObject = null;
                clicked = false;
                break;
        }
    }

    /// <summary>
    /// Tells you if touch blocked by UI (in canvas from 'graphicRaycasters')
    /// </summary>
    /// <param name="touch"></param>
    /// <returns></returns>
    private bool IsBlockedByUI(Touch touch)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = touch.position
        };
        List<RaycastResult> results = new List<RaycastResult>();
        foreach (var graphicRaycaster in graphicRaycasters)
        {
            graphicRaycaster.Raycast(pointerEventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.tag == "BlockTouches")
                {
                    return true;
                }
            }
        }

        return false;
    }
}
