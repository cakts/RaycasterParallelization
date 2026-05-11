using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Raytracer
{
    class Program
    {
        // Render settings
        private static double vfov = 40;
        private static int imageWidth = 400;
        private static int imageHeight = 225;
        private static double focusDist = 10.0;
        private static int samplesPerPixel = 20;
        private static int maxDepth = 20;
        private static int? seed = null; // null = random seed

        // Output settings
        private static int numberOfTests = 1;
        private static bool showPpmFile = true;
        private static string outputFile = "render.ppm";

        // Camera position
        private static Point3 lookFrom = new Point3(13, 4, 3);
        private static Point3 lookAt = new Point3(0, 0, 0);
        private static Vec3 vUp = new Vec3(0, 1, 0);
        private static double defocusAngle = 0.6;

        static void Main(string[] args)
        {
            while (true)
            {
                DisplayMenu();

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    RunTests();
                }
                else if (choice == "2")
                {
                    ChangeSettings();
                }
                else if (choice == "3")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice.\n");
                }
            }
        }

        static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("\n--- Render Settings ---");
            Console.WriteLine($"FOV: {vfov}");
            Console.WriteLine($"Image Width: {imageWidth}");
            Console.WriteLine($"Image Height: {imageHeight}");
            Console.WriteLine($"Focus Distance: {focusDist}");
            Console.WriteLine($"Samples Per Pixel: {samplesPerPixel}");
            Console.WriteLine($"Max Depth: {maxDepth}");
            Console.WriteLine($"Seed: {(seed.HasValue ? seed.Value.ToString() : "Random")}");

            Console.WriteLine("\n--- Output Settings ---");
            Console.WriteLine($"Number of Tests: {numberOfTests}");
            Console.WriteLine($"Show PPM File: {(showPpmFile ? "Yes" : "No")}");

            Console.WriteLine("\n1: Start Testing");
            Console.WriteLine("2: Change Settings");
            Console.WriteLine("3: Exit");
            Console.Write("\nSelect option: ");
        }

        static void ChangeSettings()
        {
            Console.Clear();
            Console.WriteLine("\n--- Change Settings ---");
            Console.WriteLine("\nRender Settings:");
            Console.WriteLine("1: FOV");
            Console.WriteLine("2: Image Width");
            Console.WriteLine("3: Image Height");
            Console.WriteLine("4: Focus Distance");
            Console.WriteLine("5: Samples Per Pixel");
            Console.WriteLine("6: Max Depth");
            Console.WriteLine("7: Seed");

            Console.WriteLine("\nOutput Settings:");
            Console.WriteLine("8: Number of Tests");
            Console.WriteLine("9: Show PPM File");

            Console.WriteLine("\n0: Back to Main Menu");
            Console.Write("\nSelect setting to change: ");

            string choice = Console.ReadLine();

            Console.Clear();

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Write("Enter FOV: ");
                        vfov = double.Parse(Console.ReadLine());
                        break;
                    case "2":
                        Console.Write("Enter Image Width: ");
                        imageWidth = int.Parse(Console.ReadLine());
                        break;
                    case "3":
                        Console.Write("Enter Image Height: ");
                        imageHeight = int.Parse(Console.ReadLine());
                        break;
                    case "4":
                        Console.Write("Enter Focus Distance: ");
                        focusDist = double.Parse(Console.ReadLine());
                        break;
                    case "5":
                        Console.Write("Enter Samples Per Pixel: ");
                        samplesPerPixel = int.Parse(Console.ReadLine());
                        break;
                    case "6":
                        Console.Write("Enter Max Depth: ");
                        maxDepth = int.Parse(Console.ReadLine());
                        break;
                    case "7":
                        Console.Write("Enter Seed (leave empty for random): ");
                        string seedInput = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(seedInput))
                        {
                            seed = null;
                        }
                        else
                        {
                            seed = int.Parse(seedInput);
                        }
                        break;
                    case "8":
                        Console.Write("Enter Number of Tests: ");
                        numberOfTests = int.Parse(Console.ReadLine());
                        break;
                    case "9":
                        Console.Write("Show PPM File? (y/n): ");
                        showPpmFile = Console.ReadLine().ToLower().StartsWith("y");
                        break;
                    case "0":
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void RunTests()
        {
            Console.Clear();
            Console.WriteLine($"\n=== Starting {numberOfTests} Test(s) ===\n");

            List<TestResult> results = new List<TestResult>();

            for (int testNum = 1; testNum <= numberOfTests; testNum++)
            {
                Console.WriteLine($"Running Test {testNum}/{numberOfTests}...");

                // Determine seed for world generation
                int actualSeed = seed ?? (int)DateTime.Now.Ticks + testNum;

                Console.WriteLine($"Using seed: {actualSeed}");

                // Build the world with the specified seed
                HittableList world = BuildWorld(actualSeed);

                // Setup camera
                Camera cam = new Camera();
                double aspectRatio = (double)imageWidth / imageHeight;
                cam.AspectRatio = aspectRatio;
                cam.ImageWidth = imageWidth;
                cam.SamplesPerPixel = samplesPerPixel;
                cam.MaxDepth = maxDepth;
                cam.VFov = vfov;
                cam.LookFrom = lookFrom;
                cam.LookAt = lookAt;
                cam.VUp = vUp;
                cam.DefocusAngle = defocusAngle;
                cam.FocusDist = focusDist;

                // Start timer
                long startTime = Stopwatch.GetTimestamp();

                // Render
                cam.Render(world, outputFile);

                // Stop timer
                TimeSpan elapsedTime = Stopwatch.GetElapsedTime(startTime);

                Console.WriteLine($"Test {testNum} complete: {elapsedTime.TotalSeconds:F3}s\n");

                // Store result
                results.Add(new TestResult
                {
                    TestNumber = testNum,
                    ExecutionTime = elapsedTime.TotalSeconds,
                    OutputFile = outputFile,
                    ActualSeed = actualSeed,
                    Settings = GetCurrentSettings()
                });

                // Show/refresh PPM file after each test if enabled
                if (showPpmFile)
                {
                    try
                    {
                        PpmViewer.Show(outputFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Could not display image: {ex.Message}");
                    }
                }
            }

            Console.Clear();

            // Save results to CSV
            SaveResultsToCsv(results);

            // Display summary
            DisplaySummary(results);
        }

        static HittableList BuildWorld(int seed)
        {
            Random random = new Random(seed);
            HittableList world = new HittableList();

            // Ground
            Material groundMaterial = new Lambertian(new Color(0.5, 0.5, 0.5));
            world.Add(new Sphere(new Point3(0, -1000, 0), 1000, groundMaterial));

            // Random small spheres
            int materialIndex = 0;
            for (int a = -8; a < 8; a++)
            {
                for (int b = -8; b < 8; b++)
                {
                    double radius = random.NextDouble() * (1.2 - 0.25) + 0.25;
                    Point3 center = new Point3(
                        a * 2 + (random.NextDouble() * 0.2 - 0.1) * (1.2 - radius),
                        radius + random.NextDouble(),
                        b * 2 + (random.NextDouble() * 0.2 - 0.1) * (1.2 - radius)
                    );

                    Material sphereMaterial;
                    int matType = materialIndex % 3;
                    materialIndex++;

                    if (matType == 0)
                    {
                        // Diffuse
                        Vec3 albedoVec = new Vec3(random.NextDouble(), random.NextDouble(), random.NextDouble());
                        Color albedo = new Color(albedoVec.X(), albedoVec.Y(), albedoVec.Z());
                        sphereMaterial = new Lambertian(albedo);
                    }
                    else if (matType == 1)
                    {
                        // Metal
                        Vec3 albedoVec = new Vec3(random.NextDouble() * 0.5 + 0.5,
                                                  random.NextDouble() * 0.5 + 0.5,
                                                  random.NextDouble() * 0.5 + 0.5);
                        Color albedo = new Color(albedoVec.X(), albedoVec.Y(), albedoVec.Z());
                        double fuzz = random.NextDouble() * 0.5;
                        sphereMaterial = new Metal(albedo, fuzz);
                    }
                    else
                    {
                        // Glass
                        sphereMaterial = new Dielectric(1.5);
                    }

                    world.Add(new Sphere(center, radius, sphereMaterial));
                }
            }

            return world;
        }

        static Dictionary<string, string> GetCurrentSettings()
        {
            return new Dictionary<string, string>
            {
                { "VFov", vfov.ToString() },
                { "ImageWidth", imageWidth.ToString() },
                { "ImageHeight", imageHeight.ToString() },
                { "FocusDist", focusDist.ToString("F2") },
                { "SamplesPerPixel", samplesPerPixel.ToString() },
                { "MaxDepth", maxDepth.ToString() },
                { "Seed", seed.HasValue ? seed.Value.ToString() : "Random" }
            };
        }

        static void SaveResultsToCsv(List<TestResult> results)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string csvFile = $"test_results_{timestamp}.csv";

            try
            {
                using (StreamWriter writer = new StreamWriter(csvFile))
                {
                    writer.Write("TestNumber,ExecutionTime(s),OutputFile");
                    if (results.Count > 0)
                    {
                        foreach (var setting in results[0].Settings.Keys)
                        {
                            writer.Write($",{setting}");
                        }
                    }
                    writer.WriteLine();

                    foreach (var result in results)
                    {
                        writer.Write($"{result.TestNumber},{result.ExecutionTime:F3},{result.OutputFile}");
                        foreach (var setting in result.Settings.Values)
                        {
                            writer.Write($",{setting}");
                        }
                        writer.WriteLine();
                    }
                }

                Console.WriteLine($"\nResults saved to: {csvFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError saving CSV: {ex.Message}");
            }
        }

        static void DisplaySummary(List<TestResult> results)
        {
            Console.WriteLine("\n--- Test Summary ---");
            Console.WriteLine($"Total Tests: {results.Count}");

            if (results.Count > 0)
            {
                double avgTime = results.Average(r => r.ExecutionTime);
                double minTime = results.Min(r => r.ExecutionTime);
                double maxTime = results.Max(r => r.ExecutionTime);

                Console.WriteLine($"Average Time: {avgTime:F3}s");
                Console.WriteLine($"Min Time: {minTime:F3}s");
                Console.WriteLine($"Max Time: {maxTime:F3}s");

                if (results.Count > 1)
                {
                    double stdDev = Math.Sqrt(results.Average(r => Math.Pow(r.ExecutionTime - avgTime, 2)));
                    Console.WriteLine($"Std Dev: {stdDev:F3}s");
                }
            }
        }
    }

    class TestResult
    {
        public int TestNumber { get; set; }
        public double ExecutionTime { get; set; }
        public string OutputFile { get; set; }
        public int ActualSeed { get; set; }
        public Dictionary<string, string> Settings { get; set; }
    }
}
