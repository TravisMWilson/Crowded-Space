using System.Linq;
using UnityEngine;

public class WarpTunnel : MonoBehaviour {
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float maxAngle = 5f;

    private Transform[] jointsBranch1;
    private Transform[] jointsBranch2;
    private Transform mainJoint;

    private Vector3[] jointRotations1;
    private Vector3[] jointRotations2;

    private void Start() {
        mainJoint = transform.Find("joint27");

        PrepareBranch("joint26", ref jointsBranch1, ref jointRotations1);
        PrepareBranch("joint28", ref jointsBranch2, ref jointRotations2);
    }

    private void Update() {
        RotateJoints(ref jointsBranch1, ref jointRotations1);
        RotateJoints(ref jointsBranch2, ref jointRotations2);
    }

    private void PrepareBranch(string jointName, ref Transform[] jointsBranch, ref Vector3[] jointRotations) {
        jointsBranch = mainJoint.Find(jointName).GetChild(0).GetComponentsInChildren<Transform>()
                 .Where(t => t.name.StartsWith("joint"))
                 .ToArray();

        jointRotations = new Vector3[jointsBranch.Length];

        for (int i = 0; i < jointsBranch.Length; i++) {
            jointRotations[i] = jointsBranch[i].localRotation.eulerAngles;
        }
    }

    private void RotateJoints(ref Transform[] jointsBranch, ref Vector3[] jointRotations) {
        for (int i = 0; i < jointsBranch.Length; i++) {
            jointRotations[i].y = Mathf.LerpAngle(jointRotations[i].y, Random.Range(-maxAngle, maxAngle), rotationSpeed * Time.deltaTime);
            jointRotations[i].z = Mathf.LerpAngle(jointRotations[i].z, Random.Range(-maxAngle, maxAngle), rotationSpeed * Time.deltaTime);

            jointRotations[i].y = Mathf.Clamp(jointRotations[i].y, -maxAngle, maxAngle);
            jointRotations[i].z = Mathf.Clamp(jointRotations[i].z, -maxAngle, maxAngle);

            float addY = jointRotations[i].y * rotationSpeed * Time.deltaTime;
            float addZ = jointRotations[i].z * rotationSpeed * Time.deltaTime;

            jointsBranch[i].Rotate(0, addY, addZ);
        }
    }
}
