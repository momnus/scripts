using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


// Скрипт для вызова рикошетов. Переделан из скрипта для вызовов разрушений из пака Third-Person Shooter от компании Invector 
namespace Invector.vShooter
{
    [vClassHeader("Ricochet", openClose = false)]
    public class AKRicochet : vMonoBehaviour
    {
        [SerializeField] private layermask;
        [SerializeField] private AK.Wwise.Event Ricochet;
                
                
        public virtual void CreateSound(RaycastHit hitInfo)
        {
            CreateSound(hitInfo.collider.gameObject, hitInfo.point, hitInfo.normal);
        
        }

        public virtual void CreateSound(GameObject target, Vector3 position, Vector3 normal)
        {
            if (layermask == (layermask | (1 << target.layer)))
            {
                var Material = target.tag;
                //if (Material != null)
                //{
                    RaycastHit hit;
                    if (Physics.SphereCast(new Ray(position + (normal * 0.1f), -normal), 0.0001f, out hit, 1f, layermask))
                    {
                        if (hit.collider.gameObject == target)
                        {
                            AkSoundEngine.SetSwitch("Ricochet", Material, target.gameObject);
                            Ricochet.Post(target.gameObject);
                            print(Material);
                         }
                        }
                  //  }
                }
            }
        }

       

           
        }
    

