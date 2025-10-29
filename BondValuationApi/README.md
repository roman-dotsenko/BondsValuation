# Bond Valuation API

## Overview

REST API for uploading bond position CSV files and retrieving valuation results from Azure Blob Storage.

## Endpoints

### Health Check
```http
GET /api/BondValuation
```

Returns API status and version information.

**Response:**
```json
{
  "message": "Bond Valuation API is running.",
  "timestamp": "2025-01-31T10:00:00Z",
  "version": "1.0.0"
}
```

---

### Get Valuations
```http
GET /api/BondValuation/valuations?count=5
```

Retrieves the last N valuation result files from blob storage output folder.

**Query Parameters:**
- `count` (optional): Number of results to retrieve. Default: 5, Max: 20

**Response:**
```json
[
  {
    "fileName": "bonds_valued_20250131_100000.csv",
  "lastModified": "2025-01-31T10:00:00Z",
    "sizeInBytes": 1024,
    "results": [
      {
        "bondId": "B001",
    "type": "InflationLinked",
        "presentValue": 450.25,
        "notes": "Approximate value - actual value depends on realized inflation rates"
    },
      {
        "bondId": "B002",
        "type": "Bond",
        "presentValue": 480.50,
        "notes": null
      }
    ]
  }
]
```

**Status Codes:**
- `200 OK`: Success
- `500 Internal Server Error`: Server error

---

### Upload Bond Positions
```http
POST /api/BondValuation/valuations
Content-Type: multipart/form-data
```

Uploads a CSV file containing bond positions to blob storage for processing.

**Request:**
- Form Data:
  - `file`: CSV file (required)

**CSV Format:**
```csv
BondID;Issuer;Rate;FaceValue;PaymentFrequency;Rating;Type;YearsToMaturity;DiscountFactor;DeskNotes
B001;Healthcare Trust;Inflation+0.92%;500;Quarterly;BBB+;Inflation-Linked;13.7;0.54715;Indexing details unclear
B002;National Utilities;3.10%;500;Semi-Annual;A;Bond;5.5;0.78498;Corporate bond
```

**Response:**
```json
{
  "success": true,
  "message": "File uploaded successfully and will be processed shortly",
  "blobName": "input/20250131_100000_bonds.csv",
  "uploadedAt": "2025-01-31T10:00:00Z"
}
```

**Status Codes:**
- `200 OK`: File uploaded successfully
- `400 Bad Request`: Invalid file or format
- `500 Internal Server Error`: Upload failed

**Error Response:**
```json
{
  "error": "Invalid CSV format",
  "details": "Missing required column: BondID"
}
```

---
