using TakeMeToResults.UI.ViewControllers;
using Zenject;

namespace TakeMeToResults.Installers
{
    internal class TakeMeToResultsMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ResultsButtonController>().AsSingle();
        }
    }
}
