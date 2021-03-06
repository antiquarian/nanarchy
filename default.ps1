Framework '4.0'
FormatTaskName "--- {0} ---"

properties {
    $version = if ("$env:BUILD_NUMBER".length -gt 0) { "$env:BUILD_NUMBER" } else { "0.0.0.0" }
   
    $TestCategory = if ("$env:testCategory".length -gt 0) { "$env:testCategory" } else { "e2e" }
    $configuration = if ("$env:configuration".length -gt 0) { "$env:configuration" } else { "Release" }
    $slnPlatform = 'Any CPU'
    $csprojPlatform = 'AnyCPU'

    $root = Resolve-Path '.'

    $primary = 'Nanarchy'
    $tsln = Join-Path $root "$primary.sln"
    $nunitrunner = Join-Path $root 'tools\nunit\nunit-console.exe'
    $nunitreporter = Join-Path $root 'tools\nunit-reporting\NUnitHTMLReportGenerator.exe'

    $includeCategoryExpression = ""
    $excludeCategoryExpression = 'disabled'
}

task default -depends WriteEnvironmentInformation, UnitTests
task trans -depends WriteEnvironmentInformation, NanarchyUnitTests
task UnitTests -depends WriteEnvironmentInformation, NanarchyUnitTests

task WriteEnvironmentInformation {
    Write-Host "In default.ps1 Test category --> [$TestCategory]"
    Write-Host "In default.ps1 Source root --> [$root]"
    Write-Host "In default.ps1 Version --> [$version]"
    Write-Host "In default.ps1 Configuration --> [$configuration]"
}

task NanarchyUnitTests -depends CompileNanarchy {
    $testDlls = Join-Path $root "$primary.Tests\bin\$configuration\Nanarchy.Tests.dll"

    try {
        exec { & $nunitrunner $testDlls /nologo /noshadow /framework:net-4.5 /process:separate /xml:TestResult.Nanarchy.xml } "Test Failure"
    } catch {
        Write-Error $_.Exception.Message
    } finally {
        & $nunitreporter TestResult.Nanarchy.xml TestResult.Nanarchy.xml.html
    }
}

task Compile -depends CompileNanarchy

task CompileNanarchy -depends CleanNanarchy {
    Compile-Solution $tsln $version
}

task Clean -depends CleanNanarchy

task CleanNanarchy {
    Remove-Item -ErrorAction SilentlyContinue TestResult.Nanarchy.xml*
    Clean-Solution $tsln
}

# helpers below
# -------------------------------------

function Create-CommonAssemblyInfo($project, $out) {
    $birthYear = 2015
    $date = Get-Date
    $year = $date.Year
    $copyrightSpan = if ($year -eq $birthYear) { $year } else { "$birthYear-$year" }
    $copyright = "Copyright (c) Antiq Inc $copyrightSpan"

    "using System.Reflection;

    [assembly: AssemblyProduct(""$project"")]
    [assembly: AssemblyCompany(""Antiq Inc"")]
    [assembly: AssemblyCopyright(""$copyright"")]
    [assembly: AssemblyFileVersion(""$version"")]
    [assembly: AssemblyVersion(""$version"")]

    #if DEBUG
    [assembly: AssemblyConfiguration(""Debug"")]
    #else
    [assembly: AssemblyConfiguration(""Release"")]
    #endif" | out-file "$out\CommonAssemblyInfo.cs" -encoding "ASCII"
}

function Clean-Directory($dir) {
    New-Item -ItemType Directory -Force $dir
    Remove-Item -Recurse -Force $dir\*.*
}

function Compile-Solution($solution, $v) {
    exec {
        msbuild /verbosity:m /nologo $solution `
            /p:Platform=$slnPlatform `
            /p:Configuration=$configuration `
            /m 
    }
}

function Compile-Project($project, $v, $isRebuild) {
    $target = "/t:build"

    if ($isRebuild) {
        $target= "/t:rebuild"
    }

    exec { msbuild /verbosity:m /nologo $project /p:Configuration=$configuration /m $target }
}

function Clean-Solution($solution) {
    exec { msbuild /nologo /verbosity:m /t:Clean $solution /Property:Platform=$slnPlatform /Property:Configuration=$configuration }
}

function Force-Resolve-Path {
    <#
    .SYNOPSIS
        Calls Resolve-Path but works for files that don't exist.
    .REMARKS
        From http://devhawk.net/2010/01/21/fixing-powershells-busted-resolve-path-cmdlet/
    #>

    param ([string] $FileName)

    $FileName = Resolve-Path $FileName -ErrorAction SilentlyContinue `
                                       -ErrorVariable _frperror
    if (-not($FileName)) {
        $FileName = $_frperror[0].TargetObject
    }

    return $FileName
}

function Create-Zip([string] $sourcefolder, [string] $zipfile, [bool] $overwritezip) {
    if (($overwritezip) -and (Test-Path $zipfile)) {
        Remove-Item -force -path $zipfile | Out-Null
    }

    [Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem") | Out-Null
    [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcefolder, $zipfile)
}

function Delete-BinObj() {
    Get-ChildItem $root -Recurse -include bin,obj | Remove-Item -Force -Recurse
}

function script:poke-xml($filePath, $xpath, $value, $namespaces = @{}) {
    [xml] $fileXml = Get-Content $filePath

    if ($namespaces -ne $null -and $namespaces.Count -gt 0) {
        $ns = New-Object Xml.XmlNamespaceManager $fileXml.NameTable
        $namespaces.GetEnumerator() | %{ $ns.AddNamespace($_.Key,$_.Value) }
        $node = $fileXml.SelectSingleNode($xpath,$ns)
    } else {
        $node = $fileXml.SelectSingleNode($xpath)
    }

    if ($node -eq $null) {
        return
    }

    if ($node.NodeType -eq "Element") {
        $node.InnerText = $value
    } else {
        $node.Value = $value
    }

    $fileXml.Save($filePath)
}
