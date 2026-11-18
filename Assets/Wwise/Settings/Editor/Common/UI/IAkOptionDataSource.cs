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
using Wwise.SDK;

namespace AK.Wwise.Unity.Settings.Editor.Common.UI
{
    /// <summary>
    /// The source of AkOption data the settings UI reads and writes, keyed by platform.
    /// </summary>
    public interface IAkOptionDataSource
    {
        /// <summary>The effective defaults baseline (SDK defaults + integration overrides) for a platform.</summary>
        List<AkOptionNamespace> LoadEffectiveDefaults(string platformId);

        /// <summary>Project-level overrides for a platform.</summary>
        List<AkOptionNamespace> LoadProjectOverrides(string platformId);

        /// <summary>Persist the project-level overrides for a platform.</summary>
        void WriteProjectOverrides(string platformId, List<AkOptionNamespace> overrides);
    }

    /// <summary>Sound Engine initialization options, backed by <see cref="AkOptionStore"/>.</summary>
    public sealed class AkOptionStoreSource : IAkOptionDataSource
    {
        public List<AkOptionNamespace> LoadEffectiveDefaults(string platformId)
            => AkOptionStore.LoadEffectiveDefaults(platformId);

        public List<AkOptionNamespace> LoadProjectOverrides(string platformId)
            => AkOptionStore.LoadProjectOverrides(platformId);

        public void WriteProjectOverrides(string platformId, List<AkOptionNamespace> overrides)
            => AkOptionStore.WriteProjectOverrides(platformId, overrides);
    }

    /// <summary>Wwise runtime settings, backed by <see cref="WwiseRuntimeSettingsStore"/>.</summary>
    public sealed class WwiseRuntimeSettingsSource : IAkOptionDataSource
    {
        // LoadDefaults already returns the effective defaults (common + platform) for runtime settings.
        public List<AkOptionNamespace> LoadEffectiveDefaults(string platformId)
            => WwiseRuntimeSettingsStore.LoadDefaults(platformId);

        public List<AkOptionNamespace> LoadProjectOverrides(string platformId)
            => WwiseRuntimeSettingsStore.LoadProjectOverrides(platformId);

        public void WriteProjectOverrides(string platformId, List<AkOptionNamespace> overrides)
        {
            WwiseRuntimeSettingsStore.WriteProjectOverrides(platformId, overrides);
            // Apply the new log level in edit mode immediately
            WwiseLoggerEditorInitializer.ApplyLogLevel();
        }
    }
}
