namespace SecuredActions
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
                return;

            switch (args[0])
            {
                case Common.Constants.Copy:
                    {
                        Common.FileActions.Copy(args[1]);
                        break;
                    }
                case Common.Constants.Hash:
                    {
                        Common.FileActions.Hash(args[1]);
                        break;
                    }
                case Common.Constants.Zip:
                    {
                        Common.FileActions.Zip(args[1]);
                        break;
                    }
            }
        }
    }
}