using System;
using System.Collections.Generic;
using System.Linq;
using Riftborne.Core.TIme;
using Riftborne.Unity.Bootstrap.Runtime;
using Riftborne.Unity.Debugging;
using Riftborne.Unity.Input;
using Riftborne.Unity.UI;
using Riftborne.Unity.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Riftborne.Unity.Bootstrap
{
    public sealed class GameRuntimeComposer : IStartable, IDisposable
    {
        private readonly IObjectResolver _root;
        private readonly SimulationParameters _simParams;
        private readonly IReadOnlyList<IRuntimeInitializer> _initializers;

        private IScopedObjectResolver _runtimeScope;

        public GameRuntimeComposer(
            IObjectResolver root,
            SimulationParameters simParams,
            IEnumerable<IRuntimeInitializer> initializers)
        {
            _root = root;
            _simParams = simParams;
            _initializers = initializers
                .OrderBy(x => x.Order)
                .ToArray();
        }

        public void Start()
        {
            Initialize();
        }

        public void Dispose()
        {
            if (_runtimeScope != null)
            {
                _runtimeScope.Dispose();
                _runtimeScope = null;
            }
        }

        private void Initialize()
        {
            Time.fixedDeltaTime = _simParams.TickDeltaTime;

            _runtimeScope = _root.CreateScope(builder =>
            {
                builder.RegisterInstance(_simParams);

                for (int i = 0; i < _initializers.Count; i++)
                {
                    _initializers[i].Initialize(builder);
                }
                builder.RegisterComponentInHierarchy<PlayerStatsDebugLog>();
                // --- Scene components (в root, чтобы composer мог их найти) ---
                builder.RegisterComponentInHierarchy<EntityView>();
                builder.RegisterComponentInHierarchy<PlayerAvatarBinding>();
                
                builder.RegisterComponentInHierarchy<PlayerHudBarsPresenter>();
                builder.RegisterComponentInHierarchy<WorldStaggerBarView>();
                builder.RegisterComponentInHierarchy<PlayerInputController>();
                builder.RegisterComponentInHierarchy<PlayerInputAdapter>();

            });
        }
    }
}