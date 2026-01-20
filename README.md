# school-onboarding-offboarding-platform

┌──────────────────────────────────────────────────────────────┐
│                          PERSON                              │
│  (Student or Staff member in system)                         │
│  PK: Id                                                       │
│  FirstName, LastName, PersonType, Identifier, Status         │
└────────────────┬─────────────────────────────────┬───────────┘
                 │                                 │
        1:Many   │                                 │   1:Many
                 ▼                                 ▼
    ┌─────────────────────────┐      ┌──────────────────────────┐
    │ LIFECYCLE_REQUEST       │      │ ASSET_ASSIGNMENT         │
    │ (Onboard/Offboard)      │      │ (Device Assignment)      │
    │ PK: Id                  │      │ PK: Id                   │
    │ FK: PersonId            │      │ FK: PersonId             │
    │ RequestType, Status     │      │ SnipeItAssetId, AssetTag │
    └──────────┬──────────────┘      └──────────────────────────┘
               │
        1:Many │
               ├─────────────────────────┬──────────────────┐
               ▼                         ▼                  ▼
    ┌──────────────────────┐   ┌──────────────────┐   ┌────────────────┐
    │ LIFECYCLE_TASK       │   │ TICKET_LINK      │   │ AUDIT_LOG      │
    │ (Workflow Steps)     │   │ (OSTicket Link)  │   │ (Change Trail) │
    │ PK: Id               │   │ PK: Id           │   │ PK: Id         │
    │ TaskType, Completed  │   │ OsTicketId       │   │ EntityType,    │
    │ CompletedAt          │   │ TicketType       │   │ EntityId, Action│
    └──────────────────────┘   └──────────────────┘   │ OldValue, New  │
                                                      └────────────────┘