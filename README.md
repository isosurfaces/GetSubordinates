# Get-Subordinates Executable

This is a Visual Studio c# solution. It builds a console app to read user-heirarchy json file, displays Get-Subordinates for any user ID input, and write any results to file.

## Getting Started

Download and build the c# solution. Run "GetSubordinates" as the StartUp project or run the executable in the build folder \GetSubordinatesTest\bin\Debug\netcoreapp3.1\GetSubordinates.exe. Make sure Nuget Package Restore is enabled in Visual Studio to build the solution.

Launch GetSubordinates.exe and then follow the instructions in console.

### Prerequisites

When you launch GetSubordinates.exe, it will asks you for a user-heirarchy json file. You can simply press enter to load the default json file.

The default json file is named "userHeirarchy.json" and placed in the folder already. You can either edit or replace this file with other json file with the same name, or you can provide a path to other json file when prompted by GetSubordinates.exe.

```
C:\SamplePath\SampleFile.json
```

The console app will display error messages if the input file contains no users, or no roles, or any user or role fields are missing.

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

