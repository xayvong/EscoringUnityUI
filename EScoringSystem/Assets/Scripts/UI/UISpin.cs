using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpin : MonoBehaviour
{
    public float SpinSpeed;

    private void Start()
    {
        Spin(3);

    }

    public void Spin(int numbSpins)
    {
        StartCoroutine(spin(numbSpins));
    }

    private IEnumerator spin(float numbSpins)
    {
        var degrees = numbSpins * 360;
        var curRot = transform.rotation;

        var rotTo = curRot;
        rotTo.y = degrees;


        while (degrees > 0)
        {
            curRot = transform.rotation;
            var rot = Quaternion.RotateTowards(curRot, rotTo, SpinSpeed + Time.deltaTime);

            var dif = rot.y;
            degrees -= dif;
            transform.rotation = rot;

            yield return null;

        }
        transform.rotation = Quaternion.identity;
    }
}
