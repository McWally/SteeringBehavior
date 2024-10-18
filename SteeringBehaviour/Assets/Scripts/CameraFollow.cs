using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followT;

    // Update is called once per frame
    void Update()
    {
        Camera.main.transform.position = new Vector3(followT.transform.position.x, followT.transform.position.y, followT.transform.position.z -1f);
    }
}
