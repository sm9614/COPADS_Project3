using System.Reflection;
using System.Text;
using SkiaSharp;

namespace Project
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("Option can only be one of the following commands:" +
                    " cipher, generatekeystream, encrypt, decrypt, triplebits, encryptimage, decryptimage");
                return;
            }
            string option = args[0].ToLower();

            switch (option)
            {
                case "cipher":
                    if (args.Length != 3)
                    {
                        Console.WriteLine("Cipher takes 3 parameters <cipher> <seed> <tap>");
                        return;
                    }
                    string seed = args[1];
                    int.TryParse(args[2], out int tap);
                    string newSeed = Cipher(seed, tap);
                    Console.WriteLine(seed + " -seed \n" + newSeed + " " + newSeed[0]);
                    break;

                case "generatekeystream":
                    if (args.Length != 4)
                    {
                        Console.WriteLine("GenerateKeyStream takes 4 parameters <GenerateKeyStream> <seed> <tap> <step>");
                        return;
                    }
                    seed = args[1];
                    int.TryParse(args[2], out tap);
                    int.TryParse(args[3], out int step);
                    GenerateKeyStream(seed, tap, step);
                    break;

                case "encrypt":
                    if (args.Length != 2)
                    {
                        Console.WriteLine("Encrypt takes 2 parameters <Encrypt> <plaintext>");
                        return;
                    }
                    string plaintext = args[1];
                    Console.WriteLine("The ciphertext is: " + Encrypt(plaintext));
                    break;

                case "decrypt":
                    if (args.Length != 2)
                    {
                        Console.WriteLine("Decrypt takes 2 parameters <Decrypt> <ciphertext>");
                        return;
                    }
                    string ciphertext = args[1];
                    Console.WriteLine("The plaintext is: " + Decrypt(ciphertext));
                    break;

                case "triplebits":
                    if (args.Length != 5)
                    {
                        Console.WriteLine("TripleBits takes 5 parameters <TripleBits> <seed> <tap> <step> <iteration>");
                        return;
                    }
                    seed = args[1];
                    int.TryParse(args[2], out tap);
                    int.TryParse(args[3], out step);
                    int.TryParse(args[4], out int iteration);
                    TripleBits(seed, tap, step, iteration);
                    break;

                case "encryptimage":
                    if (args.Length != 4)
                    {
                        Console.WriteLine("EncryptImage takes 4 parameters <EncryptImage> <imagefile> <seed> <tap>");
                        return;
                    }
                    string imagefile = args[1];
                    seed = args[2];
                    int.TryParse(args[3], out tap);
                    EncryptImage(imagefile, seed, tap);
                    break;

                case "decryptimage":
                    if (args.Length != 4)
                    {
                        Console.WriteLine("DecryptImage takes 4 parameters <EncryptImage> <imagefile> <seed> <tap>");
                        return;
                    }
                    imagefile = args[1];
                    seed = args[2];
                    int.TryParse(args[3], out tap);
                    Console.WriteLine(DecryptImage(imagefile, seed, tap));
                    break;

                default:
                    Console.WriteLine("Option can only be one of the following commands:" +
                    " Cipher, GenerateKeyStream, Encrypt, Decrypt, TripleBits, EncryptImage, DecryptImage");
                    break;

            }

        }

        private static bool DecryptImage(string imagefile, string seed, int tap)
        {
            throw new NotImplementedException();
        }

        private static void EncryptImage(string imagefile, string seed, int tap)
        {
            try
            {
                FileStream fsRead = File.OpenRead(imagefile);
                SKBitmap bitmap = SKBitmap.Decode(fsRead);
                string newSeed = Cipher(seed, tap);
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        byte random8BitRed = GenerateRandom8Bit(ref newSeed, tap);
                        byte random8BitGreen = GenerateRandom8Bit(ref newSeed, tap);
                        byte random8BitBlue = GenerateRandom8Bit(ref newSeed, tap);

                        byte red = bitmap.GetPixel(x, y).Red;
                        byte newRed = (byte)(red ^ random8BitRed);

                        byte green = bitmap.GetPixel(x, y).Green;
                        byte newGreen = (byte)(green ^ random8BitGreen);

                        byte blue = bitmap.GetPixel(x, y).Blue;
                        byte newBlue = (byte)(blue ^ random8BitBlue);

                        SKColor newColor = new SKColor(newRed, newGreen, newBlue);
                        bitmap.SetPixel(x, y, newColor);
                    }
                }
                try
                {
                    FileStream fsWrite = File.OpenWrite("flowerENCRYPTED.png");
                    bitmap.Encode(fsWrite, SKEncodedImageFormat.Png, 100);
                    fsWrite.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                fsRead.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        private static void TripleBits(string seed, int tap, int step, int iteration)
        {
            Console.WriteLine(seed + " -seed");
            for (int i = 0; i < iteration; i++)
            {
                int value = 1;
                for (int j = 0; j < step; j++)
                {
                    seed = Cipher(seed, tap);
                    int lsb = (int)char.GetNumericValue(seed[seed.Length - 1]);
                    value = value * 3 + lsb;
                }
                Console.WriteLine(seed + " " + value);
            }
        }

        private static string Decrypt(string ciphertext)
        {
            // Since Encrypting the ciphertext gives the plain text we can just call 
            // Encrypt on the ciphertext to get the plaintext
            return Encrypt(ciphertext);
        }

        private static string Encrypt(string plaintext)
        {
            string? keystream = "";
            try
            {
                StreamReader sr = new StreamReader("keystream.txt");
                keystream = sr.ReadLine();
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            return XOR(keystream, plaintext);
        }

        private static void GenerateKeyStream(string seed, int tap, int step)
        {
            Console.WriteLine(seed + " -seed");
            List<int> keystream = new List<int>();
            for (int i = 0; i < step; i++)
            {
                seed = Cipher(seed, tap);
                int lsb = (int)char.GetNumericValue(seed[seed.Length - 1]);
                Console.WriteLine(seed + " " + lsb);
                keystream.Add(lsb);
            }

            try
            {
                StreamWriter sw = new StreamWriter("keystream.txt");
                sw.WriteLine(string.Join("", keystream));
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            Console.WriteLine("The Keystream: " + string.Join("", keystream));

        }

        private static string Cipher(string seed, int tap)
        {

            int value = (int)char.GetNumericValue(seed[tap]);
            int msb = (int)char.GetNumericValue(seed[0]);
            int newValue = value ^ msb;
            string newSeed = seed.Substring(1);
            newSeed += newValue.ToString();
            return newSeed;

        }

        private static string XOR(string str1, string str2)
        {
            int maxLength = Math.Max(str1.Length, str2.Length);

            // Adds leading 0's to the smaller bit string
            if (str1.Length < maxLength)
            {
                str1 = str1.PadLeft(maxLength, '0');
            }
            else
            {
                str2 = str2.PadLeft(maxLength, '0');
            }

            StringBuilder XORStr = new StringBuilder();
            // Does the XOR by going through each individual bit
            for (int i = 0; i < maxLength; i++)
            {
                if (str1[i] == '1' && str2[i] == '1')
                {
                    XORStr.Append('0');
                }
                else if ((str1[i] == '1' && str2[i] == '0') || (str1[i] == '0' && str2[i] == '1'))
                {
                    XORStr.Append('1');
                }
                else
                {
                    XORStr.Append('0');
                }
            }

            return XORStr.ToString();
        }
        private static byte GenerateRandom8Bit(ref string seed, int tap)
        {
            StringBuilder random8Bit = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                seed = Cipher(seed, tap);
                int lsb = (int)char.GetNumericValue(seed[seed.Length - 1]);
                random8Bit.Append(lsb);
            }
            return Convert.ToByte(random8Bit.ToString(), 2);
        }
    }
}