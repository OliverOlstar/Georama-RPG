using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MoveBetweenPoints : MonoBehaviour
{
    [System.Serializable]
    public class Point
    {
        public Vector3 point = new Vector3();
        public float seconds = 1.0f;
        public float delay = 0.0f;
        public AnimationCurve motion01 = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));
    }

    [SerializeField, InfoBox("ERROR: Required 2 or more points.", InfoMessageType.Error, "@points.Length < 2")] private Point[] points = new Point[2];
    [SerializeField] private Transform moveTransform = null;

    private int index = 0;
    private float progress01 = 0;
    private float moveTime = 0;
    private Vector3 initalPosition = new Vector3();

    private void Start() 
    {
        initalPosition = transform.position;
        moveTransform.position = points[index].point + initalPosition;
    }

	private void Reset()
	{
		moveTransform = transform;
	}

	private void FixedUpdate() 
    {
        if (Time.time > moveTime)
        {
            if (progress01 < 1)
            {
                progress01 += Time.fixedDeltaTime / points[index].seconds;
                float t = points[index].motion01.Evaluate(progress01);
                moveTransform.position = Vector3.Lerp(points[index].point, points[SafeIndex(index + 1)].point, t) + initalPosition;
            }
            else
            {
                moveTime = Time.time + points[index].delay;
                index = SafeIndex(index + 1);
                progress01 = 0.0f;
            }
        }
    }

    private void OnDrawGizmosSelected() 
    {
        if (points.Length < 2)
            return;

        Vector3 offset = Application.isPlaying ? initalPosition : transform.position;
        
        Gizmos.color = Color.cyan;
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawCube(points[i].point + offset, Vector3.one * 0.1f);
            Gizmos.DrawLine(points[i].point + offset, points[SafeIndex(i + 1)].point + offset);
        }
    }

    private int SafeIndex(int pIndex)
    {
        if (pIndex >= points.Length)
        {
            return pIndex - points.Length;
        }
        else
        {
            return pIndex;
        }
    }
}
