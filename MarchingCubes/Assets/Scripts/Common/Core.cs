namespace Assets.Common
{
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.UI;

    public static class Core
    {
        public static void DrawCross(this Vector3 v, Color? color = null, float? duration = null, float halfLength = 1f)
        {
            color = color == null ? Color.black : color.Value;
            duration = duration == null ? Time.deltaTime : duration.Value;

            Debug.DrawLine(v + new Vector3(halfLength, 0, 0), v + new Vector3(-halfLength, 0, 0), color.Value, duration.Value);
            Debug.DrawLine(v + new Vector3(0, halfLength, 0), v + new Vector3(0, -halfLength, 0), color.Value, duration.Value);
            Debug.DrawLine(v + new Vector3(0, 0, halfLength), v + new Vector3(0, 0, -halfLength), color.Value, duration.Value);
        }

        const string DefaultShader = "Unlit/Color";

        static Material material;

        public static GameObject DrawLine(Vector3 from, Vector3 to, Color color, float thickness, Material mat = null,Transform parent = null)
        {
            GameObject localParent = new GameObject("LineParent");
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            line.name = "line";
            localParent.transform.position = new Vector3(0, -1, 0);
            line.transform.parent = localParent.transform;

            localParent.transform.position = from;
            localParent.transform.LookAt(to);
            localParent.transform.Rotate(new Vector3(1, 0, 0), 90);
            if (mat != null)
            {
                line.GetComponent<Renderer>().material = mat;
            }
            line.GetComponent<Renderer>().material.color = color;
            localParent.SetScale(new Vector3(thickness, (from - to).magnitude / 2, thickness));
            if (parent != null)
            {
                localParent.transform.SetParent(parent);
            }
            return line;
        }

        public static void DrawBox(float halfSize, float thickness, Vector3 center, Transform grandParent)
        {
            Transform parent = new GameObject("CubeParent").transform;
            parent.SetParent(grandParent);

            DrawLine(center + new Vector3(-halfSize, -halfSize, -halfSize), center + new Vector3(halfSize, -halfSize, -halfSize), Color.black, thickness, parent: parent);
            DrawLine(center + new Vector3(-halfSize, -halfSize, -halfSize), center + new Vector3(-halfSize, halfSize, -halfSize), Color.black, thickness, parent: parent);
            DrawLine(center + new Vector3(-halfSize, -halfSize, -halfSize), center + new Vector3(-halfSize, -halfSize, halfSize), Color.black, thickness, parent: parent);
            DrawLine(center + new Vector3(halfSize, halfSize, halfSize), center + new Vector3(-halfSize, halfSize, halfSize), Color.black, thickness, parent: parent);
            DrawLine(center + new Vector3(halfSize, halfSize, halfSize), center + new Vector3(halfSize, -halfSize, halfSize), Color.black, thickness, parent: parent);
            DrawLine(center + new Vector3(halfSize, halfSize, halfSize), center + new Vector3(halfSize, halfSize, -halfSize), Color.black, thickness, parent: parent);
            DrawLine(center + new Vector3(-halfSize, halfSize, halfSize), center + new Vector3(-halfSize, halfSize, -halfSize), Color.black, thickness, parent: parent);
            DrawLine(center + new Vector3(halfSize, halfSize, -halfSize), center + new Vector3(halfSize, -halfSize, -halfSize), Color.black, thickness, parent: parent);
            DrawLine(center + new Vector3(-halfSize, -halfSize, halfSize), center + new Vector3(halfSize, -halfSize, halfSize), Color.black, thickness, parent: parent);
            DrawLine(center + new Vector3(halfSize, -halfSize, halfSize), center + new Vector3(halfSize, -halfSize, -halfSize), Color.black, thickness, parent: parent);
            DrawLine(center + new Vector3(-halfSize, -halfSize, halfSize), center + new Vector3(-halfSize, halfSize, halfSize), Color.black, thickness, parent: parent);
            DrawLine(center + new Vector3(-halfSize, halfSize, -halfSize), center + new Vector3(halfSize, halfSize, -halfSize), Color.black, thickness, parent: parent);
        }

        public static void SetMat(this GameObject go, Material mat = null)
        {
            if (mat == null && material == null)
            {
                material = Resources.Load("Materials/wire", typeof(Material)) as Material;
            }

            if (mat == null)
            {
                go.GetComponent<Renderer>().material = material;
            }
            else
            {
                go.GetComponent<Renderer>().material = mat;
            }
        }

        public static void SetShader(this GameObject go, string shader = DefaultShader)
        {
            go.GetComponent<Renderer>().material.shader = Shader.Find(shader);
        }

        public static void SetColor(this GameObject go, Color col)
        {
            go.GetComponent<Renderer>().material.color = col;
        }

        public static void SetPos(this GameObject go, Vector3 pos)
        {
            go.transform.position = pos;
        }

        public static void SetScale(this GameObject go, Vector3 scale)
        {
            go.transform.localScale = scale;
        }

        public static void SetScale(this GameObject go, float scale)
        {
            go.transform.localScale = new Vector3(scale, scale, scale);
        }

        public static float GetScale(this GameObject go)
        {
            Vector3 ls = go.transform.localScale;
            if (ls.x != ls.y || ls.y != ls.z)
            {
                Debug.LogWarning("The scale your are trying to access by using go.GetScale is not uniform!");
            }

            return go.transform.localScale.x;
        }

        public static Vector3 GetPos(this GameObject go)
        {
            return go.transform.position;
        }

        public static void OffsetPos(this GameObject go, Vector3 pos)
        {
            go.transform.position += pos;
        }

        ///RectTransform
        public static void SetRTSize(this GameObject go, float x, float y)
        {
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
        }
        public static void SetRTSize(this GameObject go, Vector2 vec)
        {
            go.GetComponent<RectTransform>().sizeDelta = vec;
        }
        public static void SetRTPos(this GameObject go, float x, float y)
        {
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }
        public static void SetRTPos(this GameObject go, Vector2 vec)
        {
            go.GetComponent<RectTransform>().anchoredPosition = vec;
        }
        ///...

        ///TODO: Clean this up!
        public static void SetImageColor(this GameObject go, Color col)
        {
            bool usedSecondImage = false;
            bool foundImage = false;

            Image image = go.GetComponent<Image>();

            if (image == null)
            {
                usedSecondImage = true;
            }
            else
            {
                foundImage = true;
            }

            if (foundImage == false)
            {
                image = go.GetComponentInChildren<Image>();
            }

            if (image != null)
            {
                foundImage = true;
            }

            if (foundImage)
            {
                image.color = col;

                if (usedSecondImage)
                {
                    Debug.LogWarning("Image In Children Used!");
                }
            }
        }

        public static void SetImageMaterialColor(this GameObject go, Color col)
        {
            go.GetComponent<Image>().material.color = col;
        }

        public static void RotateAroundUnitSphere(this GameObject go, Vector3 axis, float angle)
        {
            Transform parent = go.transform.parent;
            GameObject rotator = new GameObject("Rotator");
            go.transform.parent = rotator.transform;
            rotator.transform.Rotate(axis, angle);
            go.transform.parent = parent;
            GameObject.Destroy(rotator);
        }

        public static void MyRotate(this GameObject go, Vector3 axis, float angle)
        {
            GameObject rotator = new GameObject("Rotator");
            go.transform.parent = rotator.transform;
            rotator.transform.Rotate(axis, angle);
            go.transform.parent = null;
            GameObject.Destroy(rotator);
        }
    }

    public static class Int3Extensions
    {
        public static Vector3 ToVec3(this int3 i3)
        {
            return new Vector3(i3.x, i3.y, i3.z);
        }
    }
}
