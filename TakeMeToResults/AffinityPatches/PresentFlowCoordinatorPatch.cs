using HMUI;
using SiraUtil.Affinity;
using System;

namespace TakeMeToResults.AffinityPatches
{
    internal class PresentFlowCoordinatorPatch : IAffinity
    {
        public event Action FlowCoordinatorChanged;

        [AffinityPatch(typeof(FlowCoordinator), "PresentFlowCoordinator")]
        private void PresentFlowCoordinator()
        {
            FlowCoordinatorChanged?.Invoke();
        }
    }
}
