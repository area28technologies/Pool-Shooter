using DTT.BubbleShooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTracer : MonoBehaviour
{
    [SerializeField] GameObject dotPrefab;
    [SerializeField] Transform tracerParent;
    [SerializeField] float tracerLength;
    [SerializeField] float dotNum;

    private List<GameObject> dotList = new();
    private float dotDistance;

    private void OnEnable()
    {
        foreach (var dot in dotList)
        {
            Destroy(dot);
        }
        dotList.Clear();
        dotDistance = tracerLength / (dotNum - 1);
        for (int i = 0; i < dotNum; i++)
        {
            var dot = ObjectPooling.Instance.SpawnObject(dotPrefab, tracerParent, new Vector3(0, i * dotDistance, 0), Quaternion.identity);
            dotList.Add(dot);
        }
    }


}
