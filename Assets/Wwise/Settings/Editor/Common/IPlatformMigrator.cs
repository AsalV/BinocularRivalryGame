/*******************************************************************************
The content of this file includes portions of the proprietary AUDIOKINETIC Wwise
Technology released in source code form as part of the game integration package.
The content of this file may not be used without valid licenses to the
AUDIOKINETIC Wwise Technology.
Note that the use of the game engine is subject to the Unity(R) Terms of
Service at https://unity3d.com/legal/terms-of-service

License Usage

Licensees holding valid licenses to the AUDIOKINETIC Wwise Technology may use
this file in accordance with the end user license agreement provided with the
software or, alternatively, in accordance with the terms contained
in a written agreement between you and Audiokinetic Inc.
Copyright (c) 2026 Audiokinetic Inc.
*******************************************************************************/

using System.Collections.Generic;
using AK.Wwise.Unity.Settings;
using Wwise.SDK;

namespace AK.Wwise.Unity.Settings.Editor
{
    /// <summary>
    /// Per-platform advanced-settings migrator. Implementations are discovered by
    /// <see cref="AkOptionMigration"/> via <see cref="UnityEditor.TypeCache"/>.
    /// </summary>
    public interface IPlatformMigrator
    {
        /// <summary>
        /// Name registered for the platform's settings ScriptableObject
        /// (e.g. "Web" for <c>AkWebGLSettings</c>).
        /// </summary>
        string SettingsName { get; }

        /// <summary>
        /// AkOption platform id (e.g. "Web"). See <see cref="AkOptionPathGetter.RegisteredPlatforms"/>.
        /// </summary>
        string PlatformId { get; }


        /// <summary>
        /// Produce the platform-specific overrides for migration. Emit only entries
        /// that differ from <paramref name="sdkDefaults"/>. Mutate <paramref name="userOverrides"/> if necessary.
        /// </summary>
        List<AkOptionNamespace> Migrate(AkCommonUserSettings userSettings,
            List<AkOptionNamespace> userOverrides,
            List<AkOptionNamespace> sdkDefaults)
        {
            return new List<AkOptionNamespace>();
        }

        /// <summary>
        /// Produce the platform-specific overrides for migration. Emit only entries
        /// that differ from <paramref name="sdkDefaults"/>. Mutate <paramref name="advancedOverrides"/> if necessary.
        /// </summary>
        List<AkOptionNamespace> Migrate(AkCommonAdvancedSettings advancedSettings,
            List<AkOptionNamespace> advancedOverrides,
            List<AkOptionNamespace> sdkDefaults)
        {
            return new List<AkOptionNamespace>();
        }

        /// <summary>
        /// Produce the platform-specific overrides for migration. Emit only entries
        /// that differ from <paramref name="sdkDefaults"/>. Mutate <paramref name="commOverrides"/> if necessary.
        /// </summary>
        List<AkOptionNamespace> Migrate(AkCommonCommSettings commSettings,
            List<AkOptionNamespace> commOverrides,
            List<AkOptionNamespace> sdkDefaults)
        {
            return new List<AkOptionNamespace>();
        }
    }
}
