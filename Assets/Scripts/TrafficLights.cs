using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLights : MonoBehaviour
{

    public GameObject luz;

    public Transform posVerde;
    public Transform posAmbar;
    public Transform posRojo;

    private bool rojo;
    private bool ambar;
    private bool verde;

    private void Start()
    {
        verde = true;
    }

    void Update()
    {
        if (rojo == true)
        {
            luz.transform.position = posRojo.position;
            luz.GetComponent<Light>().color = Color.red;
            StartCoroutine(luzRoja());
            ambar = false;

        }
        else if (ambar == true)
        {
            luz.transform.position = posAmbar.position;
            luz.GetComponent<Light>().color = Color.yellow;
            StartCoroutine(luzAmbar());
            verde = false;

        }
        else if (verde == true)
        {
            luz.transform.position = posVerde.position;
            luz.GetComponent<Light>().color = Color.green;
            StartCoroutine(luzVerde());
            rojo = false;
        }
    }
    IEnumerator luzVerde()
    {
        yield return new WaitForSeconds(6);

        ambar = true;
    }
    IEnumerator luzAmbar()
    {
        yield return new WaitForSeconds(2);

        rojo = true;
    }
    IEnumerator luzRoja()
    {
        yield return new WaitForSeconds(3);
        verde = true;
        rojo = false;
    }
}
