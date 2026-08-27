import { useCallback, useState } from 'react';
import { useDropzone } from 'react-dropzone';
import styles from './AudioUploader.module.css';
import { addAudioFiles } from '../Api/audio';
import { ApiStatus } from '../Types/api';
import type { AudioFile } from '../Types/audio';
import { s } from '../Utilities/string';

interface IProps {
  projectId: string;
  onUploadFiles: (uploadedFiles: AudioFile[]) => void;
}

const AudioUploader = ({ projectId, onUploadFiles } : IProps) => {
  const [isUploading, setIsUploading] = useState(false);
  const [uploadResult, setUploadResult] = useState<string | null>(null);

  const onDrop = useCallback(async (acceptedFiles: File[]) => {
    if (acceptedFiles.length === 0) return;

    setIsUploading(true);
    setUploadResult(null);

    addAudioFiles(projectId, acceptedFiles).then(result => {
      if (result.ok) {
        const uploadedFiles = result.value.items.filter(f => f.status === ApiStatus.Success);
        setUploadResult(`Success! Uploaded ${uploadedFiles.length} file${s(uploadedFiles)}.`);
        onUploadFiles(uploadedFiles.map(f => ({id: f.id, fileName: f.fileName})));
      } else {
        setUploadResult(`Upload failed: ${result.error.status} - ${result.error.title}.`)
      }
    }).catch(error => {
      console.error('Error uploading audio files:', error);
      setUploadResult("An unexpected error occurred while uploading.");
    }).finally(() => {
      setIsUploading(false);
    });
  }, [projectId]);

  const {
    getRootProps,
    getInputProps,
    isDragActive,
    isDragReject
  } = useDropzone({
    onDrop,
    accept: {
      'audio/mpeg': ['.mp3'],
      'audio/wav': ['.wav'],
      'audio/x-wav': ['.wav']
    }
  });



  return (
    <div>
      <div {...getRootProps()} className={`${styles.dropzone} ${isDragActive ? styles.dropzoneActive : ''}`}>
        <input {...getInputProps()} />
        {
          isUploading ? (
            <p>Uploading files to server, please wait...</p>
          ) : isDragReject ? (
            <p className={styles.errorText}>Unsupported file format. Please upload MP3 or WAV files.</p>
          ) : isDragActive ? (
            <p>Drop your audio files here...</p>
          ) : (
            <p>Drag and drop some MP3 or WAV files here, or click to select</p>
          )
        }
      </div>
      
      {uploadResult && (
        <div className={styles.resultContainer}>
          <strong>Result:</strong> {uploadResult}
        </div>
      )}
    </div>
  );
};

export default AudioUploader;
