using System.Diagnostics;
using System.Net;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Marketplace.Core.Models;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Marketplace.Test.Infrastructure;

/// <summary>
///     Test fixture for S3 integration testing with Garage S3 service
///     Manages test bucket lifecycle and provides test file utilities
/// </summary>
public class S3TestFixture : IAsyncLifetime
{
    private const string ContainerName = "garage";
    private static readonly SemaphoreSlim InitializationSemaphore = new(1, 1);
    private static bool _isInitialized;

    private readonly S3Configuration _config;
    private readonly ILogger<S3TestFixture> _logger;

    public S3TestFixture(S3Configuration config, ILogger<S3TestFixture> logger)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public static S3Configuration TestS3Config { get; private set; } = null!;
    private static AmazonS3Client? TestS3Client { get; set; }
    private static string TestBucketName => TestS3Config.BucketName;

    public async Task InitializeAsync()
    {
        await InitializationSemaphore.WaitAsync();
        try
        {
            if (_isInitialized)
            {
                _logger.LogInformation("S3 test fixture already initialized, re-using existing client...");

                // Even if already initialized, we MUST clean the bucket for test isolation
                // This ensures each test class instance starts with a clean S3 state
                _logger.LogInformation("Cleaning S3 bucket for test isolation...");
                await CleanupTestBucketAsync();

                return;
            }

            _logger.LogInformation("Starting S3 test fixture initialization...");
            await EnsureGarageContainerIsRunningAsync();

            // Initialize S3 configuration from the actual config
            TestS3Config = new S3Configuration
            {
                ServiceUrl = _config.ServiceUrl,
                AccessKey = _config.AccessKey,
                SecretKey = _config.SecretKey,
                Region = _config.Region,
                BucketName = _config.BucketName
            };

            // Initialize S3 client
            var s3Config = new AmazonS3Config
            {
                ServiceURL = TestS3Config.ServiceUrl,
                ForcePathStyle = true,
                UseHttp = true,
                AuthenticationRegion = TestS3Config.Region
            };

            TestS3Client = new AmazonS3Client(TestS3Config.AccessKey, TestS3Config.SecretKey, s3Config);

            _logger.LogInformation("S3 client initialized, ensuring test bucket exists...");
            await EnsureTestBucketExistsAsync();

            // Clean bucket after initial creation to ensure clean state
            _logger.LogInformation("Cleaning S3 bucket after initialization...");
            await CleanupTestBucketAsync();

            _logger.LogInformation("S3 test fixture initialization complete.");

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
            // Clean up test files after each test class completes
            // The static S3 client remains alive for the next test class to use
            await CleanupTestBucketAsync();
            _logger.LogInformation("S3TestFixture disposed, bucket cleaned for next test");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cleanup test bucket during dispose: {Message}", ex.Message);
        }
        // Note: We don't dispose TestS3Client since it's static and shared across test instances
        // It will be disposed when the test process terminates
    }

    /// <summary>
    ///     Creates a test file stream with specified content
    /// </summary>
    public static MemoryStream CreateTestFileStream(string content = "Test file content for integration testing")
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        return new MemoryStream(bytes);
    }

    /// <summary>
    ///     Creates a test file stream with binary content (simulates image/video)
    /// </summary>
    public static MemoryStream CreateTestBinaryFileStream(int sizeInBytes = 1024)
    {
        var bytes = new byte[sizeInBytes];
        new Random().NextBytes(bytes);
        return new MemoryStream(bytes);
    }

    /// <summary>
    ///     Uploads a test file directly to S3 for testing download scenarios
    /// </summary>
    public async Task<string> UploadTestFileAsync(string fileName, Stream content)
    {
        if (TestS3Client == null)
            throw new InvalidOperationException(
                "TestS3Client is not initialized. Ensure InitializeAsync has been called.");

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
    ///     Checks if an S3 object exists in the specified bucket
    /// </summary>
    public async Task<bool> DoesS3ObjectExistAsync(string bucketName, string key)
    {
        if (TestS3Client == null)
            throw new InvalidOperationException(
                "TestS3Client is not initialized. Ensure InitializeAsync has been called.");

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
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    /// <summary>
    ///     Cleans up all test files from the bucket
    /// </summary>
    private async Task CleanupTestBucketAsync()
    {
        if (TestS3Client == null)
        {
            _logger.LogWarning("TestS3Client is null, skipping cleanup");
            return;
        }

        try
        {
            // Clean up all objects in the test bucket (both test-files/ and products/)
            var listRequest = new ListObjectsV2Request
            {
                BucketName = TestBucketName
                // No prefix - delete ALL objects in bucket for complete cleanup
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
                _logger.LogInformation("Cleaned up {Count} test files from S3 (including products/* and test-files/*)",
                    listResponse.S3Objects.Count);
            }
            else
            {
                _logger.LogInformation("No files to clean up from S3 bucket");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cleanup test files: {Message}", ex.Message);
        }
    }

    private static async Task<bool> TryConnectToGarageAsync()
    {
        try
        {
            using var httpClient = new HttpClient();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var response = await httpClient.GetAsync("http://localhost:3900", cts.Token);
            return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Forbidden;
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
            throw new InvalidOperationException(
                $"Failed to start Garage container. Exit code: {composeProcess.ExitCode}");
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
            await TestS3Client!.HeadBucketAsync(bucketRequest);
            _logger.LogInformation("Test bucket '{BucketName}' already exists.", TestBucketName);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Bucket doesn't exist, create it
            _logger.LogInformation("Creating test bucket '{BucketName}'...", TestBucketName);
            await TestS3Client!.PutBucketAsync(new PutBucketRequest
            {
                BucketName = TestBucketName,
                UseClientRegion = true
            });
            _logger.LogInformation("Test bucket '{BucketName}' created successfully.", TestBucketName);
        }
    }
}