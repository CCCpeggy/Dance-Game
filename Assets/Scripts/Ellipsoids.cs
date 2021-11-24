using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ellipsoids : MonoBehaviour
{
    public GameObject obj1, obj2;

    public static Ellipsoids CreateEllipsoid(GameObject obj1, GameObject obj2) {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.parent = obj1.transform;
        Ellipsoids ellipsoid = obj.AddComponent<Ellipsoids>();
        ellipsoid.SetObject(obj1, obj2);
        return ellipsoid;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (obj1 != null && obj2 != null) {
            setEllipsoid(obj1.transform.position, obj2.transform.position);
            gameObject.SetActive(true);
        }
        else {
            gameObject.SetActive(false);
        }
    }

    public void setEllipsoid(Vector3 pos1, Vector3 pos2)
    {
        Vector3 targetDir = pos1 - pos2;
        Vector3 targetPos = targetDir / 2 + pos2;

        float semiMajor = Vector3.Distance(pos1, pos2);
        float semiMinor = semiMajor / 5;

        Vector3 scale = new Vector3(semiMinor, semiMajor, semiMinor);

        transform.position = targetPos;

        transform.localScale = scale;

        targetDir.Normalize();
        var rotation = Quaternion.FromToRotation(Vector3.up, targetDir);
        transform.rotation = rotation;
    }

    public void SetObject(GameObject obj1, GameObject obj2) {
        this.obj1 = obj1;
        this.obj2 = obj2;
    }
}
