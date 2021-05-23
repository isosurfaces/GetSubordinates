# Get-Subordinates Executable

This is a Visual Studio c# solution. It builds a console app to read user-heirarchy json file, displays Subordinates for input ID, and writes result to file.

## Getting Started

1. Download and build the c# solution.

2. Run "GetSubordinates" as the StartUp project or run the executable in the build folder \GetSubordinatesTest\bin\Debug\netcoreapp3.1\GetSubordinates.exe. Make sure Nuget Package Restore is enabled in Visual Studio when building the solution, or you can restore Nuget packages manually before building.

3. Follow the instructions in console.

4. The result will also be saved as subordinates.json in the executable folder.

5. In debug mode, call GetSubOrdinates(id) from an instance of UserRoleManager.

### Prerequisites

When you launch GetSubordinates.exe, it will asks you for a user-heirarchy json file. You can simply press enter to load the default json file.

The default file is named "userHeirarchy.json" and placed in the Resources folder already. You can edit or replace this file, or you may provide a path to another file when prompted.

```
E.g. C:\SamplePath\SampleFile.json
```

The console will return an error messages if the input file contains no users, or no roles, or any user or role fields are missing.

## Running the tests

Please use Visual Studio Test Explorer to run the tests.

### Unit tests

Unit tests can be found in the GetSubordinatesTest project -- GetSubordinatesTest class.

```
ValidateFile_Path
ValidateFile_Deserialisation
ValidateInput_UserRoleFields
ValidateInput_UserRoleDuplicates
GetSubordinatesFromID
```

