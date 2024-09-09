using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;
        

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        /* Bullet Objects & Settings */
        public GameObject bulletPrefab;
        public GameObject bulletPrefab2;
        public GameObject bulletPrefab3;
        public Transform firePoint;
        public Transform firePoint2;
        public Transform firePoint3;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        /* variables for skills cooldown */
        public float timer;
        public float fireRate;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            timer = 0f;
            fireRate = 1.5f;
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");

                if (Input.GetKeyDown(KeyCode.Z) && timer >= fireRate) 
                {
                    ShootBullet(bulletPrefab, firePoint);
                    timer = 0f;
                    Debug.Log("Skill 1");
                }

                if (Input.GetKeyDown(KeyCode.X) && timer >= fireRate) 
                {
                    ShootBullet(bulletPrefab2, firePoint2);
                    timer = 0f;
                    Debug.Log("Skill 2");
                }

                if (Input.GetKeyDown(KeyCode.C) && timer >= fireRate)
                {
                    ShootBullet(bulletPrefab3, firePoint3);
                    timer = 0f;
                    Debug.Log("Skill 3");
                }

                // 
                timer += Time.deltaTime;

                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        void ShootBullet(GameObject bulletToFire, Transform firePointPosition)
        {
            // Instantiate a bullet at the fire point position and rotation
            GameObject bullet = Instantiate(bulletToFire, firePointPosition.position, firePointPosition.rotation);

            // Access the BulletController script and set its speed
            //BulletController1 bulletController1 = bullet.GetComponent<BulletController1>();
            //BulletController2 bulletController2 = bullet.GetComponent<BulletController2>();
            //bulletController1.speed = bulletController2.speed = 30f;
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}