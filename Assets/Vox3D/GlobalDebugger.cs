using System.Collections.Generic;
using UnityEngine;

    public class GlobalDebugger : MonoBehaviour
    {

        List<(Vector3, Vector3)> lines;

        // Start is called before the first frame update
        void Start()
        {
            lines = new List<(Vector3, Vector3)>();
        }

        public static void DrawLine(Vector3 from, Vector3 to)
        {
            GameObject.Find("BasicDemo").GetComponent<GlobalDebugger>().lines.Add((from, to));
        }

        public void OnDrawGizmos()
        {
            if (lines != null) lines.ForEach((line) => Gizmos.DrawLine(line.Item1, line.Item2));
        }

    }
