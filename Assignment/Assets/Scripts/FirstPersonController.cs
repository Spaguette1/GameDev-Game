using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



#pragma warning disable 618, 649
namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;



        //aditions

        public GameObject bullet;
        public GameObject GameKillCounter;

        public HealthBar healthBar;
        public int maxHealth = 100;
        public int currentHealth;

        public GameObject pistolUI;
        public GameObject shotgunUI;
        public GameObject rifleUI;

        public GameObject deathMenuUI;

        private int killCount = 0;
        public Text killCountText;


        public float weaponDelay;
        private float currentDelay;
        public float shootPower;
        public int bullets;
        public float maxSpread = 0.1f;

        public enum weaponState {
            Pistol,
            Shotgun,
            Rifle
        }

        public weaponState currentWeaponState;




        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);

            currentWeaponState = weaponState.Pistol;

            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }

        // Update is called once per frame
        private void Update()
        {

            if (PauseMenu.GameIsPaused == true)
            {
                return;
            }


            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;

            if (currentDelay > 0) {
                currentDelay -= Time.deltaTime;
            }
            else {
                currentDelay = 0.0f;
            }



            /* Checking the kill count of the player and changing the weapon state based on the kill
            count. */
            if (GameKillCounter.GetComponent<GameKills>().killCount < 4) {
                currentWeaponState = weaponState.Pistol;

                pistolUI.SetActive(true);
                shotgunUI.SetActive(false);
                rifleUI.SetActive(false);
            }
            else if (GameKillCounter.GetComponent<GameKills>().killCount < 10) {
                currentWeaponState = weaponState.Shotgun;

                pistolUI.SetActive(false);
                shotgunUI.SetActive(true);
                rifleUI.SetActive(false);
            }
            else {
                currentWeaponState = weaponState.Rifle;

                pistolUI.SetActive(false);
                shotgunUI.SetActive(false);
                rifleUI.SetActive(true);
            }
            
            
            /* A switch statement that is changing the weaponDelay, shootPower, and bullets variables
            based on the currentWeaponState. */
            switch (currentWeaponState) {

                case weaponState.Pistol:
                    weaponDelay = 1.5f;
                    shootPower = 2500f;
                    bullets = 1;
                    break;
                case weaponState.Shotgun:
                    weaponDelay = 2.5f;
                    shootPower = 1000f;
                    bullets = 9;
                    break;
                case weaponState.Rifle:
                    weaponDelay = 0.15f;
                    shootPower = 4000f;
                    bullets = 1;
                    break;

            }

            /* Spawning a bullet in front of the player and adding force to it. */
            if (Input.GetButton("Fire1") && currentDelay == 0) {
                
                Vector3 playerPos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
                Transform cam = Camera.main.transform;

                Vector3 direct = cam.TransformDirection(Vector3.forward);
                float spawnDistance = 2;
                
                Vector3 spawnPos = playerPos + direct*spawnDistance;

                if (currentWeaponState == weaponState.Shotgun) {
                    var i=0;
                    while (i < bullets)
                    {
                        Vector3 spreadDirect = direct + new Vector3(Random.Range(-maxSpread, maxSpread), Random.Range(-maxSpread, maxSpread), Random.Range(-maxSpread, maxSpread));
                        GameObject bulletInstance = Instantiate(bullet, spawnPos, transform.rotation);
                        bulletInstance.GetComponent<Rigidbody>().AddForce (spreadDirect * shootPower);
                        i++;
                    }
                }
                else {
                    GameObject bulletInstance = Instantiate(bullet, spawnPos, transform.rotation);
                    bulletInstance.GetComponent<Rigidbody>().AddForce (direct * shootPower);
                    Physics.IgnoreCollision(bulletInstance.GetComponent<Collider>(), GetComponent<Collider>(), true);
                }


                

                currentDelay = weaponDelay;
            }

            if (currentHealth <= 0)
            {
                PauseMenu.GameIsPaused = true;
                Time.timeScale = 0f;
                currentHealth = 100;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                deathMenuUI.SetActive(true);
            }

        }

        public void ApplyDamage(int damage) {
            currentHealth -= damage;
            healthBar.setHealth(currentHealth);
        }

        // Increment Score
	    public void UpdatekillCount(int addkillCount) {
		killCount = killCount + addkillCount;
		// update the GUI score here
		killCountText.text = "Kill Count:" + " " + killCount.ToString();
	    }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        
        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }

    
}
