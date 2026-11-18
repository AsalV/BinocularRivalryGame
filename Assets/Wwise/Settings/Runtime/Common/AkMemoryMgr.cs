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
    internal static class AkMemoryMgr
    {

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_MemoryMgr_Init", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern AKRESULT AK_MemoryMgr_Init();

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_MemoryMgr_Term", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern void AK_MemoryMgr_Term();

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_MemoryMgr_IsInitialized", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U1)]
        internal static extern bool AK_MemoryMgr_IsInitialized();

    }
}