using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using IPA.Utilities;
using System;
using System.ComponentModel;
using System.Reflection;
using Zenject;

namespace TakeMeToResults.UI.ViewControllers
{
    internal class ResultsButtonController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private readonly TitleViewController titleViewController;
        private readonly ResultsViewController resultsViewController;
        private readonly MainFlowCoordinator mainFlowCoordinator;

        private ViewController leftScreenViewController;
        private ViewController rightScreenViewController;
        private ViewController bottomScreenViewController;
        private ViewController topScreenViewController;
        private ViewController mainScreenViewController;

        public event PropertyChangedEventHandler PropertyChanged;

        public ResultsButtonController(HierarchyManager hierarchyManager, ResultsViewController resultsViewController, MainFlowCoordinator mainFlowCoordinator)
        {
            ScreenSystem screenSystem = hierarchyManager.GetField<ScreenSystem, HierarchyManager>("_screenSystem");
            titleViewController = screenSystem.titleViewController;
            this.resultsViewController = resultsViewController;
            this.mainFlowCoordinator = mainFlowCoordinator;
        }

        public void Initialize()
        {
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "TakeMeToResults.UI.Views.ResultsButton.bsml"), titleViewController.gameObject, this);
            resultsViewController.continueButtonPressedEvent += GetViewControllers;
        }

        public void Dispose()
        {
            resultsViewController.continueButtonPressedEvent -= GetViewControllers;
        }

        private void GetViewControllers(ResultsViewController resultsViewController)
        {
            FlowCoordinator deepestChildFlowCoordinator = DeepestChildFlowCoordinator(mainFlowCoordinator);

            leftScreenViewController = deepestChildFlowCoordinator.GetField<ViewController, FlowCoordinator>("_leftScreenViewController");
            rightScreenViewController = deepestChildFlowCoordinator.GetField<ViewController, FlowCoordinator>("_rightScreenViewController");
            bottomScreenViewController = deepestChildFlowCoordinator.GetField<ViewController, FlowCoordinator>("_bottomScreenViewController");
            topScreenViewController = deepestChildFlowCoordinator.GetField<ViewController, FlowCoordinator>("_topScreenViewController");
            mainScreenViewController = deepestChildFlowCoordinator.topViewController;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ButtonActive)));
        }

        [UIAction("results-click")]
        private void ResultsClick()
        {
            FlowCoordinator deepestChildFlowCoordinator = DeepestChildFlowCoordinator(mainFlowCoordinator);

            if (mainScreenViewController != null)
            {
                deepestChildFlowCoordinator.InvokeMethod<object, FlowCoordinator>("PresentViewController", new object[] { mainScreenViewController, null, ViewController.AnimationDirection.Vertical, true });
            }

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
        private bool ButtonActive => mainScreenViewController != null;
    }
}
