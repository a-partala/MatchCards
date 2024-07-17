using Zenject;
using UnityEngine;

public class StorageInstaller : MonoInstaller
{
    public enum SaveMethod
    {
        JSON,
        Base64,
        PlayerPrefs
    }

    [SerializeField] private SaveMethod saveMethod;

    public override void InstallBindings()
    {
        switch (saveMethod)
        {
            case SaveMethod.JSON:
                Container.Bind<IStorage>().To<JsonStorage>().AsSingle();
                break;
            case SaveMethod.Base64:
                Container.Bind<IStorage>().To<Base64Storage>().AsSingle();
                break;
            case SaveMethod.PlayerPrefs:
                Container.Bind<IStorage>().To<PlayerPrefsStorage>().AsSingle();
                break;
        }
        Container.Bind<SaveService>().AsSingle().NonLazy();
    }
}