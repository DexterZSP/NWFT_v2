/**
 * Copyright (c) code written by Germán López Gutiérrez
 */

using UnityEngine;

/// <summary>
/// Controlador de animaciones del personaje.
/// </summary>
public class SC_AnimationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SC_newCharacterMovement _characterMovement;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _animator;

    void Update()
    {
        _animator.SetFloat("CharacterSpeed", _characterController.velocity.magnitude);
        _animator.SetBool("IsGrounded", _characterController.isGrounded);
        _animator.SetBool("isSliding", _characterMovement.isSliding);
    }
}
