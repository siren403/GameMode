using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameMode;
using GameMode.Editor.BuildProcess;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class BuildProcessTests
{
    private EditorBuildSettingsScene[] _scenes;

    private void Backup() => _scenes = EditorBuildSettings.scenes;
    private void Restore() => EditorBuildSettings.scenes = _scenes;

    [SetUp]
    public void SetUp()
    {
        Backup();
    }

    // A Test behaves as an ordinary method
    [Test]
    public void AddMasterSceneToFirstPasses()
    {
        EditorBuildSettings.scenes = Array.Empty<EditorBuildSettingsScene>();
        AddMasterSceneProcess.Execute(AppSettings.Instance);

        var path = AssetDatabase.GetAssetPath(AppSettings.Instance.MasterScene);
        Assert.Contains(path, EditorBuildSettings.scenes.Select(s => s.path).ToList());
        Restore();
    }
}