using System.Diagnostics;
using Amazon.S3;
using Amazon.S3.Model;
using Marketplace.Core.Models;
using Xunit;

namespace Marketplace.Test.Infrastructure;

/// <summary>
/// Test fixture for S3 integration testing with Garage S3 service
/// Manages test bucket lifecycle and provides test file utilities
/// </summary>
public class S3TestFixture : IAsyncLifetime
{
    private const string ContainerName = "garage";
    private const string TestBucketName = "test-marketplace";
    private static readonly SemaphoreSlim InitializationSemaphore = new(1, 1);
    private static bool _isInitialized;
    
    public S3Configuration TestS3Config { get; private set; } = null!;
    private AmazonS3Client TestS3Client { get; set; } = null!;

    public async Task InitializeAsync()
    {
        await InitializationSemaphore.WaitAsync();
        try
        {
            if (_isInitialized)
            {
                Console.WriteLine("S3 test fixture already initialized, skipping...");
                return;
            }

            Console.WriteLine("Starting S3 test fixture initialization...");
            await EnsureGarageContainerIsRunningAsync();
            
            // Initialize S3 configuration
            TestS3Config = new S3Configuration
            {
                ServiceUrl = "http://localhost:3900",
                AccessKey = "GKe47111f5fec42f2be1044b88",
                SecretKey = "714f830382165e223dc35167bce223e3d48654903feef5297fe9cb9953e5affe",
                BucketName = TestBucketName
            };
            
            // Initialize S3 client
            var s3Config = new AmazonS3Config
            {
                ServiceURL = TestS3Config.ServiceUrl,
                ForcePathStyle = true,
                UseHttp = true,
            };
            
            TestS3Client = new AmazonS3Client(TestS3Config.AccessKey, TestS3Config.SecretKey, s3Config);
            
            Console.WriteLine("S3 client initialized, ensuring test bucket exists...");
            await EnsureTestBucketExistsAsync();
            Console.WriteLine("S3 test fixture initialization complete.");

            _isInitialized = true;
        }
        finally
        {
            InitializationSemaphore.Release();
        }
    }

    public async Task DisposeAsync()
    {
        try
        {
            // Clean up test files
            await CleanupTestBucketAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to cleanup test bucket: {ex.Message}");
        }
        finally
        {
            TestS3Client.Dispose();
        }
    }

    /// <summary>
    /// Creates a test file stream with specified content
    /// </summary>
    public static MemoryStream CreateTestFileStream(string content = "Test file content for integration testing")
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        return new MemoryStream(bytes);
    }

    /// <summary>
    /// Creates a test file stream with binary content (simulates image/video)
    /// </summary>
    public static MemoryStream CreateTestBinaryFileStream(int sizeInBytes = 1024)
    {
        var bytes = new byte[sizeInBytes];
        new Random().NextBytes(bytes);
        return new MemoryStream(bytes);
    }

    /// <summary>
    /// Uploads a test file directly to S3 for testing download scenarios
    /// </summary>
    public async Task<string> UploadTestFileAsync(string fileName, Stream content)
    {
        var objectKey = $"test-files/{fileName}";
        
        var request = new PutObjectRequest
        {
            BucketName = TestBucketName,
            Key = objectKey,
            InputStream = content,
            ContentType = "application/octet-stream"
        };

        await TestS3Client.PutObjectAsync(request);
        return objectKey;
    }

    /// <summary>
    /// Cleans up all test files from the bucket
    /// </summary>
    private async Task CleanupTestBucketAsync()
    {
        try
        {
            var listRequest = new ListObjectsV2Request
            {
                BucketName = TestBucketName,
                Prefix = "test-files/"
            };

            var listResponse = await TestS3Client.ListObjectsV2Async(listRequest);
            
            if (listResponse.S3Objects.Count > 0)
            {
                var deleteRequest = new DeleteObjectsRequest
                {
                    BucketName = TestBucketName,
                    Objects = listResponse.S3Objects.ConvertAll(obj => new KeyVersion { Key = obj.Key })
                };

                await TestS3Client.DeleteObjectsAsync(deleteRequest);
                Console.WriteLine($"Cleaned up {listResponse.S3Objects.Count} test files from S3");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to cleanup test files: {ex.Message}");
        }
    }

    /// <summary>
    /// Checks if an S3 object exists in the specified bucket
    /// </summary>
    public async Task<bool> DoesS3ObjectExistAsync(string bucketName, string key)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = key
            };
            
            await TestS3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    private static async Task<bool> TryConnectToGarageAsync()
    {
        try
        {
            using var httpClient = new HttpClient();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var response = await httpClient.GetAsync("http://localhost:3900", cts.Token);
            return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Forbidden;
        }
        catch
        {
            return false;
        }
    }

    private static async Task EnsureGarageContainerIsRunningAsync()
    {
        // First, try to connect directly to Garage
        if (await TryConnectToGarageAsync())
        {
            Console.WriteLine("Garage S3 service is already accessible, no need to start container.");
            return;
        }

        // Check if container is running
        var checkProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = $"ps -q -f name={ContainerName}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        checkProcess.Start();
        var output = await checkProcess.StandardOutput.ReadToEndAsync();
        await checkProcess.WaitForExitAsync();

        if (string.IsNullOrWhiteSpace(output))
        {
            Console.WriteLine($"Container {ContainerName} is not running, attempting to start it...");
            await StartGarageContainerAsync();
        }
        else
        {
            Console.WriteLine($"Container {ContainerName} is already running with ID: {output.Trim()}");
        }

        // Wait for Garage to be ready
        await WaitForGarageAsync();
    }

    private static async Task StartGarageContainerAsync()
    {
        // Use docker-compose to start the garage service
        var composeProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker-compose",
                Arguments = "up -d garage",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        composeProcess.Start();
        var output = await composeProcess.StandardOutput.ReadToEndAsync();
        var error = await composeProcess.StandardError.ReadToEndAsync();
        await composeProcess.WaitForExitAsync();

        if (composeProcess.ExitCode != 0)
        {
            Console.WriteLine($"Docker compose output: {output}");
            Console.WriteLine($"Docker compose error: {error}");
            throw new InvalidOperationException($"Failed to start Garage container. Exit code: {composeProcess.ExitCode}");
        }
    }

    private static async Task WaitForGarageAsync()
    {
        const int maxAttempts = 30;
        var attempt = 0;

        while (attempt < maxAttempts)
        {
            if (await TryConnectToGarageAsync())
            {
                Console.WriteLine("Garage S3 service is ready!");
                return;
            }

            Console.WriteLine($"Garage not ready (attempt {attempt + 1}/{maxAttempts}), waiting...");
            attempt++;
            await Task.Delay(2000);
        }

        throw new InvalidOperationException("Garage container did not start within expected time.");
    }

    private async Task EnsureTestBucketExistsAsync()
    {
        try
        {
            // Check if bucket exists
            var bucketRequest = new HeadBucketRequest
            {
                BucketName = TestBucketName
            };
            await TestS3Client.HeadBucketAsync(bucketRequest);
            Console.WriteLine($"Test bucket '{TestBucketName}' already exists.");
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Bucket doesn't exist, create it
            Console.WriteLine($"Creating test bucket '{TestBucketName}'...");
            await TestS3Client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = TestBucketName,
                UseClientRegion = true
            });
            Console.WriteLine($"Test bucket '{TestBucketName}' created successfully.");
        }
    }
}