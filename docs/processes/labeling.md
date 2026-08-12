# Frontend Layer
## Overview:
Users will be able to click and drag along audio files to specify areas of annotation. These areas can then be given labels and descriptions either by being prepackaged or provided by users. These annotations will then be stored on the backend by sending annotations as a JSON format. These formatted annotations should provide information corresponding to the backend database structure.

### User Story:
    As an annotation user, I want to click and drag areas of an audio file, so that I can annotate specific portions of audio.

    As a collaborator, I want my annotations to simultaneously appear on other users ends, so that I can work alongside others to annotate.

## UI:
**[Planning for this feature, attempts to work alongside the planned JSON export of annotations]**

When a user click and drags along the presented audio file, the frontend needs to take note of the start and end time and the user who created the highlighted area. (start_time,end_time,annotator_id)

A box can then be provided under the area to allow users to input prepackaged or new labels to annotate the area. (label)

If users would want to adjust timing, two sliders can be provided to precisely adjust the area of annotation.

**[In-depth descriptions can be unnecessary at the moment]**
### Label Attributes
| Attributes | Type | Requirement | Description |
| :--- | :--- | :---| :---|
| `labels` | string[]| Required | Short tags to annotate audio |
| `description` | string | Optional | Additonal information |

To allow users to utilise prepackage labels, users should be able apply said labels to annotation areas. Doing so, can be could be done either through selecting a format when annotating or dragging and dropping them to where they are needed.

For a better user experience, users might also prefer to make their own formats and save them for later use. **[If we have time]**

# Backend Layer
After an annotation is saved, its `start_time`, `end_time`, `label attributes`, `annotator_id` will be sent over the backend. 

**[An API request for CRUD updates will work here]**

## Potential Collaboration
New annotations can be added to the `annotations` attribute of the given audio file and shown across all users. Annotations added should only be given an `annotation_id` once they reach the backend.

**[Updates to the frontend can be done using websockets]**