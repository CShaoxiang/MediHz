# Project Level
If desired an entire mediHz project in a single json file.

## Attributes
| Attribute | Type | Requirement | Description |
| :--- | :--- | :--- | :--- |
| `project_id` | String | Required | A unique internal identifier for the project. |
| `project_name` | String | Required | The human-readable title of the project. |
| `description` | String | Optional | A brief summary of the projects contents and purpose. |
| `annotator_ids` | Array of Strings | Optional | A list of unique identifiers for the users or clinicians who have contributed annotations to the project. |
| `files` | Array of Objects | Required | A list containing the file-level objects, representing the individual audio tracks associated with this project. |

## Example Json
```json
{
  "project_id": "proj_medihz_001",
  "project_name": "Cardiology Ward Baseline",
  "description": "Initial annotation of heartbeat audio files to standardise cardiac event detection.",
  "annotator_ids": [
    "clinician_84",
    "clinician_102"
  ],
  "files": [
    // ... File level objects ...
  ]
}
```

# File Level
The Json export for an individual audio file. If desired these can be exported individually.

## Attributes
| Attribute | Type | Requirement | Description |
| :--- | :--- | :--- | :--- |
| `file_id` | String | Required | A unique internal identifier for the audio file. |
| `file_name` | String | Required | The original name of the uploaded audio file. |
| `file_type` | String | Optional | The format or MIME type of the audio file (e.g., "audio/wav" or ".wav"). |
| `duration_seconds` | Float | Required | The total length of the audio file in seconds. |
| `sample_rate` | Integer | Required | The sampling rate of the audio file in Hertz (Hz). |
| `channels` | Integer | Optional | The number of audio channels (e.g., 1 for mono, 2 for stereo). |
| `annotations` | Array of Objects | Required | A list containing the annotation-level objects, representing the individual annotations made on this audio file. |

## Example Json
```json
{
  "file_id": "file_medihz_001",
  "file_name": "patient_84_apical_listen.wav",
  "file_type": "audio/wav",
  "duration_seconds": 14.5,
  "sample_rate": 4000,
  "channels": 1,
  "annotations": [
    // ... Label level objects ...
  ]
}
```

# Annotation Level
These are the objects in the 'annotations' arrary at the file level. They cannot be exported individually.

## Attributes
| Attribute | Type | Requirement | Description |
| :--- | :--- | :--- | :--- |
| `annotation_id` | String | Required | A unique identifier for the specific annotation, useful for updating or deleting records in a database. |
| `label` | String | Required | The primary classification or tag applied to this audio segment (e.g., "S1", "S2", "murmur"). |
| `start_time` | Float | Required | The starting point of the annotation in absolute seconds. |
| `end_time` | Float | Required | The ending point of the annotation in absolute seconds. |
| `annotator_id` | String | Required | The ID of the specific user who created this annotation. |

## Example Json
```json
{
  "annotation_id": "annot_77491",
  "label": "S1",
  "start_time": 1.250,
  "end_time": 1.410,
  "annotator_id": "clinician_84",
}
```