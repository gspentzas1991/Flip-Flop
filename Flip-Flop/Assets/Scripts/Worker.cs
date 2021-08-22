using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the variables for the worker and gear animators
/// </summary>
public class Worker : MonoBehaviour
{
    Animator animator;
    [SerializeField] Animator gearAnimator;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Sets the worker and gear animation booleans to true
    /// </summary>
    public void StartWorking()
    {
        animator.SetBool("isWorking",true);
        gearAnimator.SetBool("isSpinning",true);
        audioSource.Play();
    }

    /// <summary>
    /// Sets the worker and gear animation booleans to false
    /// </summary>
    public void StopWorking()
    {
        animator.SetBool("isWorking", false);
        gearAnimator.SetBool("isSpinning", false);
        audioSource.Stop();
    }
}
