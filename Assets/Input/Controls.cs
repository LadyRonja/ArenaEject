//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Input/Controls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Controls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""7c4a2d4d-ad5a-4b31-957d-0e10b94b993f"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""e692304a-70e2-4d4d-aefd-5d2a0d1c0f47"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Aiming"",
                    ""type"": ""Value"",
                    ""id"": ""d052a213-79e0-4326-9f84-0ad6628db94a"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""NorthButtonDown"",
                    ""type"": ""Button"",
                    ""id"": ""42701f2d-2798-4152-b4a7-da77e6853097"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Value"",
                    ""id"": ""5a72e6b8-ab94-4057-a9ab-b4627649cdb4"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""SouthButtonDown"",
                    ""type"": ""Button"",
                    ""id"": ""8543844e-b225-4925-ba9b-6dc468ca225e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""WestButtonDown"",
                    ""type"": ""Button"",
                    ""id"": ""f9288405-74ae-476d-8a0f-68d60154f977"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""EastButtonDown"",
                    ""type"": ""Button"",
                    ""id"": ""59553127-8b67-419e-ad89-5e2bfb012c91"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Start"",
                    ""type"": ""Button"",
                    ""id"": ""5849d005-fa6a-4ab2-88dc-761aebb6f671"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AltFire"",
                    ""type"": ""Value"",
                    ""id"": ""2edff1c5-a0db-470c-884e-8736627cad6f"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""RightBumperDown"",
                    ""type"": ""Button"",
                    ""id"": ""c3408bde-5733-42ae-a1f6-a7c5c36f13e6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftBumperDown"",
                    ""type"": ""Button"",
                    ""id"": ""ec3095d8-9628-4cb7-bd50-323e180e52a6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftTrigger"",
                    ""type"": ""Value"",
                    ""id"": ""689a3769-5713-4d91-bedf-75cabf31ccc3"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""DPadRight"",
                    ""type"": ""Button"",
                    ""id"": ""91b02fa0-c28a-4d89-a974-9bd1a23582de"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DPadLeft"",
                    ""type"": ""Button"",
                    ""id"": ""73b5c80a-898c-4b22-bcd9-44819012d29e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DPadUp"",
                    ""type"": ""Button"",
                    ""id"": ""f3484cd9-83c6-4b38-8455-099be8fb352b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""beb1880b-8592-46dd-8f6a-4724fc38b555"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2690fe76-228e-44ee-bff3-061c9d0f80bc"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aiming"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7ad39064-1683-46fd-8ff1-d0b05b4cc609"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NorthButtonDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""08863d26-377f-495f-bf06-f3e6f72a2729"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""63ad9af2-9b70-432c-868e-1b008d2ebeb7"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SouthButtonDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5d3be04e-4ae6-4d4e-9583-848e39c02840"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WestButtonDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4d1a1cbd-6e60-4190-b430-c93f654bf395"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EastButtonDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""79f92b26-ccec-4f3d-b03d-56e1cc63a92e"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1af5c3f3-bb35-4a5b-97e1-78bbf16a0e7d"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AltFire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9587b0d6-e68a-4598-9076-794d2be94a6a"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightBumperDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""25af07a6-94e4-4316-82d7-425fb18b5733"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftBumperDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2c3bf112-40c3-4bcd-b24d-9b0aafc4e6d8"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""52f066c8-d927-4c5d-b8a4-36b136fd43b4"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DPadRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""edd55933-254f-4b99-9216-3603a37d55cc"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DPadLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d0f29410-7bf2-4fe1-ab03-912d5573f388"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DPadUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Movement = m_Gameplay.FindAction("Movement", throwIfNotFound: true);
        m_Gameplay_Aiming = m_Gameplay.FindAction("Aiming", throwIfNotFound: true);
        m_Gameplay_NorthButtonDown = m_Gameplay.FindAction("NorthButtonDown", throwIfNotFound: true);
        m_Gameplay_Fire = m_Gameplay.FindAction("Fire", throwIfNotFound: true);
        m_Gameplay_SouthButtonDown = m_Gameplay.FindAction("SouthButtonDown", throwIfNotFound: true);
        m_Gameplay_WestButtonDown = m_Gameplay.FindAction("WestButtonDown", throwIfNotFound: true);
        m_Gameplay_EastButtonDown = m_Gameplay.FindAction("EastButtonDown", throwIfNotFound: true);
        m_Gameplay_Start = m_Gameplay.FindAction("Start", throwIfNotFound: true);
        m_Gameplay_AltFire = m_Gameplay.FindAction("AltFire", throwIfNotFound: true);
        m_Gameplay_RightBumperDown = m_Gameplay.FindAction("RightBumperDown", throwIfNotFound: true);
        m_Gameplay_LeftBumperDown = m_Gameplay.FindAction("LeftBumperDown", throwIfNotFound: true);
        m_Gameplay_LeftTrigger = m_Gameplay.FindAction("LeftTrigger", throwIfNotFound: true);
        m_Gameplay_DPadRight = m_Gameplay.FindAction("DPadRight", throwIfNotFound: true);
        m_Gameplay_DPadLeft = m_Gameplay.FindAction("DPadLeft", throwIfNotFound: true);
        m_Gameplay_DPadUp = m_Gameplay.FindAction("DPadUp", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private List<IGameplayActions> m_GameplayActionsCallbackInterfaces = new List<IGameplayActions>();
    private readonly InputAction m_Gameplay_Movement;
    private readonly InputAction m_Gameplay_Aiming;
    private readonly InputAction m_Gameplay_NorthButtonDown;
    private readonly InputAction m_Gameplay_Fire;
    private readonly InputAction m_Gameplay_SouthButtonDown;
    private readonly InputAction m_Gameplay_WestButtonDown;
    private readonly InputAction m_Gameplay_EastButtonDown;
    private readonly InputAction m_Gameplay_Start;
    private readonly InputAction m_Gameplay_AltFire;
    private readonly InputAction m_Gameplay_RightBumperDown;
    private readonly InputAction m_Gameplay_LeftBumperDown;
    private readonly InputAction m_Gameplay_LeftTrigger;
    private readonly InputAction m_Gameplay_DPadRight;
    private readonly InputAction m_Gameplay_DPadLeft;
    private readonly InputAction m_Gameplay_DPadUp;
    public struct GameplayActions
    {
        private @Controls m_Wrapper;
        public GameplayActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Gameplay_Movement;
        public InputAction @Aiming => m_Wrapper.m_Gameplay_Aiming;
        public InputAction @NorthButtonDown => m_Wrapper.m_Gameplay_NorthButtonDown;
        public InputAction @Fire => m_Wrapper.m_Gameplay_Fire;
        public InputAction @SouthButtonDown => m_Wrapper.m_Gameplay_SouthButtonDown;
        public InputAction @WestButtonDown => m_Wrapper.m_Gameplay_WestButtonDown;
        public InputAction @EastButtonDown => m_Wrapper.m_Gameplay_EastButtonDown;
        public InputAction @Start => m_Wrapper.m_Gameplay_Start;
        public InputAction @AltFire => m_Wrapper.m_Gameplay_AltFire;
        public InputAction @RightBumperDown => m_Wrapper.m_Gameplay_RightBumperDown;
        public InputAction @LeftBumperDown => m_Wrapper.m_Gameplay_LeftBumperDown;
        public InputAction @LeftTrigger => m_Wrapper.m_Gameplay_LeftTrigger;
        public InputAction @DPadRight => m_Wrapper.m_Gameplay_DPadRight;
        public InputAction @DPadLeft => m_Wrapper.m_Gameplay_DPadLeft;
        public InputAction @DPadUp => m_Wrapper.m_Gameplay_DPadUp;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void AddCallbacks(IGameplayActions instance)
        {
            if (instance == null || m_Wrapper.m_GameplayActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Add(instance);
            @Movement.started += instance.OnMovement;
            @Movement.performed += instance.OnMovement;
            @Movement.canceled += instance.OnMovement;
            @Aiming.started += instance.OnAiming;
            @Aiming.performed += instance.OnAiming;
            @Aiming.canceled += instance.OnAiming;
            @NorthButtonDown.started += instance.OnNorthButtonDown;
            @NorthButtonDown.performed += instance.OnNorthButtonDown;
            @NorthButtonDown.canceled += instance.OnNorthButtonDown;
            @Fire.started += instance.OnFire;
            @Fire.performed += instance.OnFire;
            @Fire.canceled += instance.OnFire;
            @SouthButtonDown.started += instance.OnSouthButtonDown;
            @SouthButtonDown.performed += instance.OnSouthButtonDown;
            @SouthButtonDown.canceled += instance.OnSouthButtonDown;
            @WestButtonDown.started += instance.OnWestButtonDown;
            @WestButtonDown.performed += instance.OnWestButtonDown;
            @WestButtonDown.canceled += instance.OnWestButtonDown;
            @EastButtonDown.started += instance.OnEastButtonDown;
            @EastButtonDown.performed += instance.OnEastButtonDown;
            @EastButtonDown.canceled += instance.OnEastButtonDown;
            @Start.started += instance.OnStart;
            @Start.performed += instance.OnStart;
            @Start.canceled += instance.OnStart;
            @AltFire.started += instance.OnAltFire;
            @AltFire.performed += instance.OnAltFire;
            @AltFire.canceled += instance.OnAltFire;
            @RightBumperDown.started += instance.OnRightBumperDown;
            @RightBumperDown.performed += instance.OnRightBumperDown;
            @RightBumperDown.canceled += instance.OnRightBumperDown;
            @LeftBumperDown.started += instance.OnLeftBumperDown;
            @LeftBumperDown.performed += instance.OnLeftBumperDown;
            @LeftBumperDown.canceled += instance.OnLeftBumperDown;
            @LeftTrigger.started += instance.OnLeftTrigger;
            @LeftTrigger.performed += instance.OnLeftTrigger;
            @LeftTrigger.canceled += instance.OnLeftTrigger;
            @DPadRight.started += instance.OnDPadRight;
            @DPadRight.performed += instance.OnDPadRight;
            @DPadRight.canceled += instance.OnDPadRight;
            @DPadLeft.started += instance.OnDPadLeft;
            @DPadLeft.performed += instance.OnDPadLeft;
            @DPadLeft.canceled += instance.OnDPadLeft;
            @DPadUp.started += instance.OnDPadUp;
            @DPadUp.performed += instance.OnDPadUp;
            @DPadUp.canceled += instance.OnDPadUp;
        }

        private void UnregisterCallbacks(IGameplayActions instance)
        {
            @Movement.started -= instance.OnMovement;
            @Movement.performed -= instance.OnMovement;
            @Movement.canceled -= instance.OnMovement;
            @Aiming.started -= instance.OnAiming;
            @Aiming.performed -= instance.OnAiming;
            @Aiming.canceled -= instance.OnAiming;
            @NorthButtonDown.started -= instance.OnNorthButtonDown;
            @NorthButtonDown.performed -= instance.OnNorthButtonDown;
            @NorthButtonDown.canceled -= instance.OnNorthButtonDown;
            @Fire.started -= instance.OnFire;
            @Fire.performed -= instance.OnFire;
            @Fire.canceled -= instance.OnFire;
            @SouthButtonDown.started -= instance.OnSouthButtonDown;
            @SouthButtonDown.performed -= instance.OnSouthButtonDown;
            @SouthButtonDown.canceled -= instance.OnSouthButtonDown;
            @WestButtonDown.started -= instance.OnWestButtonDown;
            @WestButtonDown.performed -= instance.OnWestButtonDown;
            @WestButtonDown.canceled -= instance.OnWestButtonDown;
            @EastButtonDown.started -= instance.OnEastButtonDown;
            @EastButtonDown.performed -= instance.OnEastButtonDown;
            @EastButtonDown.canceled -= instance.OnEastButtonDown;
            @Start.started -= instance.OnStart;
            @Start.performed -= instance.OnStart;
            @Start.canceled -= instance.OnStart;
            @AltFire.started -= instance.OnAltFire;
            @AltFire.performed -= instance.OnAltFire;
            @AltFire.canceled -= instance.OnAltFire;
            @RightBumperDown.started -= instance.OnRightBumperDown;
            @RightBumperDown.performed -= instance.OnRightBumperDown;
            @RightBumperDown.canceled -= instance.OnRightBumperDown;
            @LeftBumperDown.started -= instance.OnLeftBumperDown;
            @LeftBumperDown.performed -= instance.OnLeftBumperDown;
            @LeftBumperDown.canceled -= instance.OnLeftBumperDown;
            @LeftTrigger.started -= instance.OnLeftTrigger;
            @LeftTrigger.performed -= instance.OnLeftTrigger;
            @LeftTrigger.canceled -= instance.OnLeftTrigger;
            @DPadRight.started -= instance.OnDPadRight;
            @DPadRight.performed -= instance.OnDPadRight;
            @DPadRight.canceled -= instance.OnDPadRight;
            @DPadLeft.started -= instance.OnDPadLeft;
            @DPadLeft.performed -= instance.OnDPadLeft;
            @DPadLeft.canceled -= instance.OnDPadLeft;
            @DPadUp.started -= instance.OnDPadUp;
            @DPadUp.performed -= instance.OnDPadUp;
            @DPadUp.canceled -= instance.OnDPadUp;
        }

        public void RemoveCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameplayActions instance)
        {
            foreach (var item in m_Wrapper.m_GameplayActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnAiming(InputAction.CallbackContext context);
        void OnNorthButtonDown(InputAction.CallbackContext context);
        void OnFire(InputAction.CallbackContext context);
        void OnSouthButtonDown(InputAction.CallbackContext context);
        void OnWestButtonDown(InputAction.CallbackContext context);
        void OnEastButtonDown(InputAction.CallbackContext context);
        void OnStart(InputAction.CallbackContext context);
        void OnAltFire(InputAction.CallbackContext context);
        void OnRightBumperDown(InputAction.CallbackContext context);
        void OnLeftBumperDown(InputAction.CallbackContext context);
        void OnLeftTrigger(InputAction.CallbackContext context);
        void OnDPadRight(InputAction.CallbackContext context);
        void OnDPadLeft(InputAction.CallbackContext context);
        void OnDPadUp(InputAction.CallbackContext context);
    }
}
