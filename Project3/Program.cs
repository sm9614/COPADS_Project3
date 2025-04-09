
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
                    Console.WriteLine(Encrypt(plaintext));
                    break;

                case "decrypt":
                    if (args.Length != 2)
                    {
                        Console.WriteLine("Decrypt takes 2 parameters <Decrypt> <ciphertext>");
                        return;
                    }
                    string ciphertext = args[1];
                    Console.WriteLine(Decrypt(ciphertext));
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
                    Console.WriteLine(TripleBits(seed, tap, step, iteration));
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
                    Console.WriteLine(EncryptImage(imagefile, seed, tap));
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

        private static bool EncryptImage(string imagefile, string seed, int tap)
        {
            throw new NotImplementedException();
        }

        private static bool TripleBits(string seed, int tap, int step, int iteration)
        {
            throw new NotImplementedException();
        }

        private static bool Decrypt(string ciphertext)
        {
            throw new NotImplementedException();
        }

        private static bool Encrypt(string plaintext)
        {
            throw new NotImplementedException();
        }

        private static void GenerateKeyStream(string seed, int tap, int step)
        {
            Console.WriteLine(seed + " -seed");
            List<int> keystream = new List<int>();
            for (int i = 0; i < step; i++)
            {
                seed = Cipher(seed, tap);
                int lsb = seed[seed.Length - 1] - '0'; // - '0' converts the ascii number to the actual value
                Console.WriteLine(seed + " " + lsb);
                keystream.Add(lsb);
            }
            File.WriteAllText("keystream.txt", string.Join("", keystream));
            Console.WriteLine("The Keystream: " + string.Join("", keystream));

        }

        private static string Cipher(string seed, int tap)
        {

            int value = seed[tap]  - '0'; // - '0' converts the ascii number to the actual value
            int msb = seed[0] - '0';
            int newValue = value ^ msb;
            string newSeed = seed.Substring(1);
            newSeed += newValue.ToString();
            return newSeed;

        }
    }
}