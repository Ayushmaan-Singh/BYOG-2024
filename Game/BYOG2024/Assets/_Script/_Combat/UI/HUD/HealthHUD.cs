using System;
using System.Collections;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using AstekUtility.Observer.Unmanaged;
using Entity.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Combat.UI
{
	using HealthObserver=AstekUtility.Observer.Unmanaged.IObserver<(float maxHP,float currentHP)>;
	public class HealthHUD : MonoBehaviour,HealthObserver
	{

		[SerializeField] private Image availableHealthImage;
		[SerializeField] private TextMeshProUGUI healthAmountText;

		private IEnumerator Start()
		{
			yield return new WaitWhile(() =>
			{
				ServiceLocator forSceneOf = ServiceLocator.ForSceneOf(this);
				return forSceneOf?.Get<PlayerMediator>() == null;
			});
			
			PlayerMediator mediator = ServiceLocator.ForSceneOf(this).Get<PlayerMediator>();
			mediator.healthSubject.Attach(this);
			float health = Mathf.Clamp(mediator.CurrentHp / mediator.MaxHp, 0, 1);
			availableHealthImage.fillAmount = health;
			healthAmountText.text = $"{(int)(health * 100)}%";
		}

		private void OnDestroy()
		{
			PlayerMediator mediator = ServiceLocator.ForSceneOf(this)?.Get<PlayerMediator>();
			mediator?.healthSubject.Detach(this);
		}

		public void OnNotify(ISubject<(float maxHP, float currentHP)> subject)
		{
			float health = Mathf.Clamp(subject.Data.currentHP / subject.Data.maxHP, 0, 1);
			availableHealthImage.fillAmount = health;
			healthAmountText.text = $"{(int)(health * 100)}%";
		}
	}
}