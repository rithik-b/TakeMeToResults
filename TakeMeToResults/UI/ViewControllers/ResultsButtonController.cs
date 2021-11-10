using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using IPA.Utilities;
using System;
using System.ComponentModel;
using System.Reflection;
using TakeMeToResults.AffinityPatches;
using UnityEngine;
using Zenject;

namespace TakeMeToResults.UI
{
    internal class ResultsButtonController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private readonly TitleViewController titleViewController;
        private readonly ResultsViewController resultsViewController;
        private readonly MainFlowCoordinator mainFlowCoordinator;
        private readonly PresentFlowCoordinatorPatch presentFlowCoordinatorPatch;

        private ViewController leftScreenViewController;
        private ViewController rightScreenViewController;
        private ViewController bottomScreenViewController;
        private ViewController topScreenViewController;
        private ViewController mainScreenViewController;
        private FlowCoordinator deepestChildFlowCoordinator;

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Action ShowOther;

        [UIComponent("results-button")]
        private readonly RectTransform resultsButtonTransform;

        public ResultsButtonController(HierarchyManager hierarchyManager, ResultsViewController resultsViewController, MainFlowCoordinator mainFlowCoordinator,
            PresentFlowCoordinatorPatch presentFlowCoordinatorPatch)
        {
            ScreenSystem screenSystem = hierarchyManager.GetField<ScreenSystem, HierarchyManager>("_screenSystem");
            titleViewController = screenSystem.titleViewController;
            this.resultsViewController = resultsViewController;
            this.mainFlowCoordinator = mainFlowCoordinator;
            this.presentFlowCoordinatorPatch = presentFlowCoordinatorPatch;
            ShowOther = ShowOtherViewControllers;
        }

        public void Initialize()
        {
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "TakeMeToResults.UI.Views.ResultsButton.bsml"), titleViewController.gameObject, this);
            resultsButtonTransform.gameObject.name = "TakeMeToResults";
            resultsViewController.continueButtonPressedEvent += GetViewControllers;
            presentFlowCoordinatorPatch.FlowCoordinatorChanged += UpdateFlowAndButtonState;
        }

        public void Dispose()
        {
            resultsViewController.continueButtonPressedEvent -= GetViewControllers;
            presentFlowCoordinatorPatch.FlowCoordinatorChanged -= UpdateFlowAndButtonState;
        }

        private void GetViewControllers(ResultsViewController resultsViewController)
        {
            deepestChildFlowCoordinator = DeepestChildFlowCoordinator(mainFlowCoordinator);

            leftScreenViewController = deepestChildFlowCoordinator.GetField<ViewController, FlowCoordinator>("_leftScreenViewController");
            rightScreenViewController = deepestChildFlowCoordinator.GetField<ViewController, FlowCoordinator>("_rightScreenViewController");
            bottomScreenViewController = deepestChildFlowCoordinator.GetField<ViewController, FlowCoordinator>("_bottomScreenViewController");
            topScreenViewController = deepestChildFlowCoordinator.GetField<ViewController, FlowCoordinator>("_topScreenViewController");
            mainScreenViewController = deepestChildFlowCoordinator.topViewController;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ButtonActive)));
        }

        private void UpdateFlowAndButtonState()
        {
            deepestChildFlowCoordinator = DeepestChildFlowCoordinator(mainFlowCoordinator);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ButtonActive)));
        }

        [UIAction("results-click")]
        private void ResultsClick()
        {
            if (!(deepestChildFlowCoordinator is SinglePlayerLevelSelectionFlowCoordinator))
            {
                return;
            }

            if (mainScreenViewController != null)
            {
                deepestChildFlowCoordinator.InvokeMethod<object, FlowCoordinator>("PresentViewController", new object[] { mainScreenViewController, ShowOther, ViewController.AnimationDirection.Vertical, false });
            }
        }

        private void ShowOtherViewControllers()
        {
            if (leftScreenViewController != null)
            {
                deepestChildFlowCoordinator.InvokeMethod<object, FlowCoordinator>("SetLeftScreenViewController", new object[] { leftScreenViewController, ViewController.AnimationType.In });
            }

            if (rightScreenViewController != null)
            {
                deepestChildFlowCoordinator.InvokeMethod<object, FlowCoordinator>("SetRightScreenViewController", new object[] { rightScreenViewController, ViewController.AnimationType.In });
            }

            if (bottomScreenViewController != null)
            {
                deepestChildFlowCoordinator.InvokeMethod<object, FlowCoordinator>("SetBottomScreenViewController", new object[] { bottomScreenViewController, ViewController.AnimationType.In });
            }

            if (topScreenViewController != null)
            {
                deepestChildFlowCoordinator.InvokeMethod<object, FlowCoordinator>("SetTopScreenViewController", new object[] { topScreenViewController, ViewController.AnimationType.In });
            }
        }

        private FlowCoordinator DeepestChildFlowCoordinator(FlowCoordinator root)
        {
            var flow = root.childFlowCoordinator;
            if (flow == null) return root;
            if (flow.childFlowCoordinator == null || flow.childFlowCoordinator == flow)
            {
                return flow;
            }
            return DeepestChildFlowCoordinator(flow);
        }

        [UIValue("button-active")]
        private bool ButtonActive => mainScreenViewController != null && deepestChildFlowCoordinator is SinglePlayerLevelSelectionFlowCoordinator;
    }
}
