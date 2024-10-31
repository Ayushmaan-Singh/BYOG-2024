using System;
using System.Collections;
using AstekUtility;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Observer.Unmanaged;
using Combat.Player;
using Entity.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Combat.UI
{
	using HealthObserver = AstekUtility.Observer.Unmanaged.IObserver<(float maxHP, float currentHP)>;
	public class HealthHUD : MonoBehaviour, HealthObserver
	{

		[SerializeField] private PlayerRuntimeSet playerRTSet;
		[SerializeField] private Image availableHealthImage;
		[SerializeField] private TextMeshProUGUI healthAmountText;

		private IEnumerator Start()
		{
			yield return new WaitWhile(() => !playerRTSet.Owner.OrNull());

			playerRTSet.Owner.HealthSubject.Attach(this);
			float health = Mathf.Clamp(playerRTSet.Owner.CurrentHp / playerRTSet.Owner.MaxHp, 0, 1);
			availableHealthImage.fillAmount = health;
			healthAmountText.text = $"{(int)(health * 100)}%";
		}

		private void OnDisable()
		{
			playerRTSet.Owner?.HealthSubject.Detach(this);
		}

		public void OnNotify(ISubject<(float maxHP, float currentHP)> subject)
		{
			float health = Mathf.Clamp(subject.Data.currentHP / subject.Data.maxHP, 0, 1);
			availableHealthImage.fillAmount = health;
			healthAmountText.text = $"{(int)(health * 100)}%";
		}
	}
}