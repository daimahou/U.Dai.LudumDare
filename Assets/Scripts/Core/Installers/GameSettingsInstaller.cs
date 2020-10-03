using UnityEngine;
using Zenject;

namespace Core.Installers
{
    [CreateAssetMenu(fileName = "Game Settings", menuName = "Installers/Game Settings")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        [SerializeField] private MoveHandler.Settings m_moveSettings;
        [SerializeField] private Controller.Settings m_controllerSettings;
        
        public override void InstallBindings()
        {
            Container.BindInstance(m_moveSettings).AsSingle();
            Container.BindInstance(m_controllerSettings).AsSingle();
        }
    }
}