using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>{

	private AudioClip[] seClips;
	private AudioClip[] bgmClips;

	private Dictionary<string, int> seIndexs = new Dictionary<string, int>();
	private Dictionary<string, int> bgmIndexs = new Dictionary<string, int>();

	const int NumChannel = 16;

	private AudioSource bgmSource;
	private AudioSource[] seSources = new AudioSource[NumChannel];

	Queue<int> seRequestQueue = new Queue<int>();
	
	void Awake ()
	{
		bgmSource = gameObject.AddComponent<AudioSource>();

		for (int i = 0; i < seSources.Length; i++)
		{
			seSources[i] = gameObject.AddComponent<AudioSource>();
		}

		seClips = Resources.LoadAll<AudioClip>("Audio/SE");
		bgmClips = Resources.LoadAll<AudioClip>("Audio/BGM");

		for (int i = 0; i < seClips.Length; i++)
		{
			seIndexs[seClips[i].name] = i;
		}

		for (int i = 0; i < bgmClips.Length; i++)
		{
			bgmIndexs[bgmClips[i].name] = i;
		}
	}
	
	void Update ()
	{
		int count = seRequestQueue.Count;
		if (count != 0)
		{
			int sound_index = seRequestQueue.Dequeue();
			PlaySeImpl(sound_index);
		}
	}

	private void PlaySeImpl(int index)
	{
		if (0 > index || seClips.Length <= index)
		{
			return;
		}

		foreach (AudioSource source in seSources)
		{
			if (false == source.isPlaying)
			{
				source.clip = seClips[index];
				source.Play();
				return;
			}
		}
	}

	public void PlayBGM(string _name,bool _loop)
	{
		PlayBGM(bgmIndexs[_name],_loop);
	}

	public void PlayBGM(int _index,bool _loop)
	{
		if (0 > _index || bgmClips.Length <= _index)
		{
			return;
		}

		if (bgmSource.clip == bgmClips[_index])
		{
			return;
		}

		bgmSource.loop = _loop;
		bgmSource.Stop();
		bgmSource.clip = bgmClips[_index];
		bgmSource.Play();
	}

	public void StopBGM()
	{
		bgmSource.Stop();
		bgmSource.clip = null;
	}

	public void PlaySE(string _name)
	{
		PlaySE(GetSEIndex(_name));
	}

	public void PlaySE(int _index)
	{
		if (!seRequestQueue.Contains(_index))
		{
			seRequestQueue.Enqueue(_index);
		}
	}

	private int GetBGMIndex(string _name)
	{
		return bgmIndexs[_name];
	}

	private int GetSEIndex(string _name)
	{
		return seIndexs[_name];
	}

	public void ClearAllSeRequest()
	{
		seRequestQueue.Clear();
	}
}
