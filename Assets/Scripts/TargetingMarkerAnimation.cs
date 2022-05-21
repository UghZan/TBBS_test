using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingMarkerAnimation : MonoBehaviour
{
    [SerializeField] float bobAmount;
    [SerializeField] float bobSpeed;
    Vector3 localPos;
    // Start is called before the first frame update
    void Start()
    {
        localPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = localPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobAmount;
    }
}
