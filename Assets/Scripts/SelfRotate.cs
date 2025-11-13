using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotate : MonoBehaviour
{
    public float xAngle, yAngle, zAngle;
    void Update()
    {
        this.transform.Rotate(xAngle, yAngle, zAngle, Space.Self);
        StartCoroutine(Hide());
    }
    IEnumerator Hide()
    {
        WaitForSeconds _w = new WaitForSeconds(1.5f);
        yield return _w;
        gameObject.SetActive(false);
    }
}
