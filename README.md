# C# Raytracer - Sequential and Parallel Implementations
3D raytracer implemented in C#, based on Peter  Shirley's "Ray Tracing in One Weekend" tutorial. Contains both sequential and parallelized versions.

---

## Project Structure

```
/
├── Sequential/   # Sequential implementation (.cs files + .csproj)
└── Parallel/     # Parallel implementation (.cs files + .csproj)
```

Each folder contains all source `.cs` files and a `.csproj` project file.

---

## Getting Started

### Prerequisites

- [.NET SDK 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) or later

### Building

Navigate to either the `Sequential` or `Parallel` folder and run:

```bash
dotnet build -c Release
```

### Running

Run the executable directly from the build output directory:

```
bin/Release/net8.0/Raytracer_Sequential.exe
```

> This is also where the rendered image file and `.CSV` test result files are saved.

---

## Usage

An interactive menu is available at runtime to adjust render settings such as resolution and samples per pixel.

### Default Settings

| Setting          | Value       |
|------------------|-------------|
| Resolution       | 400 × 225   |
| Samples per pixel | 20         |

### High-Quality Render Example

The cover image for the accompanying report was rendered at:

| Setting          | Value       |
|------------------|-------------|
| Resolution       | 800 × 450   |
| Samples per pixel | 100        |

---

## Test Environment

| Component | Details                          |
|-----------|----------------------------------|
| CPU       | AMD Ryzen 5 7600X (6C / 12T)    |
| OS        | Windows 11                       |
| Runtime   | .NET 8.0                         |