using EachOneMatters.Gameplay.PlayerUnits;
using UnityEngine;

namespace EachOneMatters.Pool
{
    public class ChunkPool : ObjectPool<Chunk>
    {
        public void GetEffect(Transform transform)
        {
            Chunk effect = GetObject();
            effect.transform.position = transform.position;
            effect.transform.rotation = Quaternion.LookRotation(transform.forward);
            effect.OnFinishedEffect += ReturnEffect;
        }

        private void ReturnEffect(Chunk effect)
        {
            effect.OnFinishedEffect -= ReturnEffect;
            ReturnObject(effect);
        }
    }
}