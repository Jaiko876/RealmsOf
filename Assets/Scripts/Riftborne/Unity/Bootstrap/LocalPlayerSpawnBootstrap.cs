using System;
using Riftborne.App.Commands.Queue;
using Riftborne.App.Time;
using Riftborne.Core.Input.Commands;
using Riftborne.Core.Model;
using UnityEngine;
using VContainer;

namespace Riftborne.Unity.Bootstrap
{
    public sealed class LocalPlayerSpawnBootstrap : MonoBehaviour
    {
        [SerializeField] private bool spawnOnStart = true;

        [Header("Who")]
        [SerializeField] private int playerId = 0;

        [Header("What")]
        [SerializeField] private string avatarPrefabKey = "Player";

        [Header("Where")]
        [SerializeField] private Vector2 spawnPosition = new Vector2(0f, 0f);

        [Header("Optional fixed entity id (-1 = allocator)")]
        [SerializeField] private int fixedEntityId = -1;

        private ICommandQueue _queue;
        private ITickClock _clock;

        [Inject]
        public void Construct(ICommandQueue queue, ITickClock clock)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        private void Start()
        {
            if (!spawnOnStart)
                return;

            int tick = _clock.CurrentTick;

            var pid = new PlayerId(playerId);

            if (fixedEntityId >= 0)
            {
                _queue.Enqueue(new SpawnPlayerAvatarCommand(
                    tick,
                    pid,
                    avatarPrefabKey,
                    spawnPosition.x,
                    spawnPosition.y,
                    new GameEntityId(fixedEntityId)));
            }
            else
            {
                _queue.Enqueue(new SpawnPlayerAvatarCommand(
                    tick,
                    pid,
                    avatarPrefabKey,
                    spawnPosition.x,
                    spawnPosition.y));
            }
        }
    }
}