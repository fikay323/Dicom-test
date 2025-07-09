
# 🧠 DICOM Upload & Viewer API

This project provides a full-stack DICOM file handling service — allowing you to **upload**, **render**, and **query DICOM tags** via a RESTful .NET 9 Web API and an Angular frontend. Uploaded files are stored in **Azure Blob Storage**, and rendered into **PNG format** for visualization.

---

## 🚀 Features

- ✅ Upload `.dcm` files via Angular UI
- 🖼 Render and preview DICOM images (converted to PNG)
- 🔍 Extract specific DICOM tags (e.g., `0010,0010` or `PatientName`)
- ☁️ Cloud-ready with Azure Blob Storage
- 🔐 Secure configuration via environment variables or Key Vault
- 🧪 Tested with xUnit, Moq, FluentAssertions

---

## 🛠 Technologies

- **Backend:** .NET 9, ASP.NET Core, FellowOak.Dicom, Azure SDK
- **Frontend:** Angular, RxJS, SCSS
- **Storage:** Azure Blob Storage

---

## 📦 Setup Instructions

### 🧰 1. Clone the Repository

```bash
git clone https://github.com/your-username/dicom-upload-viewer.git
cd dicom-upload-viewer
```

---

### ⚙️ 2. Configure Azure Blob Storage

Create a storage account and blob container (`dicom-files`) in Azure.

Update your `appsettings.json` for development:

```json
"AzureBlobStorage": {
  "ContainerName": "dicom-files"
}
```

Set the connection string using environment variables:

```bash
# Windows (PowerShell)
$env:AzureBlobStorage__ConnectionString = "DefaultEndpointsProtocol=...;AccountKey=..."

# macOS/Linux
export AzureBlobStorage__ConnectionString="DefaultEndpointsProtocol=...;AccountKey=..."
```

> ✅ You can also use `dotnet user-secrets` or Azure Key Vault.

---

### ⚙️ 3. Run the .NET Backend

```bash
cd server
dotnet restore
dotnet build
dotnet run
```

> API will run at `https://localhost:5245`

---

### 🌐 4. Run the Angular Frontend

```bash
cd dicom-viewer
npm install
ng serve --port 4201
```

> Frontend runs at `http://localhost:4201`

---

## 🔍 API Endpoints

| Method | Endpoint                        | Description                          |
|--------|----------------------------------|--------------------------------------|
| POST   | `/api/dicom/upload`             | Upload a DICOM file                  |
| GET    | `/api/dicom/render?filename=...` | Render uploaded file as PNG          |
| GET    | `/api/dicom/header?filename=...&tag=...` | Query tag value from uploaded DICOM |

---

## 🧑‍💻 Example DICOM Tags

Use these in `tag=` query or via Angular UI:

| Tag         | Keyword         | Description             |
|-------------|------------------|-------------------------|
| `0010,0010` | `PatientName`    | Patient's full name     |
| `0010,0020` | `PatientID`      | Unique patient ID       |
| `0008,0060` | `Modality`       | Imaging modality        |
| `0020,000D` | `StudyInstanceUID` | Unique study ID       |

---

## 🧼 Security Notes

- Do **not** hardcode secrets in `appsettings.json`
- Use `.env`, user secrets, or Azure Key Vault for connection strings
- Blob containers should be set to **private** access level

---

## 🗺 Folder Structure

```
server/                  → .NET Web API
  Controllers/
  Contracts/
  Services/
  appsettings.json

dicom-viewer/            → Angular Frontend
  src/app/
    dicom-api.service.ts
    viewer.component.ts
```

---

## 📄 License

MIT License. Free to use and extend.

---

## 👨‍💻 Author

Built by **Caleb Fagbenro** — [GitHub](https://github.com/fikay323) | [LinkedIn](https://www.linkedin.com/in/oluwafikayomi-fagbenro/)
