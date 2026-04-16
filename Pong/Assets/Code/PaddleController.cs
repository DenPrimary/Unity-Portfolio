using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
public class PaddleController : MonoBehaviour
{
    public float keyboardSens = 8f;
    public float topPoint = 4.21f;
    public float bottomPoint = -4.21f;
    public bool isBeingReset = false;

    private Vector3 startPosition;

    void Update()
    {
        if (isBeingReset) return;

        HandleKeyboardInput();
    }

    private void HandleKeyboardInput() 
    {
        float keyboardInput = 0f;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            keyboardInput = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) 
        { 
            keyboardInput = -1f;
        }

        if (keyboardInput != 0f) 
        {
            transform.Translate(Vector3.up * keyboardInput * keyboardSens * Time.deltaTime);
            ClampPosition();
        }
    }

    private void ClampPosition() 
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, bottomPoint, topPoint);
        transform.position = clampedPosition;
    }

    public void SetResetState(bool resetting)
    {
        isBeingReset = resetting;
    }
}
