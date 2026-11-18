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

using System.Runtime.InteropServices;

namespace Wwise.SDK
{
    /// <summary>
    /// Wrapper for AkOption, and Ak_SoundEngine_Init
    /// </summary>
    public class AkOptionController
    {
        /// <summary>
        /// Sets an integer-valued option.
        /// </summary>
        /// <param name="option">One of the option keys from the AkOption enums.</param>
        /// <param name="value">The option value.</param>
        /// <param name="scope">The scope in which to set the value. For global options, use 0. For plug-in options, use the plug-in ID.</param>
        /// <returns>
        /// - <c>AKRESULT.AK_Success</c>: Option was set.
        /// - <c>AKRESULT.AK_InvalidParameter</c>: Option is not of type Boolean or Integer. Use <see cref="AK_Option_SetF"/> for Real-valued options, <see cref="AK_Option_SetP"/> for Pointer-valued options, and <see cref="AK_Option_SetS"/> for String-valued options.
        /// - <c>AKRESULT.AK_InvalidID</c>: <paramref name="option"/> is invalid. When <paramref name="scope"/> is 0, only values contained in the AkOption enums are accepted.
        /// </returns>
        public static AKRESULT AK_Option_SetI(uint option, long value, uint scope = 0)
        {
            return Wwise.SDK.AkOption.AK_Option_SetI(option, value, scope);
        }

        /// <summary>
        /// Sets a real-valued option.
        /// </summary>
        /// <param name="option">One of the option keys from the AkOption enums.</param>
        /// <param name="value">The option value.</param>
        /// <param name="scope">The scope in which to set the value. For global options, use 0. For plug-in options, use the plug-in ID.</param>
        /// <returns>
        /// - <c>AKRESULT.AK_Success</c>: Option was set.
        /// - <c>AKRESULT.AK_InvalidParameter</c>: Option is not of type Real. Use <see cref="AK_Option_SetI"/> for Bool- or Integer-valued options, <see cref="AK_Option_SetP"/> for Pointer-valued options, and <see cref="AK_Option_SetS"/> for String-valued options.
        /// - <c>AKRESULT.AK_InvalidID</c>: <paramref name="option"/> is invalid. When <paramref name="scope"/> is 0, only values contained in the AkOption enums are accepted.
        /// </returns>
        public static AKRESULT AK_Option_SetF(uint option, double value, uint scope = 0)
        {
            return Wwise.SDK.AkOption.AK_Option_SetF(option, value, scope);
        }

        /// <summary>
        /// Sets a pointer-valued option.
        /// </summary>
        /// <param name="option">One of the option keys from the <see cref="AkOptionEntry"/> enum.</param>
        /// <param name="value">The option value.</param>
        /// <param name="scope">The scope in which to set the value. For global options, use 0. For plug-in options, use the plug-in ID.</param>
        /// <returns>
        /// - <c>AKRESULT.AK_Success</c>: Option was set.
        /// - <c>AKRESULT.AK_InvalidParameter</c>: Option is not of type Pointer. Use <see cref="AK_Option_SetI"/> for Bool- or Integer-valued options, <see cref="AK_Option_SetF"/> for Real-valued options, and <see cref="AK_Option_SetS"/> for String-valued options.
        /// - <c>AKRESULT.AK_InvalidID</c>: <paramref name="option"/> is invalid. When <paramref name="scope"/> is 0, only values contained in the AkOption enums are accepted.
        /// </returns>
        public static AKRESULT AK_Option_SetP(uint option, System.IntPtr value, uint scope = 0)
        {
            return Wwise.SDK.AkOption.AK_Option_SetP(option, value, scope);
        }

        /// <summary>
        /// Sets a string-valued option.
        /// </summary>
        /// <param name="option">One of the option keys from the <see cref="AkOptionEntry"/> enum.</param>
        /// <param name="value">The option value. This is a pointer to a UTF-8 string. <b>This string must be freed after, and only after, the SoundEngine has been initialized.</b></param>
        /// <param name="scope">The scope in which to set the value. For global options, use 0. For plug-in options, use the plug-in ID.</param>
        /// <returns>
        /// - <c>AKRESULT.AK_Success</c>: Option was set.
        /// - <c>AKRESULT.AK_InvalidParameter</c>: Option is not of type String. Use <see cref="AK_Option_SetI"/> for Bool- or Integer-valued options, <see cref="AK_Option_SetF"/> for Real-valued options, and <see cref="AK_Option_SetP"/> for Pointer-valued options.
        /// - <c>AKRESULT.AK_InvalidID</c>: <paramref name="option"/> is invalid. When <paramref name="scope"/> is 0, only values contained in the AkOption enums are accepted.
        /// </returns>
        public static AKRESULT AK_Option_SetS(uint option, System.IntPtr value, uint scope = 0)
        {
            return Wwise.SDK.AkOption.AK_Option_SetS(option, value, scope);
        }

        /// <summary>
        /// Retrieve the value for an option of type Integer or Boolean.
        /// If the option was never set, its default value is returned.
        /// </summary>
        /// <param name="option">One of the option keys from the <see cref="AkOptionEntry"/> enum.</param>
        /// <param name="scope">The scope in which to read the value. For global options, use 0. For plug-in options, use the plug-in ID.</param>
        /// <returns>The current value for this option. If the specified option key is not of type Bool or Integer, 0 is returned.</returns>
        public static long AK_Option_GetI(uint option, uint scope = 0)
        {
            return Wwise.SDK.AkOption.AK_Option_GetI(option, scope);
        }
        
        /// <summary>
        /// Sets a Boolean-valued option.
        /// </summary>
        /// <param name="option">One of the option keys from the AkOption enums.</param>
        /// <param name="value">The option value.</param>
        /// <param name="scope">The scope in which to set the value. For global options, use 0. For plug-in options, use the plug-in ID.</param>
        /// <returns>
        /// - <c>AKRESULT.AK_Success</c>: Option was set.
        /// - <c>AKRESULT.AK_InvalidParameter</c>: Option is not of type Boolean or Integer.
        /// - <c>AKRESULT.AK_InvalidID</c>: <paramref name="option"/> is invalid.
        /// </returns>
        public static AKRESULT AK_Option_SetB(uint option, bool value, uint scope = 0)
        {
            return AK_Option_SetI(option, value ? 1L : 0L, scope);
        }

        /// <summary>
        /// Retrieve the value for an option of type Boolean.
        /// If the option was never set, its default value is returned.
        /// </summary>
        /// <param name="option">One of the option keys from the AkOption enum.</param>
        /// <param name="scope">The scope in which to read the value. For global options, use 0. For plug-in options, use the plug-in ID.</param>
        /// <returns>The current value for this option. Any non-zero integer value is treated as true.</returns>
        public static bool AK_Option_GetB(uint option, uint scope = 0)
        {
            return AK_Option_GetI(option, scope) != 0;
        }

        /// <summary>
        /// Retrieve the value for an option of type Real.
        /// If the option was never set, its default value is returned.
        /// </summary>
        /// <param name="option">One of the option keys from the <see cref="AkOptionEntry"/> enum.</param>
        /// <param name="scope">The scope in which to read the value. For global options, use 0. For plug-in options, use the plug-in ID.</param>
        /// <returns>The current value for this option. If the specified option key is not of type Real, 0 is returned.</returns>
        public static double AK_Option_GetF(uint option, uint scope = 0)
        {
            return Wwise.SDK.AkOption.AK_Option_GetF(option, scope);
        }

        /// <summary>
        /// Retrieve the value for an option of type Pointer.
        /// If the option was never set, its default value is returned.
        /// </summary>
        /// <param name="option">One of the option keys from the <see cref="AkOptionEntry"/> enum.</param>
        /// <param name="scope">The scope in which to read the value. For global options, use 0. For plug-in options, use the plug-in ID.</param>
        /// <returns>The current value for this option. If the specified option key is not of type Pointer, <c>IntPtr.Zero</c> is returned.</returns>
        public static System.IntPtr AK_Option_GetP(uint option, uint scope = 0)
        {
            return Wwise.SDK.AkOption.AK_Option_GetP(option, scope);
        }

        /// <summary>
        /// Retrieve the value for an option of type String.
        /// If the option was never set, its default value is returned.
        /// </summary>
        /// <param name="option">One of the option keys from the <see cref="AkOptionEntry"/> enum.</param>
        /// <param name="scope">The scope in which to read the value. For global options, use 0. For plug-in options, use the plug-in ID.</param>
        /// <returns>The current value for this option. If the specified option key is not of type String, <c>null</c> is returned.</returns>
        public static string AK_Option_GetS(uint option, uint scope = 0)
        {
            return Marshal.PtrToStringUTF8(Wwise.SDK.AkOption.AK_Option_GetS(option, scope));
        }

        /// <summary>
        /// Resets all options to default values, across all scopes.
        /// </summary>
        public static void AK_Option_ResetToDefaults()
        {
            Wwise.SDK.AkOption.AK_Option_ResetToDefaults();
        }

        /// <summary>
        /// Resets all plug-in options to 0, across all plug-in scopes.
        /// Options in the global scope keep their values. This call frees all memory allocations by the AK::Option subsystem.
        /// </summary>
        public static void AK_Option_ResetPluginScopes()
        {
            Wwise.SDK.AkOption.AK_Option_ResetPluginScopes();
        }
    }
}