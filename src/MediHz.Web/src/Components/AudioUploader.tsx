import { useCallback, useState } from 'react'
import { useDropzone } from 'react-dropzone'
import { toast } from 'sonner'
import { Card } from './Shadcn/card'
import { Spinner } from './Shadcn/spinner'
import { addAudioFiles } from '../Api/audio'
import { ApiStatus } from '../Types/api'
import type { AudioFile } from '../Types/audio'
import { s } from '../Utilities/string'

interface IProps {
  projectId: string
  onUploadFiles: (uploadedFiles: AudioFile[]) => void
}

const AudioUploader = ({ projectId, onUploadFiles }: IProps) => {
  const [isUploading, setIsUploading] = useState(false)

  const onDrop = useCallback(
    async (acceptedFiles: File[]) => {
      if (acceptedFiles.length === 0) return

      setIsUploading(true)

      addAudioFiles(projectId, acceptedFiles)
        .then((result) => {
          if (result.ok) {
            const uploadedFiles = result.value.items.filter((f) => f.status === ApiStatus.Success)
            toast.success(`Uploaded ${uploadedFiles.length} file${s(uploadedFiles)}.`)
            onUploadFiles(uploadedFiles.map((f) => ({ id: f.id, fileName: f.fileName })))
          } else {
            toast.error(`Upload failed: ${result.error.status} - ${result.error.title}.`)
          }
        })
        .catch((error) => {
          console.error('Error uploading audio files:', error)
          toast.error('An unexpected error occurred while uploading.')
        })
        .finally(() => {
          setIsUploading(false)
        })
    },
    [projectId, onUploadFiles],
  )

  const { getRootProps, getInputProps, isDragActive, isDragReject } = useDropzone({
    onDrop,
    accept: {
      'audio/mpeg': ['.mp3'],
      'audio/wav': ['.wav'],
      'audio/x-wav': ['.wav'],
    },
  })

  return (
    <Card
      {...getRootProps()}
      data-drag-active={isDragActive}
      className="cursor-pointer items-center justify-center gap-0 border-2 border-dashed px-4 text-center text-muted-foreground ring-0 transition-colors hover:bg-accent data-[drag-active=true]:bg-accent"
    >
      <input {...getInputProps()} />
      {isUploading ? (
        <p className="flex items-center gap-2">
          <Spinner />
          Uploading files to server, please wait...
        </p>
      ) : isDragReject ? (
        <p className="text-destructive">Unsupported file format. Please upload MP3 or WAV files.</p>
      ) : isDragActive ? (
        <p>Drop your audio files here...</p>
      ) : (
        <p>Drag and drop some MP3 or WAV files here, or click to select</p>
      )}
    </Card>
  )
}

export default AudioUploader
