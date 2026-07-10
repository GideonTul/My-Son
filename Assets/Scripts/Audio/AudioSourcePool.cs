using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    /// <summary>
    /// Pools PooledAudioSource instances so gameplay never pays for runtime
    /// Instantiate/Destroy of AudioSources. Grows up to maxSize, then recycles
    /// the oldest currently-playing voice instead of growing further.
    /// </summary>
    public class AudioSourcePool
    {
        private readonly Queue<PooledAudioSource> _available = new Queue<PooledAudioSource>();
        private readonly List<PooledAudioSource> _all = new List<PooledAudioSource>();
        private readonly Transform _parent;
        private readonly int _maxSize;

        public AudioSourcePool(Transform parent, int prewarmCount, int maxSize)
        {
            _parent = parent;
            _maxSize = Mathf.Max(1, maxSize);

            for (int i = 0; i < prewarmCount; i++)
            {
                _available.Enqueue(CreateNew());
            }
        }

        private PooledAudioSource CreateNew()
        {
            var go = new GameObject("PooledAudioSource");
            go.transform.SetParent(_parent, false);
            var pooled = go.AddComponent<PooledAudioSource>();
            go.SetActive(false);
            _all.Add(pooled);
            return pooled;
        }

        public PooledAudioSource Get()
        {
            PooledAudioSource pooled = _available.Count > 0 ? _available.Dequeue() : null;

            if (pooled == null)
            {
                pooled = _all.Count < _maxSize ? CreateNew() : FindOldestPlaying();
            }

            pooled.gameObject.SetActive(true);
            return pooled;
        }

        public void Release(PooledAudioSource pooled)
        {
            pooled.gameObject.SetActive(false);
            _available.Enqueue(pooled);
        }

        private PooledAudioSource FindOldestPlaying()
        {
            PooledAudioSource oldest = _all[0];
            foreach (var p in _all)
            {
                if (p.StartTime < oldest.StartTime) oldest = p;
            }
            return oldest;
        }
    }
}
