using System.Globalization;

// This is easier than European currencies for some reason
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

// Select the exercises you want to run

// ExerciseTests.RunAll();
SortingTests.RunAll();



// Create Tests in ExerciseTests.cs

// Run the project to see the output of the tests