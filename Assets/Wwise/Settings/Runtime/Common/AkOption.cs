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
    internal static class AkOption
    {
        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_Option_SetI", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern AKRESULT AK_Option_SetI(uint option, long value, uint scope);

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_Option_SetF", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern AKRESULT AK_Option_SetF(uint option, double value, uint scope);

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_Option_SetP", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern AKRESULT AK_Option_SetP(uint option, System.IntPtr value, uint scope);

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_Option_SetS", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern AKRESULT AK_Option_SetS(uint option, System.IntPtr value, uint scope);

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_Option_GetI", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern long AK_Option_GetI(uint option, uint scope);

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_Option_GetF", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern double AK_Option_GetF(uint option, uint scope);

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_Option_GetP", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern System.IntPtr AK_Option_GetP(uint option, uint scope);

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_Option_GetS", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern System.IntPtr AK_Option_GetS(uint option, uint scope);

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_Option_ResetToDefaults", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern void AK_Option_ResetToDefaults();

        [System.Runtime.InteropServices.DllImport(AkPInvokeLibraryNames.LibraryName, EntryPoint="AK_Option_ResetPluginScopes", CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern void AK_Option_ResetPluginScopes();

    }
}
