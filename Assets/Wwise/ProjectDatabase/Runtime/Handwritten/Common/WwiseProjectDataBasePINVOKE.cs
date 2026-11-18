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
#if UNITY_EDITOR
using System.Runtime.InteropServices;
public class WwiseProjectDataBasePINVOKE
{
    /*
     * Utility
    */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_Init", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Init(string InDirectoryPath, string InDirectoryPlatformName);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_SetCurrentPlatform", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetCurrentPlatform(string InDirectoryPlatformName);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_SetCurrentLanguage", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetCurrentLanguage(string InLanguageName);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetGuidInt", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GetGuidInt(global::System.IntPtr InGuid, ref uint A, ref uint B, ref uint C, ref uint D);

    /*
     * SoundBank Ref
    */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAllSoundBanksRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAllSoundBanksRef();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSoundBankCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetSoundBankCount();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSoundBankRefIndex", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetSoundBankRefIndex(global::System.IntPtr soundBankRefPtr, int index);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteSoundBanksArrayRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteSoundBanksArrayRef(global::System.IntPtr soundBankRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSoundBankRefString", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetSoundBankRefString(string soundBankName, string soundBankType);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSoundBankName", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetSoundBankName(global::System.IntPtr soundBankRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSoundBankPath", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetSoundBankPath(global::System.IntPtr soundBankRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSoundBankObjectPath", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetSoundBankObjectPath(global::System.IntPtr soundBankRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSoundBankLanguage", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetSoundBankLanguage(global::System.IntPtr soundBankRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSoundBankLanguageId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetSoundBankLanguageId(global::System.IntPtr soundBankRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSoundBankGuid", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetSoundBankGuid(global::System.IntPtr soundBankRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSoundBankShortId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetSoundBankShortId(global::System.IntPtr soundBankRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_IsUserBank", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool IsUserBank(global::System.IntPtr soundBankRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_IsInitBank", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool IsInitBank(global::System.IntPtr soundBankRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_IsSoundBankValid", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool IsSoundBankValid(global::System.IntPtr soundBankRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSoundBankMedia", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetSoundBankMedia(global::System.IntPtr soundBankRefPtr, int index);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSoundBankMediasCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetSoundBankMediasCount(global::System.IntPtr soundBankRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSoundBankEvent", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetSoundBankEvent(global::System.IntPtr soundBankRefPtr, int index);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSoundBankEventsCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetSoundBankEventsCount(global::System.IntPtr soundBankRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteSoundBankRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteSoundBankRef(global::System.IntPtr soundBankRefPtr);

    /*
     * Media Ref
    */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetMediaName", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetMediaName(global::System.IntPtr mediaRef);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetMediaPath", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetMediaPath(global::System.IntPtr mediaRef);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetMediaShortId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetMediaShortId(global::System.IntPtr mediaRef);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetMediaLanguage", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetMediaLanguage(global::System.IntPtr mediaRef);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetMediaIsStreaming", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool GetMediaIsStreaming(global::System.IntPtr mediaRef);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetMediaLocation", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetMediaLocation(global::System.IntPtr mediaRef);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetMediaCachePath", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetMediaCachePath(global::System.IntPtr mediaRef);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteMediaRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteMediaRef(global::System.IntPtr mediaRefPtr);

    /*
     * Event Ref
    */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAllEventsRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAllEventsRef();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetEventCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetEventCount();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetEventRefString", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetEventRefString(string InString);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetEventName", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetEventName(global::System.IntPtr eventRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetEventPath", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetEventPath(global::System.IntPtr eventRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetEventGuid", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetEventGuid(global::System.IntPtr eventRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetEventShortId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetEventShortId(global::System.IntPtr eventRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetEventMaxAttenuation", CallingConvention = CallingConvention.Cdecl)]
    public static extern float GetEventMaxAttenuation(global::System.IntPtr eventRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetEventMinDuration", CallingConvention = CallingConvention.Cdecl)]
    public static extern float GetEventMinDuration(global::System.IntPtr eventRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetEventMaxDuration", CallingConvention = CallingConvention.Cdecl)]
    public static extern float GetEventMaxDuration(global::System.IntPtr eventRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetEventMedia", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetEventMedia(global::System.IntPtr soundBankRefPtr, int index);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetEventMediasCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetEventMediasCount(global::System.IntPtr eventRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteEventRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteEventRef(global::System.IntPtr mediaRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteEventArrayRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteEventArrayRef(global::System.IntPtr mediaRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetEvent", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetEvent(global::System.IntPtr eventArrayRefPtr, int index);

    /*
     * Platform Ref
    */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetPlatformRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetPlatformRef(string InString);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetPlatformName", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetPlatformName(global::System.IntPtr platformRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetPlatformGuid", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetPlatformGuid(global::System.IntPtr platformRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeletePlatformRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeletePlatformRef(global::System.IntPtr platformRefPtr);

    /*
    * Plugin Ref
    */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAllPluginRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAllPluginRef();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetPluginCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetPluginCount();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetPluginRefIndex", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetPluginRefIndex(global::System.IntPtr soundBankRefPtr, int index);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetPluginId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetPluginId(global::System.IntPtr pluginRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetPluginName", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetPluginName(global::System.IntPtr pluginRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetPluginDLL", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetPluginDLL(global::System.IntPtr pluginRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetPluginStaticLib", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetPluginStaticLib(global::System.IntPtr pluginRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeletePluginRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeletePluginRef(global::System.IntPtr pluginRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeletePluginArrayRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeletePluginArrayRef(global::System.IntPtr pluginArrayRefPtr);


    /*
     * Switch Group Ref
     */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAllSwitchGroupRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAllSwitchGroupRef();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitchGroupCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetSwitchGroupCount();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteSwitchGroupRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteSwitchGroupRef(global::System.IntPtr switchGroupRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteSwitchGroupArrayRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteSwitchGroupArrayRef(global::System.IntPtr switchGroupRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitchGroup", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetSwitchGroup(global::System.IntPtr switchArrayRefPtr, int index);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitchGroupName", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetSwitchGroupName(global::System.IntPtr switchGroupRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitchGroupPath", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetSwitchGroupPath(global::System.IntPtr switchGroupRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitchGroupGuid", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetSwitchGroupGuid(global::System.IntPtr switchGroupRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitchGroupShortId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetSwitchGroupShortId(global::System.IntPtr switchGroupRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitchGroupRefString", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetSwitchGroupRefString(string InString);

    /*
     * Switch Ref
     */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAllSwitchRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAllSwitchRef();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitchCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetSwitchCount();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteSwitchRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteSwitchRef(global::System.IntPtr switchRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteSwitchArrayRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteSwitchArrayRef(global::System.IntPtr switchRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitch", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetSwitch(global::System.IntPtr switchArrayRefPtr, int index);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitchName", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetSwitchName(global::System.IntPtr switchRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitchPath", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetSwitchPath(global::System.IntPtr switchRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitchGuid", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetSwitchGuid(global::System.IntPtr switchRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitchShortId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetSwitchShortId(global::System.IntPtr switchRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitchGroupId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetSwitchGroupId(global::System.IntPtr switchRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetSwitchRefString", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetSwitchRefString(string InString);


    /*
     * State Group Ref
     */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAllStateGroupRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAllStateGroupRef();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetStateGroupCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetStateGroupCount();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteStateGroupRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteStateGroupRef(global::System.IntPtr StateGroupRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteStateGroupArrayRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteStateGroupArrayRef(global::System.IntPtr StateGroupRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetStateGroup", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetStateGroup(global::System.IntPtr StateArrayRefPtr, int index);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetStateGroupName", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetStateGroupName(global::System.IntPtr StateGroupRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetStateGroupPath", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetStateGroupPath(global::System.IntPtr StateGroupRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetStateGroupGuid", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetStateGroupGuid(global::System.IntPtr StateGroupRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetStateGroupShortId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetStateGroupShortId(global::System.IntPtr StateGroupRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetStateGroupRefString", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetStateGroupRefString(string InString);

    /*
     * State Ref
     */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAllStateRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAllStateRef();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetStateCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetStateCount();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteStateRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteStateRef(global::System.IntPtr StateRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteStateArrayRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteStateArrayRef(global::System.IntPtr StateRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetState", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetState(global::System.IntPtr StateArrayRefPtr, int index);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetStateName", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetStateName(global::System.IntPtr StateRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetStatePath", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetStatePath(global::System.IntPtr StateRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetStateGuid", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetStateGuid(global::System.IntPtr StateRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetStateShortId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetStateShortId(global::System.IntPtr StateRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetStateGroupId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetStateGroupId(global::System.IntPtr StateRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetStateRefString", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetStateRefString(string InString);
    /*
     * AcousticTexture Ref
     */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAllAcousticTextureRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAllAcousticTextureRef();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAcousticTextureCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetAcousticTextureCount();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteAcousticTextureRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteAcousticTextureRef(global::System.IntPtr AcousticTextureRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteAcousticTextureArrayRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteAcousticTextureArrayRef(global::System.IntPtr AcousticTextureRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAcousticTexture", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAcousticTexture(global::System.IntPtr AcousticTextureArrayRefPtr, int index);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAcousticTextureName", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetAcousticTextureName(global::System.IntPtr AcousticTextureRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAcousticTexturePath", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetAcousticTexturePath(global::System.IntPtr AcousticTextureRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAcousticTextureGuid", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetAcousticTextureGuid(global::System.IntPtr AcousticTextureRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAcousticTextureShortId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetAcousticTextureShortId(global::System.IntPtr AcousticTextureRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAcousticTextureRefString", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAcousticTextureRefString(string InString);

    /*
     * GameParameter Ref
     */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAllGameParameterRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAllGameParameterRef();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetGameParameterCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetGameParameterCount();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteGameParameterRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteGameParameterRef(global::System.IntPtr GameParameterRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteGameParameterArrayRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteGameParameterArrayRef(global::System.IntPtr GameParameterRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetGameParameter", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetGameParameter(global::System.IntPtr GameParameterArrayRefPtr, int index);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetGameParameterName", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetGameParameterName(global::System.IntPtr GameParameterRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetGameParameterPath", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetGameParameterPath(global::System.IntPtr GameParameterRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetGameParameterGuid", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetGameParameterGuid(global::System.IntPtr GameParameterRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetGameParameterShortId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetGameParameterShortId(global::System.IntPtr GameParameterRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetGameParameterRefString", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetGameParameterRefString(string InString);

    /*
     * Trigger Ref
     */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAllTriggerRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAllTriggerRef();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetTriggerCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetTriggerCount();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteTriggerRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteTriggerRef(global::System.IntPtr TriggerRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteTriggerArrayRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteTriggerArrayRef(global::System.IntPtr TriggerRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetTrigger", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetTrigger(global::System.IntPtr TriggerArrayRefPtr, int index);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetTriggerName", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetTriggerName(global::System.IntPtr TriggerRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetTriggerPath", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetTriggerPath(global::System.IntPtr TriggerRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetTriggerGuid", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetTriggerGuid(global::System.IntPtr TriggerRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetTriggerShortId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetTriggerShortId(global::System.IntPtr TriggerRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetTriggerRefString", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetTriggerRefString(string InString);

    /*
     * Bus Ref
     */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAllBusRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAllBusRef();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetBusCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetBusCount();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteBusRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteBusRef(global::System.IntPtr BusRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteBusArrayRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteBusArrayRef(global::System.IntPtr BusRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetBus", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetBus(global::System.IntPtr BusArrayRefPtr, int index);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetBusName", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetBusName(global::System.IntPtr BusRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetBusPath", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetBusPath(global::System.IntPtr BusRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetBusGuid", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetBusGuid(global::System.IntPtr BusRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetBusShortId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetBusShortId(global::System.IntPtr BusRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetBusRefString", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetBusRefString(string InString);

    /*
     * AuxBus Ref
     */

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAllAuxBusRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAllAuxBusRef();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAuxBusCount", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetAuxBusCount();

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteAuxBusRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteAuxBusRef(global::System.IntPtr AuxBusRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_DeleteAuxBusArrayRef", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteAuxBusArrayRef(global::System.IntPtr AuxBusRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAuxBus", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAuxBus(global::System.IntPtr AuxBusArrayRefPtr, int index);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAuxBusName", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetAuxBusName(global::System.IntPtr AuxBusRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAuxBusPath", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetAuxBusPath(global::System.IntPtr AuxBusRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAuxBusGuid", CallingConvention = CallingConvention.Cdecl)]
    public static extern System.IntPtr GetAuxBusGuid(global::System.IntPtr AuxBusRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAuxBusShortId", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetAuxBusShortId(global::System.IntPtr AuxBusRefPtr);

    [DllImport("WwiseProjectDatabase.dll", EntryPoint = "Wwise_ProjectDB_GetAuxBusRefString", CallingConvention = CallingConvention.Cdecl)]
    public static extern global::System.IntPtr GetAuxBusRefString(string InString);
}
#endif
