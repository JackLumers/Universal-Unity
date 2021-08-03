# Universal-Unity
Scripts for easyer game development in Unity. Not properly documented yet and also can depend on other frameworks, I'll work on that later.

[Russian fast-written doc for innner usage](https://docs.google.com/document/d/15KHWurD4m-SBTkwx2fv_B5KpM4ONifXPUAMkMnKCq7I/edit?usp=sharing)

# Installing
Install all dependencies and add to your Unity project as a submodule. 
Recommended path: Assets/UniversalUnity

Later I'll release package as a Unity package.

# Dependencies
Unity packages:
- Unity [Addressables](https://docs.unity3d.com/Manual/com.unity.addressables.html)
- [UniTask](https://github.com/Cysharp/UniTask) for better async/await integration for Unity. 
Can be added as package if Unity >= 2019.3.4f1, Unity >= 2020.1a21.

Other libraries:
- [DOTween](http://dotween.demigiant.com/getstarted.php) for data-driven animation scripting.
Note: add UNITASK_DOTWEEN_SUPPORT in ProjectSettings -> Player -> Other Settings -> Scripting Define Symbols
- [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) for getting NuGet packages.
- [FileHelpers](https://github.com/MarcosMeli/FileHelpers) for csv files parsing.

