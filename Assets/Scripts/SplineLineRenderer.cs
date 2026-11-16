using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways] // para que se actualice en el editor sin darle al play
[RequireComponent(typeof(SplineContainer), typeof(LineRenderer))]
public class SplineLineRenderer : MonoBehaviour
{
    [Range(1, 200)]
    public int resolution = 50;

    private SplineContainer _splineContainer;
    private LineRenderer _lineRenderer;

    private void OnEnable()
    {
        _splineContainer = GetComponent<SplineContainer>();
        _lineRenderer = GetComponent<LineRenderer>();

        Spline.Changed += OnSplineChanged;

        DrawSpline();
    }

    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    private void OnSplineChanged(Spline spline, int arg2, SplineModification modification)
    {
        // por algun motivo el Spline.Changed es static y se llama cada vez que CUALQUIER spline se actualiza
        // asi que hay que comprovar primero que el que ha cambiado haya sido este spline
        if (_splineContainer != null && _splineContainer.Spline == spline)
        {
            DrawSpline();
        }
    }

    public void DrawSpline()
    {
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.positionCount = resolution;

        Vector3[] points = new Vector3[resolution];

        for (int i = 0; i < resolution; i++)
        {
            float t = (float)i / (resolution - 1);

            float3 pos = _splineContainer.EvaluatePosition(t);

            points[i] = new Vector3(pos.x, pos.y, 0f);
        }

        _lineRenderer.SetPositions(points);
    }
}
