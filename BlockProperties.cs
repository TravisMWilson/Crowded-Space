using UnityEngine;

public class BlockProperties : MonoBehaviour {
    [SerializeField] private bool allowTop = true;
    [SerializeField] private bool allowBottom = true;
    [SerializeField] private bool allowLeft = true;
    [SerializeField] private bool allowRight = true;
    [SerializeField] private bool allowFront = true;
    [SerializeField] private bool allowBack = true;
    [SerializeField] private bool disallowAllSides = false;

    [SerializeField] private bool multiBlock = false;
    [SerializeField] private bool canRotate = true;

    public bool IsSideAllowed(string side) {
        return side switch {
            "top" => allowTop,
            "bottom" => allowBottom,
            "left" => allowLeft,
            "right" => allowRight,
            "front" => allowFront,
            "back" => allowBack,
            "any" => disallowAllSides,
            _ => false,
        };
    }

    public bool IsMultiBlock() => multiBlock;
    public bool CanRotate() => canRotate;
}