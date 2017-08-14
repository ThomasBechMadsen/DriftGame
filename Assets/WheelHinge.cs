using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelHinge : MonoBehaviour {

    public CarController controller;
    public float maxTurnAngle;
    public float turnTime;

    enum TireTurnTarget { center, left, right };
    TireTurnTarget currentTarget = TireTurnTarget.center;
    Coroutine wheelturn;

    void FixedUpdate()
    {
        if (controller.hAxis == 0)
        {
            if (currentTarget != TireTurnTarget.center)
            {
                currentTarget = TireTurnTarget.center;
                if (wheelturn != null)
                {
                    StopCoroutine(wheelturn);
                }
                wheelturn = StartCoroutine(tireTurn(transform.localRotation.eulerAngles.y, 0));
            }
        }
        else if (controller.hAxis > 0)
        {
            if (currentTarget != TireTurnTarget.right)
            {
                currentTarget = TireTurnTarget.right;
                if (wheelturn != null)
                {
                    StopCoroutine(wheelturn);
                }
                wheelturn = StartCoroutine(tireTurn(transform.localRotation.eulerAngles.y, maxTurnAngle));
            }
        }
        else
        {
            if (currentTarget != TireTurnTarget.left)
            {
                currentTarget = TireTurnTarget.left;
                if (wheelturn != null)
                {
                    StopCoroutine(wheelturn);
                }
                wheelturn = StartCoroutine(tireTurn(transform.localRotation.eulerAngles.y, -maxTurnAngle));
            }
        }
    }

    IEnumerator tireTurn(float startAngle, float endAngle)
    {
        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime;
            transform.localRotation = Quaternion.AngleAxis(Mathf.LerpAngle(startAngle, endAngle, progress/turnTime), Vector3.up);
            yield return null;
        }
    }
}
