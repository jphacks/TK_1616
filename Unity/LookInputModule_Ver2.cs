using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class LookInputModule_Ver2 : BaseInputModule
{


    private static LookInputModule _singleton;
    public static LookInputModule singleton
    {
        get
        {
            return _singleton;
        }
    }


    public string submitButtonName = "Fire1";


    public string controlAxisName = "Horizontal";
    public bool useSmoothAxis = true;
    public float smoothAxisMultiplier = 0.01f;
    public float steppedAxisStepsPerSecond = 10f;
    private bool _guiRaycastHit;
    public bool guiRaycastHit
    {
        get
        {
            return _guiRaycastHit;
        }
    }
    private bool _controlAxisUsed;
    public bool controlAxisUsed
    {
        get
        {
            return _controlAxisUsed;
        }
    }
    private bool _buttonUsed;
    public bool buttonUsed
    {
        get
        {
            return _buttonUsed;
        }
    }

    public enum Mode { Pointer, Submit };

    public Mode mode = Mode.Pointer;

    public bool useLookDrag = true;
    public bool useLookDragSlider = true;
    public bool useLookDragScrollbar = false;

    public bool useCursor = true;
    public float normalCursorScale = 0.0005f;
    public bool scaleCursorWithDistance = true;

    public RectTransform cursor;

    public bool useSelectColor = true;
    public bool useSelectColorOnButton = false;
    public bool useSelectColorOnToggle = false;
    public Color selectColor = Color.blue;

    public bool ignoreInputsWhenLookAway = true;
    public bool deselectWhenLookAway = false;
    private PointerEventData lookData;
    private Color currentSelectedNormalColor;
    private bool currentSelectedNormalColorValid;
    private Color currentSelectedHighlightedColor;
    private GameObject currentLook;
    private GameObject currentPressed;
    private GameObject currentDragging;
    private float nextAxisActionTime;
    private PointerEventData GetLookPointerEventData()
    {
        Vector2 lookPosition;
        lookPosition.x = Screen.width / 2;
        lookPosition.y = Screen.height / 2;
        if (lookData == null)
        {
            lookData = new PointerEventData(eventSystem);
        }
        lookData.Reset();
        lookData.delta = Vector2.zero;
        lookData.position = lookPosition;
        lookData.scrollDelta = Vector2.zero;
        eventSystem.RaycastAll(lookData, m_RaycastResultCache);
        lookData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        if (lookData.pointerCurrentRaycast.gameObject != null)
        {
            _guiRaycastHit = true;
        }
        else
        {
            _guiRaycastHit = false;
        }
        m_RaycastResultCache.Clear();
        return lookData;
    }

    private void UpdateCursor(PointerEventData lookData)
    {
        if (cursor != null)
        {
            if (useCursor)
            {
                if (lookData.pointerEnter != null)
                {
                    RectTransform draggingPlane = lookData.pointerEnter.GetComponent<RectTransform>();
                    Vector3 globalLookPos;
                    if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, lookData.position, lookData.enterEventCamera, out globalLookPos))
                    {
                        cursor.gameObject.SetActive(true);
                        cursor.position = globalLookPos;
                        cursor.rotation = draggingPlane.rotation;
                        if (scaleCursorWithDistance)
                        {
                            // scale cursor with distance
                            float lookPointDistance = (globalLookPos - lookData.enterEventCamera.transform.position).magnitude;
                            float cursorScale = lookPointDistance * normalCursorScale;
                            if (cursorScale < normalCursorScale)
                            {
                                cursorScale = normalCursorScale;
                            }
                            Vector3 cursorScaleVector;
                            cursorScaleVector.x = cursorScale;
                            cursorScaleVector.y = cursorScale;
                            cursorScaleVector.z = cursorScale;
                            cursor.localScale = cursorScaleVector;
                        }
                    }
                    else
                    {
                        cursor.gameObject.SetActive(false);
                    }
                }
                else
                {
                    cursor.gameObject.SetActive(false);
                }
            }
            else
            {
                cursor.gameObject.SetActive(false);
            }
        }
    }
    private void SetSelectedColor(GameObject go)
    {
        if (useSelectColor)
        {
            if (!useSelectColorOnButton && go.GetComponent<Button>())
            {
                currentSelectedNormalColorValid = false;
                return;
            }
            if (!useSelectColorOnToggle && go.GetComponent<Toggle>())
            {
                currentSelectedNormalColorValid = false;
                return;
            }
            Selectable s = go.GetComponent<Selectable>();
            if (s != null)
            {
                ColorBlock cb = s.colors;
                currentSelectedNormalColor = cb.normalColor;
                currentSelectedNormalColorValid = true;
                currentSelectedHighlightedColor = cb.highlightedColor;
                cb.normalColor = selectColor;
                cb.highlightedColor = selectColor;
                s.colors = cb;
            }
        }
    }

    private void RestoreColor(GameObject go)
    {
        if (useSelectColor && currentSelectedNormalColorValid)
        {
            Selectable s = go.GetComponent<Selectable>();
            if (s != null)
            {
                ColorBlock cb = s.colors;
                cb.normalColor = currentSelectedNormalColor;
                cb.highlightedColor = currentSelectedHighlightedColor;
                s.colors = cb;
            }
        }
    }

    public void ClearSelection()
    {
        if (eventSystem.currentSelectedGameObject)
        {
            RestoreColor(eventSystem.currentSelectedGameObject);
            eventSystem.SetSelectedGameObject(null);
        }
    }

    private void Select(GameObject go)
    {
        ClearSelection();
        if (ExecuteEvents.GetEventHandler<ISelectHandler>(go))
        {
            SetSelectedColor(go);
            eventSystem.SetSelectedGameObject(go);
        }
    }

    private bool SendUpdateEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return false;
        BaseEventData data = GetBaseEventData();
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
        return data.used;
    }

    public override void Process()
    {
        _singleton = this;

        SendUpdateEventToSelectedObject();

        PointerEventData lookData = GetLookPointerEventData();
        currentLook = lookData.pointerCurrentRaycast.gameObject;

        if (deselectWhenLookAway && currentLook == null)
        {
            ClearSelection();
        }

        // handle enter and exit events (highlight)
        // using the function that is already defined in BaseInputModule
        HandlePointerExitAndEnter(lookData, currentLook);

        // update cursor
        UpdateCursor(lookData);

        if (!ignoreInputsWhenLookAway || ignoreInputsWhenLookAway && currentLook != null)
        {
            // button down handling
            _buttonUsed = false;
            if (Input.GetButtonDown(submitButtonName))
            {
                ClearSelection();
                lookData.pressPosition = lookData.position;
                lookData.pointerPressRaycast = lookData.pointerCurrentRaycast;
                lookData.pointerPress = null;
                if (currentLook != null)
                {
                    currentPressed = currentLook;
                    GameObject newPressed = null;
                    if (mode == Mode.Pointer)
                    {
                        newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, lookData, ExecuteEvents.pointerDownHandler);
                        if (newPressed == null)
                        {
                            // some UI elements might only have click handler and not pointer down handler
                            newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, lookData, ExecuteEvents.pointerClickHandler);
                            if (newPressed != null)
                            {
                                currentPressed = newPressed;
                            }
                        }
                        else
                        {
                            currentPressed = newPressed;

                            ExecuteEvents.Execute(newPressed, lookData, ExecuteEvents.pointerClickHandler);
                        }
                    }
                    else if (mode == Mode.Submit)
                    {
                        newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, lookData, ExecuteEvents.submitHandler);
                        if (newPressed == null)
                        {
                            // try select handler instead
                            newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, lookData, ExecuteEvents.selectHandler);
                        }
                    }
                    if (newPressed != null)
                    {
                        lookData.pointerPress = newPressed;
                        currentPressed = newPressed;
                        Select(currentPressed);
                        _buttonUsed = true;
                    }
                    if (mode == Mode.Pointer)
                    {
                        if (useLookDrag)
                        {
                            bool useLookTest = true;
                            if (!useLookDragSlider && currentPressed.GetComponent<Slider>())
                            {
                                useLookTest = false;
                            }
                            else if (!useLookDragScrollbar && currentPressed.GetComponent<Scrollbar>())
                            {
                                useLookTest = false;

                                if (ExecuteEvents.Execute(currentPressed, lookData, ExecuteEvents.beginDragHandler))
                                {
                                    ExecuteEvents.Execute(currentPressed, lookData, ExecuteEvents.endDragHandler);
                                }
                            }
                            if (useLookTest)
                            {
                                ExecuteEvents.Execute(currentPressed, lookData, ExecuteEvents.beginDragHandler);
                                lookData.pointerDrag = currentPressed;
                                currentDragging = currentPressed;
                            }
                        }
                        else if (currentPressed.GetComponent<Scrollbar>())
                        {

                            if (ExecuteEvents.Execute(currentPressed, lookData, ExecuteEvents.beginDragHandler))
                            {
                                ExecuteEvents.Execute(currentPressed, lookData, ExecuteEvents.endDragHandler);
                            }
                        }
                    }
                }
            }
        }


        if (Input.GetButtonUp(submitButtonName))
        {
            if (currentDragging)
            {
                ExecuteEvents.Execute(currentDragging, lookData, ExecuteEvents.endDragHandler);
                if (currentLook != null)
                {
                    ExecuteEvents.ExecuteHierarchy(currentLook, lookData, ExecuteEvents.dropHandler);
                }
                lookData.pointerDrag = null;
                currentDragging = null;
            }
            if (currentPressed)
            {
                ExecuteEvents.Execute(currentPressed, lookData, ExecuteEvents.pointerUpHandler);
                lookData.rawPointerPress = null;
                lookData.pointerPress = null;
                currentPressed = null;
            }
        }


        if (currentDragging != null)
        {
            ExecuteEvents.Execute(currentDragging, lookData, ExecuteEvents.dragHandler);
        }

        if (!ignoreInputsWhenLookAway || ignoreInputsWhenLookAway && currentLook != null)
        {

            _controlAxisUsed = false;
            if (eventSystem.currentSelectedGameObject && controlAxisName != null && controlAxisName != "")
            {
                float newVal = Input.GetAxis(controlAxisName);
                if (newVal > 0.01f || newVal < -0.01f)
                {
                    if (useSmoothAxis)
                    {
                        Slider sl = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
                        if (sl != null)
                        {
                            float mult = sl.maxValue - sl.minValue;
                            sl.value += newVal * smoothAxisMultiplier * mult;
                            _controlAxisUsed = true;
                        }
                        else
                        {
                            Scrollbar sb = eventSystem.currentSelectedGameObject.GetComponent<Scrollbar>();
                            if (sb != null)
                            {
                                sb.value += newVal * smoothAxisMultiplier;
                                _controlAxisUsed = true;
                            }
                        }
                    }
                    else
                    {
                        _controlAxisUsed = true;
                        float time = Time.unscaledTime;
                        if (time > nextAxisActionTime)
                        {
                            nextAxisActionTime = time + 1f / steppedAxisStepsPerSecond;
                            AxisEventData axisData = GetAxisEventData(newVal, 0.0f, 0.0f);
                            if (!ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisData, ExecuteEvents.moveHandler))
                            {
                                _controlAxisUsed = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
