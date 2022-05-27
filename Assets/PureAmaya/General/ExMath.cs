using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PureAmaya.General
{
    public class ExMath
    {
        /// <summary>
        /// 返回整数类型的绝对值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int Abs(int index)
        {
            if (index < 0)
            {
                return -index;
            }
            else
            {
                return index;
            }
        }

        /// <summary>
        /// 返回单精度浮点的绝对值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static float Abs(float index)
        {
            if (index < 0)
            {
                return -index;
            }
            else
            {
                return index;
            }
        }

        /// <summary>
        /// 在给定精度范围内，判断两个浮点数是否相同
        /// </summary>
        /// <param name="precision">判断精度</param>
        /// <param name="allowEqual">是否允许差值相等</param>
        /// <returns></returns>
        public static bool Approximation(float precision, float value1, float value2, bool allowEqual = true)
        {
            switch (allowEqual)
            {
                case true:
                    return Abs(value1 - value2) <= precision;

                case false:
                    return Abs(value1 - value2) < precision;

            }

        }

        /// <summary>
        /// 指数函数计算（可能性能高点）
        /// </summary>
        /// <param name="exponent">指数</param>
        /// <param name="Base">底数</param>
        /// <returns></returns>
        public static int ExponentialFunction(int exponent, int Base = 10)
        {
            for (int i = 0; i < exponent; i++)
            {
                Base *= Base;
            }

            return Base;

        }


        /// <summary>
        /// 是否在给定的范围内（闭区间）
        /// </summary>
        /// <param name="value">比较的值</param>
        /// <param name="min">最小值（含）</param>
        /// <param name="max">最大值（含）</param>
        /// <param name="fixMinMax">修复最大最小混淆吗</param>
        /// <returns></returns>
        public static bool InRange(float value, float min, float max, bool fixMinMax = true)
        {

            switch (fixMinMax)
            {
                case true:
                    if (min > max)
                    {
                        float S = min;
                        min = max;
                        max = S;
                    }
                    break;

                case false:
                    if (min > max)
                    {
                        Debug.LogError("最小值大于最大值");
                    }
                    break;
            }


            if (value >= min && value <= max)
            {
                return true;
            }
            else
            {
                return
                    false;
            }
        }

        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="angleDeg">角度角</param>
        /// <param name="useAccuratePi">使用精确的圆周率</param>
        /// <returns></returns>
        public static float Deg2Rad(float angleDeg, bool useAccuratePi = false)
        {
            if (useAccuratePi)
            {
                return 3.14f / 180f * angleDeg;
            }
            else
            {
                return Mathf.Deg2Rad * angleDeg;
            }
        }

    }
}
