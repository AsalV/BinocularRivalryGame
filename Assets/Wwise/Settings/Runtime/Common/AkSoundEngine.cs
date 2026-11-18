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

    // TODO Use LibraryImport instead of DllImport as soon as Unity gets to .NET 7
    internal static class AkSoundEngine
    {

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_SoundEngine_Init", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern AKRESULT AK_SoundEngine_Init();

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_SoundEngine_RenderAudio", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern AKRESULT AK_SoundEngine_RenderAudio([System.Runtime.InteropServices.In][System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U1)]bool in_bAllowSyncRender);

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_SoundEngine_Term", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern void AK_SoundEngine_Term();

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_SoundEngine_IsInitialized", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U1)]
        internal static extern bool AK_SoundEngine_IsInitialized();

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_SoundEngine_GeneratePlayingID", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern uint AK_SoundEngine_GeneratePlayingID();

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_SoundEngine_GetIDFromString", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern uint AK_SoundEngine_GetIDFromString([System.Runtime.InteropServices.In]System.IntPtr in_pszString);

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_SoundEngine_GetIDFromStringW", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern uint AK_SoundEngine_GetIDFromStringW([System.Runtime.InteropServices.In]System.IntPtr in_pszString);

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_SoundEngine_GetOutputID", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern ulong AK_SoundEngine_GetOutputID([System.Runtime.InteropServices.In]uint in_idShareset, [System.Runtime.InteropServices.In]uint in_idDevice);

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_StreamMgr_SetCurrentLanguage", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern AKRESULT AK_StreamMgr_SetCurrentLanguage([System.Runtime.InteropServices.In]System.IntPtr in_pszCurrentLanguage);

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_StreamMgr_AddBasePath", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern AKRESULT AK_StreamMgr_AddBasePath([System.Runtime.InteropServices.In]System.IntPtr in_pszBasePath);

    }
}