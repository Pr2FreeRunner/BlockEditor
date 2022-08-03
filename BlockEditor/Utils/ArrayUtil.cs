using System;

namespace BlockEditor.Utils
{
    public class ArrayUtil
    {

        public static T[,] CreateSquare<T>(T[,] matrix)
        {
            if (matrix == null)
                return null;

            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);
            var size = Math.Max(width, height);
            var result = new T[size, size];

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    if (i >= height || j >= width)
                        result[i, j] = default(T);
                    else
                        result[i, j] = matrix[i, j];
                }
            }

            return result;
        }

        public static T[,] RotateLeft<T>(T[,] matrix)
        {
            if (matrix == null)
                return null;

            var square = CreateSquare(matrix);
            var size = square.GetLength(0);
            var result = new T[size, size];

            for (int i = 0; i < size; ++i)
                for (int j = 0; j < size; ++j)
                    result[i, j] = square[size - j - 1, i];

            return result;
        }

        public static T[,] MinimizeSize<T>(T[,] matrix)
        {
            if (matrix == null)
                return null;

            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);

            var startRow = 0;
            var startColumn = 0;
            var endRow = height;
            var endColumn = width;

            for (int row = 0; row < height; row++)
            {
                bool any = false;
                for (int column = 0; column < width; column++)
                {
                    any = matrix[row, column] != null;

                    if (any)
                        break;
                }

                if (any)
                    break;
                else
                    startRow++;
            }

            for (int column = 0; column < width; column++)
            {
                bool any = false;

                for (int row = 0; row < height; row++)
                {
                    any = matrix[row, column] != null;

                    if (any)
                        break;
                }

                if (any)
                    break;
                else
                    startColumn++;
            }

            for (int row = height - 1; row >= 0; row--)
            {
                bool any = false;

                for (int column = 0; column < width; column++)
                {
                    any = matrix[row, column] != null;

                    if (any)
                        break;
                }

                if (any)
                    break;
                else
                    endRow--;
            }

            for (int column = width - 1; column >= 0; column--)
            {
                bool any = false;

                for (int row = 0; row < height; row++)
                {
                    any = matrix[row, column] != null;

                    if (any)
                        break;
                }

                if (any)
                    break;
                else
                    endColumn--;
            }

            return CreateSubArray(matrix, startRow, startColumn, endRow, endColumn);
        }


        public static T[,] CreateSubArray<T>(T[,] matrix, int startRow, int startColumn, int endRow, int endColumn)
        {
            if (matrix == null)
                return null;

            var newHeight = endRow - startRow;
            var newColumn = endColumn - startColumn;

            if (newHeight < 0 || newColumn < 0)
                return null;

            if (endRow > matrix.GetLength(0) || endColumn > matrix.GetLength(1))
                return null;

            var result = new T[newHeight, newColumn];

            for (int i = startRow; i < endRow; i++)
                for (int j = startColumn; j < endColumn; j++)
                    result[i - startRow, j - startColumn] = matrix[i, j];

            return result;
        }

    }
}
