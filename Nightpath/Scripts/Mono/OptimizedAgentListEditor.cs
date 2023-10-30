using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nightpath
{
    [Serializable]
    public enum GridFormationStyle
    {
        Rectangle,
        Circle,
        Elliptical
    }
    public class OptimizedAgentListEditor : ExpandedBehaviour
    {
        [HideInInspector] public OptimizedAgentManager agentManager;

        [HideInInspector] public GridFormationStyle gridFormationStyle;
        [HideInInspector] public float tileDiameter;
        [HideInInspector] public Vector2Int gridSize = Vector2Int.one;

        [NonSerialized] public bool drawGizmosTrigger;

        [HideInInspector] public int equivalent = 1;

        private List<Vector3> posList = new List<Vector3>();

        private Vector2Int tempGridSize = Vector2Int.zero;
        private float tempTileDiameter;
        private Vector3 tempPosition;
        private float tempAngle;
        private GridFormationStyle tempGridFormationStyle;

        private bool AnyChanges => tempGridSize != gridSize
                                   || tileDiameter != tempTileDiameter
                                   || tempPosition != transform.position
                                   || tempAngle != transform.eulerAngles.y
                                   || tempGridFormationStyle != gridFormationStyle;

        private void Start()
        {
            Destroy(this);
        }

        private void OnDrawGizmos()
        {
            if (!debuggingSettings.drawDebugging || !agentManager || !drawGizmosTrigger) return;
            if (AnyChanges)
            {
                var pos = CreateGrids();
                agentManager.agents = pos;
                const int divideStart = 100000;
                if (pos.Count > divideStart)
                {
                    int divide = 2 + 2 * (pos.Count / divideStart);
                    CompleteGrid(divide);
                    equivalent = divide;
                    var rc = pos.Count / divide;
                    if (rc % 2 == 1) rc++;
                    Vector2[] reducedPos = new Vector2[rc];
                    
                    int index = 0;
                    for (int i = 0; i < pos.Count - 1 && index < reducedPos.Length; i += divide * 2)
                    {
                        reducedPos[index] = pos[i];
                        if (index + 1 < reducedPos.Length)
                        {
                            reducedPos[index + 1] = pos[i + 1];
                        }
                        index += 2;
                    }

                    posList = reducedPos.ToList().ConvertAll(x => new Vector3(x.x, transform.position.y, x.y));
                }
                else
                {
                    posList = pos.ConvertAll(x => new Vector3(x.x, transform.position.y, x.y));
                    CompleteGrid(2);
                    equivalent = 2;
                }

                RenewTemp();
            }

            Gizmos.color = Color.red;

            Gizmos.DrawLineList(posList.ToArray());

            void CompleteGrid(int divide)
            {
                var mod = posList.Count % divide;
                for (int i = 0; i < mod; i++)
                {
                    posList.Add(posList[^1]);
                }
            }
        }

        private void RenewTemp()
        {
            tempGridSize = gridSize;
            tempTileDiameter = tileDiameter;
            tempPosition = transform.position;
            tempAngle = transform.eulerAngles.y;
            tempGridFormationStyle = gridFormationStyle;
        }

        private static Vector2 RotatePointAroundPivot(Vector2 point, Vector2 pivot, float angles)
        {
            var pd = point - pivot;
            var res = Quaternion.Euler(new Vector3(0, angles, 0))
                      * new Vector3(pd.x, 0, pd.y)
                      + new Vector3(pivot.x, 0, pivot.y);
            return new Vector2(res.x, res.z);
        }

        public List<Vector2> CreateGrids()
        {
            return gridFormationStyle switch
            {
                GridFormationStyle.Rectangle => CreateRectangleGrids(),
                GridFormationStyle.Circle => CreateCircularGrids(),
                GridFormationStyle.Elliptical => CreateEllipticalGrids(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public List<Vector2> CreateRectangleGrids()
        {
            var agentCount = gridSize.x * gridSize.y;
            var array = new Vector2[agentCount];
            Vector2 gridCenter = new Vector2(transform.position.x, transform.position.z);
            var half = tileDiameter * 0.5f;
            Vector2 halfGridSize = new Vector2(gridSize.x * half, gridSize.y * half);

            int index = 0;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector2 gridPosition = new Vector2(x * tileDiameter, y * tileDiameter) - halfGridSize + gridCenter;
                    gridPosition = RotatePointAroundPivot(gridPosition, gridCenter, transform.eulerAngles.y);
                    array[index++] = gridPosition;
                }
            }

            return array.ToList();
        }

        public List<Vector2> CreateCircularGrids()
        {
            var agentCount = gridSize.x * gridSize.y;
            var array = new List<Vector2>(agentCount);
            Vector2 gridCenter = new Vector2(transform.position.x, transform.position.z);
            var half = tileDiameter * 0.5f;
            Vector2 halfGridSize = new Vector2(gridSize.x * half, gridSize.y * half);

            float radius = Mathf.Min(gridSize.x, gridSize.y) * half;

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector2 gridPosition = new Vector2(x * tileDiameter, y * tileDiameter) - halfGridSize + gridCenter;
                    gridPosition = RotatePointAroundPivot(gridPosition, gridCenter, transform.eulerAngles.y);

                    // Check if the point is within the circle
                    if (Vector2.Distance(gridPosition, gridCenter) <= radius)
                    {
                        array.Add(gridPosition);
                    }
                }
            }

            return array;
        }
        
        public List<Vector2> CreateEllipticalGrids()
        {
            var agentCount = gridSize.x * gridSize.y;
            var array = new List<Vector2>(agentCount);
            Vector2 gridCenter = new Vector2(transform.position.x, transform.position.z);
            var half = tileDiameter * 0.5f;
            Vector2 halfGridSize = new Vector2(gridSize.x * half, gridSize.y * half);

            float radiusX = gridSize.x * half;
            float radiusY = gridSize.y * half;

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector2 gridPosition = new Vector2(x * tileDiameter, y * tileDiameter) - halfGridSize + gridCenter;
                    gridPosition = RotatePointAroundPivot(gridPosition, gridCenter, transform.eulerAngles.y);

                    // Check if the point is within the ellipse
                    float ellipseValue = Mathf.Pow((gridPosition.x - gridCenter.x) / radiusX, 2) + Mathf.Pow((gridPosition.y - gridCenter.y) / radiusY, 2);
                    if (ellipseValue <= 1)
                    {
                        array.Add(gridPosition);
                    }
                }
            }

            return array;
        }

    }
}