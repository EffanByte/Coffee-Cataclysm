using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityHFSM;
using Playroom;
using SimpleJSON;
using System;
public enum BlendState
{
    NoBlend,
    LowBlend,
    MediumBlend,
    HighBlend,
    CoffeeBlendBrown,
    CoffeeBlendWhite,
    MixedCoffeeBlend // 👈 new blend
}

[Serializable]
public class InsertBeanPayload
{
    public string MakerId;
    public HeldItemType BeanType;
}
public class CoffeeMakerInteraction : MonoBehaviour
{   

    public string MakerId;
    private StateMachine<BlendState> blendFSM;

    [Header("Brewing Settings")]
    [SerializeField] private float brewDuration = 5f;
    public float vibrationMagnitude = 0.01f;

    [Header("Progress Bar UI")]
    public Slider progressBar;
    public Color doneColor = Color.yellow;
    private Vector3 originalPosition;
    private float timer = 0f;
    private bool isBrewing = false;
    public HeldItemType blendType = HeldItemType.None;
    public bool isBlendReady = false;
    [SerializeField] private float blendTimer = 0f;
    [SerializeField] private float blendTimeout = 7f;

    private PlayroomKit _playroomKit;
    public List<HeldItemType> insertedBeans = new();

    void Start()
    {
        _playroomKit = PlayroomManager.Instance.GetPlayroomKit();

        blendFSM = new StateMachine<BlendState>();

        blendFSM.AddState(BlendState.NoBlend, new State<BlendState>());
        blendFSM.AddState(BlendState.LowBlend, new State<BlendState>());
        blendFSM.AddState(BlendState.HighBlend, new State<BlendState>());


        blendFSM.SetStartState(BlendState.NoBlend);

        blendFSM.AddTransition(BlendState.NoBlend, BlendState.LowBlend, _ => timer >= brewDuration * 0.2f && timer < brewDuration * 0.5f);
        blendFSM.AddTransition(BlendState.LowBlend, BlendState.HighBlend, _ => timer >= brewDuration * 0.5f && timer < brewDuration * 1f);

        blendFSM.Init();

        originalPosition = transform.localPosition;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
            progressBar.value = 0;
        }

    }

    void Update()
    {
        blendFSM.OnLogic();
        float percent = timer / brewDuration;

        if (isBrewing)
        {
            timer += Time.deltaTime;
            if (timer < brewDuration)
            {
                transform.localPosition = originalPosition + UnityEngine.Random.insideUnitSphere * vibrationMagnitude;
                if (progressBar != null)
                    progressBar.value = percent;
            }
            else
            {
                Debug.Log("Brewing finished!");
                FinishBrewing();
            }
        }

        if (isBlendReady)
        {
            blendTimer += Time.deltaTime;
            if (blendTimer >= blendTimeout)
            {
                Debug.Log("Blend burned!");
                isBlendReady = false;
                blendTimer = 0f;
            }
        }
    }

    public void InsertBean(HeldItemType beanType)
    {
        var node = new JSONObject();
        string payload = $"{MakerId}|?|{beanType.ToString()}";
        _playroomKit.RpcCall("HandleInsertBean", payload, PlayroomKit.RpcMode.ALL);
    }

    public void ProcessInsertBean(HeldItemType beanType)
    {
        if (insertedBeans.Count == 0)
            ProcessStartBrewing();
        insertedBeans.Add(beanType);
        timer -= timer * 0.25f;
        Debug.Log($"Inserted bean: {beanType}, total beans: {insertedBeans.Count}");
    }

    private void ProcessStartBrewing()
    {
        isBrewing = true;
        timer = 0;
        originalPosition = transform.localPosition;
        if (progressBar != null) progressBar.gameObject.SetActive(true);
    }
    public void StartBrewing()
    {
        isBrewing = true;
        timer = 0;
        originalPosition = transform.localPosition;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0;
        }
    }


    void FinishBrewing()
    {
        isBrewing = false;
        transform.localPosition = originalPosition;

        isBlendReady = true;
        blendTimer = 0f;

        bool hasBrown = insertedBeans.Contains(HeldItemType.CoffeeBeanBrown);
        bool hasWhite = insertedBeans.Contains(HeldItemType.CoffeeBeanWhite);

        if (hasBrown && hasWhite)
            blendType = HeldItemType.MixedCoffeeBlend;
        else if (hasBrown)
        {
            blendType = HeldItemType.CoffeeBlendBrown;
            Debug.Log("Ayein");
        }
        else if (hasWhite)
            blendType = HeldItemType.CoffeeBlendWhite;

        insertedBeans.Clear();
        Invoke(nameof(HideProgressBar), 4f);
    }

    void HideProgressBar()
    {
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
    }

    public bool CanStartBrewing()
    {
        return !isBrewing && !isBlendReady;
    }

    public bool IsBlendReady()
    {
        return isBlendReady;
    }

    public void GiveBlendToPlayer()
    {
        string payload = $"{MakerId}|?|{blendType.ToString()}";
        _playroomKit.RpcCall("HandleReceiveBlend", payload, PlayroomKit.RpcMode.ALL);
    }

    // helper to find by ID
    public static CoffeeMakerInteraction GetById(string makerId)
    {
        var all = GameObject.FindGameObjectsWithTag("CoffeeMaker");
        for (int i = 0; i < all.Length; i++)
        {
            Debug.Log(i.ToString() + " " + all[i].name);
            var cm = all[i].GetComponent<CoffeeMakerInteraction>();
            if (cm != null && cm.MakerId == makerId)
                return cm;
        }

        Debug.LogWarning($"[CoffeeMaker] no instance found with MakerId '{makerId}'");
        return null;
    }
}

