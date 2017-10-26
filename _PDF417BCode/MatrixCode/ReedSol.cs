using System;
using System.Collections.Generic;
using System.Text;


namespace _PDF417BCode.Internal
{
    public class ReedSol
    {

        private int gfpoly;
        private int symsize;    // in bits
        private int logmod;	    // 2**symsize - 1
        private int rlen;

        private int[] log = null;
        private int[] alog = null;
        private int[] rspoly = null;


        public void rs_init_gf(int poly)
        {
            int m, b, p, v;

            // C# does not need that:
            //
            // Return storage from previous setup
            //if (log)
            //{
            //    free(log);
            //    free(alog);
            //    free(rspoly);
            //    rspoly = NULL;
            //}


            // Find the top bit, and hence the symbol size
            for (b = 1, m = 0; b <= poly; b <<= 1)
                m++;
            b >>= 1;
            m--;
            gfpoly = poly;
            symsize = m;

            // Calculate the log/alog tables
            logmod = (1 << m) - 1;
            log = new int[logmod + 1];  // C:  log = (int*)malloc(sizeof(int) * (logmod + 1));
            alog = new int[logmod];     // C:  alog = (int*)malloc(sizeof(int) * logmod);

            for (p = 1, v = 0; v < logmod; v++)
            {
                alog[v] = p;
                log[p] = v;
                p <<= 1;
                if ((p & b) != 0)
                    p ^= poly;
            }

        }


        public void rs_init_code(int nsym, int index)
        {
            int i, k;

            // C# does not need that:
            //if (rspoly)
            //    free(rspoly);

            rspoly = new int[nsym + 1];   // C:  rspoly = (int*)malloc(sizeof(int) * (nsym + 1));

            rlen = nsym;

            rspoly[0] = 1;
            for (i = 1; i <= nsym; i++)
            {
                rspoly[i] = 1;
                for (k = i - 1; k > 0; k--)
                {
                    if (rspoly[k] != 0)
                        rspoly[k] =
                            alog[(log[rspoly[k]] + index) % logmod];
                    rspoly[k] ^= rspoly[k - 1];
                }
                rspoly[0] = alog[(log[rspoly[0]] + index) % logmod];
                index++;
            }
        }


        public void rs_encode(int len, byte[] data,out  byte[] res)
        {
            int i, k, m;

            res = new byte[rlen];
            for (i = 0; i < rlen; i++)
                res[i] = 0;
            for (i = 0; i < len; i++)
            {
                m = res[rlen - 1] ^ data[i];
                for (k = rlen - 1; k > 0; k--)
                {
                    if (m != 0 && rspoly[k] != 0)
                        res[k] = (byte)(res[k - 1] ^ alog[(log[m] + log[rspoly[k]]) % logmod]);
                    else
                        res[k] = res[k - 1];
                }
                if (m != 0 && rspoly[0] != 0)
                    res[0] = (byte)alog[(log[m] + log[rspoly[0]]) % logmod];
                else
                    res[0] = 0;
            }
        }


    }
}
