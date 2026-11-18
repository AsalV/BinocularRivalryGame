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

namespace Wwise.SDK
{

    public enum AkOptionType : int
    {
        AkOptionType_Boolean = 0,
        AkOptionType_Integer = 1,
        AkOptionType_Real = 2,
        AkOptionType_Pointer = 3,
        AkOptionType_String = 4,
        AkOptionType_ChannelConfig = 5,
    }

    public enum AkOption_MemoryMgr : int
    {
        AkOption_MemoryMgr_START = 0,
        AkOption_MemoryMgr_NUM = 3,
        AkOption_MemoryMgr_Hooks = 0,
        AkOption_MemoryMgr_DebugLevel = 1,
        AkOption_MemoryMgr_StompDebugMemIDMask = 2,
    }

    public enum AkOption_TempAlloc : int
    {
        AkOption_TempAlloc_START = 256,
        AkOption_TempAlloc_NUM = 8,
        AkOption_TempAlloc_MinimumBlockCount = 256,
        AkOption_TempAlloc_MinimumBlockSize = 257,
        AkOption_TempAlloc_MaximumUnusedBlocks = 258,
        AkOption_TempAlloc_EnableDebugDetailedStats = 259,
        AkOption_TempAlloc_EnableDebugClearMemory = 260,
        AkOption_TempAlloc_EnableDebugSentinels = 261,
        AkOption_TempAlloc_EnableDebugFlushBlocks = 262,
        AkOption_TempAlloc_EnableDebugStandaloneAllocs = 263,
    }

    public enum AkOption_BookmarkAlloc : int
    {
        AkOption_BookmarkAlloc_START = 512,
        AkOption_BookmarkAlloc_NUM = 7,
        AkOption_BookmarkAlloc_MinimumBlockCount = 512,
        AkOption_BookmarkAlloc_MinimumBlockSize = 513,
        AkOption_BookmarkAlloc_MaximumUnusedBlocks = 514,
        AkOption_BookmarkAlloc_DebugDetailedStats = 515,
        AkOption_BookmarkAlloc_DebugClearMemory = 516,
        AkOption_BookmarkAlloc_DebugEnableSentinels = 517,
        AkOption_BookmarkAlloc_DebugStandaloneAllocs = 518,
    }

    public enum AkOption_MemoryArena_Primary : int
    {
        AkOption_MemoryArena_Primary_START = 768,
        AkOption_MemoryArena_Primary_NUM = 14,
        AkOption_MemoryArena_Primary_Hooks = 768,
        AkOption_MemoryArena_Primary_EnableSba = 769,
        AkOption_MemoryArena_Primary_SbaInitSize = 770,
        AkOption_MemoryArena_Primary_SbaSpanSize = 771,
        AkOption_MemoryArena_Primary_SbaMaximumUnusedSpans = 772,
        AkOption_MemoryArena_Primary_TlsfInitSize = 773,
        AkOption_MemoryArena_Primary_TlsfSpanSize = 774,
        AkOption_MemoryArena_Primary_TlsfLargeSpanSize = 775,
        AkOption_MemoryArena_Primary_TlsfSpanOverhead = 776,
        AkOption_MemoryArena_Primary_TlsfMaximumUnusedMediumSpans = 777,
        AkOption_MemoryArena_Primary_TlsfMaximumUnusedLargeSpans = 778,
        AkOption_MemoryArena_Primary_AllocSizeLarge = 779,
        AkOption_MemoryArena_Primary_AllocSizeHuge = 780,
        AkOption_MemoryArena_Primary_MemReservedLimit = 781,
    }

    public enum AkOption_MemoryArena_Media : int
    {
        AkOption_MemoryArena_Media_START = 1024,
        AkOption_MemoryArena_Media_NUM = 14,
        AkOption_MemoryArena_Media_Hooks = 1024,
        AkOption_MemoryArena_Media_EnableSba = 1025,
        AkOption_MemoryArena_Media_SbaInitSize = 1026,
        AkOption_MemoryArena_Media_SbaSpanSize = 1027,
        AkOption_MemoryArena_Media_SbaMaximumUnusedSpans = 1028,
        AkOption_MemoryArena_Media_TlsfInitSize = 1029,
        AkOption_MemoryArena_Media_TlsfSpanSize = 1030,
        AkOption_MemoryArena_Media_TlsfLargeSpanSize = 1031,
        AkOption_MemoryArena_Media_TlsfSpanOverhead = 1032,
        AkOption_MemoryArena_Media_TlsfMaximumUnusedMediumSpans = 1033,
        AkOption_MemoryArena_Media_TlsfMaximumUnusedLargeSpans = 1034,
        AkOption_MemoryArena_Media_AllocSizeLarge = 1035,
        AkOption_MemoryArena_Media_AllocSizeHuge = 1036,
        AkOption_MemoryArena_Media_MemReservedLimit = 1037,
    }

    public enum AkOption_MemoryArena_Profiler : int
    {
        AkOption_MemoryArena_Profiler_START = 1280,
        AkOption_MemoryArena_Profiler_NUM = 14,
        AkOption_MemoryArena_Profiler_Hooks = 1280,
        AkOption_MemoryArena_Profiler_EnableSba = 1281,
        AkOption_MemoryArena_Profiler_SbaInitSize = 1282,
        AkOption_MemoryArena_Profiler_SbaSpanSize = 1283,
        AkOption_MemoryArena_Profiler_SbaMaximumUnusedSpans = 1284,
        AkOption_MemoryArena_Profiler_TlsfInitSize = 1285,
        AkOption_MemoryArena_Profiler_TlsfSpanSize = 1286,
        AkOption_MemoryArena_Profiler_TlsfLargeSpanSize = 1287,
        AkOption_MemoryArena_Profiler_TlsfSpanOverhead = 1288,
        AkOption_MemoryArena_Profiler_TlsfMaximumUnusedMediumSpans = 1289,
        AkOption_MemoryArena_Profiler_TlsfMaximumUnusedLargeSpans = 1290,
        AkOption_MemoryArena_Profiler_AllocSizeLarge = 1291,
        AkOption_MemoryArena_Profiler_AllocSizeHuge = 1292,
        AkOption_MemoryArena_Profiler_MemReservedLimit = 1293,
    }

    public enum AkOption_MemoryArena_Device : int
    {
        AkOption_MemoryArena_Device_START = 1536,
        AkOption_MemoryArena_Device_NUM = 14,
        AkOption_MemoryArena_Device_Hooks = 1536,
        AkOption_MemoryArena_Device_EnableSba = 1537,
        AkOption_MemoryArena_Device_SbaInitSize = 1538,
        AkOption_MemoryArena_Device_SbaSpanSize = 1539,
        AkOption_MemoryArena_Device_SbaMaximumUnusedSpans = 1540,
        AkOption_MemoryArena_Device_TlsfInitSize = 1541,
        AkOption_MemoryArena_Device_TlsfSpanSize = 1542,
        AkOption_MemoryArena_Device_TlsfLargeSpanSize = 1543,
        AkOption_MemoryArena_Device_TlsfSpanOverhead = 1544,
        AkOption_MemoryArena_Device_TlsfMaximumUnusedMediumSpans = 1545,
        AkOption_MemoryArena_Device_TlsfMaximumUnusedLargeSpans = 1546,
        AkOption_MemoryArena_Device_AllocSizeLarge = 1547,
        AkOption_MemoryArena_Device_AllocSizeHuge = 1548,
        AkOption_MemoryArena_Device_MemReservedLimit = 1549,
    }

    public enum AkOption_StreamMgr : int
    {
        AkOption_StreamMgr_START = 1792,
        AkOption_StreamMgr_NUM = 14,
        AkOption_StreamMgr_LowLevelIOHook = 1792,
        AkOption_StreamMgr_IOMemory = 1793,
        AkOption_StreamMgr_IOMemorySize = 1794,
        AkOption_StreamMgr_IOMemoryAlignment = 1795,
        AkOption_StreamMgr_MemAllocType = 1796,
        AkOption_StreamMgr_Granularity = 1797,
        AkOption_StreamMgr_AutoStreamTargetBufferLength = 1798,
        AkOption_StreamMgr_MaxLatencyMs = 1799,
        AkOption_StreamMgr_LookAheadRatio = 1800,
        AkOption_StreamMgr_MaxConcurrentIO = 1801,
        AkOption_StreamMgr_UseStreamCache = 1802,
        AkOption_StreamMgr_MaxPinnedCacheSize = 1803,
        AkOption_StreamMgr_EnablePackageFallback = 1804,
        AkOption_StreamMgr_UseSubfoldering = 1805,
    }

    public enum AkOption_SoundEngine : int
    {
        AkOption_SoundEngine_START = 2048,
        AkOption_SoundEngine_NUM = 6,
        AkOption_SoundEngine_Hooks = 2048,
        AkOption_SoundEngine_PluginPath = 2049,
        AkOption_SoundEngine_MaxNumPaths = 2050,
        AkOption_SoundEngine_ContinuousPlaybackLookAheadMS = 2051,
        AkOption_SoundEngine_FloorPlane = 2052,
        AkOption_SoundEngine_GameUnitsPerMeters = 2053,
    }

    public enum AkOption_AudioMgr : int
    {
        AkOption_AudioMgr_START = 2304,
        AkOption_AudioMgr_NUM = 10,
        AkOption_AudioMgr_EnableThread = 2304,
        AkOption_AudioMgr_NumSamplesPerFrame = 2305,
        AkOption_AudioMgr_SampleRate = 2306,
        AkOption_AudioMgr_MultipleBuffering = 2307,
        AkOption_AudioMgr_CommandQueueSize = 2308,
        AkOption_AudioMgr_DebugOutOfRangeLimit = 2309,
        AkOption_AudioMgr_EnableDebugOutOfRangeCheck = 2310,
        AkOption_AudioMgr_EnableOfflineRendering = 2311,
        AkOption_AudioMgr_MaxHardwareTimeoutMs = 2312,
        AkOption_AudioMgr_EnableHwCodecLowLatencyMode = 2313,
    }

    public enum AkOption_BankMgr : int
    {
        AkOption_BankMgr_START = 2560,
        AkOption_BankMgr_NUM = 3,
        AkOption_BankMgr_EnableThread = 2560,
        AkOption_BankMgr_EnableManualGameSyncPreparation = 2561,
        AkOption_BankMgr_BankReadBufferSize = 2562,
    }

    public enum AkOption_Output : int
    {
        AkOption_Output_START = 2816,
        AkOption_Output_NUM = 7,
        AkOption_Output_AudioDeviceShareset = 2816,
        AkOption_Output_DeviceID = 2817,
        AkOption_Output_PanningRule = 2818,
        AkOption_Output_EnableAutoPanningRule = 2819,
        AkOption_Output_ChannelConfig = 2820,
        AkOption_Output_MaxSystemAudioObjects = 2821,
        AkOption_Output_EnableVerboseSystemLog = 2822,
    }

    public enum AkOption_JobMgr : int
    {
        AkOption_JobMgr_START = 3072,
        AkOption_JobMgr_NUM = 10,
        AkOption_JobMgr_MaxActiveWorkers_Generic = 3072,
        AkOption_JobMgr_MaxActiveWorkers_AudioProcessing = 3073,
        AkOption_JobMgr_MaxActiveWorkers_Acoustics = 3074,
        AkOption_JobMgr_NumMemorySlabs = 3075,
        AkOption_JobMgr_MemorySlabSize = 3076,
        AkOption_JobMgr_EnableWorkerThreads = 3077,
        AkOption_JobMgr_NumWorkerThreads = 3078,
        AkOption_JobMgr_WorkerThreadExecutionTime = 3079,
        AkOption_JobMgr_WorkerThreadProperties = 3080,
        AkOption_JobMgr_HangDetectionTimeoutMs = 3081,
    }

    public enum AkOption_Acoustics : int
    {
        AkOption_Acoustics_START = 3328,
        AkOption_Acoustics_NUM = 26,
        AkOption_Acoustics_Enable = 3328,
        AkOption_Acoustics_MaxSoundPropagationDepth = 3329,
        AkOption_Acoustics_MovementThreshold = 3330,
        AkOption_Acoustics_NumberOfPrimaryRays = 3331,
        AkOption_Acoustics_MaxReflectionOrder = 3332,
        AkOption_Acoustics_MaxDiffractionOrder = 3333,
        AkOption_Acoustics_MaxDiffractionPaths = 3334,
        AkOption_Acoustics_MaxGlobalReflectionPaths = 3335,
        AkOption_Acoustics_MaxEmitterRoomAuxSends = 3336,
        AkOption_Acoustics_DiffractionOnReflectionsOrder = 3337,
        AkOption_Acoustics_MaxDiffractionAngleDegrees = 3338,
        AkOption_Acoustics_MaxPathLength = 3339,
        AkOption_Acoustics_CPULimitPercentage = 3340,
        AkOption_Acoustics_LoadBalancingSpread = 3341,
        AkOption_Acoustics_SmoothingConstantMs = 3342,
        AkOption_Acoustics_AdjacentRoomBleed = 3343,
        AkOption_Acoustics_EnableGeometricDiffractionAndTransmission = 3344,
        AkOption_Acoustics_CalcEmitterVirtualPosition = 3345,
        AkOption_Acoustics_TransmissionOperation = 3346,
        AkOption_Acoustics_ClusteringMinPoints = 3347,
        AkOption_Acoustics_ClusteringMaxDistance = 3348,
        AkOption_Acoustics_ClusteringDeadZoneDistance = 3349,
        AkOption_Acoustics_ReflectionPathSearchEffort = 3350,
        AkOption_Acoustics_DiffractionFieldNodeBudget = 3351,
        AkOption_Acoustics_MaxRoomPropagationDistance = 3352,
        AkOption_Acoustics_SmoothingMode = 3353,
    }

    public enum AkOption_Profiler : int
    {
        AkOption_Profiler_START = 3584,
        AkOption_Profiler_NUM = 3,
        AkOption_Profiler_Hooks = 3584,
        AkOption_Profiler_MonitorQueuePoolSize = 3585,
        AkOption_Profiler_CPUMonitorQueueMaxSize = 3586,
    }

    public enum AkOption_Comm : int
    {
        AkOption_Comm_START = 3840,
        AkOption_Comm_NUM = 7,
        AkOption_Comm_Enable = 3840,
        AkOption_Comm_DiscoveryPort = 3841,
        AkOption_Comm_CommandPort = 3842,
        AkOption_Comm_API = 3843,
        AkOption_Comm_EnableSystemCommsManagement = 3844,
        AkOption_Comm_AppNetworkName = 3845,
        AkOption_Comm_ProxyServerUrl = 3846,
    }

    public enum AkOption_Threads : int
    {
        AkOption_Threads_START = 4096,
        AkOption_Threads_NUM = 18,
        AkOption_Threads_PrimaryStreamDevice_Priority = 4096,
        AkOption_Threads_PrimaryStreamDevice_StackSize = 4097,
        AkOption_Threads_PrimaryStreamDevice_AffinityMask = 4098,
        AkOption_Threads_AudioMgr_Priority = 4099,
        AkOption_Threads_AudioMgr_StackSize = 4100,
        AkOption_Threads_AudioMgr_AffinityMask = 4101,
        AkOption_Threads_Suspend_Priority = 4102,
        AkOption_Threads_Suspend_StackSize = 4103,
        AkOption_Threads_Suspend_AffinityMask = 4104,
        AkOption_Threads_BankMgr_Priority = 4105,
        AkOption_Threads_BankMgr_StackSize = 4106,
        AkOption_Threads_BankMgr_AffinityMask = 4107,
        AkOption_Threads_Profiler_Priority = 4108,
        AkOption_Threads_Profiler_StackSize = 4109,
        AkOption_Threads_Profiler_AffinityMask = 4110,
        AkOption_Threads_JobWorker_Priority = 4111,
        AkOption_Threads_JobWorker_StackSize = 4112,
        AkOption_Threads_JobWorker_AffinityMask = 4113,
    }
}