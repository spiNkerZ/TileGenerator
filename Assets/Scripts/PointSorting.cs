using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class PointSorting : IComparer<Vector3>
    {
        private Vector2 m_Origin;
        public Vector2 origin { get { return m_Origin; } set { m_Origin = value; } }
        public PointSorting(Vector3 origin)
        {
            m_Origin = new Vector2(origin.x,origin.z);
        }
        public int Compare(Vector3 first, Vector3 second)
        {
            first = new Vector2(first.x, first.z);
            second = new Vector2(second.x, second.z);
            Vector2 pointA = first;
            Vector2 pointB = second;

            return IsClockwise(pointB, pointA, m_Origin);
        }
        public static int IsClockwise(Vector2 first, Vector2 second, Vector2 origin)
        {
            if (first == second)
                return 0;

            Vector2 firstOffset = first - origin;
            Vector2 secondOffset = second - origin;

            float angle1 = Mathf.Atan2(firstOffset.x, firstOffset.y);
            float angle2 = Mathf.Atan2(secondOffset.x, secondOffset.y);

            if (angle1 < angle2)
                return 1;

            if (angle1 > angle2)
                return -1;

            return (firstOffset.sqrMagnitude < secondOffset.sqrMagnitude) ? 1 : -1;
        }
    }
