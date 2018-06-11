
namespace CareFusion.Lib.StorageSystem.Xml
{
    /// <summary>
    /// Class which implements some useful extension methods for byte arrays.
    /// </summary>
    internal static class ByteArrayExtensions
    {
        /// <summary>
        /// Processes a case sensitive search of one byte array within a second one and
        /// returns the zero based index if the second array has been found. 
        /// Tests have proven that this method is way faster than the generic Array.BinarySearch approach.
        /// </summary>
        /// <param name="sourceArray">The source array to search in.</param>
        /// <param name="searchArray">The array to search for.</param>
        /// <returns>Zero based index if the array has been found;<c>-1</c> otherwise.</returns>
        public static int GetIndexOf(this byte[] sourceArray, byte[] searchArray)
        {
            int result = -1;

            for (int i = 0, max = sourceArray.Length - searchArray.Length; i <= max; ++i)
            {
                result = i;

                for (int k = 0; k < searchArray.Length; ++k)
                {
                    if (searchArray[k] != sourceArray[i + k])
                    {
                        result = -1;
                        break;
                    }
                }

                if (result >= 0)
                {
                    break;
                }
            }

            return result;
        }
    }
}
