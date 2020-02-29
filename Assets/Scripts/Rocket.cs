using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Rocket : MonoBehaviour
{
    [SerializeField]float rcsThrust = 100f;
    [SerializeField] float mainThrust = 20f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip success;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathSoundParticles;
    [SerializeField] ParticleSystem successParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool isTransitioning = false;

    bool collisionsDisabled = false;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if(Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }        
    }

    private void RespondToDebugKeys()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //print(SceneManager.GetActiveScene().buildIndex);
        //print(SceneManager.sceneCount);
        if(isTransitioning || collisionsDisabled){ return; }
        switch(collision.gameObject.tag)
        {
            case "Friendly":
                print("Ok");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                print("Dead");
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        print("Dead");
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathSoundParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void StartSuccessSequence()
    {
        print("Finish");
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
        isTransitioning = false;
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        //print(SceneManager.sceneCountInBuildSettings);
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        //float forceThisFrame = mainThrust * Time.deltaTime; Not applying this as physics engine is handling it.
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        

        float rotationThisFrame = rcsThrust * Time.deltaTime; //manual handling, hence delta time needed to control gameplay for different fps

        if (Input.GetKey(KeyCode.A))
        {
            rigidBody.freezeRotation = true; //Take manual control of rotation, physics engine causes too much spin due to gravity
            transform.Rotate(Vector3.forward*rotationThisFrame);
            rigidBody.freezeRotation = false; //Resume physics controls
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rigidBody.freezeRotation = true; //Take manual control of rotation, physics engine causes too much spin due to gravity
            transform.Rotate(-Vector3.forward*rotationThisFrame);
            rigidBody.freezeRotation = false; //Resume physics controls
        }
        
    }
  
}
