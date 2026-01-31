using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Tools
{
    /// <summary>
    /// A place for all types of helper functions. Must include Using Tools.
    /// </summary>
    public static class HelperFunctions
    {
        /// <summary>
        /// Quickly generates a unique ID for a progression object.
        /// </summary>
        /// <param name="type">Type of object this is.</param>
        /// <param name="key">Unique gUID of object.</param>
        /// <returns>Unique uint for easy designation of scene progression objects.</returns>
        public static uint GenerateUniqueID(ProgressionObject type, uint key)
        {
            uint id = (uint)type;
            id = id << 24;
            id |= key & 0x00FFFFFF;
            return id;
        }

        public static uint GenerateUniqueID(ProgressionObject type, string key)
        {
            uint id = (uint)type;
            id = id << 24;
            id |= (uint)key.GetHashCode() & 0x00FFFFFF;
            return id;
        }

        public static IEnumerator DisableAfterTime(GameObject go, float t)
        {
            yield return new WaitForSeconds(t);
            go.SetActive(false);
        }

        public static bool IsInLayerMask(GameObject obj, LayerMask mask) => (mask.value & (1 << obj.layer)) != 0;
        public static bool IsInLayerMask(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;
    }

    /// <summary>
    /// Helpers for changing and setting sprite icons for text.
    /// </summary>
    public static class SpriteHelper
    {
        /// <summary>
        /// Sets the default sprite asset in TMP Settings.
        /// </summary>
        /// <param name="spriteAsset">Sprite asset to become new default.</param>
        public static void ChangeDefaultSpriteAsset(ref TMP_SpriteAsset spriteAsset)
        {
            Type tmpSettings = typeof(TMP_Settings);

            FieldInfo defaultSpriteAsset = tmpSettings.GetField("m_defaultSpriteAsset", BindingFlags.NonPublic | BindingFlags.Instance);
            if (defaultSpriteAsset != null)
            {
                TMP_Settings tmpSettingsInstance = TMP_Settings.instance;

                // Sets new sprite asset.
                defaultSpriteAsset.SetValue(tmpSettingsInstance, spriteAsset);
            }
        }

        /// <summary>
        /// Formats a string which has a code matching the Key of the given dictionary.
        /// </summary>
        /// <param name="format">string to check and format.</param>
        /// <param name="values">Dictionary with a key to search for and value to replace.</param>
        /// <returns>String with the value of the Key in place.</returns>
        public static string StringFormat(string format, IDictionary<string, string> values)
        {
            var matches = Regex.Matches(format, @"\{(.+?)\}");
            List<string> words = (from Match match in matches select match.Groups[1].Value).ToList();

            return words.Aggregate(
                format,
                (current, key) =>
                {
                    int colonIndex = key.IndexOf(':');
                    return current.Replace(
                    "{" + key + "}",
                    colonIndex > 0
                        ? string.Format("{0:" + key.Substring(colonIndex + 1) + "}", values[key.Substring(0, colonIndex)])
                        : values[key] == null ? string.Empty : values[key].ToString());
                });
        }
    }

    /// <summary>
    /// Helper class for quick localization methods.
    /// </summary>
    public static class LocalizationHelper
    {
        private static string localizedText = string.Empty;
        /// <summary>
        /// Gets localized string from a table and locale to change text via code.
        /// </summary>
        /// <param name="table">Table to search.</param>
        /// <param name="locale">Locale from table to use.</param>
        /// <returns>Localized string.</returns>
        public static string GetLocalizedString(string table, string locale)
        {
            localizedText = string.Empty;

            if (Application.isPlaying)
            {
                var operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(table, locale);
                operation.WaitForCompletion();
                if (operation.IsDone)
                {
                    localizedText = operation.Result;
                }
                else
                {
                    operation.Completed += (operation) => localizedText = operation.Result;
                }
            }

            return localizedText;
        }
    }

    /// <summary>
    /// Helper functions for Volume conversions.
    /// </summary>
    public static class VolumeHelper
    {
        /// <summary>
        /// Converts from the 0 to -80 range needed for Mixer into a 1 to 0 range for percentage text.
        /// </summary>
        public static float VolumeDecibelsToPerc(float vol)
        {
            float percentage = (1.0f - Mathf.Abs(vol / 80.0f)) * 100.0f;
            return Mathf.Round(percentage);
        }

        /// <summary>
        /// Converts from 100 to 0 range into a 0 to -80 range for Mixer.
        /// </summary>
        public static float VolumePercToDecibels(float vol)
        {
            float decibels = ((vol / 100.0f) - 1.0f) * 80.0f;
            return decibels;
        }

        /// <summary>
        /// Converts a value to the log10 of itself for volume.
        /// </summary>
        /// <param name="val">Volume value.</param>
        /// <returns>Logrithmic volume.</returns>
        public static float LogarithmicVolume(float val)
        {
            return Mathf.Log10(val) * 20.0f;
        }

        /// <summary>
        /// Turns a logarithmic volume value into a 0 to 100 percentage range.
        /// </summary>
        /// <param name="vol">Log volume.</param>
        /// <returns>Percentage.</returns>
        public static int LogarithmicVolToPercent(float vol)
        {
            float x = (Mathf.Pow(10, LogarithmicVolume(vol) / 10.0f)) * 100.0f;
            return Mathf.FloorToInt(x);
        }
    }
}