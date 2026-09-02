# CoffeeNChill — Next Developer Handoff

## What You Need To Do

The C# Azure Functions have been implemented, but the **Azure Portal setup has NOT been completed yet**.

test the code first, there should be no issues, then create the azure portal stuff (resource group and storage account) and add the connection string to  local settings.json.

### 1. Inspect the Existing Code

Before making changes, inspect the storage-related code, especially:

- `Program.cs`
- `local.settings.json`
- `*.csproj`
- `Services/FileShareService.cs`
- `Models/StaffDocument.cs`
- `Functions/Documents/`
- All Menu Item Functions and their related services

Determine how the project currently expects to connect to Azure File Storage and Azure Table Storage.

### 2. Configure Azure File Storage

Open the existing Azure Storage Account:

`coffeenchillgroupproj`

Go to:

`Data storage → File shares`

Verify that the required Azure File Share exists.

Check `FileShareService.cs` to determine what File Share name the application expects.

The Staff Document flow should be:

`UploadStaffDocument → FileShareService → Azure File Share`

The following Functions should use the File Share:

- `UploadStaffDocument`
- `ListStaffDocuments`
- `DownloadStaffDocument`

### 3. Resolve the Table Storage Issue

The existing account is `FileStorage`, so it does not support Azure Table Storage.

The Menu Item functionality still needs a Table Storage solution.

The preferred approach is to use a separate `StorageV2` account for Azure Table Storage while keeping the existing `FileStorage` account for Staff Documents.

The resulting setup would be:

`StorageV2 → Azure Tables → Menu Items`

and:

`Existing FileStorage → Azure File Share → Staff Documents`

If the group does not want another storage account, inspect the Menu Item code and determine whether the storage implementation can be changed to another suitable solution.

**Do not remove or change the existing FileStorage account**, because it is needed for Staff Documents.

### 4. Configure `local.settings.json`

Make sure the required Azure Storage settings are present.

Example:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "YOUR_CONNECTION_STRING",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
