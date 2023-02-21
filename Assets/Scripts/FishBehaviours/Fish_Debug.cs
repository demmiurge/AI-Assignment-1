using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_Debug : MonoBehaviour
{
    public GameObject plankton;
    public GameObject planktonPrefab;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Destroy(plankton);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Instantiate(planktonPrefab, transform.position, transform.rotation);
        }
    }
}
