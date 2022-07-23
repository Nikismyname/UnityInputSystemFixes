using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	using System.Collections.Generic;
	using System.Linq;

	public class StarterAssetsInputs : MonoBehaviour
	{
		private const string ANY_PATH = "<Keyboard>/anyKey";
		private const string MOD_PREFIX = "modifier";
		
		[SerializeField] private InputActionAsset mapAsset;
		
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		private InputActionMap map;

		private List<InputActionBundle> allBundles;
		private InputActionBundle onUse1;
		private InputActionBundle onUse2;
		private InputActionBundle onUse3;
			
		private void Start()
		{
			this.map = this.mapAsset.actionMaps[0];
			this.onUse1 = this.ParseInput("Use1");
			this.onUse2 = this.ParseInput("Use2");
			this.onUse3 = this.ParseInput("Use3");
			this.allBundles = new List<InputActionBundle>() { this.onUse1, this.onUse2, this.onUse3 };
		}

		public bool Use1
		{
			get => this.onUse1.Triggered && !this.CheckForCollision(this.onUse1);
			set => this.onUse1.Triggered = value;
		}

		public bool Use2
		{
			get => this.onUse2.Triggered && !this.CheckForCollision(this.onUse2);
			set => this.onUse2.Triggered = value;
		}
        
		public bool Use3
		{
			get => this.onUse3.Triggered && !this.CheckForCollision(this.onUse3);
			set => this.onUse3.Triggered = value;
		}
		
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		// no mods
		public void OnUse1(InputValue value) => this.Use1 = value.isPressed;
		
		// one mod - shift
		public void OnUse2(InputValue value) => this.Use2 = value.isPressed;
		
		// two mods - shift + ctrl
		public void OnUse3(InputValue value) => this.Use3 = value.isPressed;
#endif

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
		
		// counts the non empty modifiers
		private InputActionBundle ParseInput(string key)
		{
			InputAction onUseInput = this.map.actions.Single(x => x.name == key);
			
			int count = 0;
			foreach (InputBinding item in onUseInput.bindings)
				if (item.name.StartsWith(MOD_PREFIX) && item.path != ANY_PATH) 
					count++;

			InputActionBundle result = new()
			{
				NonAnyMods = count,
				Triggered = false,
			};
            
			return result;
		}
		
		// checks that there is not another action (currently triggered) with more mods that the one being evaluated.
		private bool CheckForCollision(InputActionBundle bundleIn)
		{
			foreach (InputActionBundle otherBundle in this.allBundles)
			{
				if (otherBundle == bundleIn) continue;
				if (otherBundle.Triggered == false) continue;
                
				if (otherBundle.NonAnyMods > bundleIn.NonAnyMods)
				{
					return true;
				}
			}

			return false;
		}
	}
	
	public class InputActionBundle
	{
		public int  NonAnyMods { get; set; }

		public bool Triggered { get; set; }
	}
}