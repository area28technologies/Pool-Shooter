using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimRandom : MonoBehaviour
{
    public GameObject[] objects; 

    void Start()
    {
        foreach (GameObject obj in objects)
        {
            float delay = Random.Range(0f, 3f);
            StartCoroutine(ToggleObjectRandomly(obj, delay));
        }
    }

    IEnumerator ToggleObjectRandomly(GameObject obj, float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            obj.SetActive(!obj.activeSelf);
            delay = Random.Range(1f, 4f); 
        }
    }
}
