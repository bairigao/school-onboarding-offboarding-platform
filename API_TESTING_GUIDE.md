# School Onboarding/Offboarding Platform - API Testing Guide

## Server Status
✅ API Server running on: **http://localhost:5007**

---

## Testing the API

### 1. Create a Person (Student or Staff)
**Endpoint:** `POST /api/people`

```bash
curl -X POST http://localhost:5007/api/people \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Smith",
    "identifier": "john.smith@school.edu",
    "personType": "student",
    "startDate": "2026-01-15",
    "notes": "New student enrolling"
  }'
```

**Response (201 Created):**
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Smith",
  "fullName": "John Smith",
  "identifier": "john.smith@school.edu",
  "personType": "student",
  "status": "onboarding",
  "startDate": "2026-01-15",
  "endDate": null,
  "notes": "New student enrolling",
  "createdAt": "2026-01-12T14:15:00Z",
  "updatedAt": "2026-01-12T14:15:00Z"
}
```

---

### 2. List All People
**Endpoint:** `GET /api/people`

```bash
curl http://localhost:5007/api/people
```

**Optional Filters:**
- `?personType=student` - Filter by type (student/staff)
- `?status=onboarding` - Filter by status (onboarding/active/offboarding/offboarded)
- `?page=1&pageSize=20` - Pagination

---

### 3. Get a Specific Person
**Endpoint:** `GET /api/people/{id}`

```bash
curl http://localhost:5007/api/people/1
```

---

### 4. Update Person Information
**Endpoint:** `PUT /api/people/{id}`

```bash
curl -X PUT http://localhost:5007/api/people/1 \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Jonathan",
    "status": "active",
    "notes": "Updated name"
  }'
```

---

### 5. View Available Devices from Snipe-IT
**Endpoint:** `GET /api/assets`

```bash
curl http://localhost:5007/api/assets
```

**Optional Filters:**
- `?page=1&pageSize=50` - Pagination

---

### 6. Get a Specific Device
**Endpoint:** `GET /api/assets/{id}`

```bash
curl http://localhost:5007/api/assets/123
```

---

### 7. Submit Onboarding Request (EO/HR Only)
**Endpoint:** `POST /api/lifecycle-requests`

Headers:
- `X-User-Id: john.doe` - Current user ID
- `X-User-Role: enrolment` or `hr`

```bash
curl -X POST http://localhost:5007/api/lifecycle-requests \
  -H "Content-Type: application/json" \
  -H "X-User-Id: john.doe" \
  -H "X-User-Role: enrolment" \
  -d '{
    "personId": 1,
    "requestType": "onboard",
    "submittedRole": "enrolment",
    "submittedBy": "john.doe",
    "effectiveDate": "2026-01-20",
    "notes": "Priority enrollment"
  }'
```

**Response (201 Created):**
```json
{
  "id": 1,
  "personId": 1,
  "person": { /* person details */ },
  "requestType": "onboard",
  "status": "pending",
  "submittedRole": "enrolment",
  "submittedBy": "john.doe",
  "effectiveDate": "2026-01-20",
  "notes": "Priority enrollment",
  "tasks": [
    {
      "id": 1,
      "taskType": "assign_device",
      "required": true,
      "completed": false,
      "completedAt": null
    },
    {
      "id": 2,
      "taskType": "issue_badge",
      "required": false,
      "completed": false
    },
    {
      "id": 3,
      "taskType": "collect_keys",
      "required": false,
      "completed": false
    }
  ],
  "createdAt": "2026-01-12T14:20:00Z"
}
```

---

### 8. View Lifecycle Requests
**Endpoint:** `GET /api/lifecycle-requests`

Headers:
- `X-User-Role: enrolment/hr/it/admin` - User role (controls visibility)
- `X-User-Id: john.doe` - Current user ID (EO/HR only see own requests)

```bash
# EO/HR sees only their own requests
curl http://localhost:5007/api/lifecycle-requests \
  -H "X-User-Role: enrolment" \
  -H "X-User-Id: john.doe"

# IT/Admin sees all requests
curl http://localhost:5007/api/lifecycle-requests \
  -H "X-User-Role: it"
```

**Optional Filters:**
- `?status=pending` - Filter by status (pending/in_progress/completed)
- `?requestType=onboard` - Filter by type (onboard/offboard)
- `?personId=1` - Filter by person
- `?page=1&pageSize=20` - Pagination

---

### 9. Get Specific Request Details
**Endpoint:** `GET /api/lifecycle-requests/{id}`

```bash
curl http://localhost:5007/api/lifecycle-requests/1
```

---

### 10. Update Lifecycle Request Status (IT/Admin Only)
**Endpoint:** `PUT /api/lifecycle-requests/{id}`

```bash
curl -X PUT http://localhost:5007/api/lifecycle-requests/1 \
  -H "Content-Type: application/json" \
  -H "X-User-Role: it" \
  -d '{
    "status": "in_progress",
    "effectiveDate": "2026-01-20",
    "notes": "Starting onboarding process"
  }'
```

---

### 11. View Tasks for a Request
**Endpoint:** `GET /api/lifecycle-tasks/request/{requestId}`

```bash
curl http://localhost:5007/api/lifecycle-tasks/request/1 \
  -H "X-User-Role: it"
```

**Response:**
```json
[
  {
    "id": 1,
    "lifecycleRequestId": 1,
    "taskType": "assign_device",
    "required": true,
    "completed": false,
    "completedAt": null,
    "notes": null
  }
]
```

---

### 12. Complete a Task (IT Only) - Device Checkin Example
**Endpoint:** `PUT /api/lifecycle-tasks/{id}`

For device return/checkin, include device info in Notes field:

```bash
curl -X PUT http://localhost:5007/api/lifecycle-tasks/1 \
  -H "Content-Type: application/json" \
  -H "X-User-Role: it" \
  -d '{
    "completed": true,
    "notes": "MACBOOK-A123|Good condition, minor scratches"
  }'
```

**Response (200 OK):**
```json
{
  "id": 1,
  "lifecycleRequestId": 1,
  "taskType": "return_device",
  "required": true,
  "completed": true,
  "completedAt": "2026-01-12T14:25:00Z",
  "notes": "MACBOOK-A123|Good condition, minor scratches"
}
```

---

## User Roles & Permissions

| Operation | EO | HR | IT | Admin |
|-----------|----|----|----|----|
| Add Person | ✅ | ✅ | ❌ | ✅ |
| Update Person | ✅ | ✅ | ❌ | ✅ |
| List People | ✅ | ✅ | ✅ | ✅ |
| Submit Request | ✅ | ✅ | ❌ | ✅ |
| View Own Requests | ✅ | ✅ | - | - |
| View All Requests | ❌ | ❌ | ✅ | ✅ |
| Modify Own Request | ✅* | ✅* | - | - |
| Update Any Request | ❌ | ❌ | ✅ | ✅ |
| Complete Tasks | ❌ | ❌ | ✅ | ✅ |
| View Devices | ✅ | ✅ | ✅ | ✅ |

*EO/HR can only modify pending requests they submitted

---

## Test Workflow: Complete Onboarding

### Step 1: Create a Student (EO)
```bash
curl -X POST http://localhost:5007/api/people \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Sarah","lastName":"Johnson","identifier":"sarah.j@school.edu","personType":"student","startDate":"2026-02-01"}'
```
Response: `Person ID = 1`

### Step 2: Submit Onboarding Request (EO)
```bash
curl -X POST http://localhost:5007/api/lifecycle-requests \
  -H "Content-Type: application/json" \
  -H "X-User-Id: officer.smith" \
  -H "X-User-Role: enrolment" \
  -d '{"personId":1,"requestType":"onboard","submittedRole":"enrolment","submittedBy":"officer.smith","effectiveDate":"2026-02-01"}'
```
Response: `Request ID = 1` with 3 auto-created tasks

### Step 3: IT Views All Requests
```bash
curl http://localhost:5007/api/lifecycle-requests \
  -H "X-User-Role: it"
```
Response: Shows all requests including the one just created

### Step 4: IT Updates Request to In Progress
```bash
curl -X PUT http://localhost:5007/api/lifecycle-requests/1 \
  -H "Content-Type: application/json" \
  -H "X-User-Role: it" \
  -d '{"status":"in_progress"}'
```

### Step 5: IT Views Tasks for This Request
```bash
curl http://localhost:5007/api/lifecycle-tasks/request/1 \
  -H "X-User-Role: it"
```

### Step 6: IT Completes All Required Tasks
```bash
# Complete assign_device (task ID 1)
curl -X PUT http://localhost:5007/api/lifecycle-tasks/1 \
  -H "Content-Type: application/json" \
  -H "X-User-Role: it" \
  -d '{"completed":true,"notes":"Device assigned: LAPTOP-001"}'

# Note: When all required tasks are completed, request auto-updates to "completed"
```

---

## Troubleshooting

**Error: Cannot create person with duplicate identifier**
- Solution: Use a unique email/identifier

**Error: Person not found (404)**
- Solution: Verify PersonId exists, check GET /api/people first

**Error: Forbidden (403)**
- Solution: Check X-User-Role header is set correctly (need "it" or "admin" to modify requests)

**Error: Cannot convert type int to string**
- Solution: Ensure JSON fields are in correct format (no extra quotes around numbers)

---

## Next Steps

1. Test all endpoints using the curl commands above
2. Verify role-based access control (try requesting as different roles)
3. Test the complete onboarding workflow
4. Check database directly to see created records
5. Monitor logs in the terminal for request flow

---

**Database:** SQL Server running in Docker on localhost:1433  
**Logs:** Console output shows all operations with timestamps  
**Swagger API Docs:** (Not configured yet - can be added if needed)
