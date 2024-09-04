using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SleepDev.Chunks
{
    public class ChunkManager : MonoBehaviour, IChunkManager
    {
        [SerializeField] private List<BaseChunk> _chunks;
        [SerializeField] private ChunkManagerSettings _settings;
        [SerializeField] private ChunksBuilder _chunksBuilder;
        [SerializeField] private Camera _camera;
        [SerializeField] private float _cameraZ = 100;
        #if UNITY_EDITOR
        [Header("Editor Only")]
        public bool drawRectsOnChunks;
        public bool drawCameraFrustum;
        #endif
        
        private float _radius; 
        private bool _isActive;
        private CancellationTokenSource _tokenSource;
        private List<BaseChunk> _activeChunks;

        private Vector3[] _cameraCorners = new Vector3[4];
        private Vector2[] _cameraCornersXZ = new Vector2[4];
        private Vector2[] _chunkCornersXZ = new Vector2[4];
        
        private Plane XZPlane = new Plane(Vector3.up, Vector3.zero);
        private const float GizmoDrawTime = 1f;

        private readonly List<Vector2> _cornerDirections = new()
        {
            new Vector2(-1, 1), // top left
            new Vector2(1, 1), // top right
            new Vector2(1, -1), // bot right
            new Vector2(-1, -1) // bot left
        };
        
        public Camera ActiveCamera
        {
            get => _camera;
            set => SetNewCamera(value);
        }

        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }

        private void Awake()
        {
            _radius = _settings.defaultRadius;
            _camera = Camera.main;
        }

        public void Activate()
        {
            if (_isActive)
                return;
            _isActive = true;
            _tokenSource = new CancellationTokenSource();
            CheckingRadius(_tokenSource.Token);
        }

        public void Deactivate()
        {
            _isActive = false;
            _tokenSource?.Cancel();
        }
        
        private void SetNewCamera(Camera camera)
        {
            _camera = camera;
        }

        private async void CheckingRadius(CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                ActivateInFrustum();
                await Task.Yield();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ActivateInFrustum()
        {
            var camTr = _camera.transform;
            var camRot = camTr.rotation;
            var camPos = camTr.position;
            _camera.CalculateFrustumCorners(new Rect(0,0,1,1), _cameraZ, 
            Camera.MonoOrStereoscopicEye.Mono, _cameraCorners);
            for (var i = 0; i < 4; i++)
            {
                _cameraCorners[i] =  camRot * _cameraCorners[i];
                var ray = new Ray(camPos, _cameraCorners[i] - camPos);
                if (!XZPlane.Raycast(ray, out var enter))
                    enter = 250;
                var p = ray.GetPoint(enter);
                _cameraCornersXZ[i] = new Vector2(p.x, p.z);
#if UNITY_EDITOR
                if (drawCameraFrustum)
                    Debug.DrawLine(camPos, p, Color.red, GizmoDrawTime);
#endif
            }

            var size = _chunks[0].Size * .5f * 1.4142135624f;
            foreach (var ch in _chunks)
            {
                var chunkPos = ch.transform.position;
                for (var i = 0; i < 4; i++)
                {
                    var center = new Vector2(chunkPos.x, chunkPos.z);
                    _chunkCornersXZ[i] = center + _cornerDirections[i] * size;
                }
#if UNITY_EDITOR
                if (drawRectsOnChunks)
                {
                    Debug.DrawLine(_chunkCornersXZ[0].ToVec3XZ(0), _chunkCornersXZ[2].ToVec3XZ(0), Color.green, GizmoDrawTime);
                    Debug.DrawLine(_chunkCornersXZ[3].ToVec3XZ(0), _chunkCornersXZ[1].ToVec3XZ(0), Color.blue, GizmoDrawTime);
                }
#endif
                
            }
        }
        
        public void ActivateAll()
        {
            foreach (var ch in _chunks)
                ch.gameObject.SetActive(true);
        }

        public void DeactivateAll()
        {
            foreach (var ch in _chunks)
                ch.gameObject.SetActive(false);
        }
        
        #if UNITY_EDITOR
        public void E_BuildChunks()
        {
            _chunksBuilder.Parent = transform;
            _chunksBuilder.BuildChunks();
            _chunks = _chunksBuilder.Chunks;
            UnityEditor.EditorUtility.SetDirty(this);
        }

        public void E_DeleteChunks()
        {
            _chunksBuilder.Parent = transform;
            _chunksBuilder.DeleteSpawned();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        public void E_FetchChunksFromBuilder()
        {
            _chunks = _chunksBuilder.Chunks;
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif

    }
}