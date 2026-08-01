# MediHz

MediHz is a light-weight audio annotation tool for use by medical professionals in the healthcare industry. It aims to provide a simple and intuitive interface for creating annotations on audio files collaboratively at scale.

## Contributing

## Quickstart

TODO.

## Technology Stack

- Backend: .NET Core.
- Frontend: React, TypeScript.
- Database: PostgreSQL.

## Development Process

1. Create a branch according to the [Branching Strategy](#branching-strategy).
1. Create atomic commits on the branch with appropriate [Commit Messages](#commits).
1. Push your branch to the origin and create a [Merge Request](#review), ensuring the code is tested thoroughly and passes [CI/CD Checks](#cd/cd).
1. Squash merge the branch onto main when ready.
1. Once merged begin the [Deployment Process](#deployment).

### Branching Strategy

This project makes use of [Trunk Based Development](https://trunkbaseddevelopment.com/). Developers are expected to develop features on a branch off of `main`.

Branches should align with [Commit Names](#commits); `<type>/<short-description>`. For example:
- `feature/audio-annotation-editor`
- `fix/audio-upload-failure`

### Commits

Commits should be atomic, this means each commit should do just one thing. Commit messages concisely describe the work done using the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/#summary) specification.

### Review

Each Merge Request (MR) must be reviewed by one contributor that did not contribute code to the MR. Reviews are expected to be raise valid concerns around functionality, code quality, maintainability, and clarify concepts in code.

MRs should follow the MR template in the repo. Which is automatically used when creating one through the web-ui or can be referenced from `./.gitlab/merge-request-templates/default.md` by agents wishing to raise MRs on behalf of the user.

The MR title should align with [Commit Names](#commits).

#### CD/CD

TBD

### Deployment

TBD
