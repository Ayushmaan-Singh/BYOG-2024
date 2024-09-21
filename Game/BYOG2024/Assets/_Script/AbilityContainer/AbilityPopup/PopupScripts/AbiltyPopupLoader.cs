using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Entity.Abilities;

public class AbiltyPopupLoader : MonoBehaviour
{
    [SerializeField] private GameObject abilityPanelObj;
    [SerializeField] private GameObject abilityBaseObj;
    [SerializeField] private GameObject abilitiesPrefab;
    [SerializeField] private Button noBtn;
    public InputMouse inputMouse;
    public RectTransform parentTransform;
    public AbilityBase[] abilityBases;
    private PopupInput popupInput;
    public delegate void PopupActivatedEvent();
    public static event PopupActivatedEvent OnPopupActivated;

    public delegate void PrefabsInstantiatedEvent();
    public static event PrefabsInstantiatedEvent OnPrefabsInstantiated;
    // Start is called before the first frame update
    private void Awake()
    {
        popupInput = new PopupInput();
        popupInput.ActivatePopup.Popup.performed += OnActivatePopup;
        popupInput.ActivatePopup.LeftClick.performed += OnClickLeft;
        popupInput.ActivatePopup.RightClick.performed += OnClickRight;
        popupInput.ActivatePopup.RightClick.performed += OnClickRight;
    }
    private void OnEnable()
    {
        popupInput.ActivatePopup.Popup.Enable();
        popupInput.ActivatePopup.LeftClick.Enable();
        popupInput.ActivatePopup.RightClick.Enable();
        OnPrefabsInstantiated += PrefabsInastantiated;

        // OnPopupActivated += RespondToPopupActivation;
        noBtn.onClick.AddListener(DeactivatePopup);
    }
    
    private void OnDisable()
    {
        popupInput.ActivatePopup.Popup.Disable();
        popupInput.ActivatePopup.LeftClick.Disable();
        popupInput.ActivatePopup.RightClick.Disable();
        OnPrefabsInstantiated -= PrefabsInastantiated;
        //OnPopupActivated -= RespondToPopupActivation;
        noBtn.onClick.RemoveListener(DeactivatePopup);
    }
    private void OnActivatePopup(InputAction.CallbackContext context)
    {
        RespondToPopupActivation();
    }
    public void PrefabsInastantiated()
    {
        OnPrefabsInstantiated?.Invoke();
    }
    public void OnClickLeft(InputAction.CallbackContext context)
    {
        inputMouse = InputMouse.LeftClick;
    }
    public void OnClickRight(InputAction.CallbackContext context)
    {
        inputMouse = InputMouse.RightClick;
    }
    public void ActivatePopup()
    {
        if (abilityPanelObj != null)
        {
            abilityPanelObj.SetActive(true);
            Debug.Log("Popup activated.");

            // Raise the popup activated event
            OnPopupActivated?.Invoke();
        }
        
        // Update is called once per frame
    }
    public void InstantiateAbilityPrefabs(AbilityBase[] abilities)
    {
        for (int i = 0; i < abilities.Length; i++)
        {
            // Instantiate the ability prefab under the parent
            GameObject prefab = Instantiate(abilitiesPrefab, parentTransform);

            // Get references to the Image and Text components in the prefab
            Image abilityImage = prefab.GetComponent<Image>();
            Text abilityText = prefab.GetComponent<RectTransform>().GetChild(0).GetComponent<Text>();

            // Set the sprite and text from the AbilityBase data
            abilityImage.sprite = abilities[i].AbilityIcon;
            abilityText.text = abilities[i].AbilityDescription;
        }
    }    
    private void RespondToPopupActivation()
    {
        Debug.Log("Subscriber: Popup activated event received.");
       
        ActivatePopup();
    }
    private void DeactivatePopup()
    {
        abilityPanelObj.SetActive(false);
    }
}
public enum InputMouse
{
    LeftClick,
    RightClick
}


