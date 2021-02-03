using System;
using System.IO;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class SettingsManager
{

    private static UnityEvent _onSettingsLoaded;
    public static UnityEvent OnSettingsLoaded
    {
        get
        {
            if (_onSettingsLoaded == null) { _onSettingsLoaded = new UnityEvent(); }
            return _onSettingsLoaded;
        }
    }
    static List<Setting> settingsList = new List<Setting>();
    static string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "diverse-city");
    public static void LoadSettings()
    {
        string[] readLines = File.ReadAllLines(Path.Combine(configDir, "diverse-city.conf"));
        foreach (string line in readLines)
        {
            string[] splitLine = line.Split('='); // separate id from values
            if (line.Contains("# COMMENT: ")) { continue; }; // skipping comments
            Setting lineSetting = GetSettingFromID(splitLine[0]);
            object newValue = null;
            int intVal;
            float floatVal;
            bool boolVal;
            if (int.TryParse(splitLine[1], NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out intVal)) { newValue = intVal; }
            else if (float.TryParse(splitLine[1], NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out floatVal)) { newValue = floatVal; }
            else if (bool.TryParse(splitLine[1], out boolVal)) { newValue = boolVal; }
            else { newValue = splitLine[1]; };
            if (lineSetting.Value.GetType() == newValue.GetType()) { lineSetting.Value = newValue; }
            else { throw new MismatchedSettingValueTypeException($"Wanted: {lineSetting.Value.GetType().ToString()}, got: {newValue.GetType().ToString()}."); };
        }
        OnSettingsLoaded.Invoke();
    }

    public static object GetSettingValue(string settingID)
    {
        return GetSettingFromID(settingID).Value;
    }

    public static void CreateSetting(string id, object defaultValue)
    {
        Setting newSetting = new Setting(id, defaultValue);
        settingsList.Add(newSetting);
    }

    static Setting GetSettingFromID(string ID)
    {
        foreach (Setting setting in settingsList)
        {
            if (setting.ID == ID)
            {
                return setting;
            }
        }
        throw new SettingNotFoundException($"ID: {ID}");
    }
}

[Serializable]
public class Setting
{
    public Setting(string id, object defaultValue)
    {
        ID = id;
        DefaultValue = Value = defaultValue;
    }
    public string ID { get; private set; }
    public object DefaultValue { get; private set; }
    // public Type TypeOfValue; // currently commented out just in case. I am first going for the option of just checking if the value can be parsed first as a number, then bool then will be left as string... ? should work??
    public object Value;
    public string Name { get; private set; }
}

#region Exceptions
[Serializable]
public class SettingNotFoundException : Exception
{
    public SettingNotFoundException() : base() { }
    public SettingNotFoundException(string message) : base(message) { }
    public SettingNotFoundException(string message, Exception inner) : base(message, inner) { }
    protected SettingNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

[Serializable]
public class DuplicateSettingException : Exception
{
    public DuplicateSettingException() : base() { }
    public DuplicateSettingException(string message) : base(message) { }
    public DuplicateSettingException(string message, Exception inner) : base(message, inner) { }
    protected DuplicateSettingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

[Serializable]
public class MismatchedSettingValueTypeException : Exception
{
    public MismatchedSettingValueTypeException() : base() { }
    public MismatchedSettingValueTypeException(string message) : base(message) { }
    public MismatchedSettingValueTypeException(string message, Exception inner) : base(message, inner) { }
    protected MismatchedSettingValueTypeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
#endregion