#if !(UNITY_QNX) // Disable under unsupported platforms.
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

[UnityEngine.AddComponentMenu("Wwise/Acoustics/WwiseAcousticsListener")]
[UnityEngine.RequireComponent(typeof(AkAudioListener))]
[UnityEngine.RequireComponent(typeof(AkRoomAwareObject))]
[UnityEngine.DisallowMultipleComponent]
///@brief Add this script on the game object that represent a listener.  This is normally added to the Camera object or the Player object, but can be added to any game object when implementing 3D busses.  \c isDefaultListener determines whether the game object will be considered a default listener - a listener that automatically listens to all game objects that do not have listeners attached to their AkGameObjListenerList's.
/// \sa
/// - <a href="https://www.audiokinetic.com/library/edge/?source=SDK&id=soundengine__listeners.html" target="_blank">Integrating Listeners</a> (Note: This is described in the Wwise SDK documentation.)
public class WwiseAcousticsListener : UnityEngine.MonoBehaviour
#if UNITY_EDITOR
	, AK.Wwise.IMigratable
#endif
{
	private static WwiseAcousticsListener s_AcousticsListener;
	private static readonly WwiseAcousticsListenerList acousticsListeners = new WwiseAcousticsListenerList();
	private AkAudioListener AkAudioListener;

	/// <summary>
	///     Returns the "single" Wwise Acoustics listener.
	/// </summary>
	public static AkAudioListener TheAcousticsListener
	{
		get { return s_AcousticsListener != null ? s_AcousticsListener.AkAudioListener : null; }
	}

	[System.Obsolete(AkUnitySoundEngine.Ak_SpatialAudio_Rename_2026_1_0 + " Use TheAcousticsListener instead.")]
	public static AkAudioListener TheSpatialAudioListener
	{
		get => TheAcousticsListener;
	}

	/// <summary>
	///     Returns the list of active Unity Game Objects that are designated to be Acoustics listeners.
	/// </summary>
	public static WwiseAcousticsListenerList AcousticsListeners
	{
		get { return acousticsListeners; }
	}

	[System.Obsolete(AkUnitySoundEngine.Ak_SpatialAudio_Rename_2026_1_0 + "Use AcousticsListeners instead.")]
	public static WwiseAcousticsListenerList SpatialAudioListeners
	{
		get => AcousticsListeners;
	}

	private void Awake()
	{
		AkAudioListener = GetComponent<AkAudioListener>();
	}

	private void OnEnable()
	{
		acousticsListeners.Add(this);
	}

	private void OnDisable()
	{
		acousticsListeners.Remove(this);
	}

	/// <summary>
	///     This class represents the list of active Unity Game Objects that are designated to be Acoustics listeners.
	///     Currently, only one Acoustics listener can be active at a time.
	/// </summary>
	public class WwiseAcousticsListenerList
	{
		private readonly System.Collections.Generic.List<WwiseAcousticsListener> listenerList =
			new System.Collections.Generic.List<WwiseAcousticsListener>();

		public System.Collections.Generic.List<WwiseAcousticsListener> ListenerList
		{
			get { return listenerList; }
		}

		/// <summary>
		///     Uniquely adds listeners to the list
		/// </summary>
		/// <param name="listener"></param>
		/// <returns></returns>
		public bool Add(WwiseAcousticsListener listener)
		{
			if (listener == null)
			{
				return false;
			}

			if (listenerList.Contains(listener))
			{
				return false;
			}

			listenerList.Add(listener);
			Refresh();
			return true;
		}

		/// <summary>
		///     Removes listeners from the list
		/// </summary>
		/// <param name="listener"></param>
		/// <returns></returns>
		public bool Remove(WwiseAcousticsListener listener)
		{
			if (listener == null)
			{
				return false;
			}

			if (!listenerList.Remove(listener))
			{
				return false;
			}

			Refresh();
			return true;
		}

		private void Refresh()
		{
			if (ListenerList.Count == 1)
			{
				if (s_AcousticsListener != null)
				{
					AkUnitySoundEngine.UnregisterAcousticsListener(s_AcousticsListener.gameObject);
				}

				s_AcousticsListener = ListenerList[0];

				AkUnitySoundEngine.RegisterAcousticsListener(s_AcousticsListener.gameObject);
			}
			else if (ListenerList.Count == 0 && s_AcousticsListener != null)
			{
				AkUnitySoundEngine.UnregisterAcousticsListener(s_AcousticsListener.gameObject);
				s_AcousticsListener = null;
			}
		}
	}

	[System.Obsolete(AkUnitySoundEngine.Ak_SpatialAudio_Rename_2026_1_0 + " Use WwiseAcousticsListenerList instead.")]
	public class SpatialAudioListenerList : WwiseAcousticsListenerList
	{ }

#if UNITY_EDITOR
	#region WwiseMigration
	bool AK.Wwise.IMigratable.Migrate(UnityEditor.SerializedObject obj)
	{
		if (!AkUtilities.IsMigrationRequired(AkUtilities.MigrationStep.NewScriptableObjectFolder_v2019_2_0))
			return false;

		UnityEditor.Undo.AddComponent<AkRoomAwareObject>(gameObject);

		return true;
	}
	#endregion
#endif
}
#endif // #if !(UNITY_QNX) // Disable under unsupported platforms.