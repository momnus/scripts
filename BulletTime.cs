using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

// Скрипт запускающий состояние BulletTime (замедление времени). Замедляет время и меняет питч звуков в 2 раза, также в звуке меняется атмосыера, срабатывают разные события.
// В картинке меняются настройки пост-процессинга. 
public class BulletTime : MonoBehaviour
{

    #region Variables
    [Header("Wwise")]
    [SerializeField] private AK.Wwise.Event StartBT;    // Wwise-ивент начала BulletTime
    [SerializeField] private AK.Wwise.Event StopBT;     // Wwise-ивент окончания BulletTime
    [SerializeField] private string BT_RTPC;            // RTPC кривая BulletTime (меняет питч в 2 раза) (настраивается в самом Wwise)
    [Space]
    [Header("Time Parameters")]
    [SerializeField] private float minTimeScale = 0.5f;     // Значения шкалы времени во время BulletTime
    [SerializeField] private float transitionTime = 1.0f;   // Значения шкалы времени вобычном режиме
    [SerializeField] private bool isBulletTime = false;     // булева переменная для проверки состояния BulletTime
    [Space]
    [Header("Post-Processing & Light")]
    [SerializeField] private PostProcessVolume camera;      // Объект на котором висят настройки пост-процессинга (у меня это основная камера)
    [SerializeField] private PostProcessProfile MainProfile; // базовый профиль пост-процессинга
    [SerializeField] private PostProcessProfile BT_Profile; // профиль пост-процессинга для BulletTime
    [SerializeField] private float ProfileDelay = 2f; // задержка смены профиля пост-процессинга
    [SerializeField] private GameObject AddLight; // дополнительный источник света во время BulletTime
    
    
    private ChromaticAberration cromAb;
    private LensDistortion lens;
    private float timeMod = 2.0f;
    private float rangeMod = 10.5f;

    #endregion


    void Start()
    {
                // настройки для day-night контроллера ( в итоге вынес их отдельно в сам day-night controller)   

        //  dnc = gameObject.GetComponent<DayNightController>();
        //   dnc_normal = dnc.daySpeedMultiplier;
        //   dnc_BT = dnc_normal / 2;
    }

    void Update()
    {
     //   print(dnc.daySpeedMultiplier);
        AkSoundEngine.SetRTPCValue(BT_RTPC, Time.timeScale); // постоянная проверка значений RTPC 
      
        if (Input.GetButtonDown("Ability")) //запуск BulletTime по нажатию кнопки "Ability"
        {
            isBulletTime = !isBulletTime;

            if(isBulletTime)
            {
                StartBT.Post(this.gameObject);
                                
                camera.profile.TryGetSettings(out cromAb);
                camera.profile.TryGetSettings(out lens);
                
            }
            else
            {
                StopBT.Post(this.gameObject);
            }

            StopCoroutine("ToggleBulletTime"); // Запуск корутины
            StartCoroutine("ToggleBulletTime");
            StopCoroutine("ProfileChange"); 
            StartCoroutine("ProfileChange");
            //      Abberation();

        }
    }

    void Abberation()
    {
        //    if (isBulletTime)
        //   {
        cromAb.intensity.value = Mathf.Sin(Time.realtimeSinceStartup * timeMod) + rangeMod;
        lens.intensity.value = Mathf.Sin((Time.realtimeSinceStartup * timeMod) + rangeMod);
        //    }
        //     else { return; }
    }

    void OnGUI() // ГУИ где видно, что включился BulletTime
    {

        string TextGui = "BulletTime " + (isBulletTime ? "On " : "Off");

        if (isBulletTime)
        {
            GUI.Box(new Rect(Screen.width - 200, Screen.height - 70, 150, 30), TextGui);
        }
    }

        #region Coroutines
        IEnumerator ToggleBulletTime() // Корутина изменения времени
    {
        float start = Time.timeScale;
        float target = isBulletTime ? minTimeScale : 1.0f;
        float lastTick = Time.realtimeSinceStartup; 
        float t = 0.0f;
        while (t <= 1.0f)
        {
            t += (Time.realtimeSinceStartup - lastTick) * (1.0f / transitionTime);
            t += Time.deltaTime * (1.0f / transitionTime);
            lastTick = Time.realtimeSinceStartup;
            Time.timeScale = Mathf.Lerp(start, target, t); 
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }
    }
   
       
    IEnumerator ProfileChange() // задержка смены профиля, чтобы смена происходила одновременно со окончанием звуковой отбивки
    {
        yield return new WaitForSeconds(ProfileDelay);

        if (isBulletTime)
        {
            camera.profile = BT_Profile;
            AddLight.SetActive(true);
           
        }
        else
        {
            camera.profile = MainProfile;
            AddLight.SetActive(false);
        }
    }

    #endregion
}
