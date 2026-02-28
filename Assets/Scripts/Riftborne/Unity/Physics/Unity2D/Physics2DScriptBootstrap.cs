using UnityEngine;

namespace Riftborne.Unity.Physics.Unity2D
{

    /// <summary>
    /// Keeps Physics2D in Script mode, so we can step it from Core tick loop.
    /// </summary>
    public static class Physics2DScriptBootstrap
    {
        public static void EnsureScriptMode()
        {
            Physics2D.simulationMode = SimulationMode2D.Script;
        }
    }
}
