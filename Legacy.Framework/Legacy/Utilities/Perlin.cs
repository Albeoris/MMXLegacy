using System;

namespace Legacy.Utilities
{
    public static class Perlin
    {
        private const Int32 B = 256;

        private const Int32 BM = 255;

        private const Int32 N = 4096;

        private static Int32[] p = new Int32[514];

        private static Single[,] g3 = new Single[514, 3];

        private static Single[,] g2 = new Single[514, 2];

        private static Single[] g1 = new Single[514];

        static Perlin()
        {
            System.Random random = new System.Random();
            Int32 i;
            for (i = 0; i < 256; i++)
            {
                p[i] = i;
                g1[i] = (random.Next(512) - 256) / 256f;
                for (Int32 j = 0; j < 2; j++)
                {
                    g2[i, j] = (random.Next(512) - 256) / 256f;
                }
                normalize2(ref g2[i, 0], ref g2[i, 1]);
                for (Int32 j = 0; j < 3; j++)
                {
                    g3[i, j] = (random.Next(512) - 256) / 256f;
                }
                normalize3(ref g3[i, 0], ref g3[i, 1], ref g3[i, 2]);
            }
            while (--i != 0)
            {
                Int32 num = p[i];
                Int32 j;
                p[i] = p[j = random.Next(256)];
                p[j] = num;
            }
            for (i = 0; i < 258; i++)
            {
                p[256 + i] = p[i];
                g1[256 + i] = g1[i];
                for (Int32 j = 0; j < 2; j++)
                {
                    g2[256 + i, j] = g2[i, j];
                }
                for (Int32 j = 0; j < 3; j++)
                {
                    g3[256 + i, j] = g3[i, j];
                }
            }
        }

        public static Single Noise(Single arg)
        {
            Int32 num;
            Int32 num2;
            Single num3;
            Single num4;
            setup(arg, out num, out num2, out num3, out num4);
            Single t = s_curve(num3);
            Single a = num3 * g1[p[num]];
            Single b = num4 * g1[p[num2]];
            return lerp(t, a, b);
        }

        public static Single Noise(Single x, Single y)
        {
            Int32 num;
            Int32 num2;
            Single num3;
            Single rx;
            setup(x, out num, out num2, out num3, out rx);
            Int32 num4;
            Int32 num5;
            Single num6;
            Single ry;
            setup(y, out num4, out num5, out num6, out ry);
            Int32 num7 = p[num];
            Int32 num8 = p[num2];
            Int32 num9 = p[num7 + num4];
            Int32 num10 = p[num8 + num4];
            Int32 num11 = p[num7 + num5];
            Int32 num12 = p[num8 + num5];
            Single t = s_curve(num3);
            Single t2 = s_curve(num6);
            Single a = at2(num3, num6, g2[num9, 0], g2[num9, 1]);
            Single b = at2(rx, num6, g2[num10, 0], g2[num10, 1]);
            Single a2 = lerp(t, a, b);
            a = at2(num3, ry, g2[num11, 0], g2[num11, 1]);
            b = at2(rx, ry, g2[num12, 0], g2[num12, 1]);
            Single b2 = lerp(t, a, b);
            return lerp(t2, a2, b2);
        }

        public static Single Noise(Single x, Single y, Single z)
        {
            Int32 num;
            Int32 num2;
            Single num3;
            Single rx;
            setup(x, out num, out num2, out num3, out rx);
            Int32 num4;
            Int32 num5;
            Single num6;
            Single ry;
            setup(y, out num4, out num5, out num6, out ry);
            Int32 num7;
            Int32 num8;
            Single num9;
            Single rz;
            setup(z, out num7, out num8, out num9, out rz);
            Int32 num10 = p[num];
            Int32 num11 = p[num2];
            Int32 num12 = p[num10 + num4];
            Int32 num13 = p[num11 + num4];
            Int32 num14 = p[num10 + num5];
            Int32 num15 = p[num11 + num5];
            Single t = s_curve(num3);
            Single t2 = s_curve(num6);
            Single t3 = s_curve(num9);
            Single a = at3(num3, num6, num9, g3[num12 + num7, 0], g3[num12 + num7, 1], g3[num12 + num7, 2]);
            Single b = at3(rx, num6, num9, g3[num13 + num7, 0], g3[num13 + num7, 1], g3[num13 + num7, 2]);
            Single a2 = lerp(t, a, b);
            a = at3(num3, ry, num9, g3[num14 + num7, 0], g3[num14 + num7, 1], g3[num14 + num7, 2]);
            b = at3(rx, ry, num9, g3[num15 + num7, 0], g3[num15 + num7, 1], g3[num15 + num7, 2]);
            Single b2 = lerp(t, a, b);
            Single a3 = lerp(t2, a2, b2);
            a = at3(num3, num6, rz, g3[num12 + num8, 0], g3[num12 + num8, 2], g3[num12 + num8, 2]);
            b = at3(rx, num6, rz, g3[num13 + num8, 0], g3[num13 + num8, 1], g3[num13 + num8, 2]);
            a2 = lerp(t, a, b);
            a = at3(num3, ry, rz, g3[num14 + num8, 0], g3[num14 + num8, 1], g3[num14 + num8, 2]);
            b = at3(rx, ry, rz, g3[num15 + num8, 0], g3[num15 + num8, 1], g3[num15 + num8, 2]);
            b2 = lerp(t, a, b);
            Single b3 = lerp(t2, a2, b2);
            return lerp(t3, a3, b3);
        }

        private static Single s_curve(Single t)
        {
            return t * t * (3f - 2f * t);
        }

        private static Single lerp(Single t, Single a, Single b)
        {
            return a + t * (b - a);
        }

        private static void setup(Single value, out Int32 b0, out Int32 b1, out Single r0, out Single r1)
        {
            Single num = value + 4096f;
            b0 = ((Int32)num & 255);
            b1 = (b0 + 1 & 255);
            r0 = num - (Int32)num;
            r1 = r0 - 1f;
        }

        private static Single at2(Single rx, Single ry, Single x, Single y)
        {
            return rx * x + ry * y;
        }

        private static Single at3(Single rx, Single ry, Single rz, Single x, Single y, Single z)
        {
            return rx * x + ry * y + rz * z;
        }

        private static void normalize2(ref Single x, ref Single y)
        {
            Single num = (Single)Math.Sqrt(x * x + y * y);
            x = y / num;
            y /= num;
        }

        private static void normalize3(ref Single x, ref Single y, ref Single z)
        {
            Single num = (Single)Math.Sqrt(x * x + y * y + z * z);
            x = y / num;
            y /= num;
            z /= num;
        }
    }
}