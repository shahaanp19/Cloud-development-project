# CoffeeNChill — Next Developer Handoff

## What I Have Done

- Rewrote FileShareService to use Azure blob storage instead of Azure file storage. This is because Azurite does not support file storage, which is needed for the Staff Document functionality.
- Updated all menu item functions to use price as a string instead of a decimal as azure tables do not support decimal types. 
- updated the UpdateMenuItem function to use this string price instead of a decimal price, and added checks for the parsing that takes place
- Fixed Bug in DeleteMenuItem function that was causing the function to always return 200 even when the file was already deleted/not found. Now returns 404 when the file is not found and 200 when the file is deleted successfully.
- Wrote Postman API collection for all functions in the project, including tests for all functions. This collection has been included as CoffeeNChill.postman_collection.json and can be imported into Postman to test the API endpoints.

## What You Need To Do

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

- Note that the project will throw an exception (or atleast it should) until you setup docker

### 2. Download and Install docker if you havent already. 

- You can download it from [Docker's official website](https://www.docker.com/products/docker-desktop/).
- You will also probably need WSL 2 if you are on Windows. You can follow the instructions [here](https://docs.microsoft.com/en-us/windows/wsl/install).
- Ensure that docker is running then use this command in powershell (if you are using docker desktop you will probably need to keep the app itself open for the engine to run: 
```powershell
docker run -d --name azurite -p 10000:10000 -p 10001:10001 -p 10002:10002 -v azurite-data:/data mcr.microsoft.com/azure-storage/azurite azurite --blobHost 0.0.0.0 --queueHost 0.0.0.0 --tableHost 0.0.0.0 --location /data
```
- Check that it is running with this powershell command (you would actually also see it in the docker desktop app without this command so its up to you): 
```powershell
docker ps
```
- Run the CoffeeNChill project and ensure that it connects to the Azurite storage emulator without throwing exceptions. You should see something like this:
```cmd
Functions:

        CreateMenuItem: [POST] http://localhost:7069/api/menu

        DeleteMenuItem: [DELETE] http://localhost:7069/api/menu/{category}/{id}

        DownloadStaffDocument: [GET] http://localhost:7069/api/documents/download/{fileName}

        GetAllMenuItems: [GET] http://localhost:7069/api/menu

        GetMenuItemsByCategory: [GET] http://localhost:7069/api/menu/category/{category}

        ListStaffDocuments: [GET] http://localhost:7069/api/documents

        UpdateMenuItem: [PUT] http://localhost:7069/api/menu/{category}/{id}

        UploadStaffDocument: [POST] http://localhost:7069/api/documents/upload
  ```
- Pay attention to the port number in the URLs above. It may be different for you depending on your local setup. Make sure to use the correct port number when testing the API endpoints in Postman, you might need to change the base url variable in the postman variable tab.

### 3. Test the API Endpoints

- Import the CoffeeNChill.postman_collection.json into Postman, you can find the collection file in the docs folder.
- The endpoints all have documentation and tests included in the collection.
- Use the collection to test all API endpoints and ensure they are functioning as expected. you generally have to just click send and the tests will run automatically. some of the staff document endpoints will require you to upload a file first before you can download it or list it.
- If everything is working correctly, you should see all tests pass in Postman.

### 4. Azure Functions Containerization (this part is just rewritten from the poe doc)

- Write a Dockerfile in the Azure functions project root directory. Package the compiled c# project using the offical Azure Functions base runtime.
- Build and tag the image then push it to the public Docker Hub repository. (We dont have one so you will need to make one)
  - docker build -t yourdockerhubusername/coffeenchill-functions:v1.0

### 5. Running Standalone Function Container (this part is just rewritten from the poe doc)

- Run the function container independently, passing environment variables so it connects to the Azurite container host:
  - docker run -p 7071:80 -e AzureWebJobsStorage="UseDevelopmentStorage=true" yourdockerhubusername/coffeenchill-functions:v1.0