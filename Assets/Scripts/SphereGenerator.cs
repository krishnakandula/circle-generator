using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGenerator : MonoBehaviour
{
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] [Range(2, 360)] private int yawGranularity = 30;
    [SerializeField] [Range(2, 360)] private int pitchGranularity = 30;
    [SerializeField] [Range(0.5f, 100f)] private float radius = 5;
    [SerializeField] [Range(0f, 0.5f)] private float pointCreationDelay = .05f;
    [SerializeField] [Range(-19f, 20f)] private float zOffset = 10f;
    [SerializeField] [Range(-20f, 20f)] private float yOffset = 10f;

    private List<Coroutine> coroutines = new List<Coroutine>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DestroyOldPoints();
            ClearPointCreationCoroutines();
            coroutines.Add(
                StartCoroutine(GeneratePointsOnCircle(pitchGranularity, yawGranularity, radius, pointCreationDelay,
                    yOffset, zOffset)));
        }
    }

    private void DestroyOldPoints()
    {
        List<GameObject> children = GetAllChildren();
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

    private IEnumerator GeneratePointsOnCircle(int pitchGranularity, int yawGranularity, float radius,
        float pointCreationDelay, float yOffset,
        float zOffset)
    {
        if (pointPrefab == null)
        {
            Debug.Log("Unable to generate points on a circle. Point prefab is not set.");
            yield return null;
        }

        float[,] angles = GenerateAngles(pitchGranularity, yawGranularity);
        for (int i = 0; i < angles.GetLength(0); ++i)
        {
            float pitchAngle = angles[i, 0];
            float yawAngle = angles[i, 1];
            double pitchAngleInRadians = pitchAngle * (Math.PI / 180);
            double yawAngleInRadians = yawAngle * (Math.PI / 180);

            double xCoord = radius * Math.Cos(pitchAngleInRadians) * Math.Cos(yawAngleInRadians);
            double zCoord = radius * Math.Cos(pitchAngleInRadians) * Math.Sin(yawAngleInRadians);
            double yCoord = radius * Math.Sin(pitchAngleInRadians);

            Vector3 pos = new Vector3((float) xCoord, (float) (yCoord + yOffset), (float) (zCoord + zOffset));
            CreatePoint(pos);
            yield return new WaitForSeconds(pointCreationDelay);
        }
    }


    private static float[,] GenerateAngles(int pitchGranularity, int yawGranularity)
    {
        int numAngles = pitchGranularity * yawGranularity;
        float[,] angles = new float[numAngles, 2];
        float pitchAngle = 360f / pitchGranularity;
        float yawAngle = 360f / yawGranularity;
        for (int x = 0, i = 0; x < pitchGranularity; ++x)
        {
            for (int y = 0; y < yawGranularity; ++y, ++i)
            {
                angles[i, 0] = pitchAngle * x;
                angles[i, 1] = yawAngle * y;
            }
        }

        return angles;
    }

    private void CreatePoint(Vector3 position)
    {
        Debug.Log("Creating point at position " + position);
        GameObject point = Instantiate(pointPrefab, transform);
        PointController pointController = point.GetComponent<PointController>();
        if (pointController != null)
        {
            pointController.destination = position;
        }
    }


    private List<GameObject> GetAllChildren()
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