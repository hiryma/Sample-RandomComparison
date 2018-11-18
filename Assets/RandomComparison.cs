using UnityEngine;
using System.Threading;
using UnityEngine.UI;

public class RandomComparison : MonoBehaviour
{
	[SerializeField]
	RawImage _image;
	[SerializeField]
	Text _randomTypeText;

	Texture2D _texture;
	XorShift[] _xorShifts;
	Mwc[] _mwcs;
	Standard[] _standards;
	BadLcg[] _badLcgs;
	PopularLcg[] _popularLcgs;
	EventWaitHandle _terminateEvent;
	Semaphore _jobStartSemaphore;
	Semaphore _jobEndSemaphore;
	Thread[] _threads;
	const int ThreadCount = 4;
	enum RandomType
	{
		XorShift,
		Mwc,
		Standard,
		BadLcg,
		PopularLcg,
	}
	RandomType _randomType;
	Color32[][] _pixels;
	const int Width = 512;
	const int Height = 512;
	const int PixelsPerUnit = 10000;

	void Start()
	{
#if false //UnityEngine.Randomを他のスレッドから呼んでみる
		TestUnityRandomFromOtherThread();
#elif false // 速度計測
		Benchmark();
#else
		_texture = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
		_image.texture = _texture;
		_threads = new Thread[ThreadCount];
		_pixels = new Color32[ThreadCount][];
		_xorShifts = new XorShift[ThreadCount];
		_mwcs = new Mwc[ThreadCount];
		_standards = new Standard[ThreadCount];
		_badLcgs = new BadLcg[ThreadCount];
		_popularLcgs = new PopularLcg[ThreadCount];
		_randomType = RandomType.XorShift;
		_randomTypeText.text = _randomType.ToString();
		_terminateEvent = new EventWaitHandle(false, EventResetMode.ManualReset); // Resetする日は来ない
		_jobStartSemaphore = new Semaphore(0, ThreadCount);
		_jobEndSemaphore = new Semaphore(0, ThreadCount);
		for (int i = 0; i < ThreadCount; i++)
		{
			_xorShifts[i] = new XorShift(i);
			_mwcs[i] = new Mwc(i);
			_standards[i] = new Standard(i);
			_badLcgs[i] = new BadLcg(i);
			_popularLcgs[i] = new PopularLcg(i);
			_threads[i] = new Thread(ThreadFunc);
			_threads[i].Priority = System.Threading.ThreadPriority.BelowNormal;
			_threads[i].Start(i);
			_pixels[i] = new Color32[Width * Height / ThreadCount];
		}
		Clear();
#endif
	}

	void OnDestroy()
	{
		_terminateEvent.Set();
		for (int i = 0; i < ThreadCount; i++)
		{
			_jobStartSemaphore.Release();
		}
		for (int i = 0; i < ThreadCount; i++)
		{
			_threads[i].Join();
		}
	}

	void ThreadFunc(object arg)
	{
		int index = (int)arg;
		while (!_terminateEvent.WaitOne(0))
		{
			_jobStartSemaphore.WaitOne();
			Fill(index);
		}
	}

	void Fill(int index)
	{
		var pixels = _pixels[index];
		var DividedHeight = Height / ThreadCount;
		if (_randomType == RandomType.XorShift)
		{
			for (int i = 0; i < PixelsPerUnit; i++)
			{
				var x = _xorShifts[index].Next() % Width;
				var y = _xorShifts[index].Next() % DividedHeight;
				var color = _xorShifts[index].Next();
				var r = ((color >> 10) & 0x1f) << 3;
				var g = ((color >> 5) & 0x1f) << 3;
				var b = ((color >> 0) & 0x1f) << 3;
				var offset = ((y * Width) + x);
				pixels[offset].r = (byte)r;
				pixels[offset].g = (byte)g;
				pixels[offset].b = (byte)b;
			}
		}
		else if (_randomType == RandomType.Mwc)
		{
			for (int i = 0; i < PixelsPerUnit; i++)
			{
				var x = _mwcs[index].Next() % Width;
				var y = _mwcs[index].Next() % DividedHeight;
				var color = _mwcs[index].Next();
				var r = ((color >> 10) & 0x1f) << 3;
				var g = ((color >> 5) & 0x1f) << 3;
				var b = ((color >> 0) & 0x1f) << 3;
				var offset = ((y * Width) + x);
				pixels[offset].r = (byte)r;
				pixels[offset].g = (byte)g;
				pixels[offset].b = (byte)b;
			}
		}
		else if (_randomType == RandomType.Standard)
		{
			for (int i = 0; i < PixelsPerUnit; i++)
			{
				var x = _standards[index].Next() % Width;
				var y = _standards[index].Next() % DividedHeight;
				var color = _standards[index].Next();
				var r = ((color >> 10) & 0x1f) << 3;
				var g = ((color >> 5) & 0x1f) << 3;
				var b = ((color >> 0) & 0x1f) << 3;
				var offset = ((y * Width) + x);
				pixels[offset].r = (byte)r;
				pixels[offset].g = (byte)g;
				pixels[offset].b = (byte)b;
			}
		}
		else if (_randomType == RandomType.BadLcg)
		{
			for (int i = 0; i < PixelsPerUnit; i++)
			{
				var x = _badLcgs[index].Next() % Width;
				var y = _badLcgs[index].Next() % DividedHeight;
				var color = _badLcgs[index].Next();
				var r = ((color >> 10) & 0x1f) << 3;
				var g = ((color >> 5) & 0x1f) << 3;
				var b = ((color >> 0) & 0x1f) << 3;
				var offset = ((y * Width) + x);
				pixels[offset].r = (byte)r;
				pixels[offset].g = (byte)g;
				pixels[offset].b = (byte)b;
			}
		}
		else if (_randomType == RandomType.PopularLcg)
		{
			for (int i = 0; i < PixelsPerUnit; i++)
			{
				var x = _popularLcgs[index].Next() % Width;
				var y = _popularLcgs[index].Next() % DividedHeight;
				var color = _popularLcgs[index].Next();
				var r = ((color >> 10) & 0x1f) << 3;
				var g = ((color >> 5) & 0x1f) << 3;
				var b = ((color >> 0) & 0x1f) << 3;
				var offset = ((y * Width) + x);
				pixels[offset].r = (byte)r;
				pixels[offset].g = (byte)g;
				pixels[offset].b = (byte)b;
			}
		}
		int semaphoreCount = _jobEndSemaphore.Release(); // +1
	}

	void Clear()
	{
		for (int i = 0; i < ThreadCount; i++)
		{
			for (int j = 0; j < _pixels[i].Length; j++)
			{
				_pixels[i][j] = new Color32(0, 0, 0, 0xff);
			}
		}
		CopyToTexture();
	}

	void Update()
	{
		// クリア
		if (Input.anyKeyDown)
		{
			Clear();
			// タイプ変更
			switch (_randomType)
			{
				case RandomType.XorShift: _randomType = RandomType.Mwc; break;
				case RandomType.Mwc: _randomType = RandomType.Standard; break;
				case RandomType.Standard: _randomType = RandomType.BadLcg; break;
				case RandomType.BadLcg: _randomType = RandomType.PopularLcg; break;
				case RandomType.PopularLcg: _randomType = RandomType.XorShift; break;
			}
			_randomTypeText.text = _randomType.ToString();
		}
		else
		{
			// キック
			for (int i = 0; i < ThreadCount; i++)
			{
				_jobStartSemaphore.Release();
			}
			// 終了待ち
			for (int i = 0; i < ThreadCount; i++)
			{
				if (!_jobEndSemaphore.WaitOne(1000))
				{
					Debug.Assert(false, "Semaphoreがタイムアウト!!同期にバグあるよ!!");
					break;
				}
			}
			// ピクセル充填
			CopyToTexture();
		}
	}

	void CopyToTexture()
	{
		var blockHeight = Height / ThreadCount;
		for (int i = 0; i < ThreadCount; i++)
		{
			var yOffset = blockHeight * i;
			_texture.SetPixels32(0, yOffset, Width, blockHeight, _pixels[i]);
		}
		_texture.Apply();
	}

	void TestUnityRandomFromOtherThread()
	{
		int value = 0;
		var thread = new Thread(() =>
		{
			value = Random.Range(0, 100);
		});
		thread.Start();
		thread.Join();
		Debug.Log("Thread End! " + value);
	}

	struct XorShift
	{
		uint _x;
		public XorShift(int seed)
		{
			_x = 0xffff0000 | (uint)(seed & 0xffff);
		}
		public int Next()
		{
			_x ^= _x << 13;
			_x ^= _x >> 17;
			_x ^= _x << 5;
			return (int)(_x & 0xffff);
		}
	}

	struct Mwc
	{
		uint _x;
		public Mwc(int seed)
		{
			_x = 0xffff0000 | (uint)(seed & 0xffff);
		}
		public int Next()
		{
			_x = ((_x & 0xffff) * 62904) + (_x >> 16); // from Numerical Recipes.
			return (int)(_x & 0xffff);
		}
	}

	struct Standard
	{
		System.Random _rand;
		public Standard(int seed)
		{
			_rand = new System.Random(seed);
		}
		public int Next()
		{
			return _rand.Next() & 0xffff;
		}
	}

	struct BadLcg
	{
		uint _x;
		public BadLcg(int seed)
		{
			_x = 0x7ff80001 | ((uint)(seed & 0xffff) << 1); // 奇数を強制
		}
		public int Next()
		{
			_x = (65539 * _x) & 0x7fffffff;
			return (int)(_x & 0xffff);
		}
	}

	struct PopularLcg
	{
		uint _x;
		public PopularLcg(int seed)
		{
			_x = (uint)seed;
		}
		public int Next()
		{
			_x = ((1103515245 * _x) + 12345) & 0x7fffffff;
			return (int)(_x & 0xffff);
		}
	}

	void Benchmark()
	{
		const int N = 1000 * 1000 * 100; // 1億

		var t0 = Time.realtimeSinceStartup;
		var xorShift = new XorShift();
		int sum = 0; // 結果を何かに使わないと最適化で消されそうなので用意
		for (int i = 0; i < N; i++)
		{
			sum += xorShift.Next();
		}
		var t1 = Time.realtimeSinceStartup;

		var t2 = Time.realtimeSinceStartup;
		var mwc = new Mwc();
		for (int i = 0; i < N; i++)
		{
			sum += mwc.Next();
		}
		var t3 = Time.realtimeSinceStartup;

		var t4 = Time.realtimeSinceStartup;
		var std = new System.Random();
		for (int i = 0; i < N; i++)
		{
			sum += std.Next();
		}
		var t5 = Time.realtimeSinceStartup;
		Debug.Log("XorShift: " + (t1 - t0) + " Mwc: " + (t3 - t2) + " System.Random: " + (t5 - t4) + " sum:" + sum);
	}
}
