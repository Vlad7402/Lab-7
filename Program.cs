using System;
using System.Collections.Generic;
using System.IO;

namespace Lab_7
{
    class Program
    {
        static void Main(string[] args)
        {
            //Это просто полежит здесь
            //string[] files = Directory.GetFiles(Environment.CurrentDirectory);
            string equation = GetEquation();
            if (!AreAnyMistakesInEquation(equation)) GetResult(equation);
            else File.WriteAllText("output.txt", "Ошибка в записи уравнения");
        }
        static string GetEquation()
        {
            string input = File.ReadAllText("input.txt", System.Text.Encoding.UTF8);
            string[] splitedInput = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string result = "";
            for (int i = 0; i < splitedInput.Length; i++)
            {
                result += splitedInput[i];
            }
            return result;
        }
        static bool AreAnyMistakesInEquation (string input)
        {
            bool result = false;
            if (input.Length < 3 ||
               (!char.IsDigit(input[0]) && input[0] != '-') ||
               !char.IsDigit(input[input.Length - 1]) ||
               AreForbiddenSymbols(input)
               ) 
            { result = true; }
            else
            {
                for (int i = 1; i < input.Length; i++)
                {
                    if (!char.IsDigit(input[i]) && !char.IsDigit(input[i - 1]))
                    {
                        if (IsPrefix(input, i))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }
        static bool IsPrefix (string input, int operationID)
        {
            bool result = false;
            if (operationID != 0)
            {
                if (operationID != (input.Length - 1))
                {
                    if (!char.IsDigit(input[operationID + 1])) result = true;
                }
            }
            else result = true;
            return result;
        }
        static float GetNum (string input, int numStart)
        {
            string numInString = "";
            numInString += input[numStart];
            char nextSymbol = '*';
            if ((input.Length - 1) != numStart) nextSymbol = input[numStart + 1];
            int counter = 1;
            while (char.IsDigit(nextSymbol))
            {
                numInString += nextSymbol;
                counter++;
                if (input.Length != numStart + counter) nextSymbol = input[numStart + counter];
                else break;
            }
            float result = float.Parse(numInString);
            return result;
        }
        static float[] GetAllNums (string input)
        {
            List<float> nums = new List<float>();
            int counter = 0;
            nums.Add(GetNum(input, counter));
            counter++;
            while (true)
            {
                if (input.Length - 1 <= counter) break;
                while (char.IsDigit(input[counter]))
                {
                    if (input.Length - 1 != counter) counter++;
                    else break;
                }
                if (input.Length - 1 == counter) break;
                nums.Add(GetNum(input, counter + 1));
                if (!IsPrefix(input, counter + 1)) counter += 2;
                else counter++;
            }
            float[] result = new float[nums.Count];
            for (int i = 0; i < nums.Count; i++)
            {
                result[i] = nums[i];
            }
            return result;
        }
        static bool AreForbiddenSymbols (string input)
        {
            bool result = false;
            for (int i = 0; i < input.Length; i++)
            {
                if (!char.IsDigit(input[i]) && 
                        (input[i] != '*' && 
                         input[i] != '-' &&
                         input[i] != '+' &&
                         input[i] != '/'))
                {
                    result = true;
                }
            }
            return result;
        }
        static string GetAllOperations (string input)
        {
            string result = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (!char.IsDigit(input[i]) && input[i] != '-') result += input[i];
                else if (input[i] == '-' && !IsPrefix(input, i))
                {
                    result += input[i];
                }
            }
            return result;
        }
        static float Sum (float firstNum, float secNum)
        {
            return firstNum + secNum;
        }
        static float Substraction (float firstNum, float secNum)
        {
            return firstNum - secNum;
        }
        static float Multiplecation (float firstNum, float secNum)
        {
            return firstNum * secNum;
        }
        static bool IsDevisionAvaliable (float secNum)
        {
            if (secNum != 0) return true;
            else
            {
                File.WriteAllText("output.txt", "Ошибка: деление на 0");
                Environment.Exit(0);
                return false;
            }
        }
        static float Devision (float firstNum, float secNum)
        {
            IsDevisionAvaliable(secNum);
            return firstNum / secNum;
        }
        static float GetOperationResult (float firstNum, float secNum, char operation)
        {
            float result = 0;
            if (operation == '+') result = Sum(firstNum, secNum);
            if (operation == '-') result = Substraction(firstNum, secNum);
            if (operation == '*') result =  Multiplecation(firstNum, secNum);
            if (operation == '/') result = Devision(firstNum, secNum);
            return result;
        }
        static void GetResult(string input)
        {
            List<float> nums = new List<float>();
            nums.AddRange(GetAllNums(input));
            List<char> operations = new List<char>();
            operations.AddRange(GetAllOperations(input));
            while (nums.Count > 1)
            {
                if (GetOperationID(operations.IndexOf('*'), operations.IndexOf('/')) >= 0)
                {
                    int operationID = GetOperationID(operations.IndexOf('*'), operations.IndexOf('/'));
                    float result = GetOperationResult(nums[operationID], nums[operationID + 1], operations[operationID]);
                    nums.RemoveAt(operationID);
                    nums.RemoveAt(operationID);
                    nums.Insert(operationID, result);
                    operations.RemoveAt(operationID);
                }
                else
                {
                    int operationID = GetOperationID(operations.IndexOf('+'), operations.IndexOf('-'));
                    float result = GetOperationResult(nums[operationID], nums[operationID + 1], operations[operationID]);
                    nums.RemoveAt(operationID);
                    nums.RemoveAt(operationID);
                    nums.Insert(operationID, result);
                    operations.RemoveAt(operationID);
                }
            }
            File.WriteAllText("output.txt", nums[0].ToString());
        }
        static int GetOperationID (int firstOperationID, int secOperationID)
        {
            int result = -1;
            if (secOperationID == -1) result = firstOperationID;
            else if (firstOperationID == -1) result = secOperationID;
            else if (firstOperationID < secOperationID) result = firstOperationID;
            else if (secOperationID < firstOperationID) result = secOperationID;
            return result;
        }
    }
}
