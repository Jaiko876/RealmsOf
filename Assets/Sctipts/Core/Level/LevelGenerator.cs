using System;
using System.Collections.Generic;
using Game.Core.Abstractions;

namespace Game.Core.Level
{
    public sealed class LevelGenerator
    {
        private readonly IRandomFactory _randomFactory;

        // Список пассов — расширяемость без переписывания генератора
        private readonly List<ILevelGenPass> _passes = new List<ILevelGenPass>();

        public LevelGenerator(IRandomFactory randomFactory)
        {
            _randomFactory = randomFactory;

            // Default pipeline
            _passes.Add(new HeightmapPass());
            _passes.Add(new TilesFromHeightmapPass());
            _passes.Add(new SpawnerObjectsPass());
        }

        public LevelDefinition Generate(LevelGenConfig config, LevelSeed seed)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (config.Width < 1) throw new ArgumentOutOfRangeException(nameof(config.Width));

            var rng = _randomFactory.Create(unchecked((uint)seed.Value));
            var ctx = new LevelGenContext(config, rng);

            for (int i = 0; i < _passes.Count; i++)
                _passes[i].Apply(ctx);

            return ctx.BuildDefinition();
        }

        // Позволяет “подмешать” пассы из DI, если захочешь позже (не обязательно сейчас)
        public void AddPass(ILevelGenPass pass)
        {
            if (pass == null) return;
            _passes.Add(pass);
        }
    }
}
