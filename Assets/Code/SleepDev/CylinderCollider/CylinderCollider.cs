using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SleepDev
{
    public class CylinderCollider : MonoBehaviour
    {
        [Header("EditorConfig")]
        [SerializeField] private bool _drawGizmos = true;
        [SerializeField] private bool _logMeshVerteces = false;
        [SerializeField] private bool _autoAddMeshComponents = true;
        [SerializeField] private bool _useMeshRenderer = true;
        [SerializeField] private bool _forceColliderConvex = true;
        [Header("Mesh Config")] 
        [SerializeField] private int _corners = 10;
        [SerializeField] private float _radius = 2;
        [SerializeField] private float _height = 3;
        [SerializeField] private float _centerPointsOffset = .1f;
        [SerializeField] private Vector3 _offset;
        [Header("Result Mesh")] 
        [SerializeField] private MeshCollider _meshCollider;
        [SerializeField] private Mesh _savedMesh;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Material _defaultMaterial;
        private CylinderMeshData _meshData;

        private void OnValidate()
        {
            if (_autoAddMeshComponents)
                TryAddViewComponents();
            if(_drawGizmos)
                BuildMeshData();
        }

        public void BuildAndAssign()
        {
            BuildNewMesh();
            AssignMesh();
        }
        
        public void BuildNewMesh()
        {
            BuildMeshData();
            var mesh = new Mesh();
            var count = _corners;
            var vertsCount = count * 2 + 2;
            var verts = new Vector3[vertsCount];
            var tris = new int[count * 4 * 3]; // sides (=2*count) + bot circle + top circle
            for (var i = 0; i < count; i++)
            {
                verts[i] = _meshData.topCircle[i];
                verts[i + count] = _meshData.botCircle[i];
            }
            var topPointIndex = vertsCount - 2;
            var botPointIndex = vertsCount - 1;

            verts[botPointIndex] = _meshData.botPoint;
            verts[topPointIndex] = _meshData.topPoint;

            var trisInd = 0;
            for (var i = 0; i < count; i++)
            {
                var nextCornerInd = i + 1;
                if (nextCornerInd >= count)
                    nextCornerInd = 0;
                // first triangle
                tris[trisInd + 2] = i; // top circle
                tris[trisInd + 1] = nextCornerInd; // top circle
                tris[trisInd] = i + count; // bottom circle
                // second triangle
                tris[trisInd + 5] = nextCornerInd; // top circle
                tris[trisInd + 4] = nextCornerInd + count; // bottom circle
                tris[trisInd + 3] = i + count; // bottom circle
                trisInd += 6;
            }
            // filling the top circle
            for (var i = 0; i < count; i++)
            {
                var nextCornerInd = i + 1;
                if (nextCornerInd >= count)
                    nextCornerInd = 0;
                tris[trisInd + 0] = topPointIndex;
                tris[trisInd + 1] = i;
                tris[trisInd + 2] = nextCornerInd;
                trisInd += 3;
            }
            
            // filling the bot circle
            for (var i = count; i < count * 2; i++)
            {
                var nextCornerInd = i + 1;
                if (nextCornerInd >= count * 2)
                    nextCornerInd = count;
                tris[trisInd + 2] = botPointIndex;
                tris[trisInd + 1] = i;
                tris[trisInd + 0] = nextCornerInd;
                trisInd += 3;
            }
            
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.normals = new Vector3[vertsCount];
            mesh.uv = new Vector2[vertsCount];
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            _savedMesh = mesh;
            TrySetDirty();
            SaveMeshAsset();
        }

        public void AssignMesh()
        {
            TryAddViewComponents();
            if (_meshCollider != null)
                _meshCollider.sharedMesh = _savedMesh;
            if(_meshFilter != null)
                _meshFilter.mesh = _savedMesh;
            if (_useMeshRenderer && _meshRenderer != null)
                _meshRenderer.enabled = true;
        }
        
        
        public void UpdateViewComponents()
        {
            if (_meshFilter == null)
            {
                if (_meshRenderer != null)
                    _meshRenderer.enabled = false;
            }
            else
            {
                if (_meshRenderer != null)
                    _meshRenderer.enabled = _meshFilter.sharedMesh != null;
            }
        }

        public void TryAddViewComponents()
        {
            var dirty = false;
            _meshCollider = gameObject.GetComponent<MeshCollider>();
            if (_meshCollider == null)
            {
                _meshCollider = gameObject.AddComponent<MeshCollider>();
                dirty = true;
            }
            if(_forceColliderConvex)
                _meshCollider.convex = true;
            
            if (_useMeshRenderer)
            {
                _meshFilter = gameObject.GetComponent<MeshFilter>();
                if (_meshFilter == null)
                {
                    _meshFilter = gameObject.AddComponent<MeshFilter>();
                    dirty = true;
                }
                _meshRenderer = gameObject.GetComponent<MeshRenderer>();
                if (_meshRenderer == null)
                {
                    _meshRenderer = gameObject.AddComponent<MeshRenderer>();
                    dirty = true;
                }
                _meshRenderer.sharedMaterial = _defaultMaterial;
                _meshRenderer.receiveShadows = false;
            }
      
#if UNITY_EDITOR
            if(dirty)
                UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }

        public void BuildMeshData()
        {
            if (_corners < 4)
                _corners = 4;
            _meshData = new CylinderMeshData(_corners);
            var localCenter = _offset;
            var radiusVector = new Vector3(0, 0, 1);
            var rotationStep = 360f / _corners;
            var rotationAxis = Vector3.up;
            var angle = 0f;
            var vertOffset = rotationAxis * _height * .5f;
            if(_logMeshVerteces)
                CLog.Log($"Total Count {_corners}. Rotation Step {rotationStep}");
            _meshData.topPoint = localCenter + (vertOffset + rotationAxis * _centerPointsOffset);
            _meshData.botPoint = localCenter - (vertOffset + rotationAxis * _centerPointsOffset);
            for (var i = 0; i < _corners; i++)
            {
                var vector = Quaternion.AngleAxis(angle, rotationAxis) * radiusVector;
                var cornerPos = localCenter + vector.normalized * _radius;
                var botPos = cornerPos - vertOffset;
                var topPos = cornerPos + vertOffset;
                _meshData.botCircle.Add(cornerPos - vertOffset);
                _meshData.topCircle.Add(cornerPos + vertOffset);
                if(_logMeshVerteces)
                    CLog.Log($"i {i}. Top Pos {topPos}. Bot Pos {botPos}. Angle: {angle}");
                angle += rotationStep;
            }
            TrySetDirty();
        }

        public void LogMeshData()
        {
            if (_savedMesh == null)
            {
                CLog.LogRed("_savedMesh is NULL");
                return;
            }
            var msg = "";
            msg += $"Vertices count {_savedMesh.vertices.Length}";
            msg += $"\nTriangles array length {_savedMesh.triangles.Length}. Triangles {_savedMesh.triangles.Length/3}";
            CLog.LogWhite(msg);            
        }
        
        private void SaveMeshAsset()
        {
#if UNITY_EDITOR
            var name = GetAssetName();
            try
            {
                AssetDatabase.CreateAsset(_savedMesh, $"Assets/Resources/{name}.asset");
            }
            catch (System.Exception ex)
            {
                CLog.Log(ex.Message);
                CLog.Log(ex.StackTrace);
                return;
            }
            _savedMesh = Resources.Load<Mesh>(name);
            TrySetDirty();
#endif
        }
        
        private string GetAssetName() => "cylinder_mesh" + $"_{UnityEngine.Random.Range(0, 100)}";

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_drawGizmos == false)
                return;
            if(_meshData == null)
                BuildMeshData();    
            var count = _corners;
            var gizmosColor = Color.blue;
            var oldColor = Gizmos.color;
            Gizmos.color = gizmosColor;
            for (var i = 0; i < count; i++)
            {
                var nextCornerInd = i + 1;
                if (nextCornerInd >= count)
                    nextCornerInd = 0;
                var p1 = transform.TransformPoint(_meshData.topCircle[i]);
                var p2 = transform.TransformPoint(_meshData.botCircle[i]);
                var p3 = transform.TransformPoint(_meshData.topCircle[nextCornerInd]);
                var p4 = transform.TransformPoint(_meshData.botCircle[nextCornerInd]);
                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p1);
                Gizmos.DrawLine(p2, p4);
            }
            Gizmos.color = oldColor;
        }
#endif
        
        private void TrySetDirty()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif 
        }
        
        private class CylinderMeshData
        {
            public List<Vector3> topCircle;
            public List<Vector3> botCircle;
            public Vector3 topPoint;
            public Vector3 botPoint;
            
            public CylinderMeshData(int cornersCount)
            {
                topCircle = new List<Vector3>(cornersCount);
                botCircle = new List<Vector3>(cornersCount);
            }
        }
    }
}
