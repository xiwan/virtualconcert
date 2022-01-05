using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AvatarManager
{
    public static GameObject SpawnFromAvatar(GameObject instance, Avatar avatar)
    {
        var mainRigGeometry = instance.transform.Find("Geometry");
        GameObject returnInstance = null;
        for (int j = 0; j < mainRigGeometry.transform.childCount; j++)
        {
            var ava = mainRigGeometry.transform.GetChild(j);
            ava.gameObject.SetActive(false);
            if (ava.gameObject.name.Equals(avatar.aname))
            {
                ava.gameObject.SetActive(true);
                var _overrideController = Resources.Load<AnimatorOverrideController>("AnimationControllers/" + avatar.animatorController);
                instance.gameObject.GetComponent<Animator>().runtimeAnimatorController = _overrideController;
                instance.name = avatar.aname + "_" + avatar.id + "_" + instance.GetInstanceID();
                returnInstance = instance;
            }
        }
        return returnInstance;
    }

}
