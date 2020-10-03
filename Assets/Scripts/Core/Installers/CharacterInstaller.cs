using Zenject;

namespace Core.Installers
{
    public class CharacterInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var gridObject = GetComponent<GridObject>();
            var gridMovement = GetComponent<GridMovement>();
            
            Container.Bind<MoveHandler>().AsSingle();
            Container.BindInstance(gridObject).AsSingle();
            Container.BindInstance(gridMovement).AsSingle();
        }
    }
}