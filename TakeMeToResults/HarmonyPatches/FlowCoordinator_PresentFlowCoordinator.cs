using HarmonyLib;
using HMUI;
using System;

namespace TakeMeToResults.HarmonyPatches
{
    [HarmonyPatch(typeof(FlowCoordinator), "PresentFlowCoordinator")]
    internal class FlowCoordinator_PresentFlowCoordinator
    {
        public static event Action FlowCoordinatorChanged;
        private static void Postfix()
        {
            FlowCoordinatorChanged?.Invoke();
        }
    }
}
