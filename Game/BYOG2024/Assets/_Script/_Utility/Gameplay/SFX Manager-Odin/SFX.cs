using System;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Sirenix.OdinInspector;
using UnityEngine;

#if ODIN_INSPECTOR

namespace AstekUtility.Odin.SFX
{
	[System.Serializable]
	public class SFX
	{
		[LabelText("SFX Type"), LabelWidth(100), OnValueChanged("SFXChange"), InlineButton("PlaySFX")]
		public SFXManager.SFXType SFXType = SFXManager.SFXType.UI;

		[LabelText("$_sfxLabel"), LabelWidth(100), ValueDropdown("SFXType"), OnValueChanged("SFXChange"), InlineButton("SelectSFX")]
		public SFXClip SfxToPlay;
		private string _sfxLabel = "SFX";

		[SerializeField] private bool _showSettings = false;
		[SerializeField] private bool _editSettings = false;

		[SerializeField, ShowIf("_showSettings"), EnableIf("_editSettings"),InlineEditor(InlineEditorObjectFieldModes.Hidden)] private SFXClip _sfxBase;
		[SerializeField, ShowIf("_showSettings"), EnableIf("_editSettings")] private bool _waitToPlay = true;
		[SerializeField, ShowIf("_showSettings"), EnableIf("_editSettings")] private bool _useDefault = true;

		[SerializeField, ShowIf("_showSettings"), EnableIf("_editSettings")] private AudioSource _audioSource;

		private void SFXChange()
		{
			//Keep label upto date
			_sfxLabel = $"{SFXType}SFX";
			//Keep displayed "SFX clip" up to date
			_sfxBase = SfxToPlay;
		}

		private void SelectSFX()
		{
			UnityEditor.Selection.activeObject = SfxToPlay;
		}

		//Get's list of SFX from manager, used in inspector
		private List<SFXClip> GetSFXType()
		{
			SFXManager sfxManager = SFXManager.Instance;

			if (sfxManager == null)
				return null;

			return SFXType switch
			{
				SFXManager.SFXType.UI => sfxManager.UiSFX,
				SFXManager.SFXType.Ambient => sfxManager.AmbientSFX,
				SFXManager.SFXType.Weapons => sfxManager.WeaponSFX,
				_ => null
			};
		}

		public void PlaySFX()
		{
			SFXManager.PlaySFX(SfxToPlay, _waitToPlay, _useDefault || _audioSource == null ? null : _audioSource);
		}
	}
}

#endif