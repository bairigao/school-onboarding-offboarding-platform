## person

| Column | Type | Notes |
|--------|------|-------|
| id | PK | |
| first_name | | |
| last_name | | |
| person_type | | 'student' \| 'staff' |
| identifier | | student_id OR staff_id |
| role_or_homeroom | | teacher role OR home room |
| status | | onboarding \| active \| offboarding \| offboarded |
| start_date | | |
| end_date | | |
| notes | | medical info, special cases |
| created_at | | |
| updated_at | | |

## lifecycle_request

| Column | Type | Notes |
|--------|------|-------|
| id | PK | |
| person_id | FK | -> person.id |
| request_type | | onboard \| offboard |
| effective_date | | |
| submitted_by | | name or user id |
| submitted_role | | enrolment \| hr |
| status | | pending \| in_progress \| completed |
| notes | | long email-style text |
| created_at | | |
| updated_at | | |

## lifecycle_task

| Column | Type | Notes |
|--------|------|-------|
| id | PK | |
| lifecycle_request_id | FK | |
| task_type | | assign_device \| return_device \| issue_badge \| collect_keys |
| required | | true/false |
| completed | | true/false |
| completed_at | | |
| notes | | |

## asset_assignment

| Column | Type | Notes |
|--------|------|-------|
| id | PK | |
| person_id | FK | |
| snipeit_asset_id | | |
| asset_tag | | |
| assigned_at | | |
| returned_at | | |
| condition_notes | | |

## ticket_link

| Column | Type | Notes |
|--------|------|-------|
| id | PK | |
| lifecycle_request_id | FK | |
| osticket_ticket_id | | |
| ticket_type | | onboarding \| offboarding \| issue |
| created_at | | |

## audit_log

| Column | Type | Notes |
|--------|------|-------|
| id | PK | |
| entity_type | | person \| lifecycle_request \| asset_assignment |
| entity_id | | |
| action | | created \| updated \| status_changed |
| old_value | | |
| new_value | | |
| changed_by | | |
| changed_at | | |