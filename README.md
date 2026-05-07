# Simple SO Static Access
![](<Simple SO Static Access/cover.png>)

`Simple SO Static Access` is a small utility that generates static C## code from `ScriptableObject` data.
The main idea is simple:
- you store configuration values inside a `ScriptableObject`;
- the system generates a static wrapper class;
- you access those values through static properties in code.
This is useful for global gameplay settings, balancing values, project-wide configuration, and other data that should be available from anywhere.

## Basic workflow
1. Create a class that inherits from `StaticDataSO`.
2. Add serializable fields.
3. Create an asset in Unity.
4. Generate the wrapper.
5. Use the generated static class in code.

## Creating a StaticDataSO script
You can create a new `StaticDataSO` script directly from the Unity Project window.
Path:
```text
Create -> Scripting -> Static Data SO
```
This creates a new C## script that already inherits from `StaticDataSO`.
Generated template example:
```csharp
using QuietNoize.SimpleSOStaticAccess;
using UnityEngine;

[CreateAssetMenu(menuName = "Static Data/New Static Data")]
public class NewStaticDataSO : StaticDataSO
{

}
```
This helps avoid setup mistakes and ensures the asset is compatible with the generation system.

## StaticDataSO
`StaticDataSO` is the base class used by the generation system.
Every asset that should generate static code must inherit from it.
Example:
```csharp
using QuietNoize.SimpleSOStaticAccess;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Balance Settings")]
public class GameBalanceSettingsSO : StaticDataSO
{
    [SerializeField] private float m_baseMoveSpeed;
    [SerializeField] private float m_globalDamageMultiplier;
    [SerializeField] private int m_maxInventorySlots;
}
```
The generator uses this base type to identify valid source assets.

## Important requirement
**If your asset does not inherit from `StaticDataSO`, the generation system will not work.**
The generator only processes assets derived from `StaticDataSO`.
That means:
- no inheritance → no generation;
- no generation → no static wrapper.

## How generation works
`StaticDataGenerator.SyncConstants(StaticDataSO so)` performs the generation process.
The method:
1. reads serialized fields from the asset;
2. generates C## code;
3. saves the generated `.cs` file;
4. refreshes Unity through `AssetDatabase.Refresh()`.
The generated code contains static properties with values copied from the asset.
Example result:
```csharp
public static class GameBalanceSettingsWrapper
{
    public static float baseMoveSpeed => 4.5f;
    public static float globalDamageMultiplier => 1.25f;
    public static int maxInventorySlots => 32;
}
```

## Serializable fields
The generator only includes fields that Unity can serialize.
Included:
- public fields;
- fields marked with `[SerializeField]`.
Not included:
- private fields without `[SerializeField]`;
- internal generator settings.
Example:
```csharp
[SerializeField] private int m_maxLives;
public float moveSpeed;
```
Both fields above will be included in generation.

## Custom inspector
If you need a custom inspector, there are two important rules.
### Rule 1
Your custom editor must inherit from `StaticDataSOEditor`.
Example:
```csharp
public class GameBalanceSettingsSOEditor : StaticDataSOEditor { }
```
### Rule 2
Custom inspector drawing should be implemented in `DrawCustomInspectorGUI`.
Example:
```csharp
protected override void DrawCustomInspectorGUI()
{
    EditorGUILayout.LabelField("Custom Settings");
}
```
This keeps the editor compatible with the generation pipeline.

## Generation settings
`StaticDataCodegenSettings` contains the settings used during generation.
- **IsAutoSaved:** Controls whether generated code is saved automatically.
- **WrapperCodePath:** Defines the output folder for generated code. If the value is empty, the generated file is placed next to the source asset.
- **WrapperClassName:** Defines the name of the generated static class.
- **WrapperCodeNamespace:** Defines the namespace used in generated code.

## Supported types
The generator supports:
- primitive types;
- enums;
- arrays;
- `List<T>`;
- serializable custom types.
Examples:
```csharp
int
float
bool
string
List<int>
Vector3
```
Complex objects are serialized through `JsonUtility`.

## Field name conversion
Generated property names are created automatically.
The system removes common prefixes:
- `_`
- `m_`
- `s_`
Examples:
```csharp
m_moveSpeed      -> moveSpeed
_damage          -> damage
s_maxLives       -> maxLives
```

## Example usage
### Source asset
```csharp
using QuietNoize.SimpleSOStaticAccess;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Balance Settings")]
public class GameBalanceSettingsSO : StaticDataSO
{
    [SerializeField] private float m_baseMoveSpeed;
    [SerializeField] private float m_globalDamageMultiplier;
    [SerializeField] private int m_maxInventorySlots;
}
```
### Generated wrapper
```csharp
public static class GameBalanceSettingsWrapper
{
    public static float baseMoveSpeed => 4.5f;
    public static float globalDamageMultiplier => 1.25f;
    public static int maxInventorySlots => 32;
}
```
### Runtime usage
```csharp
float speed = GameBalanceSettingsWrapper.baseMoveSpeed;
int slots = GameBalanceSettingsWrapper.maxInventorySlots;
```

## Notes
- Generation only works inside the Unity Editor.
- After generation, Unity refreshes the Asset Database automatically.
- `List<T>` values are serialized through an internal wrapper.
- Complex objects must still be Unity-serializable.

## Quick checklist
Before using the system:
- inherit your asset from `StaticDataSO`;
- make fields serializable;
- set `WrapperClassName`;
- optionally configure `WrapperCodePath`;
- inherit custom editors from `StaticDataSOEditor`;
- implement custom UI in `DrawCustomInspectorGUI`.

## Summary
`Simple SO Static Access` allows you to keep editable configuration data inside `ScriptableObject` assets while automatically generating static access code.
The most important rules are:
1. Source assets must inherit from `StaticDataSO`.
2. Custom editors must inherit from `StaticDataSOEditor`.
3. Custom inspector UI should be implemented in `DrawCustomInspectorGUI`.
