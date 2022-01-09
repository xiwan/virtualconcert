using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AvatarManager : Single<AvatarManager>
{

    public GameObject SpawnFromAvatar(GameObject instance, Avatar avatar)
    {
        Material mat01 = Resources.Load<Material>("Mats/LOW-POLY-COLORS-2");
        Material mat02 = Resources.Load<Material>("Mats/ObjectOutline");

        Material[] mats = new Material[2];
        mats[0] = mat01;
        mats[1] = mat02;

        var mainRigGeometry = instance.transform.Find("Geometry");
        GameObject returnInstance = null;
        for (int j = 0; j < mainRigGeometry.transform.childCount; j++)
        {
            var ava = mainRigGeometry.transform.GetChild(j);
            ava.gameObject.SetActive(false);
            if (ava.gameObject.name.Equals(avatar.aname))
            {
                ava.gameObject.SetActive(true);
                ava.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials = mats;
                //Debug.Log("AnimationControllers/" + avatar.animatorController);
                var _overrideController = Resources.Load<AnimatorOverrideController>("AnimationControllers/" + avatar.animatorController);
                instance.gameObject.GetComponent<Animator>().runtimeAnimatorController = _overrideController;
                instance.name = avatar.aname + "_" + avatar.id + "_" + instance.GetInstanceID();
                returnInstance = instance;
            }
        }
        return returnInstance;
    }

    public GameObject SpawnPlayerFromAvatar(GameObject instance, Avatar avatar, Transform parent = null)
    {
        GameObject _playerObject = GameObject.Instantiate(instance, Vector3.zero, Quaternion.identity, parent);
        var spawnedInstance = AvatarManager.Instance.SpawnFromAvatar(_playerObject, avatar);
        // enable player avatar sync avatar data
        var playerAvatar = spawnedInstance.GetComponent<VirtualAvatarPlayer>();
        playerAvatar.avatar = avatar;
        playerAvatar.SetCurrentState();

        return spawnedInstance;
    }

    public void AddMatsForGeometry(GameObject instance)
    {
        Material mat01 = Resources.Load<Material>("Mats/LOW-POLY-COLORS-2");
        Material mat02 = Resources.Load<Material>("Mats/ObjectOutline");

        Material[] mats = new Material[2];
        mats[0] = mat01;
        mats[1] = mat02;

        var mainRigGeometry = instance.transform.Find("Geometry");
        for (int j = 0; j < mainRigGeometry.transform.childCount; j++)
        {
            var ava = mainRigGeometry.transform.GetChild(j);
            ava.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials = mats;
        }
    }


   

}
