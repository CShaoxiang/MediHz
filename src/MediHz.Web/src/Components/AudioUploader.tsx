import React, { useCallback, useState } from 'react';
import { useDropzone } from 'react-dropzone';
import styles from './AudioUploader.module.css';

const AudioUploader = () => {
  const [isUploading, setIsUploading] = useState(false);
  const [uploadResult, setUploadResult] = useState<string | null>(null);

  const onDrop = useCallback(async (acceptedFiles: File[]) => {
    if (acceptedFiles.length === 0) return;

    setIsUploading(true);
    setUploadResult(null);

    try {
      // 1. Create FormData payload
      const formData = new FormData();
      
      // 2. Append each file. The key 'files' MUST match the C# parameter name: [FromForm] List<IFormFile> files
      acceptedFiles.forEach((file) => {
        formData.append('files', file);
      });

      // (Optional) Append datasetId if required by your controller
      // formData.append('datasetId', 'some-guid-string');

      // 3. Send POST request to your local .NET API
      // Note: Ensure the port matches your backend (5001 is standard for ASP.NET HTTPS)
      const response = await fetch('http://localhost:5001/api/AudioFileUpload/batch', {
        method: 'POST',
        // IMPORTANT: Do NOT set the 'Content-Type' header manually when using FormData.
        // The browser automatically sets it to 'multipart/form-data' with the correct boundary.
        body: formData,
      });

      if (response.ok) {
        const data = await response.json();
        const successfulCount = data.items ? data.items.filter((item: any) => item.status === 1 || item.status === 'Success').length : 0;
        setUploadResult(`Success! Uploaded ${successfulCount} files. Batch ID: ${data.batchId}`);
      } else {
        const errorText = await response.text();
        setUploadResult(`Upload failed: ${response.status} - ${errorText}`);
      }
    } catch (error) {
      console.error('Error uploading files:', error);
      setUploadResult('A network error occurred while uploading.');
    } finally {
      setIsUploading(false);
    }
  }, []);

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
    <div className={styles.container}>
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