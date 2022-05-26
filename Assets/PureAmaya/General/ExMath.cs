using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PureAmaya.General
{
    public class ExMath
    {
        /// <summary>
        /// �����������͵ľ���ֵ
        /// </summary>
        /// <param name="Int"></param>
        /// <returns></returns>
        public static int Abs(int Int)
        {
            if (Int < 0)
            {
                return -Int;
            }
            else
            {
                return Int;
            }
        }

        /// <summary>
        /// ���ص����ȸ���ľ���ֵ
        /// </summary>
        /// <param name="float"></param>
        /// <returns></returns>
        public static float Abs(float Float)
        {
            if (Float < 0)
            {
                return -Float;
            }
            else
            {
                return Float;
            }
        }

        /// <summary>
        /// �ڸ������ȷ�Χ�ڣ��ж������������Ƿ���ͬ
        /// </summary>
        /// <param name="precision">�жϾ���</param>
        /// <param name="AllowEqual">�Ƿ�������ֵ���</param>
        /// <param name="Valve">Ҫ�Ƚϵ�ֵ</param>
        /// <returns></returns>
        public static bool Approximation(float precision, float Valve1, float Valve2, bool AllowEqual = true)
        {
            switch (AllowEqual)
            {
                case true:
                    return Abs(Valve1 - Valve2) <= precision;

                case false:
                    return Abs(Valve1 - Valve2) < precision;

            }

        }

        /// <summary>
        /// ָ���������㣨�������ܸߵ㣩
        /// </summary>
        /// <param name="exponent">ָ��</param>
        /// <param name="Base">����</param>
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
        /// �Ƿ��ڸ����ķ�Χ�ڣ������䣩
        /// </summary>
        /// <param name="value">�Ƚϵ�ֵ</param>
        /// <param name="min">��Сֵ������</param>
        /// <param name="max">���ֵ������</param>
        /// <param name="FixMinMax">�޸������С������</param>
        /// <returns></returns>
        public static bool InRange(float value, float min, float max, bool FixMinMax = true)
        {

            switch (FixMinMax)
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
                        Debug.LogError("��Сֵ�������ֵ");
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
        /// �Ƕ�ת����
        /// </summary>
        /// <param name="angleDeg">�ǶȽ�</param>
        /// <param name="UseAccuratePi">ʹ�þ�ȷ��Բ����</param>
        /// <returns></returns>
        public static float Deg2Rad(float angleDeg, bool UseAccuratePi = false)
        {
            if (UseAccuratePi)
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