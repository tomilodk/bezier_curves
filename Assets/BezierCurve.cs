using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using JetBrains.Annotations;
using Newtonsoft.Json.Bson;
using Unity.VisualScripting;
using UnityEngine;


public class Point
{
    private GameObject _gameObject;

    private GameObject _line;
    private LineRenderer _lineRenderer;

    private TrailRenderer _trailRenderer;

    public Point((Point, Point) p1p2, Transform parent, Vector3 startPos, bool hasTrail = false)
    {
        p1 = p1p2.Item1;
        p2 = p1p2.Item2;

        _gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _gameObject.transform.parent = parent;
        var material = new Material(Shader.Find("Sprites/Default"));
        material.color = Color.yellow.WithAlpha(0.1f);
        _gameObject.GetComponent<Renderer>().sharedMaterial = material;

        Position = startPos;
        _gameObject.transform.position = startPos;

        if (p1 == null || p2 == null)
            return;

        _line = new GameObject();

        _lineRenderer = _line.AddComponent<LineRenderer>();
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.red.WithAlpha(0.1f);
        _lineRenderer.endColor = Color.red.WithAlpha(0.1f);
        _lineRenderer.widthMultiplier = 0.05f;
        _lineRenderer.positionCount = 2;

        if (hasTrail)
        {
            _lineRenderer.widthMultiplier = 0.3f;


            _trailRenderer = _gameObject.AddComponent<TrailRenderer>();
            _trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _trailRenderer.time = 1f;

            float alpha = 1.0f;
            var gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 0.8f),
                    new GradientColorKey(Color.white, 1.0f)
                },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(0.5f, 1.0f) }
            );
            _trailRenderer.colorGradient = gradient;

            _trailRenderer.startWidth = 0.3f;
            _trailRenderer.endWidth = 0.1f;
        }
    }

    [CanBeNull] private Point p1;
    [CanBeNull] private Point p2;

    public Vector3 Position;

    public void Update(float t)
    {
        if (p1 == null || p2 == null)
            return;

        var x = (1 - t) * p1.Position.x + t * p2.Position.x;
        var y = (1 - t) * p1.Position.y + t * p2.Position.y;
        var z = (1 - t) * p1.Position.z + t * p2.Position.z;

        Position = new Vector3(x, y, z);
        _gameObject.transform.position = Position;
        _gameObject.transform.localScale = Vector3.one * 0.5f;

        _lineRenderer.SetPosition(0, p1.Position);
        _lineRenderer.SetPosition(1, p2.Position);
    }
}

public class BezierCurve : MonoBehaviour
{
    private float t = 0f;
    private float speed = 0.5f;
    private bool direction = true;

    public List<Transform> StaticPointsInitPositions = new();

    // private List<Point> staticPoints = new();
    private List<Point> newPoints = new();

    private List<Point> points = new();

    private void Awake()
    {
        foreach (var s in StaticPointsInitPositions)
        {
            newPoints.Add(new Point((null, null), transform, s.position));
        }

        CreateMoveable();
    }


    private void FixedUpdate()
    {
        if (direction && t < 1f)
        {
            t += Time.deltaTime * speed;
        }
        else if (!direction && t > 0f)
        {
            t -= Time.deltaTime * speed;
        }
        else
        {
            direction = !direction;
        }


        UpdateMovables();
    }

    void CreateMoveable()
    {
        var neededPoints = newPoints.Count - 1;

        int offset = 0;

        for (int i = neededPoints; i > 0; i--)
        {
            for (int j = 0; j < i; j++)
            {
                var point = new Point((newPoints[j + offset], newPoints[j + 1 + offset]), transform,
                    newPoints[j].Position, hasTrail: (j == i - 1 && i == 1));
                newPoints.Add(point);

                points.Add(point);
            }

            offset = newPoints.Count - i;
        }
    }

    private void UpdateMovables()
    {
        foreach (var m in points)
        {
            m.Update(t);
        }
    }
}