using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlManager : Soliton<PlayerControlManager>
{
    public Vector2 mousePos;
    public float mousePress;

    public bool isDrag = false;

    public Vector2 pressStartPos = Vector2.zero;
    public float dragMagnitude;
    
    public float timeStartPressed=-1;
    public float timeDragDuration;

    public static bool IS_DRAG
    {
        get { return Instance.isDrag; }
    }

    public void OnMousePosition(InputAction.CallbackContext context) {
        mousePos = context.ReadValue<Vector2>();
    }

    public void OnMousePress(InputAction.CallbackContext context)
    {
        mousePress = context.ReadValue<float>();
        if (mousePress == 1) 
        { 
            StartCheckDrag(); 
        } else 
        {
            StopCheckDrag();
        }
    }

    private void Update()
    {
        UpdateCheckDrag();
    }

    private void UpdateCheckDrag() {
        if (isDrag || timeStartPressed == -1 && pressStartPos == Vector2.zero) return;
        if (
            Time.time > timeStartPressed + timeDragDuration
            || (mousePos - pressStartPos).magnitude >= dragMagnitude
            )
        {
            isDrag = true;
        }
    }

    private void StartCheckDrag() {
        timeStartPressed = Time.time;
        pressStartPos = mousePos;
    }

    private void StopCheckDrag()
    {
        timeStartPressed = -1;
        pressStartPos = Vector2.zero;
        isDrag = false;
    }
}
