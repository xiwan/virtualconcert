using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MathHelper
{
    public static float Get2Digit(float x)
    {
        return float.Parse(x.ToString("#0.00"));
    }

    public static int GetInt(float x)
    {
        return (int)x;
    }

    public static long GetLong(float x)
    {
        return (long)x;
    }

    #region Random
    /// <summary>
    /// 获取随机数int
    /// </summary>
    public static int GetRandom(int min, int max)
    {
        return GetRandom(min, max, new Random(GetRandomSeed()));
    }
    /// <summary>
    /// 获取随机数float
    /// </summary>
    public static float GetRandom(float min, float max)
    {
        return GetRandom(min, max, new Random(GetRandomSeed()));
    }

    public static int GetRandom(int min, int max, Random random)
    {
        if (random == null)
            random = new Random(GetRandomSeed());

        if (max < min)
            max = min;

        int num = random.Next(min, max + 1);
        return num;
    }

    public static float GetRandom(float min, float max, Random random)
    {
        if (random == null)
            random = new Random(GetRandomSeed());

        if (max < min)
            max = min;

        float num = NextFloat(random);
        return num;
    }

    public static int GetRandom(int min, int max, RandomType randomType)
    {
        switch (randomType)
        {
            case RandomType.RandomDay:
                return GetRandom(min, max, new Random(DateTime.Now.DayOfYear));
            default:
                return 0;
        }
    }

    public static int GetRandomSeed()
    {
        var guid = Guid.NewGuid();
        return GetRandomSeed(guid);
    }

    public static int GetRandomSeed(Guid guid)
    {
        var seed = BitConverter.ToInt32(guid.ToByteArray(), 0);
        return (seed != 0) ? seed : 1;
    }

    public static int GetEncryptRandom(int randomValue, long seed)
    {
        return (int)(((seed = seed * 201413L + 2531011L) >> 16) & 0x7fff) % randomValue;
    }

    static float NextFloat(Random random)
    {
        double mantissa = (random.NextDouble() * 2.0) - 1.0;
        // choose -149 instead of -126 to also generate subnormal floats (*)
        double exponent = Math.Pow(2.0, random.Next(-126, 128));
        return (float)(mantissa * exponent);
    }
    #endregion
}

/// <summary>
/// 随机类别
/// </summary>
public enum RandomType
{
    None = 0,

    /// <summary>
    /// 365日随机 
    /// </summary>
    RandomDay = 1,
}
