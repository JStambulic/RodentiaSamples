#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Changes the Inspector for a ConsumableObject to only allow for valid combinations of Types and Subtypes.
/// </summary>
[CustomEditor(typeof(ConsumableObject))]
public class ConsumableObjectEditor : UnityEditor.Editor
{
    #region Member Variables

    // Toolbar buttons that will be seen in Inspector.
    string[] _toolbarConsumableType = new string[3] { "HealthRestore", "EnergyRestore", "TemporaryBoost" };
    string[] _toolbarConsumableSubTypeHealth = new string[3] { "SmallHealth", "MediumHealth", "LargeHealth" };
    string[] _toolbarConsumableSubTypeEnergy = new string[3] { "SmallEnergy", "MediumEnergy", "LargeEnergy" };
    string[] _toolbarConsumableSubTypeBoost = new string[3] { "HealthBoost", "EnergyBoost", "SpeedBoost" };

    // Holds current selection from the toolbars above.
    int _currentToolbarType;
    int _currentToolbarSubType;

    private SerializedProperty _consumableType;
    private SerializedProperty _consumableSubType;

    private SerializedObject _consumableObject;

    #endregion

    #region OnEnable, OnInspector

    private void OnEnable()
    {
        _consumableObject = new SerializedObject(target);

        _consumableType = _consumableObject.FindProperty("Type");
        _consumableSubType = _consumableObject.FindProperty("TypeSubtype");

        AssignStartPositions();
    }

    /// <summary>
    /// Changes the way the ConsumableObject script looks in the Inspector tab.
    /// </summary>
    public override void OnInspectorGUI()
    {
        _consumableObject.Update();

        ShowConsumableAssigner();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _consumableObject.ApplyModifiedProperties();
        }
    }

    #endregion

    #region Assign Type

    // TYPES

    /// <summary>
    /// Assigns the Inspector to show the values currently saved on the Object.
    /// </summary>
    void AssignStartPositions()
    {
        if (_consumableType.enumValueIndex == (int)IConsumableBase.ConsumableType.HealthRestore) { _currentToolbarType = 0; }
        if (_consumableType.enumValueIndex == (int)IConsumableBase.ConsumableType.EnergyRestore) { _currentToolbarType = 1; }
        if (_consumableType.enumValueIndex == (int)IConsumableBase.ConsumableType.TemporaryBoost) { _currentToolbarType = 2; }

        if (_consumableSubType.enumValueIndex == (int)IConsumableBase.ConsumableSubtype.SmallHealth ||
            _consumableSubType.enumValueIndex == (int)IConsumableBase.ConsumableSubtype.SmallEnergy ||
            _consumableSubType.enumValueIndex == (int)IConsumableBase.ConsumableSubtype.HealthBoost)
        { _currentToolbarSubType = 0; }

        if (_consumableSubType.enumValueIndex == (int)IConsumableBase.ConsumableSubtype.MediumHealth ||
            _consumableSubType.enumValueIndex == (int)IConsumableBase.ConsumableSubtype.MediumEnergy ||
            _consumableSubType.enumValueIndex == (int)IConsumableBase.ConsumableSubtype.EnergyBoost)
        { _currentToolbarSubType = 1; }

        if (_consumableSubType.enumValueIndex == (int)IConsumableBase.ConsumableSubtype.LargeHealth ||
            _consumableSubType.enumValueIndex == (int)IConsumableBase.ConsumableSubtype.LargeEnergy ||
            _consumableSubType.enumValueIndex == (int)IConsumableBase.ConsumableSubtype.SpeedBoost)
        { _currentToolbarSubType = 2; }
    }

    /// <summary>
    /// Displays the choosable Consumable Types on the toolbar.
    /// </summary>
    public void ShowConsumableAssigner()
    {
        EditorGUILayout.LabelField("Consumable Types", EditorStyles.boldLabel);
        GUILayout.Space(5);
        GUILayout.Label("Type: ");

        // Displays a new toolbar with the HealthRestore, EnergyRestore, and TemporaryBoost options.
        _currentToolbarType = GUILayout.Toolbar(_currentToolbarType, _toolbarConsumableType);

        if (_currentToolbarType == 0) { TypeHealthRestoreSelected(); }
        if (_currentToolbarType == 1) { TypeEnergyRestoreSelected(); }
        if (_currentToolbarType == 2) { TypeTemporaryBoostSelected(); }
    }

    // SUBTYPES

    /// <summary>
    /// Selecting Health Restore will result in this new Subtype toolbar to appear beneath Type toolbar.
    /// It will hold only the Subtypes compatible with Health Restore.
    /// </summary>
    void TypeHealthRestoreSelected()
    {
        GUILayout.Label("Sub Type: ");
        // Display a toolbar with three options - (SmallHealth, MediumHealth, & LargeHealth).
        _currentToolbarSubType = GUILayout.Toolbar(_currentToolbarSubType, _toolbarConsumableSubTypeHealth);
        // Set the Consumable Type to HealthRestore on the original script.
        _consumableType.enumValueIndex = (int)IConsumableBase.ConsumableType.HealthRestore;

        if (_currentToolbarSubType == 0) { SubTypeSmallHealth(); }
        if (_currentToolbarSubType == 1) { SubTypeMediumHealth(); }
        if (_currentToolbarSubType == 2) { SubTypeLargeHealth(); }
    }

    /// <summary>
    /// Selecting Energy Restore will result in this new Subtype toolbar to appear beneath Type toolbar.
    /// It will hold only the Subtypes compatible with Energy Restore.
    /// </summary>
    void TypeEnergyRestoreSelected()
    {
        GUILayout.Label("Sub Type: ");
        // Display a toolbar with three options - (SmallEnergy, MediumEnergy, & LargeEnergy).
        _currentToolbarSubType = GUILayout.Toolbar(_currentToolbarSubType, _toolbarConsumableSubTypeEnergy);
        // Set the Consumable Type to EnergyRestore on the original script.
        _consumableType.enumValueIndex = (int)IConsumableBase.ConsumableType.EnergyRestore;

        if (_currentToolbarSubType == 0) { SubTypeSmallEnergy(); }
        if (_currentToolbarSubType == 1) { SubTypeMediumEnergy(); }
        if (_currentToolbarSubType == 2) { SubTypeLargeEnergy(); }
    }

    /// <summary>
    /// Selecting Temporary Boost will result in this new Subtype toolbar to appear beneath Type toolbar.
    /// It will hold only the Subtypes compatible with Temporary Boost.
    /// </summary>
    void TypeTemporaryBoostSelected()
    {
        GUILayout.Label("Sub Type: ");
        // Display a toolbar with three options - (HealthBoost, EnergyBoost, & SpeedBoost).
        _currentToolbarSubType = GUILayout.Toolbar(_currentToolbarSubType, _toolbarConsumableSubTypeBoost);
        // Set the Consumable Type to TemporaryBoost on the original script.
        _consumableType.enumValueIndex = (int)IConsumableBase.ConsumableType.TemporaryBoost;

        if (_currentToolbarSubType == 0) { SubTypeHealthBoost(); }
        if (_currentToolbarSubType == 1) { SubTypeEnergyBoost(); }
        if (_currentToolbarSubType == 2) { SubTypeSpeedBoost(); }
    }

    #endregion

    #region Assign Subtype

    void SubTypeSmallHealth()
    {
        _consumableSubType.enumValueIndex = (int)IConsumableBase.ConsumableSubtype.SmallHealth;
    }

    void SubTypeMediumHealth()
    {
        _consumableSubType.enumValueIndex = (int)IConsumableBase.ConsumableSubtype.MediumHealth;
    }

    void SubTypeLargeHealth()
    {
        _consumableSubType.enumValueIndex = (int)IConsumableBase.ConsumableSubtype.LargeHealth;
    }

    void SubTypeSmallEnergy()
    {
        _consumableSubType.enumValueIndex = (int)IConsumableBase.ConsumableSubtype.SmallEnergy;
    }

    void SubTypeMediumEnergy()
    {
        _consumableSubType.enumValueIndex = (int)IConsumableBase.ConsumableSubtype.MediumEnergy;
    }

    void SubTypeLargeEnergy()
    {
        _consumableSubType.enumValueIndex = (int)IConsumableBase.ConsumableSubtype.LargeEnergy;
    }

    void SubTypeHealthBoost()
    {
        _consumableSubType.enumValueIndex = (int)IConsumableBase.ConsumableSubtype.HealthBoost;
    }

    void SubTypeEnergyBoost()
    {
        _consumableSubType.enumValueIndex = (int)IConsumableBase.ConsumableSubtype.EnergyBoost;
    }

    void SubTypeSpeedBoost()
    {
        _consumableSubType.enumValueIndex = (int)IConsumableBase.ConsumableSubtype.SpeedBoost;
    }

    #endregion
}
#endif