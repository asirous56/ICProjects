using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace InvestCloudServerSide
{
    public class MatrixMultiplicationService
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        private void StartTimer()
        {
            stopwatch.Restart();
        }

        public void StopAndPrintTimer(string taskName)
        {
            stopwatch.Stop();
            Console.WriteLine($"{taskName} took {stopwatch.ElapsedMilliseconds} ms");
        }

        public async Task Initialize(int size)
        {
            await new HttpClient().GetAsync($"https://recruitment-test.investcloud.com/api/numbers/init/{size}");
        }

        public async Task<int[]> GetRowAsync(string matrixName, int idx)
        {
            var response = await new HttpClient().GetStringAsync($"https://recruitment-test.investcloud.com/api/numbers/{matrixName}/row/{idx}");
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(response);
            if (apiResponse == null)
                return new int[] { };
            else
                return apiResponse.Value ;
        }

        public async Task<int[,]> GetMatrixAsync(string matrixName, int size)
        {
            var tasks = new Task<int[]>[size];

            for (int i = 0; i < size; i++)
            {
                tasks[i] = GetRowAsync(matrixName, i);
            }

            var rows = await Task.WhenAll(tasks);

            int[,] matrix = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = rows[i][j];
                }
            }

            return matrix;
        }

        public double[,] MultiplyMatricesParallel(int[,] matrixA, int[,] matrixB)
        {
            int n = matrixA.GetLength(0);
            double[,] result = new double[n, n];

            Parallel.For(0, n, i =>
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < n; k++)
                    {
                        result[i, j] += matrixA[i, k] * matrixB[k, j];
                    }
                }
            });

            return result;
        }

        public string ConvertMatrixToString(double[,] matrix)
        {
            StringBuilder sb = new StringBuilder();
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    sb.Append(matrix[row, col]);
                }
            }

            return sb.ToString();
        }

        public string ComputeMd5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes); // Use Base64 encoding instead of Hex
            }
        }

        public async Task SubmitHashAsync(string hash)
        {
            var content = new StringContent($"\"{hash}\"", Encoding.UTF8, "application/json");
            Console.WriteLine(content.ReadAsStringAsync().Result);
            var response = await new HttpClient().PostAsync("https://recruitment-test.investcloud.com/api/numbers/validate", content);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        public async Task Run(int size)
        {
            // Step 1: Initialize datasets
            await Initialize(size);
            StartTimer();

            // Step 2: Fetch matrices A and B in parallel
            var taskA = GetMatrixAsync("A", size);
            var taskB = GetMatrixAsync("B", size);
            await Task.WhenAll(taskA, taskB);
            int[,] matrixA = await taskA;
            int[,] matrixB = await taskB;
            if (matrixA.Length == 0 || matrixB.Length == 0)
            {
                Console.WriteLine("matrix is invalid, try again");
                return;
            }

            // Step 3: Perform matrix multiplication
            var result = MultiplyMatricesParallel(matrixA, matrixB);

            // Step 4: Convert the result matrix to a string and compute MD5 hash
            string resultString = ConvertMatrixToString(result);
            string resultHash = ComputeMd5Hash(resultString);

            // Step 5: Submit the hash
            await SubmitHashAsync(resultHash);
            StopAndPrintTimer("Hash Submission");
        }

    }
}


