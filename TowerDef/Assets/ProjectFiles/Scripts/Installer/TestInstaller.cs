using Zenject;
using UnityEngine;

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("Zenject: Installer executed successfully.");
    }
}
