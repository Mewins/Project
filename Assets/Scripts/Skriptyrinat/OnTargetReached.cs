using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class OnTargetReached : MonoBehaviour
{
    public float treshold = 0.02f;
    public Transform target;
    public UnityEvent OnReached;
    bool wasReached = false;

    private void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < treshold && !wasReached)
        {
            OnReached.Invoke();
            wasReached = true;
        }
        else if (distance >= treshold)
        {
            wasReached = false;
        }    

    }
}
