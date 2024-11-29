Set-Location -Path $PSScriptRoot

function Create-PuzzleStructure {
    param (
        [int]$DayNumber
    )

    # Create the main folder with two-digit name
    $folderName = "{0:D2}" -f ${DayNumber}
    New-Item -Path $folderName -ItemType Directory -Force

    # Create and populate solutions.md
    $solutionsContent = @"
# Solutions to Day ${DayNumber}: ..

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1



## Part 2

"@
    Set-Content -Path "$folderName\solutions.md" -Value $solutionsContent

    # Create and populate README.md
    $readmeContent = @"
*This is a copy of the original puzzle description. If you want to learn more about my approach to solving these puzzles see [my solutions](solutions.md).*

---
# Day ${DayNumber}: 
"@
    Set-Content -Path "$folderName\README.md" -Value $readmeContent

    # Create part-1 and part-2 folders
    foreach ($part in 1..2) {
        $partFolder = "$folderName\part-$part"
        New-Item -Path $partFolder -ItemType Directory -Force

        # Create and populate the .csproj file
        $csprojContent = @"
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>_${folderName}_part_$part</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
"@
        Set-Content -Path "$partFolder\$folderName-part-$part.csproj" -Value $csprojContent

        $newGuid = "{$(New-Guid)}".ToUpper()

        # Create and populate the .sln file
        $slnContent = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.10.34916.146
MinimumVisualStudioVersion = 10.0.40219.1
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "$folderName-part-$part", "$folderName-part-$part.csproj", "${newGuid}"
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		${newGuid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		${newGuid}.Debug|Any CPU.Build.0 = Debug|Any CPU
		${newGuid}.Release|Any CPU.ActiveCfg = Release|Any CPU
		${newGuid}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = ${newGuid}
	EndGlobalSection
EndGlobal
"@
        Set-Content -Path "$partFolder\$folderName-part-$part.sln" -Value $slnContent

        # Create and populate the Program.cs file
        $programContent = @"
string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\${folderName}\\input.example");

var answer = 0;
foreach (var line in lines)
{

}

Console.WriteLine(answer);
"@
        Set-Content -Path "$partFolder\Program.cs" -Value $programContent
    }

    Write-Host "Folder structure for Day ${DayNumber} created successfully!"
}

# Create the folder structure
foreach ($dayNumber in 1..25) {
    Create-PuzzleStructure -DayNumber $dayNumber
}