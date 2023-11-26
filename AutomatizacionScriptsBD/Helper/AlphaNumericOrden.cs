namespace AutomatizacionScriptsBD.Helper
{
    public class AlphaNumericOrden
    {
        public static int AlphanumericCompare(string x, string y)
        {
            string[] partsX = SplitAlphaNumeric(x);
            string[] partsY = SplitAlphaNumeric(y);

            int length = Math.Min(partsX.Length, partsY.Length);
            for (int i = 0; i < length; i++)
            {
                if (partsX[i] != partsY[i])
                {
                    if (IsNumeric(partsX[i]) && IsNumeric(partsY[i]))
                    {
                        int numX = int.Parse(partsX[i]);
                        int numY = int.Parse(partsY[i]);
                        return numX.CompareTo(numY);
                    }
                    else
                    {
                        return partsX[i].CompareTo(partsY[i]);
                    }
                }
            }

            return partsX.Length.CompareTo(partsY.Length);
        }
        private static string[] SplitAlphaNumeric(string input)
        {
            return System.Text.RegularExpressions.Regex.Split(input.Replace(" ", ""), "([0-9]+)");
        }
        private static bool IsNumeric(string input)
        {
            return int.TryParse(input, out _);
        }
    }
}
