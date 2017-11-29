using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Security.Cryptography;

namespace KursZI
{
    public partial class Form1 : Form
    {
        int it = 0;
        public Form1()
        {
            InitializeComponent();
            text1.Text = "Bobo";
            gost1.Text = "kas is the best!";
            p1.Text = "1";
            q1.Text = "3";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String gostKey;
            int p;
            int q;
            String text;
            if(getStartData(out gostKey,out p, out q, out text))
            {
                List<List<String>> splitText = SplitText(text);
                List<String> keys = GenKey(gostKey);
                String result = "";
                byte[] r = new byte[text.Length*2];
                for(int i=0;i<splitText.Count;i++)
                {
                    //блоки по 32 бита
                    String Block1 = splitText[i][0];
                    String Block2 = splitText[i][1];

                    BitArray A = StrTobits(Block1);
                    BitArray B = StrTobits(Block2);

                    //Block1 = BitsToStr(A);
                    //Block2 = BitsToStr(B);
                    for(int j=0; j<32;j++)
                    {
                        it = j;
                        BitArray K = StrTobits(keys[j]);
                        BitArray temp = new BitArray(A);
                        A = B.Xor(FuncF(A,K));
                        B = temp;
                    }
                    
                    result += BitsToStr(B);
                    result += BitsToStr(A);
                }
                result1.Text = result;
                text2.Text = result;
                gost2.Text = gostKey;
                p2.Text = p1.Text;
                q2.Text = q1.Text;
            }
            else
            {
                MessageBox.Show("Введены неверные данные", "Ошибка");
            }
        }
        void isError(out String gostKey, out int p, out int q, out String text)
        {
            gostKey = "";
            p = 0; q = 0;
            text = "";
        }
        bool getStartData(out String gostKey, out int p,out int q, out String text)
        {
            
            if (!String.IsNullOrEmpty(gost1.Text) && Int32.TryParse(p1.Text,out p) && Int32.TryParse(q1.Text,out q) && !String.IsNullOrEmpty(text1.Text))
            {
                gostKey = gost1.Text;
                text = text1.Text;
                
                return true;
            }
            else
            {
                isError(out gostKey, out p, out q, out text);
                return false;
            }
        }
        //разбиение исходного текста на блоки по 32 бита
        List<List<String>> SplitText(String text)
        {
            int sizeOfBlock = 4;
            List<List<String>> result = new List<List<String>>();
            String temp = "";
            foreach (char s in text)
            {
                if(temp.Length == sizeOfBlock-1)
                {
                    temp += s;
                    String temp1 = temp.Substring(0, 2);
                    String temp2 = temp.Substring(2, 2);
                    result.Add(new List<string>() {temp1,temp2});
                    temp = "";
                }
                else
                {
                    temp += s;
                }
            }
            return result;
        }
         //разбиение ключа на 
        List<String> GenKey(String gostKey)
        {
            int sizeOfKey = 2;
            List<String> result = new List<string>();
            String temp = "";
            foreach(char s in gostKey)
            {
               if(temp.Length == sizeOfKey-1)
               {
                    temp += s;
                    result.Add(temp);
                    temp = "";
               }
                else
                {
                    temp += s;
                }
            }
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    result.Add(result[i]);
                }
            }
            for(int i=7;i>=0;i--)
            {
                result.Add(result[i]);
            }
            return result;
        }
        static byte BoolArrayToByte(bool[] bools)
        {
            String strprs = "";
            foreach (bool b in bools)
            {
                if (b)
                {
                    strprs += "1";
                }
                else
                {
                    strprs += "0";
                }
            }
            byte result = Convert.ToByte(strprs, 2);
            return result;
        }
        static BitArray StrTobits(String r)
        {
            List<byte> g = new List<byte>();
            foreach (char t in r)
            {
                byte[] temp = Encoding.Unicode.GetBytes(t.ToString());
                g.AddRange(temp);
            }
            BitArray result = new BitArray(g.ToArray());
            return result;
        }
        static String BitsToStr(BitArray b)
        {
            String s = "";
            byte[] text = new byte[b.Length / 8];
            List<bool> bit = new List<bool>();
            for (int i = 0, j = 0; i < b.Length; i++)
            {
                if (i % 8 == 7)
                {
                    bit.Add(b[i]);
                    bit.Reverse();
                    text[j] = BoolArrayToByte(bit.ToArray());
                    bit.Clear();
                    j++;
                }
                else
                {
                    bit.Add(b[i]);
                }
            }
            s = Encoding.Unicode.GetString(text);
            //s = s.Replace("\0", "");
            return s;
        }
        static byte[] BitsToByteArray(BitArray b)
        {
            //String s = "";
            byte[] text = new byte[b.Length / 8];
            List<bool> bit = new List<bool>();
            for (int i = 0, j = 0; i < b.Length; i++)
            {
                if (i % 8 == 7)
                {
                    bit.Add(b[i]);
                    bit.Reverse();
                    text[j] = BoolArrayToByte(bit.ToArray());
                    bit.Clear();
                    j++;
                }
                else
                {
                    bit.Add(b[i]);
                }
            }
            //s = Encoding.Unicode.GetString(text);
            //s = s.Replace("\0", "");
            return text;
        }
        BitArray Sum(BitArray b1, BitArray b2)
        {
            bool ost = false;
            BitArray result = new BitArray(b1.Length);
            for (int i = b1.Count - 1; i >= 0; i--)
            {

                if (b1[i] == false && b2[i] == false)
                {
                    result[i] = false;
                    if (ost == true)
                    {
                        result[i] = !result[i];
                        ost = false;
                    }
                    continue;
                }
                if ((b1[i] == false && b2[i] == true) || (b1[i] == true && b2[i] == false))
                {
                    result[i] = true;
                    if (ost == true)
                    {
                        result[i] = !result[i];
                        ost = false;
                    }
                    continue;
                }
                if (b1[i] == true && b2[i] == true)
                {
                    result[i] = false;
                    if (ost == true)
                    {
                        result[i] = !result[i];
                        ost = false;
                    }
                    ost = true;
                }
            }
            return result;
        }
        BitArray FuncF(BitArray input, BitArray Key)
        {
            int[][] SBlocks = new int[][]
            {
                new int[]{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15},
                new int[]{5,2,1,3,6,4,7,9,10,12,11,13,15,14,8,0},
                new int[]{14,8,0,5,2,1,3,6,4,7,9,10,12,11,13,15},
                new int[]{1,3,6,4,5,2,7,9,10,12,11,13,15,14,8,0},
                new int[]{11,13,15,14,5,2,1,3,6,4,7,9,10,12,8,0},
                new int[]{3,6,4,7,5,2,1,9,10,12,11,13,15,14,8,0},
                new int[]{15,14,8,5,2,1,3,6,4,7,9,10,12,11,13,0},
                new int[]{12,11,5,2,1,3,6,4,7,9,10,13,15,14,8,0}

            };
            BitArray b1 = Sum(input, Key);
            List<BitArray> SInput = splitBits(b1);
            int[] SOut = new int[8];
            for(int i=0;i<SInput.Count;i++)
            {
                SOut[i] = BitsToInt(SInput[i]);
                SOut[i] = SBlocks[i][SOut[i]];
                SInput[i] = IntToBits(SOut[i]);
            }
            BitArray[] res= SInput.ToArray();
            BitArray result = new BitArray(res.Length*4);
            for(int i=0,k=0;i<res.Length;i++)
            {
                for(int j=0;j<res[i].Length;j++)
                {
                    result[k]=res[i][j];
                    k++;
                }
            }
            
            result = Shift(result);
            return result;
        }
        List<BitArray> splitBits(BitArray input)
        {
            List<BitArray> result = new List<BitArray>();
            List<bool> temp = new List<bool>();
            foreach (bool b in input)
            {
                if (temp.Count == 3)
                {
                    temp.Add(b);
                    result.Add(new BitArray(temp.ToArray()));
                    temp.Clear();
                    
                }
                else
                {
                    temp.Add(b);
                }
            }
            return result;
        }
        int BitsToInt(BitArray b)
        {
            String temp = "";
            foreach(bool b1 in b)
            {
                if(b1==true)
                {
                    temp += 1;
                }
                else
                {
                    temp += 0;
                }
            }
            return Convert.ToInt32(temp, 2);
        }
        BitArray IntToBits(int i)
        {
            List<bool> list = new List<bool>();
            String s = Convert.ToString(i,2);
            foreach(char str in s)
            {
                if(str =='1')
                {
                    list.Add(true);
                }
                else
                {
                    list.Add(false);
                }
            }
            BitArray bitArray = new BitArray(list.ToArray());
            return bitArray;
        }
        BitArray Shift(BitArray b)
        {
            BitArray result = new BitArray(b.Count);
            int offset = 3;
            for (int i = 0; i < b.Length; i++)
            {
                result[i] = b[(i + offset) % b.Length];
            }
            return result;
        }
        bool getEndData(out String gostKey, out int p, out int q, out String text)
        {

            if (!String.IsNullOrEmpty(gost2.Text) && Int32.TryParse(p2.Text, out p) && Int32.TryParse(q2.Text, out q) && !String.IsNullOrEmpty(text2.Text))
            {
                gostKey = gost2.Text;
                text = text2.Text;

                return true;
            }
            else
            {
                isError(out gostKey, out p, out q, out text);
                return false;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            String gostKey;
            int p;
            int q;
            String text;
            if (getEndData(out gostKey, out p, out q, out text))
            {
                List<List<String>> splitText = SplitText(text);
                List<String> keys = GenKey(gostKey);
                keys.Reverse();
                String result = "";
                for (int i = 0; i < splitText.Count; i++)
                {
                    //блоки по 32 бита
                    String Block1 = splitText[i][0];
                    String Block2 = splitText[i][1];

                    BitArray A = StrTobits(Block1);
                    BitArray B = StrTobits(Block2);

                    //Block1 = BitsToStr(A);
                    //Block2 = BitsToStr(B);
                    for (int j = 0; j < 32; j++)
                    {
                        BitArray K = StrTobits(keys[j]);
                        BitArray temp = new BitArray(A);
                        A = B.Xor(FuncF(A, K));
                        B = temp;
                    }
                    result += BitsToStr(B);
                    result += BitsToStr(A);
                }
                result2.Text = result;
            }
            else
            {
                MessageBox.Show("Введены неверные данные", "Ошибка");
            }
        }
    }
}
