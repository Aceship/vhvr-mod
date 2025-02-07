using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace ValheimVRMod.Utilities
{
    public static class WeaponUtils
    {
        
        private static readonly Dictionary<string, WeaponColData> colliders = new Dictionary<string, WeaponColData>
        {
            {           // AXES
                "AxeBronze", WeaponColData.create(
                    0.0105f,  0, -0.03929f,
                    0,  -9, 0,
                    0.01f,  0.001f, 0.021f
                )}, {
                "AxeIron", WeaponColData.create(
                    0.0105f,  0, -0.03929f,
                    0,  -9, 0,
                    0.01f,  0.001f, 0.021f
                )}, {
                "AxeBlackMetal", WeaponColData.create(
                    0.058f,  0.714f, 0,
                    0,  0, 0,
                    0.1f,  0.21f, 0.01f
                )}, {
                "AxeFlint", WeaponColData.create(
                    -0.1339f,  2.2458f, 0,
                    0,  0, 0,
                    0.9623004f,  0.36668f, 0.2870208f
                )}, {
                "AxeStone", WeaponColData.create(
                    -0.608f,  2.267f, 0,
                    0,  0, 0,
                    1.000489f,  0.177166f, 0.2626824f
                )}, {   // PICKAXES
                "PickaxeStone", WeaponColData.create(
                    -0.608f,  2.267f, 0,
                    0,  0, 0,
                    1.000489f,  0.177166f, 0.2626824f
                )}, {
                "PickaxeIron", WeaponColData.create(
                0,  1.9189f, 0,
                0,  0, 0,
                2.865605f,  0.1f, 0.1f
                )}, {
                "PickaxeBronze", WeaponColData.create(
                    -0.711f,  2.219f, 0,
                    0,  0, 2.903f,
                    1.192384f,  0.1884746f, 0.1568103f
                )}, {
                "PickaxeAntler", WeaponColData.create(
                    -0.722f,  2.256f, 0,
                    0,  0, 0,
                    1.192384f,  0.1884746f, 0.1568103f
                )}, {   // TOOL
                "Hammer", WeaponColData.create(
                    0,  0.956f, 0,
                    0,  0, 0,
                    0.9030886f,  0.3803992f, 0.3803992f
                )}, {   // WEAPONS
                "Torch", WeaponColData.create(
                    0,  0.385f, 0,
                    0,  0, 0,
                    0.1f,  0.3f, 0.1f
                )}, {
                "Club", WeaponColData.create(
                    -0.013f,  -0.022f, 0.603f,
                    0,  -5.241f, 45,
                    0.1044289f,  0.09270307f, 0.5340722f
                )}, {
                "SwordBronze", WeaponColData.create(
                    0,  1.523f, 0,
                    0,  0, 0,
                    0.1934575f,  2.34697f, 0.05382534f
                )}, {
                "SwordIron", WeaponColData.create(
                    0,  2.102f, 0,
                    0,  0, 0,
                    0.1934575f,  3.425369f, 0.05382534f
                )}, {
                "SwordIronFire", WeaponColData.create(
                    0,  2.102f, 0,
                    0,  0, 0,
                    0.1934575f,  3.425369f, 0.05382534f
                )}, {
                "SwordCheat", WeaponColData.create(
                    0,  2.102f, 0,
                    0,  0, 0,
                    0.1934575f,  3.425369f, 0.05382534f
                )}, {
                "SwordSilver", WeaponColData.create(
                    0,  2.158f, 0,
                    0,  0, 0,
                    0.1101757f,  3.519603f, 0.05382534f
                )}, {
                "SwordBlackmetal", WeaponColData.create(
                    0,  0.842f, 0,
                    0,  0, 0,
                    0.09493963f,  1.120129f, 0.01100477f
                )}, {
                "SpearWolfFang", WeaponColData.create(
                    0,  -9.06f, 0,
                    0,  0, 0,
                    0.3996784f,  1.445521f, 0.4638378f
                )}, {
                "SpearFlint", WeaponColData.create(
                    0,  0, 1.238f,
                    0,  0, 0,
                    0.08946446f,  0.05617056f, 0.1811694f
                )}, {
                // SpearChitin currently has no melee attack, thus collider throws error.
                // Still keeping this commented, in case it changes some day
                //
                // "SpearChitin", WeaponColData.create( 
                //     0,  1.12f, 0.008f,
                //     0,  0, 0,
                //     0.01591795f,  0.8536723f, 0.09076092f
                // )}, {
                "SpearElderbark", WeaponColData.create(
                    0,  2.0915f, 0,
                    0,  0, 0,
                    0.07673188f,  0.3863854f, 0.02554126f
                )}, {
                "SpearBronze", WeaponColData.create(
                    0,  2.182f, 0,
                    0,  0, 0,
                    0.07756059f,  0.425059f, 0.02554126f
                )}, {
                "SledgeStagbreaker", WeaponColData.create(
                    0,  2.064f, 0,
                    0,  0, 0,
                    0.5530369f,  0.5530369f, 1.284601f
                )}, {
                "SledgeIron", WeaponColData.create(
                    0,  1.194f, 0,
                    0,  0, 0,
                    0.7411623f,  0.3304068f, 0.2240506f
                )}, {
                "MaceBronze", WeaponColData.create(
                    0,  1.946f, 0,
                    0,  45, 0,
                    0.4857313f,  0.5671427f, 0.4857313f
                )}, {
                "MaceSilver", WeaponColData.create(
                    0,  0.53f, 0,
                    0,  0, 0,
                    0.4254949f,  0.1803948f, 0.133568f
                )}, {
                "MaceIron", WeaponColData.create(
                    0,  2.548f, 0,
                    0,  45f, 0,
                    0.4857313f,  0.5671427f, 0.4857313f
                )}, {
                "MaceNeedle", WeaponColData.create(
                    0,  1.063f, 0,
                    0,  0, 0,
                    0.4f,  0.4f, 0.4f
                )}, {
                "AtgeirBlackmetal", WeaponColData.create(
                    0.101f,  1.361f, 0,
                    0,  0, 0,
                    0.0777498f,  2.7300777f, 0.01543969f
                )}, {
                "AtgeirBronze", WeaponColData.create(
                    0,  -0.111f, -1.529f,
                    7.082f,  0, 0,
                    0.02239758f,  0.1004803f, 1.9769629f
                )}, {
                "AtgeirIron", WeaponColData.create(
                    0,  -0.111f, -1.529f,
                    7.082f,  0, 0,
                    0.02239758f,  0.1004803f, 1.9769629f
                )}, {
                "Battleaxe", WeaponColData.create(
                    -0.679f,  3.496f, -0.003f,
                    0,  0, 12.943f,
                    0.44454f,  1.314052f, 0.086121f
                )}, {
                "BattleaxeCrystal", WeaponColData.create(
                    0.15f,  1.45f, 0,
                    0,  0, 0,
                    0.24f,  0.63f, 0.056f
                )}, {
                "KnifeCopper", WeaponColData.create(
                    -0.042f,  0.645f, 0,
                    0,  0, 5.632f,
                    0.1822819f,  0.6237586f, 0.03287503f
                )}, {
                "KnifeFlint", WeaponColData.create(
                    -0.142f,  0.602f, 0,
                    0,  0, 28.779f,
                    0.2098247f,  0.6237586f, 0.03287503f
                )}, {
                "KnifeBlackMetal", WeaponColData.create(
                    -0.2284f,  0.3629f, -0.0032f,
                    0,  0, -0.445f,
                    0.06340086f,  0.3513995f, 0.0144201f
                )}, {
                "KnifeChitin", WeaponColData.create(
                    0,  0.208f, 0.027f,
                    15.689f,  0, 0,
                    0.0150678f,  0.3227402f, 0.086121f
                )}, {
                "KnifeButcher", WeaponColData.create(
                    -0.3549f,  0.968f, 0.472f,
                    15.689f,  20.2f, -1.5f,
                    0.4f,  0.01f, 0.06f
                )}, {
                "KnifeSilver", WeaponColData.create(
                    0,  0.41f, 0,
                    0, 0, 0,
                    0.04f,  0.45f, 0.01f
                )}, {
                "Tankard", WeaponColData.create(
                    0,  0.28f, 0,
                    0, 0, 0,
                    0.18f,  0.03f, 0.18f
                )}, {
                "Tankard_dvergr", WeaponColData.create(
                    0,  0.31f, 0,
                    0, 0, 0,
                    0.18f,  0.03f, 0.18f
                )}, {
                "TankardAnniversary", WeaponColData.create(
                    0,  0.48f, 0,
                    0, 0, 0,
                    0.18f,  0.03f, 0.18f
                )}, {
                "TankardOdin", WeaponColData.create(
                    0,  -0.01f, 0.115f,
                    0, 0, 0,
                    0.025f, 0.025f, 0.005f
                )}, {
                "AtgeirHimminAfl", WeaponColData.create(
                    0,  1.419f, -0.002f,
                    0, 0, 0,
                    0.40725f,  2.6230943f, 0.03f
                )}, {
                "AxeJotunBane", WeaponColData.create(
                    -0.0048f,  0.6406f, 0.001f,
                    0, 0, 0,
                    0.3860153f,  0.3521031f, 0.02278163f
                )}, {
                "PickaxeBlackMetal", WeaponColData.create(
                    0.048f,  0.857f, 0.001f,
                    0, 0, -1.849f,
                    0.6818599f,  0.06523325f, 0.02278163f
                )}, {
                "SledgeDemolisher", WeaponColData.create(
                    0,  1.2215f, -0.0019f,
                    0, 0, 0,
                    0.6946211f,  0.3374455f, 0.3453262f
                )}, {
                "SpearCarapace", WeaponColData.create(
                    0,  2.0915f, 0,
                    0, 0, 0,
                    0.07673188f,  0.6555415f, 0.02554126f
                )}, {
                "SwordMistwalker", WeaponColData.create(
                    0,  0.842f, 0,
                    0,  0, 0,
                    0.09493963f,  1.120129f, 0.01100477f
                )}, {
                "THSwordKrom", WeaponColData.create(
                    0,  1.194f, 0,
                    0,  0, 0,
                    0.09493963f,  1.387952f, 0.01100477f
                )}
        };

        private static readonly Dictionary<EquipType, WeaponColData> compatibilityColliders = new Dictionary<EquipType, WeaponColData>
        {
            {
                EquipType.Pickaxe, WeaponColData.create(
                    0,  1.9189f, 0,
                    0,  0, 0,
                    2.865605f,  0.1f, 0.1f
            )}
        };

        public static WeaponColData getForName(string name,ItemDrop.ItemData item)
        {

            if (colliders.ContainsKey(name)) {
                return colliders[name];
            }
            if (item != null && compatibilityColliders.ContainsKey(EquipScript.getEquippedItem(item)))
            {
                return compatibilityColliders[EquipScript.getEquippedItem(item)];
            }
            throw new InvalidEnumArgumentException();
        }

        // Estimates the direction that the weapon is pointing by identifying the dimension on which its mesh bounds is offset the farthest.
        // This estimation therefore assumes:
        //   1. The weapon pointing direction is parallel to the x, y, or z axis of the mesh; and
        //   2. The offset of tip of the weapon is larger than its lateral, dorsal, and ventral expanse.
        public static Vector3 EstimateWeaponPointingDirection(MeshFilter weaponMeshFilter, Vector3 handPosition)
        {
            Bounds weaponLocalBounds = weaponMeshFilter.sharedMesh.bounds;
            Vector3 centerOffset = weaponLocalBounds.center - weaponMeshFilter.transform.InverseTransformPoint(handPosition);
            float maxX = Mathf.Abs(centerOffset.x) + weaponLocalBounds.extents.x;
            float maxY = Mathf.Abs(centerOffset.y) + weaponLocalBounds.extents.y;
            float maxZ = Mathf.Abs(centerOffset.z) + weaponLocalBounds.extents.z;

            Vector3 longestDimension = weaponMeshFilter.transform.forward;
            if (maxX > maxY && maxX > maxZ)
            {
                longestDimension = weaponMeshFilter.transform.right;
            }
            else if (maxY > maxZ && maxY > maxX)
            {
                longestDimension = weaponMeshFilter.transform.up;
            }

            Vector3 roughDirection = weaponMeshFilter.transform.TransformPoint(weaponLocalBounds.center) - handPosition;
            return Vector3.Project(roughDirection, longestDimension).normalized;
        }

        // Whether the straight line (t -> p + t * v) intersects with the given bounds.
        public static bool LineIntersectsWithBounds(Bounds bounds, Vector3 p, Vector3 v)
        {
            // Center the bound and the line around the original bounds center to simplify calculation.
            Bounds centeredBounds = new Bounds(Vector3.zero, bounds.size);
            Vector3 p0 = p - bounds.center;

            // Where the line intersects with the right plane of the bounds.
            Vector3 rightIntersection = p0 + v * ((bounds.extents.x - p0.x) / v.x);
            if (centeredBounds.Contains(Vector3.ProjectOnPlane(rightIntersection, Vector3.right)))
            {
                // The line intersects with the right face of the bounds.
                return true;
            }

            // Where the line intersects with the left plane of the bounds.
            Vector3 leftIntersection = p0 + v * ((-bounds.extents.x - p0.x) / v.x);
            if (centeredBounds.Contains(Vector3.ProjectOnPlane(leftIntersection, Vector3.right)))
            {
                // The line intersects with the left face of the bounds.
                return true;
            }

            // Where the line intersects with the top plane of the bounds.
            Vector3 topIntersection = p0 + v * ((bounds.extents.y - p0.y) / v.y);
            if (centeredBounds.Contains(Vector3.ProjectOnPlane(topIntersection, Vector3.up)))
            {
                // The line intersects with the top face of the bounds.
                return true;
            }

            // Where the line intersects with the bottom plane of the bounds.
            Vector3 bottomIntersection = p0 + v * ((-bounds.extents.y - p0.y) / v.y);
            if (centeredBounds.Contains(Vector3.ProjectOnPlane(bottomIntersection, Vector3.up)))
            {
                // The line intersects with the bottom face of the bounds.
                return true;
            }

            // Where the line intersects with the front plane of the bounds.
            Vector3 frontIntersection = p0 + v * ((bounds.extents.z - p0.z) / v.z);
            if (centeredBounds.Contains(Vector3.ProjectOnPlane(frontIntersection, Vector3.forward)))
            {
                // The line intersects with the front face of the bounds.
                return true;
            }

            // Where the line intersects with the rear plane of the bounds.
            Vector3 rearIntersection = p0 + v * ((-bounds.extents.z - p0.z) / v.z);
            if (centeredBounds.Contains(Vector3.ProjectOnPlane(rearIntersection, Vector3.forward)))
            {
                // The line intersects with the rear face of the bounds.
                return true;
            }

            return false;
        }        
    }
}
