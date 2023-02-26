using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanktonBehaviour : MonoBehaviour
{
    public int numberOfBites = 100;
    private Vector3 originalSize;
    private int initialNumberOfBites;

    void Start()
    {
        originalSize = transform.localScale;
        initialNumberOfBites = numberOfBites;
    }

    public void BeBitten()
    {
        numberOfBites--;
        transform.localScale = transform.localScale - originalSize / initialNumberOfBites;
        if (numberOfBites <= 0)
            gameObject.SetActive(false);
    }
}
