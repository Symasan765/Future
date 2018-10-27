using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllChangeMaterial : MonoBehaviour {

	float timeCnt;
	// Use this for initialization
	void Start () {
		changeShader(gameObject, "UnityChanToonShader/Toon_DoubleShadeWithFeather");
		timeCnt = 0.0f;
	}

	/// <summary>
	/// targetGameObject以下の子オブジェクト群のシェーダーをShaderName_toに変更する。ShaderName_fromが指定されていない場合はすべてのShaderを変更
	/// </summary>
	/// <param name="targetGameObject">対象GameObject。子要素も変更されます。</param>
	/// <param name="ShaderName_from">変更後のShader名</param>
	/// <param name="ShaderName_to">対象Shader名。未設定時は全てのShaderを変更</param>
	public static void changeShader(GameObject targetGameObject, string ShaderName_to, string ShaderName_from = "")
	{
		System.Random r = new System.Random();
		//List<GameObject> ret = new List<GameObject>();
		foreach (Transform t in targetGameObject.GetComponentsInChildren<Transform>(true)) //include inactive gameobjects
		{
			if (t.GetComponent<Renderer>() != null)
			{
				var materials = t.GetComponent<Renderer>().materials;
				for (int i = 0; i < materials.Length; i++)
				{
					Material material = materials[i];
					var tex = material.GetTexture("_MainTex");

					if (ShaderName_from == "")
					{
						material.shader = Shader.Find(ShaderName_to);
					}
					else
					{
						if (material.shader.name == ShaderName_from)
						{
							material.shader = Shader.Find(ShaderName_to);
						}
					}
					material.SetTexture("_BaseMap", tex);
					material.SetColor("_1st_ShadeColor", Color.black);
					material.SetColor("_2nd_ShadeColor", Color.black);
					material.SetColor("_Outline_Color", new Color(r.Next(255) / 255.0f,r.Next(255) / 255.0f, r.Next(255) / 255.0f));
					material.SetFloat("_Outline_Width", 30.0f);
					material.SetFloat("_Nearest_Distance", 1.0f);
					material.SetFloat("_Farthest_Distance", 1500.0f);
				}
			}
		}
	}

	private void Update()
	{
		foreach (Transform t in gameObject.GetComponentsInChildren<Transform>(true)) //include inactive gameobjects
		{
			if (t.GetComponent<Renderer>() != null)
			{
				var materials = t.GetComponent<Renderer>().materials;
				for (int i = 0; i < materials.Length; i++)
				{
					Material material = materials[i];
					var tex = material.GetTexture("_MainTex");
					float with = material.GetFloat("_Outline_Width");
					with += Random.Range(-1.0f, 1.0f);
					material.SetFloat("_Outline_Width", with);
					material.SetColor("_BaseColor", Color.white * ((Mathf.Cos(timeCnt) + 1.0f) / 2.0f));
				}
			}
		}

		timeCnt += Time.deltaTime;
	}
}
