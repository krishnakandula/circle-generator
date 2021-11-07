using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGenerator : MonoBehaviour
{
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] [Range(2, 360)] private int granularity = 30;
    [SerializeField] [Range(0.5f, 100f)] private float radius = 5;
    [SerializeField] [Range(0f, 0.5f)] private float pointCreationDelay = .05f;
    [SerializeField] [Range(-20f, 20f)] private float zOffset = 10f;
    [SerializeField] [Range(-20f, 20f)] private float yOffset = 10f;

    private List<Coroutine> coroutines = new List<Coroutine>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DestroyOldPoints();
            ClearPointCreationCoroutines();
            coroutines.Add(
                StartCoroutine(generatePointsOnCircle(granularity, radius, pointCreationDelay, yOffset, zOffset)));
        }
    }

    private void DestroyOldPoints()
    {
        List<GameObject> children = getAllChildren();
        foreach (GameObject child in children)
        {
            Destroy(child);
        }
    }

    private void ClearPointCreationCoroutines()
    {
        foreach (Coroutine coroutine in coroutines)
        {
            StopCoroutine(coroutine);
        }

        coroutines.Clear();
    }

    private IEnumerator generatePointsOnCircle(int granularity, float radius, float pointCreationDelay, float yOffset,
        float zOffset)
    {
        if (pointPrefab == null)
        {
            Debug.Log("Unable to generate points on a circle. Point prefab is not set.");
            yield return null;
        }

        float[] angles = generateAngles(granularity);
        for (int i = 0; i < angles.Length; ++i)
        {
            float angle = angles[i];
            double angleInRadians = angle * (Math.PI / 180);
            double xCoord = radius * Math.Cos(angleInRadians);
            double yCoord = radius * Math.Sin(angleInRadians);

            Vector3 pos = new Vector3((float) xCoord, (float) (yCoord + yOffset), zOffset);
            createPoint(pos);
            yield return new WaitForSeconds(pointCreationDelay);
        }
    }


    private float[] generateAngles(int granularity)
    {
        float[] angles = new float[granularity];
        float angle = 360f / granularity;
        for (int i = 0; i < angles.Length; ++i)
        {
            angles[i] = angle * i;
        }

        return angles;
    }

    private void createPoint(Vector3 position)
    {
        Debug.Log("Creating point at position " + position);
        GameObject point = Instantiate(pointPrefab, transform);
        PointController pointController = point.GetComponent<PointController>();
        if (pointController != null)
        {
            pointController.destination = position;
        }
    }


    private List<GameObject> getAllChildren()
    {
        List<GameObject> children = new List<GameObject>();
        Transform[] childTransforms = transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in childTransforms)
        {
            if (t.gameObject == gameObject)
            {
                continue;
            }

            children.Add(t.gameObject);
        }

        return children;
    }
}