using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

// Скрипт для перемешений персонажа в глубокой воде
public class AK_MPC_Collision : MonoBehaviour
{
    #region Variables
    [Header("Wwise")]
    [SerializeField] private AK.Wwise.Event Collision;       // запуск звука движения тела в воде
    [SerializeField] private float time;                    // время через которое повторно запускать звук движения тела в воде
    [SerializeField] private bool PlaySound;                 // булевая переменная для запуска звуков
    [Space]
    [SerializeField] private string Surface;                         // тип поверхности
  
    
    [Header("Character")]
    [SerializeField] private vThirdPersonMotor Pirate;         // контроллер
    [SerializeField] private bool isMoving;                   // проверка перемещается ли персонаж
    [Space]
    [Header("Debug")]
    [SerializeField] private bool Debug_Enabled = false;
    
    #endregion

    void Start()
    {
        Debug_Enabled = false;
        PlaySound = true;
        Pirate = this.gameObject.GetComponentInParent<vThirdPersonMotor>();
        //    Pirate_Anim = GetComponent<Animator>();
        //    Pirate_rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        if (Pirate.input != Vector3.zero || Pirate.customAction)
        {
            isMoving = true;
         }
        else
        {
            isMoving = false;
            if (Debug_Enabled = true) { Debug.Log("NotMoving"); }
        }
        //   if (moveVector != Vector3.zero)
        //   {  isMoving = true; }
        //   else { isMoving = false; }
     
    }



    void OnTriggerEnter(Collider Col) // при входе в коллайдер проверять поверхность, если вода запускать звук
    {
        if (Col.CompareTag("Water"))
        {
            Surface = "Water";
            AkSoundEngine.SetSwitch("Terrain", Surface, this.gameObject);
            Collision.Post(this.gameObject);
        }
      
        else { return; }
     }


   

    void OnTriggerStay(Collider Col) // запуск звуков, когда мы находимся внутри коллайдера
    {
        if (Col.CompareTag("Water"))

        {
            Surface = "Water";
          

            if (isMoving == true && PlaySound == true) // если персонаж ходит и звука нет, то запустить звук.
            {
                PlaySound = false;
                Invoke("StartSound", time); // функция задержки вызова звука
                AkSoundEngine.SetSwitch("Terrain", Surface, this.gameObject);
                Collision.Post(this.gameObject);
            }
            else { return; }
        }

   
    }
    void StartSound()
    {
             PlaySound = true;
    }
    }
    

    
